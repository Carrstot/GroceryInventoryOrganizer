using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgramProject2_InventoryList
{
    public class GroceryItem : IComparable<GroceryItem>
    {
        public string ItemName { get; set; }
        public int Quantity { get; set; }
        public string Department { get; set; }

        // Interface method for sorting alphabetically
        public int CompareTo(GroceryItem other)
        {
            return this.ItemName.CompareTo(other.ItemName);
        }
    }
}
