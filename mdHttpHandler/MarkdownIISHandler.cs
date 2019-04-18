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
            //write your handler implementation here.
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

                // the 3rd parameter must be just the file name.
                // the 4th parameter should be the physical path to the file, though it also
                //   works fine if you pass an empty string - perhaps that's only to override
                //   the usual presentation based on the path?

                var handler = factory.GetHandler(context, "GET", newTarget,
                    context.Request.MapPath(newTarget));

                // Update the context object as it should appear to your page/app, and
                // assign your new handler.
                // context.RewritePath(newTarget, "", queryString);
                context.Handler = handler;

                // .. and done

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
