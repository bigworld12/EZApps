using System;
using System.Collections.Generic;
using System.Text;

namespace EZAppz.Core
{
    public static class Helpers
    {
        public static string[] ParsePropertyParts(string prop, char splitDelimiter = '.')
        {
            //"a.b[c.d.e.f.g[h]].e"
            //is parsed to
            //{"a" , "b[c.d.e.(f.g[h])]" , "e"}
            prop = prop.Trim(' ', '\'');
            var final = new List<string>();
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
                                    final.Add(sb.ToString());
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
            return final.ToArray();
        }
    }
}
