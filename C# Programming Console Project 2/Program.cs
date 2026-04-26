//error: update the trigger for index search to allow for index searching

using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace ProgramProject2_InventoryList
{
    public class Program
    {
        static string[] departments = new string[]
        {
            "Bakery", "Produce", "Deli", "Meat", "Seafood", "Dry Goods", "Dairy",
            "Frozen", "Housewares", "Toys", "Automotive", "Pet", "Pharmacy",
            "Clothing", "Electronics", "Hygiene", "Sporting Goods"
        };

        static void Main(string[] args)
        {
            Inventory groceryInventory = new Inventory();

            //load the inventory from the file
            LoadInventoryFromFile(groceryInventory);

            while (true)
            {
                Console.Write("Enter an item (or type 'SEARCH' to find an item, 'DONE' to finish): ");
                string inputItem = Console.ReadLine();

                // Sentinel to exit loop
                //add save to file and meassage before break statement 
                if (inputItem.Equals("DONE", StringComparison.OrdinalIgnoreCase))
                {
                    SaveInventoryToFile(groceryInventory);
                    Console.WriteLine($"Inventory saved to the desktop");
                    break;
                }

                // Trigger for indexer search
                if (inputItem.Equals("SEARCH", StringComparison.OrdinalIgnoreCase))
                {
                    if (groceryInventory.Count == 0)
                    {
                        Console.WriteLine("Inventory is empty.");
                        continue;
                    }

              

                    // find item by name or index
                    Console.WriteLine("Enter the (1)name of an (2)index of item to search:");
                    String optionNum = Console.ReadLine();
                    if (optionNum == "1")
                    {
                        Console.WriteLine("Enter the name of an item to search:");
                        string name = Console.ReadLine();
                        var foundItem = groceryInventory[name];
                        if (foundItem != null)
                        {
                            Console.WriteLine($"{foundItem.ItemName} - Quantity: {foundItem.Quantity} - Department: {foundItem.Department}");
                        }
                        else
                        {
                            Console.WriteLine("Item not found.");
                        }
                    }
                    else if (optionNum == "2")
                    {
                        Console.WriteLine("Enter the index of an item");
                        if (int.TryParse(Console.ReadLine(), out int index))
                        {
                            if (index >= 0 && index < groceryInventory.Count)
                            {
                                var item = groceryInventory[index];
                                Console.WriteLine(item);
                            }
                            else
                            {
                                Console.WriteLine("Index out of range.");
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid choice.");

                    }
                        continue; // start the loop again
                }

                // exception handling for item
                bool validItem = false;
                while (!validItem)
                {
                    try
                    {
                        if (!IsValidData(inputItem))
                            throw new Exception("You must enter a valid item name (cannot be empty, whitespace, or a number).");
                        validItem = true;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        Console.Write("Enter an item (or type 'SEARCH' to find an item, 'DONE' to finish): ");
                        inputItem = Console.ReadLine();

                        // validate search or done
                        if (inputItem.Equals("DONE", StringComparison.OrdinalIgnoreCase) ||
                            inputItem.Equals("SEARCH", StringComparison.OrdinalIgnoreCase))
                        {
                            validItem = true; // exit validation
                        }
                    }
                }

                // validate item
                if (!inputItem.Equals("SEARCH", StringComparison.OrdinalIgnoreCase) &&
                    !inputItem.Equals("DONE", StringComparison.OrdinalIgnoreCase))
                {
                    // exception handling for quantity
                    int quantity = 0;
                    bool validQuantity = false;
                    Console.WriteLine("Enter the quantity. It must be a whole number greater than zero.");
                    while (!validQuantity)
                    {
                        try
                        {
                            string inputQuantity = Console.ReadLine();
                            if (!int.TryParse(inputQuantity, out quantity) || quantity <= 0)
                                throw new Exception("Invalid input. Please try again.");
                            validQuantity = true;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }

                    // Department validation
                    Console.WriteLine("Enter a department: ");
                    string inputDepartment = Console.ReadLine();
                    while (!IsPresent(inputDepartment, departments))
                    {
                        Console.WriteLine("Please enter a valid department.");
                        inputDepartment = Console.ReadLine();
                    }

                    
                    //add items to inventory using inheritance to create an alreadyowned object
                    //ask if already owned
                    Console.WriteLine("Do you already own this item? (y/n)");
                    string ownedItemResponse = Console.ReadLine();

                    if (ownedItemResponse.Equals("y", StringComparison.OrdinalIgnoreCase))
                    {
                        groceryInventory.Add(new OwnedItem
                        {
                            ItemName = inputItem,
                            Quantity = quantity,
                            Department = inputDepartment,
                            AlreadyOwned = true
                        });
                    }
                    else
                    {
                        groceryInventory.Add(new GroceryItem
                        {
                            ItemName = inputItem,
                            Quantity = quantity,
                            Department = inputDepartment
                        });
                    }
                    Console.WriteLine("Item added successfully.\n");
                    
                }
            }

            // Sort and group
            var groupedList = groceryInventory.Items
                .GroupBy(item => item.Department)
                .OrderBy(group => group.Key);

            // Display inventory accounting for owneditems
            Console.WriteLine("\nInventoryk:");
            foreach (var group in groupedList)
            {
                Console.WriteLine($"----{group.Key.ToUpper()}----");
                foreach (var item in group.OrderBy(i => i)) // uses IComparable
                {
                    if (item is OwnedItem)
                        Console.WriteLine(item);
                    else
                        Console.WriteLine($"{item.ItemName} - Quantity: {item.Quantity}");     
                }
                Console.WriteLine();
            }
            //use linq to create an list without owned items
            var purchaseList = groceryInventory.Items
                .Where(item => item is not OwnedItem)
                .OrderBy(item => item.Department)
                .ThenBy(item => item.ItemName);

            Console.WriteLine("/nPurchase List (Items to be purchased):");
            foreach (var item in purchaseList)  
                { 
                    Console.WriteLine($"{item.ItemName} - Quantity: {item.Quantity} - Department: {item.Department}");
                }

            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();

            //load the inventory from the file on program startup
            static void LoadInventoryFromFile(Inventory inventory)
            {
                string filePath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                    "inventory.txt");

                //check if the file exists
                if (!File.Exists(filePath))
                    return;

                using (StreamReader reader = new StreamReader(filePath))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] parts = line.Split(',');

                        inventory.Add(new OwnedItem
                        {
                            ItemName = parts[0].Trim(),
                            Quantity = int.Parse(parts[1]),
                            Department = parts[2].Trim(),
                            AlreadyOwned = true
                        });
                    }
                }
            }

                //save the inventpory to a desktop file
                static void SaveInventoryToFile(Inventory inventory)
            {
                string filePath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                    "inventory.txt");

                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    foreach (var item in inventory.Items)
                    {
                        writer.WriteLine($"{item.ItemName}, {item.Quantity},{item.Department}");
                    }
                }
                
            }
        }

        // Validate item name
        static bool IsValidData(string input)
        {
            return !string.IsNullOrWhiteSpace(input) && !int.TryParse(input, out _);
        }

        // Validate department exists
        static bool IsPresent(string department, string[] departments)
        {
            foreach (string dept in departments)
            {
                if (string.Equals(department, dept, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;

        
        }
    }
}