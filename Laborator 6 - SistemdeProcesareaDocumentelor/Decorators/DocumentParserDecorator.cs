using SistemdeProcesareaDocumentelor.Interfaces;
using SistemdeProcesareaDocumentelor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemdeProcesareaDocumentelor.Decorators
{
    public abstract class DocumentParserDecorator : IDocumentParser
    {
        protected readonly IDocumentParser _innerParser;

        protected DocumentParserDecorator(IDocumentParser parser)
        {
            _innerParser = parser;
        }

        public virtual Document Parse(string content)
        {
            return _innerParser.Parse(content);
        }
    }
}
