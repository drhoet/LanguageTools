using LanguageTools.Backend;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseManager {
    internal abstract class Importer {
        protected long streamLength;
        protected FileStream stream;
        public Importer(string fileName) {
            FileInfo fi = new FileInfo(fileName);
            stream = fi.OpenRead(); ;
            streamLength = fi.Length;
        }

        public abstract IEnumerable<LemmaRepository.BulkItem> Items();

        public int ProgressPercentage {
            get {
                return (int)(100 * stream.Position / streamLength);
            }
        }

        public virtual void Close() {
            stream.Close();
        }

        public List<ImportParsingException> ImportErrors { get; } = new List<ImportParsingException>();
    }
}