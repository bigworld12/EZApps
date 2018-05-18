using System;
using System.Collections.Generic;
using System.Text;

namespace EZAppz.Core
{
    public static class Helpers
    {
        public static string[] ParsePropertyParts(string prop, char splitDelimiter = '.')
        {
            //"h.a[5].b[3][9][89].e"
            //is parsed to
            //{"h", "a", "Item[5]" , "b","Item[3]" ,"Item[9]", "Item[89]" , "e"}

            var final = new List<string>();
            var clean = prop.Trim(' ', '\'', '"');            
            if (clean.StartsWith("Item["))
            {
                clean = clean.Remove(0, 4);
            }
            prop = clean.Replace(".Item[", "[").Replace("[", ".Item[").TrimStart('.');
            Stack<char> Brackets = new Stack<char>();
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < prop.Length; i++)
            {
                var c = prop[i];
                switch (c)
                {
                    case '[':
                        {
                            //openers
                            Brackets.Push(c);
                            sb.Append(c);
                            break;
                        }
                    case ']':
                        {
                            //closers
                            var latestDelim = Brackets.Pop();
                            if (latestDelim != '[')
                            {
                                throw new ArgumentException("Path wasn't in the correct format");
                            }
                            sb.Append(c);
                            break;
                        }
                    default:
                        {
                            if (Brackets.Count == 0)
                            {
                                //good
                                //start checking for '.'
                                if (c == splitDelimiter)
                                {
                                    //split
                                    var st = sb.ToString();
                                    final.Add(st);
                                    sb.Clear();
                                }
                                else
                                {
                                    sb.Append(c);
                                }
                            }
                            else
                            {
                                //don't check for dots yet
                                sb.Append(c);
                            }
                            break;
                        }
                }
            }

            if (sb.Length > 0)
            {
                final.Add(sb.ToString());
                sb.Clear();
            }
            //final.RemoveAll(x => string.IsNullOrWhiteSpace(x));
            return final.ToArray();
        }
    }
}
