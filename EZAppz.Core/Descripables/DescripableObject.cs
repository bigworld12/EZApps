using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace EZAppz.Core
{
    public class DescribableObject
    {
        public DescribableObject(bool ImportFromReflection = false)
        {
            if (ImportFromReflection)
            {
                ImportDescriptionFromReflection(true, false);
            }
            else
                InternalDictionary = new Dictionary<string, DescribableProperty>
                {
                    ["Item"] = new DescribableProperty("Item", true, new IndexerDescriptorContainer())
                };
        }
        public void ImportDescriptionFromReflection(bool RemovePrevious, bool ImportIndexersOnly)
        {
            if (RemovePrevious)
            {
                InternalDictionary = new Dictionary<string, DescribableProperty>
                {
                    ["Item"] = new DescribableProperty("Item", true, new IndexerDescriptorContainer())
                };
            }
            var t = GetType();
            var props = t.GetProperties();
            foreach (var prop in props)
            {
                var indexed = prop.GetIndexParameters();
                if (indexed.Length > 0)
                {
                    //register indexer                    
                    var mparams = new MethodParameter[indexed.Length];
                    for (int i = 0; i < indexed.Length; i++)
                    {
                        var item = indexed[i];
                        mparams[i] = new MethodParameter(item.ParameterType, item.Name);
                    }

                    Action<MethodParameterValue[], object> setter = null;
                    if (prop.CanWrite)
                    {
                        setter = (para, val) =>
                        {
                            prop.SetValue(this, val, para.Select(x => x.Value).ToArray());
                        };
                    }
                    var desc = new IndexerDescriptor(
                        (para) =>
                        {
                            return prop.GetValue(this, para.Select(x => x.Value).ToArray());
                        },
                        setter, mparams.ToArray());
                    RegisterIndexer(desc);
                }
                else if (!ImportIndexersOnly)
                {
                    InternalDictionary[prop.Name] = new DescribableProperty(prop.Name, !prop.CanWrite, prop.PropertyType.IsValueType ? Activator.CreateInstance(prop.PropertyType) : null);
                }
            }
        }
        public DescribableObject RegisterProperty<TVal>(string prop, TVal defaultValue = default(TVal), bool isReadOnly = false)
        {
            if (prop == "Item")
            {
                throw new ArgumentException("Item property name is reserved for indexer properties, please choose another name");
            }
            InternalDictionary[prop] = new DescribableProperty(prop, isReadOnly, defaultValue);
            return this;
        }
        /// <summary>
        /// registers an indexer then returns the current type
        /// </summary>
        /// <param name="descriptor"></param>
        /// <returns></returns>
        public DescribableObject RegisterIndexer(IndexerDescriptor descriptor)
        {
            IndexerDescriptorContainer idc;
            if (InternalDictionary.TryGetValue("Item", out var dc))
            {
                if (dc.Value is IndexerDescriptorContainer)
                {
                    idc = dc.Value as IndexerDescriptorContainer;
                }
                else
                {
                    //Item must always be the indexer descriptor container
                    dc.Value = idc = new IndexerDescriptorContainer();
                }
            }
            else
            {
                InternalDictionary["Item"] = new DescribableProperty("Item", true, idc = new IndexerDescriptorContainer());
            }
            idc[descriptor.GetHashCode()] = descriptor;
            return this;
        }

        public virtual object GetPropertyValue([CallerMemberName] string prop = null)
        {
            var paths = Helpers.ParsePropertyParts(prop);
            if (paths.Length == 0)
            {
                return null;
            }
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
                if (curObj.InternalDictionary.TryGetValue(purePath, out DescribableProperty dProperty))
                {
                    if (path.Contains("["))
                    {
                        IndexerDescriptorContainer idc;
                        if (dProperty.Value is IndexerDescriptorContainer)
                        {
                            idc = dProperty.Value as IndexerDescriptorContainer;
                        }
                        else if (dProperty.Value is DescribableObject dindexer)
                        {
                            idc = dindexer.InternalDictionary["Item"].Value as IndexerDescriptorContainer;
                        }
                        else
                        {
                            return null;
                        }
                        //now we have the indexer container, curObj = result value, or either final value = result value
                        var pureIndexedParameters = path.Substring(path.IndexOf('[') + 1, path.Length - purePath.Length - 2);
                        var desc = idc.GetDescriptor(pureIndexedParameters, out var vals, out bool suc);
                        if (!suc)
                        {
                            return null;
                        }
                        var matchDict = new MethodParameterValue[desc.Parameters.Length];
                        for (int j = 0; j < desc.Parameters.Length; j++)
                        {
                            var param = desc.Parameters[j];
                            matchDict[j] = new MethodParameterValue(param, vals[j]);
                        }
                        var IndexerRes = desc.GetValue<object>(matchDict);
                        //check indexer result
                        if (IndexerRes is DescribableObject d)
                        {
                            curObj = d;
                            finalVal = null;
                        }
                        else
                        {
                            curObj = null;
                            finalVal = IndexerRes;
                        }
                    }
                    else if (dProperty.Value is DescribableObject d)
                    {
                        curObj = d;
                        finalVal = d;
                    }
                    else
                    {
                        curObj = null;
                        finalVal = dProperty.Value;
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
        public virtual TVal GetPropertyValue<TVal>([CallerMemberName] string prop = null)
        {
            if (string.IsNullOrWhiteSpace(prop))
            {
                return default(TVal);
            }
            var raw = GetPropertyValue(prop);
            if (raw == null)
            {
                return default(TVal);
            }
            return (TVal)raw;
        }

        public virtual void SetPropertyValue<TVal>(TVal value, [CallerMemberName] string prop = null)
        {
            var paths = Helpers.ParsePropertyParts(prop);
            if (paths.Length == 0)
            {
                return;
            }
            int lastIndex = 0;
            //check if last path was an indexer
            /*
                 A=value -> Get Owner = this, owner["A"] = value
                 A.B=value -> Get Owner = A, owner["B"] = value
                 Item[A,B]=value -> Get Indexer = Item[A,B],indexer[A,B] = value
                 A.Item[A,B]=value -> Get Indexer = Item[A,B], indexer [A,B] = value
            */
            var lastPath = paths[paths.Length - 1];
            if (lastPath.Contains("["))
            {
                //last part is an indexer
                lastIndex = paths.Length - 1;
            }
            else
            {
                lastIndex = paths.Length - 2;
            }
            DescribableObject curObj = this;
            //end loop before reaching last part
            for (int i = 0; i <= lastIndex; i++)
            {
                if (curObj == null)
                {
                    break;
                }
                //each iteration yields a curObj or an ownerIndexer
                var path = paths[i];
                var purePath = path.Contains("[") ? path.Substring(0, path.IndexOf('[')) : path;
                if (purePath.Length == 0)
                {
                    purePath = "Item";
                }
                if (curObj.InternalDictionary.TryGetValue(purePath, out DescribableProperty dProperty))
                {
                    if (path.Contains("["))
                    {
                        IndexerDescriptorContainer idc;
                        if (dProperty.Value is IndexerDescriptorContainer)
                        {
                            idc = dProperty.Value as IndexerDescriptorContainer;
                        }
                        else if (dProperty.Value is DescribableObject dindexer)
                        {
                            idc = dindexer.InternalDictionary["Item"].Value as IndexerDescriptorContainer;
                            Console.WriteLine("A logically impossible case just happened ! please tell the developers (me) that they suck");
                        }
                        else
                        {
                            return;
                        }
                        //now we have the indexer container, curObj = result value, or either final value = result value
                        var pureIndexedParameters = path.Substring(path.IndexOf('[') + 1, path.Length - purePath.Length - 2);

                        var desc = idc.GetDescriptor(pureIndexedParameters, out var indices, out bool suc);
                        if (!suc)
                        {
                            return;
                        }
                        var matchDict = new MethodParameterValue[desc.Parameters.Length];
                        for (int j = 0; j < desc.Parameters.Length; j++)
                        {
                            var param = desc.Parameters[j];
                            matchDict[j] = new MethodParameterValue(param, indices[j]);
                        }
                        if (i == lastIndex)
                        {
                            //set indexer here
                            if (desc.IsReadOnly)
                            {
                                return;
                            }
                            curObj.Before_Set(path, value);
                            desc.SetValue(matchDict, value);
                            curObj.After_Set(path, value);
                            return;
                        }
                        else
                        {
                            //get owner object
                            var IndexerRes = desc.GetValue<object>(matchDict);
                            //check indexer result
                            if (IndexerRes is DescribableObject d)
                            {
                                curObj = d;
                            }
                            else
                            {
                                curObj = null;
                            }
                        }
                    }
                    else if (dProperty.Value is DescribableObject d)
                    {
                        curObj = d;
                    }
                    else
                    {
                        curObj = null;
                    }
                }
                else
                {
                    return;
                }
            }

            if (curObj.InternalDictionary.TryGetValue(lastPath, out var dp))
            {
                if (dp.IsReadOnly)
                {
                    return;
                }
                curObj.Before_Set(dp.Name, value);
                dp.Value = value;
                curObj.After_Set(dp.Name, value);
            }
            else
            {
                dp = new DescribableProperty(lastPath, false)
                {
                    Value = value
                };
                //unregistered property
                curObj.Before_Set(dp.Name, value);
                curObj.InternalDictionary[lastPath] = dp;
                curObj.After_Set(dp.Name, value);
            }

        }

        private IDictionary<string, DescribableProperty> InternalDictionary;

        /// <summary>
        /// gets called directly before setting a value, will be used to implment notifications later
        /// </summary>
        /// <param name="property"></param>
        /// <param name="value"></param>
        protected virtual void Before_Set(string property, object NewValue)
        { }
        /// <summary>
        /// gets called directly after setting a value, will be used to implment notifications later
        /// </summary>
        /// <param name="property"></param>
        /// <param name="value"></param>
        protected virtual void After_Set(string property, object NewValue)
        { }
    }


}
