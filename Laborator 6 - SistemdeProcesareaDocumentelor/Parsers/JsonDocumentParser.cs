using SistemdeProcesareaDocumentelor.Interfaces;
using SistemdeProcesareaDocumentelor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemdeProcesareaDocumentelor.Parsers
{
    public class JsonDocumentParser : IDocumentParser
    {
        public Document Parse(string content)
        {
            //simulare parsare json
            return new Document
            {
                Title = "Modern JSON Document",
                Content = $"Parsed JSON: {content}",
                Author = "NativeSystem"
            };
        }
    }
}
