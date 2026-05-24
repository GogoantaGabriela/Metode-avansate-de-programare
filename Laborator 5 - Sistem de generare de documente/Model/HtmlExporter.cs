using Sistem_de_generare_de_documente.Factory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sistem_de_generare_de_documente.Model
{
    public class HtmlDocumentExporter : DocumentExporter
    {
        protected override IDocumentRenderer CreateRenderer()
        {
            return new HtmlDocumentRenderer();
        }
    }
}
