using SistemdeProcesareaDocumentelor.Adapter;
using SistemdeProcesareaDocumentelor.Decorators;
using SistemdeProcesareaDocumentelor.Facade;
using SistemdeProcesareaDocumentelor.Interfaces;
using SistemdeProcesareaDocumentelor.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemdeProcesareaDocumentelor
{
    class Program
    {
        static void Main()
        {
            var processor = new DocumentProcessingFacade();

            //simulam fisiere pe disc
            File.WriteAllText("contract.xml", "<root>Acesta este un contract XML valid de peste 10 caractere.</root>");
            File.WriteAllText("date.json", "{ \"title\": \"Raport\", \"body\": \"Date JSON valide\" }");
            File.WriteAllText("invalid.xml", "<root>Scurt</root>");

            string[] files = { "contract.xml", "date.json", "invalid.xml", "lipsa.txt" };

            foreach (var file in files)
            {
                Console.WriteLine($"\n>>> Se proceseaza: {file}");

                //apelam metoda simpla a fatadei
                ProcessingResult result = processor.Process(file);

                if (result.IsSuccess)
                {
                    Console.WriteLine($"SUCCESS: {result.Message}");
                    Console.WriteLine($"Locatie: {result.SavedPath}");
                }
                else
                {
                    Console.WriteLine($"FAILED: {result.Message}");
                }
            }
        }
    }
}