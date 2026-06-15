//error: update the trigger for index search to allow for index searching

using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace ProgramProject2_InventoryList
{
    public enum Department
    {
        Bakery, Produce, Deli, Meat, Seafood, DryGoods, Dairy,
        Frozen, Housewares, Toys, Automotive, Pet, Pharmacy,
        Clothing, Electronics, Hygiene, SportingGoods
    }
    public class Program
    {
       

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
                    SaveInventoryToCsv(groceryInventory);
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

                    decimal price = 0;
                    bool validPrice = false;

                    Console.WriteLine("Enter the price:");

                    while (!validPrice)
                    {
                        string inputPrice = Console.ReadLine();

                        if (decimal.TryParse(inputPrice, out price) && price >= 0)
                        {
                            validPrice = true;
                        }
                        else
                        {
                            Console.WriteLine("Invalid price. Try again:");
                        }
                    }

                    // Department validation
                    Department inputDepartment;

                    while (true)
                    {
                        Console.WriteLine("Enter a department:");

                        foreach (var dept in Enum.GetValues(typeof(Department)))
                        {
                            Console.WriteLine(dept);
                        }

                        string input = Console.ReadLine();

                        if (Enum.TryParse(input, true, out inputDepartment))
                        {
                            break;
                        }

                        Console.WriteLine("Invalid department. Try again.");
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
                            Price = price,
                            AlreadyOwned = true
                        });
                    }
                    else
                    {
                        groceryInventory.Add(new GroceryItem
                        {
                            ItemName = inputItem,
                            Quantity = quantity,
                            Department = inputDepartment,
                            Price = price
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
            Console.WriteLine("\nInventory:");
            foreach (var group in groupedList)
            {
                Console.WriteLine($"----{group.Key.ToString()}----");
                foreach (var item in group.OrderBy(i => i)) // uses IComparable
                {
                    if (item is OwnedItem)
                        Console.WriteLine(item);
                    else
                        Console.WriteLine($"{item.ItemName} - Qty: {item.Quantity} - Price: {item.Price:C}");     
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
                    Console.WriteLine($"{item.ItemName},{item.Quantity},{item.Department},{item.Price}"); 
                }

            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();

            decimal totalValue = groceryInventory.Items
                .Sum(item => (item.Price) * item.Quantity);

            Console.WriteLine($"\nTotal Inventory Value: {totalValue:C}");




            //load the inventory from the file on program startup
            static void LoadInventoryFromFile(Inventory inventory)
            {
                string filePath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                    "inventory.csv");

                //check if the file exists
                if (!File.Exists(filePath))
                    return;

                using (StreamReader reader = new StreamReader(filePath))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] parts = line.Split(',').Select(p => p.Trim()).ToArray();

                        if (parts.Length < 4) continue;

                        if (!int.TryParse(parts[1], out int qty)) continue;

                        Department dept;
                        string deptInput = parts[2].Trim().Replace(" ", "");
                        if (!Enum.TryParse(deptInput, true, out dept))
                        {
                            Console.WriteLine($"SKIPPED (bad department): {parts[2]}");
                            continue;
                        }

                        if (!decimal.TryParse(parts[3], out decimal price))
                            continue;


                        inventory.Add(new OwnedItem
                        {
                            ItemName = parts[0].Trim(),
                            Quantity = qty,
                            Department = dept,
                            Price = price,
                            AlreadyOwned = true
                        });
                    }
                }
            }

            //save the inventpory to a csv desktop file
            static void SaveInventoryToCsv(Inventory inventory)
            {
                string filePath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                    "inventory.csv");

                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    // Header row (important for Excel readability)
                    writer.WriteLine("ItemName,Quantity,Department,Price");

                    foreach (var item in inventory.Items)
                    {
                        writer.WriteLine($"{item.ItemName},{item.Quantity},{item.Department},{item.Price}");
                    }
                }

                Console.WriteLine("Inventory exported to Excel (CSV) successfully!");
            }
        }

        // Validate item name
        static bool IsValidData(string input)
        {
            return !string.IsNullOrWhiteSpace(input) && !int.TryParse(input, out _);
        }

        // Validate department exists
        // changed to enum so array/input validation not needed, deleting method


        
    }
}