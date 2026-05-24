using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SistemdeProcesareaDocumentelor.Interfaces;
using SistemdeProcesareaDocumentelor.Models;
using System.Diagnostics;

namespace SistemdeProcesareaDocumentelor.Decorators
{
    public class LoggingDocumentParser : DocumentParserDecorator
    {
        public LoggingDocumentParser(IDocumentParser parser) : base(parser) { }

        public override Document Parse(string content)
        {
            Console.WriteLine($"[LOG] Start parsare la: {DateTime.Now:HH:mm:ss.fff}");
            var watch = Stopwatch.StartNew();

            var document = base.Parse(content);

            watch.Stop();
            Console.WriteLine($"[LOG] Finalizat in: {watch.ElapsedMilliseconds}ms");
            Console.WriteLine($"[LOG] Dimensiune document rezultat: {document.Content.Length} caractere");

            return document;
        }
    }
}
