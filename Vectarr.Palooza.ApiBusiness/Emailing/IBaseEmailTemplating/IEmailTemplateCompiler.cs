using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vectarr.Palooza.ApiBusiness.Emailing.IBaseEmailTemplating
{
    public interface IEmailTemplateCompiler
    {
        string CompileTemplate(string template, IEmailContextBuilder contextBuilder);

        string ReplaceTagsFromString(string text, IEmailContextBuilder contextBuilder);
    }
}
