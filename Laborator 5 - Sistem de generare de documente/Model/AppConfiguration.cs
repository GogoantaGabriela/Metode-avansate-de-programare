using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sistem_de_generare_de_documente.Model
{
    public sealed class AppConfiguration
    {
        private static readonly Lazy<AppConfiguration> _instance =
            new Lazy<AppConfiguration>(() => new AppConfiguration());

        public static AppConfiguration Instance => _instance.Value;

        public string OutputDirectory { get; private set; }
        public string DefaultFormat { get; private set; }
        public string DefaultAuthor { get; private set; }

        private AppConfiguration()
        {
            OutputDirectory = "./output";
            DefaultFormat = "txt";
            DefaultAuthor = "Unknown";
        }
    }
}
