using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgramProject2_InventoryList
{
    public class OwnedItem:GroceryItem
    {
        public bool AlreadyOwned { get; set; }
        public decimal Price { get; set; }

        //demonstrates polymorphism
        public override string ToString()
        {
            return $"{ItemName} - Quantity: {Quantity} - Price: {Price:C} Department: {Department} (Already Owned)";
        }
    }
}
