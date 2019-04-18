using System;
using System.IO;
using Markdig;
using Markdig.SyntaxHighlighting;


namespace mdHttpHandler
{
    public partial class md_base : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public string ProcessMarkdownFile(string fName)
        {
            // Markdig.Markdown.ToHtml()
            var pipeline = new Markdig.MarkdownPipelineBuilder()
                .UseAdvancedExtensions()
                .UseSyntaxHighlighting()
                .Build();
            string md = File.ReadAllText(fName);

            return Markdown.ToHtml(md, pipeline);
        }
    }
}