using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace EZAppz.Core
{
    public class DescribableObject
    {
        public DescribableObject() : base()
        {
            InternalDictionary = new Dictionary<string, object>
            {
                ["Item"] = new IndexerDescriptorContainer()
            };
        }
        protected virtual IDictionary<string, object> InternalDictionary { get; }
        public DescribableObject RegisterProperty<TVal>(string prop, TVal defaultValue = default(TVal))
        {
            if (prop == "Item")
            {
                throw new ArgumentException("Item property name is reserved for indexer properties, please choose another name");
            }
            InternalDictionary[prop] = defaultValue;
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
                InternalDictionary["Item"] = idc = new IndexerDescriptorContainer();
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
                if (curObj.InternalDictionary.TryGetValue(purePath, out object val))
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
                            idc = dindexer.InternalDictionary["Item"] as IndexerDescriptorContainer;
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
                        var matchDict = new MethodParameterValue[desc.Value.Parameters.Length];
                        for (int j = 0; j < desc.Value.Parameters.Length; j++)
                        {
                            var param = desc.Value.Parameters[j];
                            matchDict[j] = new MethodParameterValue(param, vals[j]);
                        }
                        var IndexerRes = desc.Value.GetValue<DescribableObject, object>(curObj, matchDict);
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
                    else if (val is DescribableObject d)
                    {
                        curObj = d;
                        finalVal = d;
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
                if (curObj.InternalDictionary.TryGetValue(purePath, out object val))
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
                            idc = dindexer.InternalDictionary["Item"] as IndexerDescriptorContainer;
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
                        var matchDict = new MethodParameterValue[desc.Value.Parameters.Length];
                        for (int j = 0; j < desc.Value.Parameters.Length; j++)
                        {
                            var param = desc.Value.Parameters[j];
                            matchDict[j] = new MethodParameterValue(param, indices[j]);
                        }
                        if (i == lastIndex)
                        {
                            //set indexer here
                            curObj.Before_Set(path, value);
                            desc.Value.SetValue<DescribableObject>(curObj, matchDict, value);
                            curObj.After_Set(path, value);
                            return;
                        }
                        else
                        {
                            //get owner object
                            var IndexerRes = desc.Value.GetValue<DescribableObject, object>(curObj, matchDict);
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
                    else if (val is DescribableObject d)
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
            curObj.Before_Set(lastPath, value);
            curObj.InternalDictionary[lastPath] = value;
            curObj.After_Set(lastPath, value);
        }



        /// <summary>
        /// gets called directly before setting a value, will be used to implment notifications later
        /// </summary>
        /// <param name="property"></param>
        /// <param name="value"></param>
        protected virtual void Before_Set(string property, object value)
        { }
        /// <summary>
        /// gets called directly after setting a value, will be used to implment notifications later
        /// </summary>
        /// <param name="property"></param>
        /// <param name="value"></param>
        protected virtual void After_Set(string property, object value)
        { }
    }


}
