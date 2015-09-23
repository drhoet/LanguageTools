using LanguageTools.Backend;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DatabaseManager {
    internal class DictCcImporter : Importer {
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

        public void parseLine(string line, List<LemmaRepository.BulkItem> itemList) {
            sbWord.Clear();
            sbGender.Clear();

            string[] split = line.Split('\t');
            try {
                if(split.Length != 3) {
                    throw new ImportParsingException("Invalid line: " + line);
                }
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
                                if(openCurlyBrackets == 0) {
                                    AddLemma(sbWord, sbGender, itemList);
                                }
                                break;
                            default:
                                if(openBrackets == 0 && openSquareBrackets == 0 && openCurlyBrackets < 2) {
                                    if(openCurlyBrackets == 1) {
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
                ImportErrors.Add(ipe);
            }
        }

        /// <summary>
        /// Helper method that adds the current lemma to a list. Follows the following rules:
        /// 1. If gender length is 0: doesn't add, since not a useful word
        /// 2. If the word length is 0: take the same word as last time. Reasoning behind this: words with double gender are present as WORD {m} {f}
        /// 3. All values get trimmed
        /// </summary>
        /// <param name="word"></param>
        /// <param name="gender"></param>
        /// <param name="list"></param>
        private void AddLemma(StringBuilder word, StringBuilder gender, List<LemmaRepository.BulkItem> list) {
            if(gender.Length > 0) {
                LemmaRepository.BulkItem item;
                item.Gender = ToTrimmedString(gender);
                item.Word = ToTrimmedString(word);
                if(item.Word.Length == 0) {
                    // take same word as last time
                    if(list.Count > 0) {
                        item.Word = list.Last().Word;
                    } else {
                        return;
                    }
                }
                list.Add(item);
            }

            sbGender.Clear();
            sbWord.Clear();
        }

        private string ToTrimmedString(StringBuilder sb) {
            int start, end;
            for(start = 0; start < sb.Length && sb[start] == ' '; ++start) { }
            for(end = sb.Length - 1; end > start && sb[end] == ' '; --end) { }
            return sb.ToString(start, end - start + 1);
        }

        public override IEnumerable<LemmaRepository.BulkItem> Items() {
            List<LemmaRepository.BulkItem> lineItems = new List<LemmaRepository.BulkItem>();
            string line;
            while((line = reader.ReadLine()) != null) {
                if(line != "" && !line.StartsWith("#")) {
                    parseLine(line, lineItems);
                    foreach(LemmaRepository.BulkItem result in lineItems) {
                        yield return result;
                    }
                    lineItems.Clear();
                }
            }
            yield break;
        }
    }
}