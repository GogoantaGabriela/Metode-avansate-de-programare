using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sistem_de_generare_de_documente.Model
{
    public class DocumentAssembler
    {
        private readonly IDocumentComponentFactory _factory;

        public DocumentAssembler(IDocumentComponentFactory factory)
        {
            _factory = factory;
        }

        public DocumentData Assemble(DocumentData data)
        {
            data.Sections.Insert(0, _factory.CreateHeader());
            data.Sections.Add(_factory.CreateFooter());
            return data;
        }
    }
}
