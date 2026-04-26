using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgramProject2_InventoryList
{
    public class Inventory
    {
        //encapsulation
        private List<GroceryItem> items = new List<GroceryItem>();

        public void Add(GroceryItem item)
        {
            items.Add(item);
        }

        //overloaded indexer
        // Indexer by position
        public GroceryItem this[int index]
        {
            get { return items[index]; }
            set { items[index] = value; }
        }

        // Optional: Indexer by name
        public GroceryItem this[string name]
        {
            get { return items.FirstOrDefault(i => i.ItemName.Equals(name, StringComparison.OrdinalIgnoreCase)); }
        }

        public int Count => items.Count;

        // Access all items for iteration or LINQ
        public IEnumerable<GroceryItem> Items => items;
    }
}
