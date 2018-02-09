using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace EZAppz.Core
{
    public class DescribableObject : Dictionary<string, object>
    {
        public DescribableObject() : base()
        {
            this["Item"] = new IndexerDescriptorContainer();
        }

        public DescribableObject RegisterProperty(string prop, object defaultValue = null)
        {
            if (prop == "Item")
            {
                throw new ArgumentException("Item property name is reserved for indexer properties, please choose another name");
            }
            this[prop] = defaultValue;
            return this;
        }
        public DescribableObject RegisterIndexer(IndexerDescriptor descriptor, string name = "Item")
        {
            IndexerDescriptorContainer idc;
            if (TryGetValue(name, out var dc))
            {
                if (dc is IndexerDescriptorContainer)
                {
                    idc = dc as IndexerDescriptorContainer;
                }
                else
                {
                    idc = new IndexerDescriptorContainer();
                }
            }
            else
            {
                this[name] = idc = new IndexerDescriptorContainer();
            }
            if (!idc.Contains(descriptor))
                idc.Add(descriptor);
            return this;
        }

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

        public object GetPropertyValue(string prop)
        {
            var paths = ParsePropertyParts(prop);
            if (paths.Length == 0)
            {
                return null;
            }
            //"a.b[c.d.e.(f.g[h])].e.Item[z]"
            //is parsed to
            //{"a" , "b[c.d.e.(f.g[h,y]),z]" , "e" , "Item[i]"}
            DescribableObject curObj = this;
            object finalVal = null;
            for (int i = 0; i < paths.Length; i++)
            {
                if (curObj == null)
                {
                    break;
                }
                //each iteration yields a finalVal and a curObj
                var path = paths[i];
                var purePath = path.Contains("[") ? path.Substring(0, path.IndexOf('[')) : path;
                if (purePath.Length == 0)
                {
                    purePath = "Item";
                }
                if (curObj.TryGetValue(purePath, out var val))
                {
                    if (path.Contains("["))
                    {
                        IndexerDescriptorContainer idc;
                        if (val is IndexerDescriptorContainer)
                        {
                            idc = val as IndexerDescriptorContainer;
                        }
                        else if (val is DescribableObject dindexer)
                        {
                            idc = dindexer["Item"] as IndexerDescriptorContainer;
                        }
                        else
                        {
                            return null;
                        }

                        //now we have the indexer container, curObj = result value, or either final value = result value
                        var pureIndexedParameters = path.Substring(path.IndexOf('['), path.Length - purePath.Length - 1);


                    }
                    else if (val is DescribableObject d)
                    {
                        curObj = d;
                        finalVal = null;
                    }
                    else
                    {
                        curObj = null;
                        finalVal = val;
                    }
                }
                else
                {
                    //property doesn't exist
                    return null;
                }
            }
            return finalVal;
        }
    }
}
