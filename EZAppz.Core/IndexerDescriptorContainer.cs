using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace EZAppz.Core
{
    public class IndexerDescriptorContainer : HashSet<IndexerDescriptor>
    {
        /// <summary>
        /// gets the suitable descriptor based on the parameters
        /// </summary>
        /// <param name="innerIndexer"></param>
        /// <returns></returns>
        internal IndexerDescriptor? GetDescriptor(string innerIndexer)
        {
            if (string.IsNullOrWhiteSpace(innerIndexer))
            {
                return null;
            }
            var parts = DescribableObject.ParsePropertyParts(innerIndexer, ',');
            if (parts.Length == 0)
            {
                var allCounts = this.Where(x => x.Parameters.Length == parts.Length);
            }
        }
    }
}
