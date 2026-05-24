using SistemdeProcesareaDocumentelor.Interfaces;
using SistemdeProcesareaDocumentelor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemdeProcesareaDocumentelor.Decorators
{
    public class ValidationException : Exception
    {
        public ValidationException(string message) : base(message) { }
    }

    public class ValidationDocumentParser : DocumentParserDecorator
    {
        public ValidationDocumentParser(IDocumentParser parser) : base(parser) { }

        public override Document Parse(string content)
        {
            Document doc = base.Parse(content);

            if (string.IsNullOrWhiteSpace(doc.Title))
                throw new ValidationException("Eroare: Titlul documentului lipseste!");

            if (doc.Content == null || doc.Content.Length < 10)
                throw new ValidationException("Continutul este prea scurt (minim 10 caractere).");

            Console.WriteLine("[VALIDARE] Documentul este valid.");
            return doc;
        }
    }
}
