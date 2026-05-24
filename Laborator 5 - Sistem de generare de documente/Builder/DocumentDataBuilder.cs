using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sistem_de_generare_de_documente
{
    public class DocumentDataBuilder
    {
        private DocumentData _data = new DocumentData();

        public DocumentDataBuilder WithTitle(string title)
        {
            _data.Title = title;
            return this;
        }

        public DocumentDataBuilder ByAuthor(string author)
        {
            _data.Author = author;
            return this;
        }

        public DocumentDataBuilder WithSection(string section)
        {
            _data.Sections.Add(section);
            return this;
        }

        public DocumentDataBuilder InLandscape()
        {
            _data.Landscape = true;
            return this;
        }

        public DocumentDataBuilder WithFootnote(string note)
        {
            _data.Footnote = note;
            return this;
        }

        public DocumentData Build()
        {
            if (string.IsNullOrEmpty(_data.Title) ||
                string.IsNullOrEmpty(_data.Author) ||
                _data.Sections.Count == 0)
            {
                throw new InvalidOperationException("Date invalide");
            }

            _data.Date = DateTime.Now;
            return _data;
        }
    }
}

