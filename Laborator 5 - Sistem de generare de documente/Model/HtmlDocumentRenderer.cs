using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sistem_de_generare_de_documente.Factory
{
    public class HtmlDocumentRenderer : IDocumentRenderer
    {
        public string Render(DocumentData data)
        {
            var sb = new StringBuilder();
            sb.Append($"<h1>{data.Title}</h1>");
            foreach (var s in data.Sections)
                sb.Append($"<p>{s}</p>");
            return sb.ToString();
        }
    }
}
