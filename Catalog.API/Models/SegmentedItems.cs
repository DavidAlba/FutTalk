using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.API.Models
{
    public class SegmentedItems<TEntity> where TEntity : class
    {
        public SegmentedItems(int index, int size, long count, IEnumerable<TEntity> items)
        {
            this.Index = index;
            this.Size = size;
            this.Count = count;
            this.Items = items;
        }

        public int Index { get; private set; }

        public int Size { get; private set; }

        public long Count { get; private set; }

        public IEnumerable<TEntity> Items { get; private set; }
    }
}
