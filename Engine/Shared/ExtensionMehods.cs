using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Engine.Shared
{
    public static class ExtensionMehods
    {
        public static int AttributeAsInt(this XmlNode node, string attributeName)
        {
            return Convert.ToInt32(AttributeAsString(node, attributeName));
        }

        public static string AttributeAsString(this XmlNode node, string attributeName)
        {
            XmlAttribute attribute = node.Attributes?[attributeName];

            if (attribute == null)
            {
                throw new Exception($"Attrebute {attributeName} doesn't exist");
            }

            return attribute.Value;
        }
    }
}
