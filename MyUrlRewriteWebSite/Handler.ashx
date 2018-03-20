<%@ WebHandler Language="C#" Class="Handler" %>

using System;
using System.Web;

using System.Configuration;
using System.Web.Configuration;

public class Handler : IHttpHandler {
    
    public void ProcessRequest (HttpContext context) {
        context.Response.ContentType = "text/html";
                
        MyUrlRewrite.MyUrlRewriteConfigSection section
            = System.Web.Configuration.WebConfigurationManager.GetSection("myUrl")
            as MyUrlRewrite.MyUrlRewriteConfigSection;

        MyUrlRewrite.MyUrlRewriteConfigRule rule = section.Rules[0];
        string source = rule.Source;
        string destination = rule.Destination;
        
        context.Response.Write(section.RebaseClientPath);
        context.Response.Write("<br/>");
        context.Response.Write("Rules: " + section.Rules.Count);
        context.Response.Write("<br/>");
        context.Response.Write(source + "&nbsp;" + destination);
    }
 
    public bool IsReusable {
        get {
            return false;
        }
    }

}