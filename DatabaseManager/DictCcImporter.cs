using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseManager
{
    class DictCcImporter : Importer
    {
        protected StreamReader reader;
        private StringBuilder sbWord = new StringBuilder();
        private StringBuilder sbGender = new StringBuilder();

        public DictCcImporter(string fileName) : base(fileName)
        {
            reader = new StreamReader(stream, Encoding.UTF8, true, 4096);
        }

        public override void Close()
        {
            reader.Close();
            base.Close();
        }

        public Item parseLine(string line)
        {
            sbWord.Clear();
            sbGender.Clear();

            string[] split = line.Split('\t');
            if (split[2] == "noun")
            {
                bool inBrackets = false;
                bool inCurlyBrackets = false;
                bool inSquareBrackets = false;
                string val = split[0];
                char c;
                for (int i = 0; i < val.Length; ++i)
                {
                    c = val[i];
                    switch (c)
                    {
                        case '(':
                            if (inBrackets)
                            {
                                throw new Exception("Double open brackets: " + val);
                            }
                            inBrackets = true;
                            break;
                        case ')':
                            if (!inBrackets)
                            {
                                throw new Exception("Unmatching brackets: " + val);
                            }
                            inBrackets = false;
                            break;
                        case '[':
                            if (inSquareBrackets)
                            {
                                throw new Exception("Double open brackets: " + val);
                            }
                            inSquareBrackets = true;
                            break;
                        case ']':
                            if (!inSquareBrackets)
                            {
                                throw new Exception("Unmatching brackets: " + val);
                            }
                            inSquareBrackets = false;
                            break;
                        case '{':
                            if (inCurlyBrackets)
                            {
                                throw new Exception("Double open brackets: " + val);
                            }
                            inCurlyBrackets = true;
                            break;
                        case '}':
                            if (!inCurlyBrackets)
                            {
                                throw new Exception("Unmatching brackets: " + val);
                            }
                            inCurlyBrackets = false;
                            break;
                        default:
                            if (!inBrackets && !inSquareBrackets)
                            {
                                if (inCurlyBrackets)
                                {
                                    sbGender.Append(c);
                                }
                                else
                                {
                                    sbWord.Append(c);
                                }
                            }
                            break;
                    }
                }
            }
            Item result;
            result.Gender = sbGender;
            result.Word = sbWord;
            return result;
        }

        public override IEnumerable<Item> Items()
        {
            string line;
            while((line = reader.ReadLine()) != null)
            {
                if (line != "" && !line.StartsWith("#"))
                {
                    Item result = parseLine(line);
                    if (result.Gender.Length > 0)
                    {
                        yield return result;
                    }
                }
            }
            yield break;
        }
    }
}
