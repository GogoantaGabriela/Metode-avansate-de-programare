using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sistem_de_generare_de_documente.Model
{
    public interface IDocumentComponentFactory
    {
        string CreateHeader();
        string CreateSection(string content);
        string CreateFooter();
    }
}
