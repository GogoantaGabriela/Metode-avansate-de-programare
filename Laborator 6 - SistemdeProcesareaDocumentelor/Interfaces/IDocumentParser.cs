using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SistemdeProcesareaDocumentelor.Models;
using System.Xml;
namespace SistemdeProcesareaDocumentelor.Interfaces
{
    public interface IDocumentParser
    {
        Document Parse(string content);
    }
}
