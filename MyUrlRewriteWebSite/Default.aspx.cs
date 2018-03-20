using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class _Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        this.Label1.Text = HttpContext.Current.Items["wbq3311"] == null ? "" : HttpContext.Current.Items["wbq3311"].ToString();

        var rewriteContext = HttpContext.Current.Items["RewriteContext"] as MyUrlRewrite.RewriteContext;

        this.Label1.Text = rewriteContext.Parameters["Folder"];

        var msg = Request.QueryString["Folder"];
    }
    protected void LinkButton1_Click(object sender, EventArgs e)
    {
        this.Label1.Text = this.TextBox1.Text;
        this.TextBox1.Text = string.Empty;
    }
}