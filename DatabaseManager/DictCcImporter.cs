using LanguageTools.Backend;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DatabaseManager
{
    internal class DictCcImporter : Importer
    {
        protected StreamReader reader;
        private StringBuilder sbWord = new StringBuilder();
        private StringBuilder sbType = new StringBuilder();
        private StringBuilder sbAbbr = new StringBuilder();

        public DictCcImporter(string fileName) : base(fileName)
        {
            reader = new StreamReader(stream, Encoding.UTF8, true, 4096);
        }

        public override void Close()
        {
            reader.Close();
            base.Close();
        }

        public void parseLine(string line, Dictionary<string, LineParser> parsers)
        {
            sbWord.Clear();
            sbType.Clear();
            sbAbbr.Clear();

            string[] split = line.Split('\t');
            try
            {
                if (split.Length != 3)
                {
                    throw new ImportParsingException("Invalid line: " + line);
                }
                LineParser parser;
                parsers.TryGetValue(split[2], out parser);
                if (parser != null)
                    parser.parseLine(split[0]);
            }
            catch (ImportParsingException ipe)
            {
                ImportErrors.Add(ipe);
            }
            catch (Exception e)
            {
                ImportErrors.Add(new ImportParsingException(e.Message));
            }
        }

        private void nounLineCallback(int type, char c, int[] bracketCounts, List<Lemma> result)
        {
            const int normal = 0;
            const int square = 1;
            const int curly = 2;

            if (type != -1)
            {
                if (bracketCounts[curly] == 0)
                {
                    AddNoun(sbWord, sbType, result);
                }
            }
            else //other char found
            {
                if (bracketCounts[normal] == 0 && bracketCounts[square] == 0 && bracketCounts[curly] < 2)
                {
                    if (bracketCounts[curly] == 1)
                    {
                        sbType.Append(c); //we are in the gender
                    }
                    else
                    {
                        sbWord.Append(c);
                    }
                }
            }
        }

        private void prepositionLineCallback(int type, char c, int[] bracketCounts, List<Lemma> result)
        {
            const int square = 0;
            const int angle = 1;

            if (type != -1)
            {
                AddPreposition(sbWord, sbType, sbAbbr, result);
            }
            else //other char found
            {
                if (bracketCounts[square] == 1)
                {
                    sbType.Append(c); // we are in the case
                }
                else if(bracketCounts[angle] == 1)
                {
                    sbAbbr.Append(c); // we are in the abbreviation
                }
                else
                {
                    sbWord.Append(c);
                }
            }

        }

        /// <summary>
        /// Helper method that adds the current noun to a list. Follows the following rules:
        /// 1. If gender length is 0: doesn't add, since not a useful word
        /// 2. If the word length is 0: take the same word as last time. Reasoning behind this: words with double gender are present as WORD {m} {f}
        /// 3. All values get trimmed
        /// </summary>
        /// <param name="word"></param>
        /// <param name="gender"></param>
        /// <param name="list"></param>
        private void AddNoun(StringBuilder word, StringBuilder gender, List<Lemma> list)
        {
            if (gender.Length > 0)
            {
                Noun item = new Noun();
                item.Gender = NounGenderConvert.ToGender(ToTrimmedString(gender));
                item.Word = ToTrimmedString(word);
                if (item.Word.Length == 0)
                {
                    // take same word as last time
                    if (list.Count > 0)
                    {
                        item.Word = list.Last().Word;
                    }
                    else
                    {
                        return;
                    }
                }
                list.Add(item);
            }

            sbType.Clear();
            sbWord.Clear();
        }

        private void AddPreposition(StringBuilder word, StringBuilder prepositionCase, StringBuilder abbr, List<Lemma> list)
        {
            if (abbr.Length > 0)
            {
                AddPreposition(abbr, prepositionCase, list);
                sbAbbr.Clear();
            } else
            {
                AddPreposition(word, prepositionCase, list);
            }
        }

        private void AddPreposition(StringBuilder word, StringBuilder prepositionCase, List<Lemma> list)
        {
            if(word.Length > 0 && prepositionCase.Length> 0)
            {
                Preposition item = new Preposition();
                item.Word = ToTrimmedString(word);
                item.PrepositionCase = PrepositionCaseConvert.ToCase(ToTrimmedString(prepositionCase));

                list.Add(item);
            }
        }

        private string ToTrimmedString(StringBuilder sb)
        {
            int start, end;
            for (start = 0; start < sb.Length && sb[start] == ' '; ++start) { }
            for (end = sb.Length - 1; end > start && sb[end] == ' '; --end) { }
            return sb.ToString(start, end - start + 1);
        }

        public override IEnumerable<Lemma> Items()
        {
            List<Lemma> lineItems = new List<Lemma>();
            Dictionary<string, LineParser> parsers = new Dictionary<string, LineParser>();
            parsers.Add("noun", new LineParser(new char[] { '(', '[', '{' }, new char[] { ')', ']', '}' }, new char[] { '}' }, true, nounLineCallback, lineItems));
            parsers.Add("prep", new LineParser(new char[] { '[', '<' }, new char[] { ']', '>' }, new char[] { '>', ']' }, true, prepositionLineCallback, lineItems));

            string line;
            while ((line = reader.ReadLine()) != null)
            {
                if (line != "" && !line.StartsWith("#"))
                {
                    parseLine(line, parsers);
                    foreach (Lemma result in lineItems)
                    {
                        yield return result;
                    }
                    lineItems.Clear();
                }
            }
            yield break;
        }
    }

    delegate void LineParserEvent(int type, char c, int[] bracketCounts, List<Lemma> result);
    class LineParser
    {
        char[] openingBrackets, closingBrackets, eventOnChars;
        int[] bracketCounts;
        bool eventOnOthers;
        LineParserEvent callback;
        List<Lemma> result;

        public LineParser(char[] openingBrackets, char[] closingBrackets, char[] eventOnChars,
            bool eventOnOthers, LineParserEvent callback, List<Lemma> result)
        {
            this.openingBrackets = openingBrackets;
            this.closingBrackets = closingBrackets;
            this.eventOnChars = eventOnChars;
            this.eventOnOthers = eventOnOthers;

            bracketCounts = new int[openingBrackets.Count()];

            this.callback = callback;
            this.result = result;
        }

        public void parseLine(string line)
        {
            Array.Clear(bracketCounts, 0, bracketCounts.Count());
            foreach (char c in line)
            {
                int openingType = Array.IndexOf(openingBrackets, c);
                if (openingType > -1)
                {
                    ++bracketCounts[openingType];
                }
                else
                {
                    int closingType = Array.IndexOf(closingBrackets, c);
                    if (closingType > -1)
                    {
                        if (bracketCounts[closingType] == 0)
                        {
                            throw new ImportParsingException("Unmatching brackets: " + line);
                        }
                        --bracketCounts[closingType];
                    }
                    else
                    {
                        if (eventOnOthers)
                            callback(-1, c, bracketCounts, result);
                    }
                }

                int eventedChar = Array.IndexOf(eventOnChars, c);
                if (eventedChar > -1)
                    callback(eventedChar, c, bracketCounts, result);
            }
        }
    }
}