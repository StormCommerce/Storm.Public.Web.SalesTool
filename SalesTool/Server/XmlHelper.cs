using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Enferno.Public.Web.SalesTool.Server
{
    public class XmlHelper
    {
        public XmlDocument SerializeToXmlDocument(object input)
        {
            var serializer = new XmlSerializer(input.GetType(), "http://schemas.enferno.se");
            XmlDocument xmlDoc;
            using (var stream = new MemoryStream())
            {
                serializer.Serialize(stream, input);
                stream.Position = 0;
                var settings = new XmlReaderSettings { IgnoreWhitespace = true };
                using (var reader = XmlReader.Create(stream, settings))
                {
                    xmlDoc = new XmlDocument();
                    xmlDoc.Load(reader);
                }
            }
            return xmlDoc;
        }

        public string TransformXml(XmlDocument xmlDoc, string xsltFileName)
        {
            var xslt = new System.Xml.Xsl.XslCompiledTransform();
            xslt.Load(System.Web.HttpContext.Current.Server.MapPath(xsltFileName));
            string text;
            using (var stream = new MemoryStream())
            {
                xslt.Transform(xmlDoc, null, stream);
                stream.Position = 1;
                using (var reader = new StreamReader(stream))
                {
                    text = reader.ReadToEnd();
                }
            }
            while (text.ToCharArray()[0] > 255)
            {
                text = text.Substring(1);
            }
            return text;
        }
    }
}