
using Constellation;
using System;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml;

namespace Constellation.Utils
{
    internal static class XmlDocumentationReader
    {
        private static readonly Regex crefRegex = new Regex("<[^>]+\\scref\\b=\"([^\"\\s*/>]*)\"\\s*/>", RegexOptions.Compiled);

        internal static void AddEnumXmlComment(string enumValue, Type enumType, PackageDescriptor.MemberDescriptor descriptor)
        {
            XmlNode xmlCommentNode = GetXmlCommentNode(enumType, "F:" + enumType.FullName.Replace("+", ".") + "." + enumValue);
            if ((xmlCommentNode != null) && string.IsNullOrEmpty(descriptor.Description))
            {
                descriptor.Description = GetDescriptionText(xmlCommentNode.SelectSingleNode("summary").InnerXml);
            }
        }

        internal static void AddXmlComment(MethodInfo method, PackageDescriptor.MessageCallbackDescriptor descriptor)
        {
            XmlNode xmlCommentNode = GetXmlCommentNode(method.DeclaringType, "M:" + method.DeclaringType.FullName.Replace("+", ".") + "." + method.Name);
            if (xmlCommentNode != null)
            {
                if (string.IsNullOrEmpty(descriptor.Description))
                {
                    descriptor.Description = GetDescriptionText(xmlCommentNode.SelectSingleNode("summary").InnerXml);
                }
                foreach (PackageDescriptor.MemberDescriptor descriptor2 in descriptor.Parameters)
                {
                    XmlNode node2 = xmlCommentNode.SelectSingleNode("//param[@name='" + descriptor2.Name + "']");
                    if ((node2 != null) && (node2.InnerText != null))
                    {
                        descriptor2.Description = GetDescriptionText(node2.InnerText);
                    }
                }
            }
        }

        internal static void AddXmlComment(PropertyInfo property, PackageDescriptor.MemberDescriptor descriptor)
        {
            XmlNode xmlCommentNode = GetXmlCommentNode(property.DeclaringType, "P:" + property.DeclaringType.FullName.Replace("+", ".") + "." + property.Name);
            if ((xmlCommentNode != null) && string.IsNullOrEmpty(descriptor.Description))
            {
                descriptor.Description = GetDescriptionText(xmlCommentNode.SelectSingleNode("summary").InnerXml);
            }
        }

        internal static void AddXmlComment(Type type, PackageDescriptor.TypeDescriptor descriptor)
        {
            XmlNode xmlCommentNode = GetXmlCommentNode(type, "T:" + type.FullName.Replace("+", "."));
            if ((xmlCommentNode != null) && string.IsNullOrEmpty(descriptor.Description))
            {
                descriptor.Description = GetDescriptionText(xmlCommentNode.SelectSingleNode("summary").InnerXml);
            }
        }

        private static string GetDescriptionText(string xml) =>
            StripTagsCharArray(crefRegex.Replace(xml, "$1"));

        private static XmlNode GetXmlCommentNode(Type typeSource, string path)
        {
            try
            {
                string location = typeSource.Assembly.Location;
                string str2 = location.Substring(0, location.LastIndexOf(".")) + ".XML";
                if (File.Exists(str2))
                {
                    XmlDocument document1 = new XmlDocument();
                    document1.Load(str2);
                    return document1.SelectSingleNode("//member[starts-with(@name, '" + path + "')]");
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        private static string StripTagsCharArray(string source)
        {
            char[] chArray = new char[source.Length];
            int index = 0;
            bool flag = false;
            for (int i = 0; i < source.Length; i++)
            {
                char ch = source[i];
                switch (ch)
                {
                    case '<':
                        flag = true;
                        break;

                    case '>':
                        flag = false;
                        break;

                    default:
                        if (!flag)
                        {
                            chArray[index] = ch;
                            index++;
                        }
                        break;
                }
            }
            return new string(chArray, 0, index).Trim();
        }
    }
}
