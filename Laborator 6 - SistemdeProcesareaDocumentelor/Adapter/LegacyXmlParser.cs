using SistemdeProcesareaDocumentelor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SistemdeProcesareaDocumentelor.Adapter
{

    public class LegacyXmlParser
    {
        public LegacyDocument ParseXml(XmlDocument xml)
        {
            return new LegacyDocument
            {
                XmlData = xml.DocumentElement?.InnerText ?? "Fara continut",
                Metadata = "Parsed by LegacySystem v1.0"
            };
        }
    }

}
