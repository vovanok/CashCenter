using System;
using System.Collections;
using System.Configuration;
using System.Collections.Specialized;
using System.Xml;
using System.IO;
using System.Reflection;

namespace CashCenter.Common
{
    public sealed class PortableSettingsProvider : SettingsProvider, IApplicationSettingsProvider
    {
        private const string ROOT_NODE_NAME = "settings";
        private const string LOCAL_SETTINGS_NODE_NAME = "localSettings";
        private const string GLOBAL_SETTINGS_NODE_NAME = "globalSettings";
        private const string CLASS_NAME = "PortableSettingsProvider";

        private XmlDocument xmlDocument;

        private string FilePath
        {
            get
            {
                return Path.Combine(
                    Path.GetDirectoryName(Assembly.GetEntryAssembly().Location),
                    $"{ApplicationName}.settings");
            }
        }

        private XmlNode LocalSettingsNode
        {
            get
            {
                XmlNode settingsNode = GetSettingsNode(LOCAL_SETTINGS_NODE_NAME);
                XmlNode machineNode = settingsNode.SelectSingleNode(Environment.MachineName.ToLowerInvariant());

                if (machineNode == null)
                {
                    machineNode = RootDocument.CreateElement(Environment.MachineName.ToLowerInvariant());
                    settingsNode.AppendChild(machineNode);
                }

                return machineNode;
            }
        }

        private XmlNode GlobalSettingsNode
        {
            get { return GetSettingsNode(GLOBAL_SETTINGS_NODE_NAME); }
        }

        private XmlNode RootNode
        {
            get { return RootDocument.SelectSingleNode(ROOT_NODE_NAME); }
        }

        private XmlDocument RootDocument
        {
            get
            {
                if (xmlDocument != null)
                    return xmlDocument;

                try
                {
                    xmlDocument = new XmlDocument();
                    xmlDocument.Load(FilePath);
                }
                catch { }

                if (xmlDocument.SelectSingleNode(ROOT_NODE_NAME) != null)
                    return xmlDocument;

                xmlDocument = GetBlankXmlDocument();
                return xmlDocument;
            }
        }

        public override string ApplicationName
        {
            get { return Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly().Location); }
            set { }
        }

        public override string Name
        {
            get { return CLASS_NAME; }
        }

        public override void Initialize(string name, NameValueCollection config)
        {
            base.Initialize(Name, config);
        }

        public override void SetPropertyValues(SettingsContext context, SettingsPropertyValueCollection collection)
        {
            foreach (SettingsPropertyValue propertyValue in collection)
                SetValue(propertyValue);

            try
            {
                RootDocument.Save(FilePath);
            }
            catch (Exception)
            {
                /* 
                 * If this is a portable application and the device has been 
                 * removed then this will fail, so don't do anything. It's 
                 * probably better for the application to stop saving settings 
                 * rather than just crashing outright. Probably.
                 */
            }
        }

        public override SettingsPropertyValueCollection GetPropertyValues(SettingsContext context, SettingsPropertyCollection collection)
        {
            var values = new SettingsPropertyValueCollection();

            foreach (SettingsProperty property in collection)
            {
                values.Add(
                    new SettingsPropertyValue(property)
                    {
                        SerializedValue = GetValue(property)
                    });
            }

            return values;
        }

        private void SetValue(SettingsPropertyValue propertyValue)
        {
            XmlNode targetNode = IsGlobal(propertyValue.Property)
               ? GlobalSettingsNode
               : LocalSettingsNode;

            XmlNode settingNode = targetNode.SelectSingleNode(string.Format("setting[@name='{0}']", propertyValue.Name));

            if (settingNode != null)
            {
                settingNode.InnerText = propertyValue.SerializedValue.ToString();
            }
            else
            {
                settingNode = RootDocument.CreateElement("setting");

                XmlAttribute nameAttribute = RootDocument.CreateAttribute("name");
                nameAttribute.Value = propertyValue.Name;

                settingNode.Attributes.Append(nameAttribute);
                settingNode.InnerText = propertyValue.SerializedValue.ToString();

                targetNode.AppendChild(settingNode);
            }
        }

        private string GetValue(SettingsProperty property)
        {
            XmlNode targetNode = IsGlobal(property) ? GlobalSettingsNode : LocalSettingsNode;
            XmlNode settingNode = targetNode.SelectSingleNode(string.Format("setting[@name='{0}']", property.Name));

            if (settingNode == null)
                return property.DefaultValue != null ? property.DefaultValue.ToString() : string.Empty;

            return settingNode.InnerText;
        }

        private bool IsGlobal(SettingsProperty property)
        {
            foreach (DictionaryEntry attribute in property.Attributes)
            {
                if ((Attribute)attribute.Value is SettingsManageabilityAttribute)
                    return true;
            }

            return false;
        }

        private XmlNode GetSettingsNode(string name)
        {
            XmlNode settingsNode = RootNode.SelectSingleNode(name);

            if (settingsNode == null)
            {
                settingsNode = RootDocument.CreateElement(name);
                RootNode.AppendChild(settingsNode);
            }

            return settingsNode;
        }

        public XmlDocument GetBlankXmlDocument()
        {
            XmlDocument blankXmlDocument = new XmlDocument();
            blankXmlDocument.AppendChild(blankXmlDocument.CreateXmlDeclaration("1.0", "utf-8", string.Empty));
            blankXmlDocument.AppendChild(blankXmlDocument.CreateElement(ROOT_NODE_NAME));

            return blankXmlDocument;
        }

        public void Reset(SettingsContext context)
        {
            LocalSettingsNode.RemoveAll();
            GlobalSettingsNode.RemoveAll();

            xmlDocument.Save(FilePath);
        }

        public SettingsPropertyValue GetPreviousVersion(SettingsContext context, SettingsProperty property)
        {
            return new SettingsPropertyValue(property);
        }

        public void Upgrade(SettingsContext context, SettingsPropertyCollection properties)
        {
        }
    }
}