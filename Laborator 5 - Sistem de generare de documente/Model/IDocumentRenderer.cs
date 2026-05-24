using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sistem_de_generare_de_documente.Factory
{
    public interface IDocumentRenderer
    {
        string Render(DocumentData data);
    }
}
