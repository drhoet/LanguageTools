using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseManager
{
    abstract class Importer
    {
        public struct Item
        {
            public StringBuilder Word;
            public StringBuilder Gender;
        }

        protected FileStream stream;
        public Importer(string fileName)
        {
            stream = File.OpenRead(fileName);
        }

        public abstract IEnumerable<Item> Items();
        
        public virtual void Close()
        {
            stream.Close();
        }
    }
}
