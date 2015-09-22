﻿using LanguageTools.Backend;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DatabaseManager {
    //TOOD: Remove stuff that comes after the gender
    //TODO: Check for lines with double gender.
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

        public LemmaRepository.BulkItem parseLine(string line) {
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
                sbGender.Clear(); // invalidate
                sbWord.Clear();
            }
            LemmaRepository.BulkItem result;
            result.Gender = sbGender.ToString();
            result.Word = sbWord.ToString();
            return result;
        }

        public override IEnumerable<LemmaRepository.BulkItem> Items() {
            string line;
            while((line = reader.ReadLine()) != null) {
                if(line != "" && !line.StartsWith("#")) {
                    LemmaRepository.BulkItem result = parseLine(line);
                    if(result.Gender.Length > 0) {
                        yield return result;
                    }
                }
            }
            yield break;
        }
    }
}