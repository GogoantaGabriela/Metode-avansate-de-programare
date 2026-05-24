using SistemdeProcesareaDocumentelor.Interfaces;
using SistemdeProcesareaDocumentelor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SistemdeProcesareaDocumentelor.Adapter
{
   
        public class XmlParserAdapter : IDocumentParser
        {
            private readonly LegacyXmlParser _legacyParser;

            public XmlParserAdapter(LegacyXmlParser legacyParser)
            {
                _legacyParser = legacyParser;
            }

            public Document Parse(string content)
            {
                //conversie string -> xml
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(content);

                LegacyDocument legacyDoc = _legacyParser.ParseXml(xmlDoc);

                //conversie LegacyDocument -> Document
                return new Document
                {
                    Title = "Imported XML",
                    Content = legacyDoc.XmlData,
                    Author = legacyDoc.Metadata
                };
            }
        }
    }

