using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sistem_de_generare_de_documente.Model
{
    public class ReportComponentFactory : IDocumentComponentFactory
    {
        public string CreateHeader() => "REPORT HEADER";
        public string CreateSection(string content) => $"[REPORT] {content}";
        public string CreateFooter() => "Report Footer";
    }
}
