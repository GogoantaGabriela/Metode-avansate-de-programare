using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sistem_de_generare_de_documente.Model
{
    public class DocumentTemplate
    {
        public string Title { get; set; }
        public List<string> Sections { get; set; } = new List<string>();

        public DocumentTemplate Clone()
        {
            return new DocumentTemplate
            {
                Title = this.Title,
                Sections = new List<string>(this.Sections)
            };
        }
    }
}
