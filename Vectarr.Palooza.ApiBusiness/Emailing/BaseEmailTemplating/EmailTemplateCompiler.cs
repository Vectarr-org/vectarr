using HtmlAgilityPack;
using System.Text.RegularExpressions;
using Vectarr.Palooza.ApiBusiness.Emailing.IBaseEmailTemplating;

namespace Vectarr.Palooza.ApiBusiness.Emailing.BaseEmailTemplating
{
    public class EmailTemplateCompiler : IEmailTemplateCompiler
    {
        public string CompileTemplate(string template, IEmailContextBuilder contextBuilder)
        {
            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(template);
            ProcessNode(document.DocumentNode, contextBuilder, "");
            return document.DocumentNode.OuterHtml;
        }

        /// <summary>
        /// Compiles a node
        /// </summary>
        /// <param name="element">The xml element to look into</param>
        /// <param name="contextBuilder">The context builder to be used on this node</param>
        /// <param name="prefix">The prefix to check for when looking for nodes</param>
        private void ProcessNode(HtmlNode element, IEmailContextBuilder contextBuilder, string prefix)
        {
            var tags = contextBuilder.GetTags();
            var iterationTags = contextBuilder.GetIterationTags();
            var allTextNodes = element.Descendants().OfType<HtmlTextNode>().ToList();
            var allCommentNodes = element.Descendants().OfType<HtmlCommentNode>().ToList();
            var allAttributes = element.Descendants().SelectMany(node => node.Attributes).ToList();

            foreach (var tag in tags.Keys)
            {
                var prefixedTag = prefix + tag;
                var regex = @"\{\{\s*" + prefixedTag + @"\s*\}\}";
                var tagValue = tags[tag];
                if (!string.IsNullOrWhiteSpace(tagValue))
                {
                    var value = tagValue.Split(" ")?.ElementAtOrDefault(0);
                    double parsedValue;
                    if (double.TryParse(value, out parsedValue) && parsedValue == 0)
                    {
                        tagValue = null;
                    }
                }

                foreach (var textNode in allTextNodes)
                {
                    textNode.Text = Regex.Replace(textNode.Text, regex, tagValue ?? "");
                }

                foreach (var commentNode in allCommentNodes)
                {
                    commentNode.Comment = Regex.Replace(commentNode.Comment, regex, tagValue ?? "");
                }

                foreach (var attribute in allAttributes)
                {
                    attribute.Value = Regex.Replace(attribute.Value, regex, tagValue ?? "");
                }
            }

            if (iterationTags != null && iterationTags.Any())
            {
                var ngRepeatNodes = element.SelectNodes("//*[@ng-repeat]");
                if (ngRepeatNodes == null)
                {
                    return;
                }

                // Process each ng-repeat node individually
                foreach (var ngRepeatNode in ngRepeatNodes)
                {
                    // First, clone the node for each iteration item
                    var clones = ParseNgRepeatContexts(ngRepeatNode, iterationTags, prefix);

                    // For each clone - which represents one item in your list
                    foreach (var clone in clones)
                    {
                        // Process conditionals WITHIN this specific clone
                        var conditionalNodesInClone = clone.SelectNodes(".//*[@condition]");
                        if (conditionalNodesInClone != null)
                        {
                            foreach (var conditionalNode in conditionalNodesInClone)
                            {
                                // Process conditional with the clone's context
                                ParseConditionalElements(conditionalNode, contextBuilder, prefix);
                            }
                        }
                    }

                    // After processing all clones, remove the original ng-repeat node
                    ngRepeatNode.Remove();
                }
            }

            // Process any remaining conditional nodes that are not inside ng-repeat blocks
            var remainingConditionalNodes = element.SelectNodes("//*[@condition]");
            if (remainingConditionalNodes != null)
            {
                foreach (var conditionalNode in remainingConditionalNodes)
                {
                    ParseConditionalElements(conditionalNode, contextBuilder, prefix);
                }
            }
        }

        private bool ParseConditionalElements(HtmlNode element, IEmailContextBuilder contextBuilder, string prefix)
        {
            bool removedElement = false;
            var conditionAttr = element.Attributes["condition"]?.Value;

            if (conditionAttr != null)
            {
                // Parse the condition (e.g., "service.Available == true")
                var conditionParts = conditionAttr.Split(new[] { "==" }, StringSplitOptions.None);
                if (conditionParts.Length == 2)
                {
                    var leftSide = conditionParts[0].Trim();
                    var rightSide = conditionParts[1].Trim();

                    // Extract the property path, handling nested properties
                    var propPath = leftSide.Split('.');

                    // Get tags from context
                    var tags = contextBuilder.GetTags();

                    // For template variables like {{service.Available}}, we need to use the correct key
                    // The prefix would be something like "service" in a loop
                    string propName;
                    if (propPath.Length > 1)
                    {
                        // Handle both direct properties and those with prefix
                        if (!string.IsNullOrEmpty(prefix) && propPath[0] == prefix)
                        {
                            // This is a prefixed property like service.Available
                            propName = string.Join(".", propPath);
                        }
                        else
                        {
                            // This might be a property without expected prefix
                            propName = string.Join(".", propPath);
                        }
                    }
                    else
                    {
                        propName = propPath[0];
                    }

                    // Try to get the value, check both with and without prefix for flexibVectarry
                    var objValue = tags.TryGetValue(propName.Split('.')[1], out var value) ? value : null;

                    // If not found and we have a prefix, try with a different combination
                    if (objValue == null && !string.IsNullOrEmpty(prefix))
                    {
                        string altPropName = $"{prefix}.{string.Join(".", propPath)}";
                        objValue = tags.TryGetValue(altPropName, out value) ? value : null;
                    }

                    // Handle comparison values
                    bool compareAsBool = false;
                    bool boolValue = false;

                    if (rightSide.Equals("true", StringComparison.OrdinalIgnoreCase))
                    {
                        compareAsBool = true;
                        boolValue = true;
                    }
                    else if (rightSide.Equals("false", StringComparison.OrdinalIgnoreCase))
                    {
                        compareAsBool = true;
                        boolValue = false;
                    }

                    // Evaluate the condition
                    bool conditionMet = false;
                    if (compareAsBool)
                    {
                        // Handle string representations of boolean values
                        conditionMet = objValue != null &&
                                      objValue.ToString().Equals(boolValue.ToString(), StringComparison.OrdinalIgnoreCase);
                    }
                    else
                    {
                        // String comparison
                        conditionMet = objValue != null && objValue.ToString() == rightSide;
                    }

                    if (!conditionMet)
                    {
                        // Condition failed, remove the element
                        element.Remove();
                        return true;
                    }

                    // Condition passed, keep the element but remove the condition attribute
                    element.Attributes["condition"].Remove();
                }
            }

            // Process child nodes recursively if this element wasn't removed
            if (!removedElement)
            {
                // Create a copy of the children collection to avoid modification issues during iteration
                var childNodes = element.ChildNodes.ToList();
                foreach (var childNode in childNodes)
                {
                    if (childNode.NodeType == HtmlNodeType.Element)
                    {
                        ParseConditionalElements(childNode, contextBuilder, prefix);
                    }
                }
            }

            return removedElement;
        }

        public string ReplaceTagsFromString(string text, IEmailContextBuilder contextBuilder)
        {
            var tags = contextBuilder.GetTags();
            var prefix = "";
            foreach (var tag in tags.Keys)
            {
                var prefixedTag = prefix + tag;
                var regex = @"\{\{\s*" + prefixedTag + @"\s*\}\}";
                text = Regex.Replace(text, regex, tags[tag] ?? "");
            }
            return text;
        }

        /// <summary>
        /// Parses the ng-repeat tag with the iteration contexts
        /// </summary>
        /// <param name="element">The element where the repeat attribute is applied</param>
        /// <param name="iterationTags">The initial iteration tags given by the context</param>
        /// <param name="prefix">The prefix before the tag. For example, orderItem.Value, orderItem. is the prefix</param>
        /// <returns>True if a tag was effectively processed, false otherwise</returns>
        private List<HtmlNode> ParseNgRepeatContexts(HtmlNode element, Dictionary<string, List<IEmailContextBuilder>> iterationTags, string prefix)
        {
            var clonedNodes = new List<HtmlNode>();
            var ngRepeatTag = element.Attributes["ng-repeat"].Value;
            var regex = @"(?<name>.+)\s+in\s+(?<list>.+)";
            var match = Regex.Match(ngRepeatTag, regex);

            if (match.Success)
            {
                var list = match.Groups["list"].Value;
                var name = match.Groups["name"].Value;

                if (iterationTags.ContainsKey(list))
                {
                    var contexts = iterationTags[list];
                    // Use normal order instead of reverse to maintain original sequence
                    foreach (var context in contexts)
                    {
                        var newElement = element.CloneNode(true);
                        newElement.Attributes["ng-repeat"].Remove();
                        element.ParentNode.InsertAfter(newElement, element);
                        clonedNodes.Add(newElement);

                        // Find all placeholders with dot notation
                        var tags = GetAllTagsWithPrefix(newElement, name + ".");
                        foreach (var tag in tags)
                        {
                            var propName = tag.Substring((name + ".").Length);
                            ProcessNodeWithProperty(newElement, context, propName, tag);
                        }

                        // Process conditional elements for this specific clone immediately
                        var conditionalNodes = newElement.SelectNodes(".//*[@condition]");
                        if (conditionalNodes != null)
                        {
                            foreach (var conditionalNode in conditionalNodes)
                            {
                                ParseConditionalElements(conditionalNode, context, name);
                            }
                        }
                    }

                    return clonedNodes;
                }
            }

            return clonedNodes;
        }

        private List<string> GetAllTagsWithPrefix(HtmlNode node, string prefix)
        {
            var tags = new List<string>();
            var regex = new Regex(@"{{([^}]+)}}");
            var matches = regex.Matches(node.InnerHtml);

            foreach (Match match in matches)
            {
                var tag = match.Groups[1].Value.Trim();
                if (tag.StartsWith(prefix))
                {
                    tags.Add(tag);
                }
            }

            return tags;
        }

        private void ProcessNodeWithProperty(HtmlNode node, IEmailContextBuilder context, string propName, string fullTag)
        {
            // Get the context's tags
            var tags = context.GetTags();

            // Find the tag value (you might need to adapt this based on your system)
            if (tags.TryGetValue(propName, out var value))
            {
                // Replace the tag in the HTML
                node.InnerHtml = node.InnerHtml.Replace("{{" + fullTag + "}}", value);
            }
        }
    }
}
