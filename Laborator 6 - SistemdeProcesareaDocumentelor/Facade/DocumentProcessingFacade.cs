using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SistemdeProcesareaDocumentelor.Adapter;
using SistemdeProcesareaDocumentelor.Decorators;
using SistemdeProcesareaDocumentelor.Interfaces;
using SistemdeProcesareaDocumentelor.Models;
using SistemdeProcesareaDocumentelor.Parsers;
using System.IO;
namespace SistemdeProcesareaDocumentelor.Facade
{
    public class DocumentProcessingFacade
    {
        public ProcessingResult Process(string filePath)
        {
            try
            {
                //citire continut din fisier(validare ca fisierul exista)
                if (!File.Exists(filePath))
                    return new ProcessingResult { IsSuccess = false, Message = "Fisierul nu exista." };

                string content = File.ReadAllText(filePath);
                string extension = Path.GetExtension(filePath).ToLower();

                //selectarea parserului de baza (folosind Adapter pentru Xml)
                IDocumentParser baseParser;
                if (extension == ".xml")
                {
                    baseParser = new XmlParserAdapter(new LegacyXmlParser());
                }
                else if (extension == ".json")
                {
                    baseParser = new JsonDocumentParser();
                }
                else
                {
                    return new ProcessingResult { IsSuccess = false, Message = "Format inexistent." };
                }

                //orchestrare
                IDocumentParser decoratedParser = new LoggingDocumentParser(
                    new CachingDocumentParser(
                        new ValidationDocumentParser(baseParser)
                    )
                );

                //parsare + validare + logging + caching
                Document doc = decoratedParser.Parse(content);

                string outputPath = SaveDocument(doc);

                return new ProcessingResult
                {
                    IsSuccess = true,
                    SavedPath = outputPath,
                    Message = "Document procesat cu succes!"
                };
            }
            catch (ValidationException ex)
            {
                return new ProcessingResult { IsSuccess = false, Message = $"Eroare de business: {ex.Message}" };
            }
            catch (Exception ex)
            {
                return new ProcessingResult { IsSuccess = false, Message = $"Eroare tehnica neasteptata: {ex.Message}" };
            }
        }

        private string SaveDocument(Document doc)
        {
            //simulam salvarea intr-un format intern
            string fileName = $"internal_storage_{Guid.NewGuid()}.txt";
            return fileName;
        }
    }
}
