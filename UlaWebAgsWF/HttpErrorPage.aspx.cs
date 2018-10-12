using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace UlaWebAgsWF
{
    public partial class HttpErrorPage : System.Web.UI.Page
    {
        protected HttpException ex = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            ex = (HttpException)Server.GetLastError();

            if (ex.GetHttpCode() >= 400 && ex.GetHttpCode() < 500)
            {
                ex = new HttpException(ex.GetHttpCode(), "Safe message for 4xx HTTP codes.", ex);
            }
            else if (ex.GetHttpCode() > 499)
            {
                ex = new HttpException(ex.GetHttpCode(), "Safe message for 5xx HTTP codes.", ex);
            }
            else
                ex = new HttpException(ex.GetHttpCode(), "Safe messages for unexpected HTTP codes", ex);

            exMessage.Text = ex.Message;
            exTrace.Text = ex.StackTrace;

            if (ex.InnerException != null)
            {
                innerTrace.Text = ex.InnerException.StackTrace;
                InnerErrorPanel.Visible = Request.IsLocal;
                innerMessage.Text = string.Format("HTTP {0} : {1}", ex.GetHttpCode(), ex.InnerException.StackTrace);
            }

            exTrace.Visible = Request.IsLocal;
        }
    }
}