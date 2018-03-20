using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Collections.Specialized;
using System.Text.RegularExpressions;

namespace MyUrlRewrite
{
// 重写的上下文，保存重写的环境参数 
public class RewriteContext
{     
    public static RewriteContext Current
    {
        get
        {
            HttpContext context = HttpContext.Current;
            if (context.Items.Contains("RewriteContext"))
                return context.Items["RewriteContext"] as RewriteContext;
            else
                return new RewriteContext();
        }
    }
    public RewriteContext()
    {
        parameters = new NameValueCollection();
        this.url = String.Empty;
    }
    public RewriteContext(NameValueCollection parameters, string url)
    {
        this.url = url;
        this.parameters = new NameValueCollection(parameters);
    }
    private NameValueCollection parameters;
    public NameValueCollection Parameters
    {
        get { return parameters; }
        set { parameters = value; }
    }
    private string url;
    public string Url
    { get { return url; } set { url = value; } }
}

// 完成重写的  Module
public class RewriteModule : System.Web.IHttpModule
{

    #region IHttpModule Members

    public void Dispose() { }

    public void Init(System.Web.HttpApplication application)
    {
        application.BeginRequest += new EventHandler(Application_BeginRequest);
        application.PreRequestHandlerExecute += new EventHandler(Application_PreRequestHandlerExecute);
    }


    void Application_BeginRequest(object sender, EventArgs e)
    {
        if (!MyUrlRewrite.Enabled)
            return;

        HttpContext context = HttpContext.Current;

        // 取得当前请求的路径
        string path = context.Request.Path;

        // 遍历所有的映射规则，进行映射处理 
        foreach (MyUrlRewriteConfigRule rule in MyUrlRewrite.Rules)
        {
            Regex regex = new Regex(MyUrlRewrite.RewriteBase + rule.Source, RegexOptions.IgnoreCase);
            Match match = regex.Match(path);
            if (match.Success)
            {
                // 映射
                string newPath = regex.Replace(path, rule.Destination);

                if (context.Request.QueryString.Count != 0) 
                {     
                    string sign = (path.IndexOf('?') == -1) ? "?" : "&";               
                    newPath = newPath + sign +
                        context.Request.QueryString.ToString();            
                }

                // 为了在页面中正确生成 PostBack 地址
                // 保存原来的请求信息
                context.Items.Add("OriginalUrl", context.Request.RawUrl);

                newPath = MyUrlRewrite.RewriteBase + newPath;

                // 重写请求地址 
                context.RewritePath(newPath, MyUrlRewrite.RebaseClientPath ); 
                return;
            }
        }
    }

    void Application_PreRequestHandlerExecute(object sender, EventArgs e)
    {
        HttpContext context = HttpContext.Current;
        if (context.CurrentHandler is Page)
        {
            Page page = context.CurrentHandler as Page;
            page.PreInit += new EventHandler(Page_PreInit);
        }
    }

    // 为了保证页面在生成  form 地址的时候，正确生成 
    // 重新将原来的地址设置回去 
    void Page_PreInit(object sender, EventArgs e)
    {
        HttpContext context = HttpContext.Current;
        if (context.Items.Contains("OriginalUrl"))
        {
            string path = context.Items["OriginalUrl"] as string;
            RewriteContext con = new RewriteContext(context.Request.QueryString, path);
            context.Items["RewriteContext"] = con;
            if (path.IndexOf("?") == -1)
               path += "?";
            //context.Items["wbq3311"] = context.Request.QueryString["Folder"];
            context.RewritePath(path);
        }
    }


    #endregion
}
}
