# How To Create This Project

## Steps:

1. Create Web Forms Project in Visual Studio 2017
1. Add NuGet Projects:
   - Markdig
   - Markdig.SyntaxHighlighting
1. Create A HttmpHandler Class: (See Annex: MarkdownIISHandler)
1. Modify the `web.config` to have *.md and *.markdown extensions.  (See Annex: webconfig)
1. Create an aspx file to host the Markdown (as Html) contents (See Annex: md_base)

## Test:

Create a file in the project directory with a *.md extension.  Start The application and then navigate to that file in the browser.



### Annex: MarkdownIISHandler.cs

```cs
using System;
using System.Web;
using System.Web.UI;
using System.IO;

namespace mdHttpHandler
{
    public class MarkdownIISHandler : IHttpHandler
    {
        /// <summary>
        /// You will need to configure this handler in the Web.config file of your 
        /// web and register it with IIS before being able to use it. For more information
        /// see the following link: https://go.microsoft.com/?linkid=8101007
        /// </summary>
        #region IHttpHandler Members

        public bool IsReusable
        {
            // Return false in case your Managed Handler cannot be reused for another request.
            // Usually this would be false in case you have some state information preserved per request.
            get { return true; }
        }

        public void ProcessRequest(HttpContext context)
        {
            // write other handler implementation here.
            // ProcessRequest1(context);
            ProcessRequest2(context);
        }

        // hello world - works: displays raw contents of context.Request.PhysicalPath
        public void ProcessRequest1(HttpContext context)
        {
            context.Response.Write("<!DOCTYPE html>");
            context.Response.Write("<html>");
            context.Response.Write("<pre id=\"md\">");
            context.Response.Write(File.ReadAllText(context.Request.PhysicalPath));
            context.Response.Write("</pre>");
            context.Response.Write("</html>");
        }

        /// <summary />
        /// <remarks>https://stackoverflow.com/questions/9930880/call-the-default-asp-net-httphandler-from-a-custom-handler</remarks>
        public void ProcessRequest2(HttpContext context)
        {
            if (File.Exists(context.Request.MapPath(context.Request.FilePath)))
            {
                // the internal constructor doesn't do anything but prevent you from instantiating
                // the factory, so we can skip it.
                PageHandlerFactory factory =
                    (PageHandlerFactory)System.Runtime.Serialization.FormatterServices
                    .GetUninitializedObject(typeof(System.Web.UI.PageHandlerFactory));

                string newTarget = "~/md_base.aspx";
                string queryString = context.Request.QueryString.ToString();

                var handler = factory.GetHandler(context, "GET", newTarget,
                    context.Request.MapPath(newTarget));

                // Update the context object as it should appear to your page/app, and
                // assign your new handler.
                // Uncomment out this line if you don't want to see the /filename.md in the url
                // context.RewritePath(newTarget, "", queryString);
                context.Handler = handler;

                handler.ProcessRequest(context);
            }
            else
            {
                context.Response.StatusCode = 404;
                context.Response.SuppressContent = true;
            }
        }

        #endregion
    }
}
```

---

### Annex: web.config

```xml
<system.webServer>
  <handlers>
    <add name="MdHandler" path="*.md" verb="*" type="mdHttpHandler.MarkdownIISHandler" resourceType="Unspecified" requireAccess="Read" preCondition="integratedMode" />
    <add name="MarkdownHandler" path="*.markdown" verb="*" type="mdHttpHandler.MarkdownIISHandler" resourceType="Unspecified" requireAccess="Read" preCondition="integratedMode" />
  </handlers>
</system.webServer>
```

---

### Annex: md_base.aspx

This aspx file uses the default Site.Master masterpage file.  Doesn't have to, but here it does.

```xml
<%@ Page Title="MD_Base" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="md_base.aspx.cs" Inherits="mdHttpHandler.md_base" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2><%: Title %></h2>
    <p>Current Time: <%=DateTime.Now.ToLongTimeString() %></p>
    <p>FilePath: <%=Request.FilePath %></p>
    <%=ProcessMarkdownFile(Server.MapPath(Request.FilePath)) %>
</asp:Content>
```

---

### Annex: md_base.cs

```cs
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
```
eof
