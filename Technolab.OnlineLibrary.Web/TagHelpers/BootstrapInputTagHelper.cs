using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Technolab.OnlineLibrary.Web.TagHelpers
{
    [HtmlTargetElement("input", Attributes = "asp-for", TagStructure = TagStructure.WithoutEndTag)]
    public class BootstrapInputTagHelper : InputTagHelper
    {
        public BootstrapInputTagHelper(IHtmlGenerator generator) : base(generator)
        {
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            base.Process(context, output);

            if (ViewContext.ViewData.ModelState.TryGetValue(For.Name, out var entry) && entry.Errors.Count > 0)
            {
                output.AddClass("is-invalid", HtmlEncoder.Default);
            }
        }
    }
}
