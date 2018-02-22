using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Numerics;

namespace EZAppz.Core
{
    public class IndexerDescriptorContainer : HashSet<IndexerDescriptor>
    {
        /// <summary>
        /// gets the suitable descriptor based on the parameters
        /// </summary>
        /// <param name="innerIndexer"></param>
        /// <returns></returns>
        public IndexerDescriptor? GetDescriptor(string innerIndexer, out object[] vals, out bool success)
        {
            if (string.IsNullOrWhiteSpace(innerIndexer))
            {
                vals = null;
                success = false;
                return null;
            }
            var parts = Helpers.ParsePropertyParts(innerIndexer, ',');
            vals = new object[parts.Length];
            if (parts.Length != 0)
            {
                foreach (var ind in this)
                {
                    if (ind.Parameters.Length != parts.Length)
                    {
                        continue;
                    }
                    bool isGood = true;
                    for (int i = 0; i < parts.Length; i++)
                    {
                        var part = parts[i];
                        var indParam = ind.Parameters[i];

                        if (CanBeOfType(part, indParam.Type, out var val))
                        {
                            vals[i] = val;
                        }
                        else
                        {
                            isGood = false;
                            break;
                        }
                    }
                    if (isGood)
                    {
                        success = true;
                        return ind;
                    }
                }
            }
            success = false;
            return null;
        }



        bool CanBeOfType(string val, Type guess, out object finalVal)
        {
            switch (Type.GetTypeCode(guess))
            {
                case TypeCode.Boolean:
                    {
                        var res = bool.TryParse(val, out var fv);
                        if (res) finalVal = fv;
                        else finalVal = null;
                        return res;
                    }
                case TypeCode.Byte:
                    {
                        var res = byte.TryParse(val, out var fv);
                        if (res) finalVal = fv;
                        else finalVal = null;
                        return res;
                    }
                case TypeCode.Char:
                    {
                        var res = char.TryParse(val, out var fv);
                        if (res) finalVal = fv;
                        else finalVal = null;
                        return res;
                    }
                case TypeCode.DateTime:
                    {
                        var res = DateTime.TryParse(val, out var fv);
                        if (res) finalVal = fv;
                        else finalVal = null;
                        return res;
                    }
                case TypeCode.Decimal:
                    {
                        var res = decimal.TryParse(val, out var fv);
                        if (res) finalVal = fv;
                        else finalVal = null;
                        return res;
                    }
                case TypeCode.Double:
                    {
                        var res = double.TryParse(val, out var fv);
                        if (res) finalVal = fv;
                        else finalVal = null;
                        return res;
                    }
                case TypeCode.Int16:
                    {
                        var res = short.TryParse(val, out var fv);
                        if (res) finalVal = fv;
                        else finalVal = null;
                        return res;
                    }
                case TypeCode.Int32:
                    {
                        var res = int.TryParse(val, out var fv);
                        if (res) finalVal = fv;
                        else finalVal = null;
                        return res;
                    }
                case TypeCode.Int64:
                    {
                        var res = long.TryParse(val, out var fv);
                        if (res) finalVal = fv;
                        else finalVal = null;
                        return res;
                    }
                case TypeCode.SByte:
                    {
                        var res = sbyte.TryParse(val, out var fv);
                        if (res) finalVal = fv;
                        else finalVal = null;
                        return res;
                    }
                case TypeCode.Single:
                    {
                        var res = float.TryParse(val, out var fv);
                        if (res) finalVal = fv;
                        else finalVal = null;
                        return res;
                    }
                case TypeCode.String:
                    finalVal = val;
                    return true;
                case TypeCode.UInt16:
                    {
                        var res = ushort.TryParse(val, out var fv);
                        if (res) finalVal = fv;
                        else finalVal = null;
                        return res;
                    }
                case TypeCode.UInt32:
                    {
                        var res = uint.TryParse(val, out var fv);
                        if (res) finalVal = fv;
                        else finalVal = null;
                        return res;
                    }
                case TypeCode.UInt64:
                    {
                        var res = ulong.TryParse(val, out var fv);
                        if (res) finalVal = fv;
                        else finalVal = null;
                        return res;
                    }

                case TypeCode.Object:
                    //todo : add more logic here
                    if (guess == typeof(object) || guess == typeof(ValueType))
                    {
                        finalVal = val;
                        return true;
                    }

                    if (typeof(IConvertible).IsAssignableFrom(guess))
                    {
                        finalVal = Convert.ChangeType(val, guess);
                        return true;
                    }
                    else
                    {
                        if (guess == typeof(BigInteger))
                        {
                            if (BigInteger.TryParse(val, out var xfinalVal))
                            {
                                finalVal = xfinalVal;
                                return true;
                            }
                            else
                            {
                                finalVal = null;
                                return false;
                            }
                        }
                        else
                        {
                            finalVal = null;
                            return false;
                        }
                    }
                case TypeCode.DBNull:
                case TypeCode.Empty:
                default:
                    finalVal = null;
                    return false;
            }
        }
    }
}
