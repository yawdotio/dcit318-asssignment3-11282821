using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

// Define an Immutable Inventory Record
public record InventoryItem(int Id, string Name, int Quantity, DateTime DateAdded) : IInventoryEntity;

//  Define Marker Interface for Logging
public interface IInventoryEntity
{
    int Id { get; }
}

// Create a Generic Inventory Logger
public class InventoryLogger<T> where T : IInventoryEntity
{
    private List<T> _log = new List<T>();
    private readonly string _filePath;

    public InventoryLogger(string filePath)
    {
        _filePath = filePath;
    }

    public void Add(T item)
    {
        _log.Add(item);
    }

    public List<T> GetAll()
    {
        return _log.ToList();
    }

    public void SaveToFile()
    {
        try
        {
            using var writer = new StreamWriter(_filePath);
            var json = JsonSerializer.Serialize(_log, new JsonSerializerOptions { WriteIndented = true });
            writer.Write(json);
            Console.WriteLine($"Successfully saved {_log.Count} items to {_filePath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving to file: {ex.Message}");
        }
    }

    public void LoadFromFile()
    {
        try
        {
            if (File.Exists(_filePath))
            {
                using var reader = new StreamReader(_filePath);
                var json = reader.ReadToEnd();
                _log = JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();
                Console.WriteLine($"Successfully loaded {_log.Count} items from {_filePath}");
            }
            else
            {
                Console.WriteLine($"File not found: {_filePath}");
                _log = new List<T>();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading from file: {ex.Message}");
            _log = new List<T>();
        }
    }
}

// Integration Layer - InventoryApp
public class InventoryApp
{
    private readonly InventoryLogger<InventoryItem> _logger;

    public InventoryApp(string filePath)
    {
        _logger = new InventoryLogger<InventoryItem>(filePath);
    }

    public void SeedSampleData()
    {
        _logger.Add(new InventoryItem(1, "Laptop", 10, DateTime.Now));
        _logger.Add(new InventoryItem(2, "Smartphone", 25, DateTime.Now));
        _logger.Add(new InventoryItem(3, "Tablet", 15, DateTime.Now));
        _logger.Add(new InventoryItem(4, "Headphones", 30, DateTime.Now));
        _logger.Add(new InventoryItem(5, "Monitor", 12, DateTime.Now));
        Console.WriteLine("Sample data seeded successfully.");
    }

    public void SaveData()
    {
        _logger.SaveToFile();
    }

    public void LoadData()
    {
        _logger.LoadFromFile();
    }

    public void PrintAllItems()
    {
        var items = _logger.GetAll();
        Console.WriteLine("\nInventory Items:");
        Console.WriteLine("----------------");
        foreach (var item in items)
        {
            Console.WriteLine($"ID: {item.Id}, Name: {item.Name}, Quantity: {item.Quantity}, Added: {item.DateAdded}");
        }
        Console.WriteLine($"Total Items: {items.Count}");
    }
}

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Inventory Management System");
        Console.WriteLine("==========================");
        
        string filePath = "inventory.json";
        
        // Create an instance of InventoryApp
        var app = new InventoryApp(filePath);
        
        // Seed sample data
        app.SeedSampleData();
        
        // Save data to file
        app.SaveData();
        
        // Clear memory and simulate a new session
        Console.WriteLine("\nSimulating application restart...");
        app = new InventoryApp(filePath);
        
        // Load data from file
        app.LoadData();
        
        // Print all items to confirm data was recovered
        app.PrintAllItems();
        
        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }
}
