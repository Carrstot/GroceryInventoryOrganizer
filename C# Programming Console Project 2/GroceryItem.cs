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
        public Department Department { get; set; }
        public decimal Price { get; set; }

        // Interface method for sorting alphabetically
        public int CompareTo(GroceryItem other)
        {
            return this.ItemName.CompareTo(other.ItemName);
        }
        public override string ToString()
        {
            return $"{ItemName} - Quantity: {Quantity} - Price: {Price:C} - Department: {Department}";
        }
    }
}
