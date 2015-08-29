using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseManager {
    abstract class Importer {
        public struct Item {
            public StringBuilder Word;
            public StringBuilder Gender;
        }

        protected long streamLength;
        protected FileStream stream;
        public Importer(string fileName) {
            FileInfo fi = new FileInfo(fileName);
            stream = fi.OpenRead(); ;
            streamLength = fi.Length;
        }

        public abstract IEnumerable<Item> Items();

        public int ProgressPercentage {
            get {
                return (int)(100 * stream.Position / streamLength);
            }
        }

        public virtual void Close() {
            stream.Close();
        }
    }
}
