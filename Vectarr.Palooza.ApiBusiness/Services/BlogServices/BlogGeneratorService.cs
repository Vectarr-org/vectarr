using System.Text.Json;
using OpenAI;
using OpenAI.Chat;
using OpenAI.Images;
using Vectarr.Palooza.Classes.Api;
using Vectarr.Palooza.DataAccess;
using Vectarr.Palooza.ApiBusiness.IServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Vectarr.Palooza.ApiBusiness.Services.BlogServices
{
    public class BlogGeneratorService
    {
        private readonly IConfiguration _configuration;
        private readonly IImageManager _imageManager;
        private readonly ILogger<BlogGeneratorService> _logger;

        public BlogGeneratorService(
            IConfiguration configuration,
            IImageManager imageManager,
            ILogger<BlogGeneratorService> logger)
        {
            _configuration = configuration;
            _imageManager = imageManager;
            _logger = logger;
        }

        public async Task GenerateDailyBlogs()
        {
            _logger.LogInformation("Starting daily blog generation...");

            try
            {
                for (int i = 0; i < 2; i++)
                {
                    await GenerateAndSaveBlog();
                }

                _logger.LogInformation("Daily blog generation completed successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during daily blog generation.");
                throw;
            }
        }

        private async Task GenerateAndSaveBlog()
        {
            var apiKey = _configuration["OpenAI:ApiKey"];
            var client = new OpenAIClient(apiKey);
            var chatClient = client.GetChatClient("gpt-4o");

            // Step 1: Generate blog content via ChatGPT
            var blogJson = await GenerateBlogContent(chatClient);

            // Step 2: Generate cover image via DALL-E
            var imageModel = await GenerateCoverImage(client, blogJson.Title, blogJson.Category);

            // Step 3: Save to database
            SaveBlogToDatabase(blogJson, imageModel);
        }

        private async Task<BlogGenerationResult> GenerateBlogContent(ChatClient chatClient)
        {
            var systemPrompt = @"You are an expert SEO blog writer for Vectarr, a manufacturing and 3D printing marketplace platform.
Write high-quality, SEO-optimized blog posts about topics relevant to manufacturing, 3D printing, CNC machining,
prototyping, product design, injection molding, laser cutting, and related industries.

You must respond with ONLY valid JSON (no markdown, no code fences) in this exact format:
{
    ""title"": ""Blog Post Title"",
    ""slug"": ""blog-post-title"",
    ""excerpt"": ""A short 1-2 sentence summary for listing cards (max 200 chars)"",
    ""description"": ""SEO meta description for the full article (max 300 chars)"",
    ""category"": ""One of: Manufacturing, 3D Printing, CNC Machining, Design, Prototyping, Materials, Industry News"",
    ""readTimeMinutes"": 8,
    ""keywords"": [""keyword1"", ""keyword2"", ""keyword3"", ""keyword4"", ""keyword5""],
    ""sections"": [
        {
            ""heading"": null,
            ""content"": ""Opening paragraph without a heading..."",
            ""order"": 0
        },
        {
            ""heading"": ""Section Heading"",
            ""content"": ""Section content with <strong>inline HTML</strong> allowed..."",
            ""order"": 1
        }
    ]
}

Requirements:
- Title should be compelling and SEO-friendly (50-70 chars)
- Generate 4-6 sections per blog post
- First section has null heading (intro paragraph)
- Include 4-6 SEO keywords
- Content should be informative, original, and at least 800 words total
- Use inline HTML tags like <strong>, <em>, <a> where appropriate
- Slug must be URL-friendly (lowercase, hyphens, no special chars)
- Pick a DIFFERENT topic each time - vary between the categories";

            var userPrompt = $"Write a new blog post. Today's date is {DateTime.UtcNow:yyyy-MM-dd}. Pick a fresh, trending topic in the manufacturing/3D printing space that would attract organic search traffic.";

            var completion = await chatClient.CompleteChatAsync(
                new ChatMessage[]
                {
                    new SystemChatMessage(systemPrompt),
                    new UserChatMessage(userPrompt)
                },
                new ChatCompletionOptions
                {
                    Temperature = 0.9f,
                    MaxOutputTokenCount = 4000
                });

            var responseText = completion.Value.Content[0].Text;

            var result = JsonSerializer.Deserialize<BlogGenerationResult>(responseText, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (result == null)
                throw new Exception("Failed to deserialize blog content from OpenAI response.");

            return result;
        }

        private async Task<ImageModel> GenerateCoverImage(OpenAIClient client, string title, string category)
        {
            var imageClient = client.GetImageClient("dall-e-3");

            var imagePrompt = $"Create a professional, modern blog cover image for an article titled \"{title}\" " +
                              $"in the {category} industry. The image should be clean, visually appealing, " +
                              "suitable for a technology/manufacturing blog. No text in the image. " +
                              "Use a modern, minimalist style with subtle gradients and professional colors.";

            var imageResult = await imageClient.GenerateImageAsync(imagePrompt, new ImageGenerationOptions
            {
                Size = GeneratedImageSize.W1792xH1024,
                Quality = GeneratedImageQuality.Standard,
                ResponseFormat = GeneratedImageFormat.Bytes
            });

            var imageBytes = imageResult.Value.ImageBytes;
            var fileName = $"blog-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid():N}.png";

            using var stream = new MemoryStream(imageBytes.ToArray());
            var imageModel = await _imageManager.UploadImage(stream, fileName, "", alt: title);

            return imageModel;
        }

        private void SaveBlogToDatabase(BlogGenerationResult blogJson, ImageModel imageModel)
        {
            var unitOfWork = new UnitOfWork();

            var blogModel = new BlogModel
            {
                Slug = blogJson.Slug,
                Title = blogJson.Title,
                Excerpt = blogJson.Excerpt,
                Description = blogJson.Description,
                ImageId = imageModel.Id,
                Category = blogJson.Category,
                ReadTimeMinutes = blogJson.ReadTimeMinutes,
                PublishedAt = DateTime.UtcNow
            };

            var savedBlog = unitOfWork.Repositories.BlogRepository.Insert(blogModel);

            // Save sections
            foreach (var section in blogJson.Sections)
            {
                var sectionModel = new BlogSectionModel
                {
                    BlogId = savedBlog.Id,
                    Heading = section.Heading,
                    Content = section.Content,
                    Order = section.Order
                };
                unitOfWork.Repositories.BlogSectionRepository.Insert(sectionModel);
            }

            // Save keywords
            foreach (var keyword in blogJson.Keywords)
            {
                var keywordModel = new BlogKeywordModel
                {
                    BlogId = savedBlog.Id,
                    Keyword = keyword
                };
                unitOfWork.Repositories.BlogKeywordRepository.Insert(keywordModel);
            }
        }
    }

    public class BlogGenerationResult
    {
        public string Title { get; set; } = null!;
        public string Slug { get; set; } = null!;
        public string Excerpt { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Category { get; set; } = null!;
        public int ReadTimeMinutes { get; set; }
        public List<string> Keywords { get; set; } = [];
        public List<BlogSectionGenerationResult> Sections { get; set; } = [];
    }

    public class BlogSectionGenerationResult
    {
        public string? Heading { get; set; }
        public string Content { get; set; } = null!;
        public int Order { get; set; }
    }
}
