using LanguageTools.Backend;
using System.Collections.Generic;
using System.IO;

namespace DatabaseManager {
    internal abstract class Importer {
        protected long streamLength;
        protected FileStream stream;
        public Importer(string fileName) {
            FileInfo fi = new FileInfo(fileName);
            stream = fi.OpenRead(); ;
            streamLength = fi.Length;
        }

        public abstract IEnumerable<Lemma> Items();

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