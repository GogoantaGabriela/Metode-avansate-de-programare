using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sistem_de_generare_de_documente.Model
{
    public class InvoiceComponentFactory : IDocumentComponentFactory
    {
        public string CreateHeader() => "INVOICE HEADER";
        public string CreateSection(string content) => $"[INV] {content}";
        public string CreateFooter() => "INVOICE FOOTER";
    }
}
