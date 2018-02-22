using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace EZAppz.Core
{
    public class DescribableObject : Dictionary<string, object>
    {
        public DescribableObject() : base()
        {
            this["Item"] = new IndexerDescriptorContainer();
        }

        public DescribableObject RegisterProperty<TVal>(string prop, TVal defaultValue = default(TVal))
        {
            if (prop == "Item")
            {
                throw new ArgumentException("Item property name is reserved for indexer properties, please choose another name");
            }
            this[prop] = defaultValue;
            return this;
        }
        public DescribableObject RegisterIndexer<TVal>(IndexerDescriptor descriptor)
        {
            IndexerDescriptorContainer idc;
            if (TryGetValue("Item", out var dc))
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
                this["Item"] = idc = new IndexerDescriptorContainer();
            }
            if (!idc.Contains(descriptor))
                idc.Add(descriptor);
            return this;
        }

        

        public virtual object GetPropertyValue(string prop)
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
                if (curObj.TryGetValue(purePath, out object val))
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
                        var pureIndexedParameters = path.Substring(path.IndexOf('[') + 1, path.Length - purePath.Length - 2);
                        var desc = idc.GetDescriptor(pureIndexedParameters, out var vals,out bool suc);
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
                        var IndexerRes = desc.Value.GetValue<DescribableObject,object>(curObj, matchDict);
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
        
    }

    
}
