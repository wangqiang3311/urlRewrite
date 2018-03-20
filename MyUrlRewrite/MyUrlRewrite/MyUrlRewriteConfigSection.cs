using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Web;

namespace MyUrlRewrite
{
// Url 重写的规则
public class MyUrlRewriteConfigRule : System.Configuration.ConfigurationElement
{
    private static readonly ConfigurationProperty _propSource =
        new ConfigurationProperty("source", typeof(string), "", ConfigurationPropertyOptions.IsRequired);
    private static readonly ConfigurationProperty _propDestination =
        new ConfigurationProperty("destination", typeof(string), "", ConfigurationPropertyOptions.IsRequired);

    public MyUrlRewriteConfigRule()
    {
        this.Properties.Add(_propSource);
        this.Properties.Add(_propDestination);
    }

    [ConfigurationProperty("source")]
    public string Source
    {
        get
        {
            return (string)base[_propSource];
        }
        set
        {
            base[_propSource] = value;
        }
    }

    [ConfigurationProperty("destination")]
    public string Destination
    {
        get
        {
            return (string)base[_propDestination];
        }
        set
        {
            base[_propDestination] = value;
        }
    }
}


// 规则的集合
[System.Configuration.ConfigurationCollection(
    typeof(MyUrlRewriteConfigRule),
    AddItemName="rule",
    CollectionType = ConfigurationElementCollectionType.BasicMap
    )]
public class MyUrlRewriteConfigRuleCollection
    : System.Configuration.ConfigurationElementCollection
{
    protected override ConfigurationElement CreateNewElement()
    {
        return new MyUrlRewriteConfigRule();
    }

    protected override object GetElementKey(ConfigurationElement element)
    {
        MyUrlRewriteConfigRule rule = (MyUrlRewriteConfigRule)element;
        return rule.Source;
    }

    // 元素的名称
    protected override string ElementName
    {
        get
        {
            return "rule";
        }
    }

    public MyUrlRewriteConfigRule this[int index]
    {
        get
        {
            return (MyUrlRewriteConfigRule)BaseGet(index);
        }
        set
        {
            BaseAdd(index, value);
        }
    } 
}


public class MyUrlRewriteConfigSection
    : System.Configuration.ConfigurationSection
{
    // ConfigurationProperty 表示配置元素中的一个属性
    // name 表示配置文件中属性的名字
    // 类型 表示映射在 .NET 中的数据类型
    // 默认值表示没有设置的情况下的默认值
    // 配置选项有四种：
    // IsDefaultCollection 数据默认集合中
    // IsKey 必须是唯一的
    // IsRequired 必须设置
    // None 该属性没有特殊要求
    private static readonly ConfigurationProperty _propEnabled =
        new ConfigurationProperty("enabled", typeof(bool), false, ConfigurationPropertyOptions.None);

    private static readonly ConfigurationProperty _propRebaseClientPath =
        new ConfigurationProperty("rebaseClientPath", typeof(bool), false, ConfigurationPropertyOptions.None);


    private static readonly ConfigurationProperty _propRules =
                new ConfigurationProperty("", typeof(MyUrlRewriteConfigRuleCollection), null, ConfigurationPropertyOptions.IsDefaultCollection);

    [ConfigurationProperty("enabled", DefaultValue = false)]
    public bool Enabled
    {
        get
        {
            return (bool)base[_propEnabled];
        }
        set
        {
            base[_propEnabled] = value;
        }
    }

    [ConfigurationProperty("rebaseClientPath", DefaultValue = false)]
    public bool RebaseClientPath
    {
        get
        {
            return (bool)base[_propRebaseClientPath];
        }
        set
        {
            base[_propRebaseClientPath] = value;
        }
    }

    // 嵌入的集合
    [ConfigurationProperty("", IsDefaultCollection = true, Options = ConfigurationPropertyOptions.IsDefaultCollection)]
    public MyUrlRewriteConfigRuleCollection Rules
    {
        get
        {
            return (MyUrlRewriteConfigRuleCollection)base[_propRules];
        }
        set
        {
            base[_propRules] = value;
        }
    }

    // 构造函数
    public MyUrlRewriteConfigSection()
    {
        this.Properties.Add(_propEnabled);
        this.Properties.Add(_propRebaseClientPath);
        this.Properties.Add(_propRules);
    }

}

public static class MyUrlRewrite
{
    public static bool Enabled
    {
        get
        {
            MyUrlRewriteConfigSection section
                = System.Web.Configuration.WebConfigurationManager.GetSection("myUrl")
                as MyUrlRewriteConfigSection;
            if (section == null)
            {
                return false;
            }
            return section.Enabled;
        }
    }


    public static bool RebaseClientPath
    {
        get
        {
            MyUrlRewriteConfigSection section
                = System.Web.Configuration.WebConfigurationManager.GetSection("myUrl")
                as MyUrlRewriteConfigSection;
            if (section == null)
            {
                return false;
            }
            return section.RebaseClientPath;
        }
    }

    public static MyUrlRewriteConfigRuleCollection Rules
    {
        get
        {
            MyUrlRewriteConfigSection section
                = System.Web.Configuration.WebConfigurationManager.GetSection("myUrl")
                as MyUrlRewriteConfigSection;
            if (section == null)
            {
                throw new System.InvalidOperationException("未能获取 myUrl 节！");
            }
            return section.Rules; ;
        }
    }

    public static string RewriteBase
    {
        get
        {
            HttpContext context = HttpContext.Current;
            string path = HttpContext.Current.Request.ApplicationPath;
            if (!path.EndsWith("/"))
            {
                return path + "/";
            }

            return path;
        }
    }

}
}
