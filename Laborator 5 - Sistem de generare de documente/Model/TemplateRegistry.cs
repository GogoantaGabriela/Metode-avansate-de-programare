using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sistem_de_generare_de_documente.Model
{
    public class TemplateRegistry
    {
        private Dictionary<string, DocumentTemplate> _templates = new Dictionary<string, DocumentTemplate>();

        public void Register(string key, DocumentTemplate template)
        {
            _templates[key] = template;
        }

        public DocumentTemplate Get(string key)
        {
            return _templates[key].Clone();
        }
    }
}
