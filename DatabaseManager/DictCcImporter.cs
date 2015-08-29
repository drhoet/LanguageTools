using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DatabaseManager {
    //TODO: performance improvements
    //TOOD: Remove stuff that comes after the gender
    //TODO: Check for lines with double gender.
    class DictCcImporter : Importer {
        protected StreamReader reader;
        private StringBuilder sbWord = new StringBuilder();
        private StringBuilder sbGender = new StringBuilder();

        public DictCcImporter(string fileName) : base(fileName) {
            reader = new StreamReader(stream, Encoding.UTF8, true, 4096);
        }

        public override void Close() {
            reader.Close();
            base.Close();
        }

        public Item parseLine(string line) {
            sbWord.Clear();
            sbGender.Clear();

            string[] split = line.Split('\t');
            try {
                if(split[2] == "noun") {
                    int openBrackets = 0;
                    int openCurlyBrackets = 0;
                    int openSquareBrackets = 0;
                    string val = split[0];
                    char c;
                    for(int i = 0; i < val.Length; ++i) {
                        c = val[i];
                        switch(c) {
                            case '(': ++openBrackets; break;
                            case ')':
                                if(openBrackets == 0) {
                                    throw new ImportParsingException("Unmatching brackets: " + val);
                                }
                                --openBrackets;
                                break;
                            case '[': ++openSquareBrackets; break;
                            case ']':
                                if(openSquareBrackets == 0) {
                                    throw new ImportParsingException("Unmatching brackets: " + val);
                                }
                                --openSquareBrackets;
                                break;
                            case '{': ++openCurlyBrackets; break;
                            case '}':
                                if(openCurlyBrackets == 0) {
                                    throw new ImportParsingException("Unmatching brackets: " + val);
                                }
                                --openCurlyBrackets;
                                break;
                            default:
                                if(openBrackets == 0 && openSquareBrackets == 0) {
                                    if(openCurlyBrackets == 0) {
                                        sbGender.Append(c);
                                    } else {
                                        sbWord.Append(c);
                                    }
                                }
                                break;
                        }
                    }
                }
            } catch(ImportParsingException ipe) {
                MessageBox.Show(ipe.Message, "Parsing exception", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            Item result;
            result.Gender = sbGender;
            result.Word = sbWord;
            return result;
        }

        public override IEnumerable<Item> Items() {
            string line;
            while((line = reader.ReadLine()) != null) {
                if(line != "" && !line.StartsWith("#")) {
                    Item result = parseLine(line);
                    if(result.Gender.Length > 0) {
                        yield return result;
                    }
                }
            }
            yield break;
        }
    }
}
