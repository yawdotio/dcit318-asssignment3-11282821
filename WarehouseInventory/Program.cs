using System;
using System.Collections.Generic;

namespace WarehouseInventory
{
    // Marker Interface for Inventory Items
    public interface IInventoryItem
    {
        int Id { get; }
        string Name { get; }
        int Quantity { get; set; }
    }

    // Product Classes
    public class ElectronicItem : IInventoryItem
    {
        public int Id { get; }
        public string Name { get; }
        public int Quantity { get; set; }
        public string Brand { get; }
        public int WarrantyMonths { get; }

        public ElectronicItem(int id, string name, int quantity, string brand, int warrantyMonths)
        {
            Id = id;
            Name = name;
            Quantity = quantity;
            Brand = brand;
            WarrantyMonths = warrantyMonths;
        }
    }

    public class GroceryItem : IInventoryItem
    {
        public int Id { get; }
        public string Name { get; }
        public int Quantity { get; set; }
        public DateTime ExpiryDate { get; }

        public GroceryItem(int id, string name, int quantity, DateTime expiryDate)
        {
            Id = id;
            Name = name;
            Quantity = quantity;
            ExpiryDate = expiryDate;
        }
    }

    // Custom Exceptions
    public class DuplicateItemException : Exception
    {
        public DuplicateItemException(string message) : base(message) { }
    }

    public class ItemNotFoundException : Exception
    {
        public ItemNotFoundException(string message) : base(message) { }
    }

    public class InvalidQuantityException : Exception
    {
        public InvalidQuantityException(string message) : base(message) { }
    }

    // Generic Inventory Repository
    public class InventoryRepository<T> where T : IInventoryItem
    {
        private Dictionary<int, T> _items = new Dictionary<int, T>();

        public void AddItem(T item)
        {
            if (_items.ContainsKey(item.Id))
            {
                throw new DuplicateItemException($"Item with ID {item.Id} already exists in the inventory.");
            }
            _items.Add(item.Id, item);
        }

        public T GetItemById(int id)
        {
            if (!_items.ContainsKey(id))
            {
                throw new ItemNotFoundException($"Item with ID {id} not found in the inventory.");
            }
            return _items[id];
        }

        public void RemoveItem(int id)
        {
            if (!_items.ContainsKey(id))
            {
                throw new ItemNotFoundException($"Item with ID {id} not found in the inventory.");
            }
            _items.Remove(id);
        }

        public List<T> GetAllItems()
        {
            return new List<T>(_items.Values);
        }

        public void UpdateQuantity(int id, int newQuantity)
        {
            if (newQuantity < 0)
            {
                throw new InvalidQuantityException("Quantity cannot be negative.");
            }

            if (!_items.ContainsKey(id))
            {
                throw new ItemNotFoundException($"Item with ID {id} not found in the inventory.");
            }

            _items[id].Quantity = newQuantity;
        }
    }

    // Warehouse Manager Class
    public class WarehouseManager
    {
        private InventoryRepository<ElectronicItem> _electronics = new InventoryRepository<ElectronicItem>();
        private InventoryRepository<GroceryItem> _groceries = new InventoryRepository<GroceryItem>();

        public void SeedData()
        {
            // Add electronic items
            _electronics.AddItem(new ElectronicItem(1, "Laptop", 10, "Dell", 24));
            _electronics.AddItem(new ElectronicItem(2, "Smartphone", 15, "Samsung", 12));
            _electronics.AddItem(new ElectronicItem(3, "Headphones", 20, "Sony", 6));

            // Add grocery items
            _groceries.AddItem(new GroceryItem(101, "Milk", 50, DateTime.Now.AddDays(7)));
            _groceries.AddItem(new GroceryItem(102, "Bread", 30, DateTime.Now.AddDays(5)));
            _groceries.AddItem(new GroceryItem(103, "Eggs", 100, DateTime.Now.AddDays(14)));
        }

        public void PrintAllItems<T>(InventoryRepository<T> repo) where T : IInventoryItem
        {
            List<T> items = repo.GetAllItems();
            
            foreach (var item in items)
            {
                Console.WriteLine($"ID: {item.Id}, Name: {item.Name}, Quantity: {item.Quantity}");
                
                if (item is ElectronicItem electronic)
                {
                    Console.WriteLine($"  Brand: {electronic.Brand}, Warranty: {electronic.WarrantyMonths} months");
                }
                else if (item is GroceryItem grocery)
                {
                    Console.WriteLine($"  Expiry Date: {grocery.ExpiryDate.ToShortDateString()}");
                }
            }
            Console.WriteLine();
        }

        public void IncreaseStock<T>(InventoryRepository<T> repo, int id, int quantity) where T : IInventoryItem
        {
            try
            {
                T item = repo.GetItemById(id);
                repo.UpdateQuantity(id, item.Quantity + quantity);
                Console.WriteLine($"Successfully increased quantity for item {id} by {quantity}.");
            }
            catch (ItemNotFoundException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            catch (InvalidQuantityException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
            }
        }

        public void RemoveItemById<T>(InventoryRepository<T> repo, int id) where T : IInventoryItem
        {
            try
            {
                repo.RemoveItem(id);
                Console.WriteLine($"Successfully removed item with ID {id}.");
            }
            catch (ItemNotFoundException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
            }
        }

        public InventoryRepository<ElectronicItem> Electronics => _electronics;
        public InventoryRepository<GroceryItem> Groceries => _groceries;
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Warehouse Inventory Management System");
            Console.WriteLine("=====================================");

            // Instantiate WarehouseManager
            WarehouseManager manager = new WarehouseManager();
            
            // Call SeedData
            manager.SeedData();
            
            // Print all grocery items
            Console.WriteLine("Grocery Items:");
            manager.PrintAllItems(manager.Groceries);
            
            // Print all electronic items
            Console.WriteLine("Electronic Items:");
            manager.PrintAllItems(manager.Electronics);
            
            Console.WriteLine("Testing Exception Handling:");
            Console.WriteLine("==========================");
            
            // Try to add a duplicate item
            try
            {
                Console.WriteLine("Attempting to add a duplicate electronic item...");
                manager.Electronics.AddItem(new ElectronicItem(1, "Duplicate Laptop", 5, "HP", 12));
            }
            catch (DuplicateItemException ex)
            {
                Console.WriteLine($"Expected error: {ex.Message}");
            }
            
            // Try to remove a non-existent item
            Console.WriteLine("\nAttempting to remove a non-existent item...");
            manager.RemoveItemById(manager.Groceries, 999);
            
            // Try to update with invalid quantity
            try
            {
                Console.WriteLine("\nAttempting to update with invalid quantity...");
                manager.Electronics.UpdateQuantity(1, -5);
            }
            catch (InvalidQuantityException ex)
            {
                Console.WriteLine($"Expected error: {ex.Message}");
            }
            
            // Test increasing stock
            Console.WriteLine("\nTesting IncreaseStock method:");
            manager.IncreaseStock(manager.Electronics, 2, 10);
            
            Console.WriteLine("\nUpdated Electronic Items:");
            manager.PrintAllItems(manager.Electronics);
            
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}