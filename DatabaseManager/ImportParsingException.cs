using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseManager {
    [Serializable]
    class ImportParsingException : Exception {
        public ImportParsingException(string message) : base(message) {
        }
    }
}
