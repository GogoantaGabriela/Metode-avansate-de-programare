using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sistem_de_generare_de_documente.Factory
{
    public abstract class DocumentExporter
    {
        protected abstract IDocumentRenderer CreateRenderer();

        public string Export(DocumentData data)
        {
            var renderer = CreateRenderer();
            return renderer.Render(data);
        }
    }
}
