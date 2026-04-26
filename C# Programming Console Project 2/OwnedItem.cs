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
        //demonstrates polymorphism
        public override string ToString()
        {
            return $"{ItemName} - Quantity: {Quantity} - Department: {Department} (Already Owned)";
        }
    }
}
