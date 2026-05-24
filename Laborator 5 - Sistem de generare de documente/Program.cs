using Sistem_de_generare_de_documente.Factory;
using Sistem_de_generare_de_documente.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sistem_de_generare_de_documente
{
    internal class Program
    {
        static void Main()
        {
            var data = new DocumentDataBuilder()
                .WithTitle("Test")
                .ByAuthor("Eu")
                .WithSection("Sectiune 1")
                .Build();

            //abstract factory
            var assembler = new DocumentAssembler(new InvoiceComponentFactory());
            data = assembler.Assemble(data);

            //factory method
            DocumentExporter exporter = new PlainTextDocumentExporter();
            string result = exporter.Export(data);

            Console.WriteLine(result);
        }
    }
}
