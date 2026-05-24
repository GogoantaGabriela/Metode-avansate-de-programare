using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SistemdeProcesareaDocumentelor.Interfaces;
using SistemdeProcesareaDocumentelor.Models;
using System.Collections.Generic;

namespace SistemdeProcesareaDocumentelor.Decorators
{
    public class CachingDocumentParser : DocumentParserDecorator
    {
        private readonly Dictionary<int, Document> _cache = new Dictionary<int, Document>();

        public CachingDocumentParser(IDocumentParser parser) : base(parser) { }

        public override Document Parse(string content)
        {
            int hash = content.GetHashCode();

            if (_cache.ContainsKey(hash))
            {
                Console.WriteLine("[CACHE] Rezultat returnat din memorie (fara parsing).");
                return _cache[hash];
            }

            Document doc = base.Parse(content);
            _cache[hash] = doc;
            return doc;
        }
    }
}
