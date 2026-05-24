using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sistem_de_generare_de_documente
{
    public  class DocumentData
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public DateTime Date { get; set; }
        public List<string> Sections { get; set; } = new List<string>();
        public bool Landscape { get; set; }
        public string Footnote { get; set; }
    }
}
