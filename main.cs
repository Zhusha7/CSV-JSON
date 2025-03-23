using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

class Program
{
    public static void Main()
    {
        string input = "C:\\dev\\CSV-JSON\\data.csv";
        string output = "C:\\dev\\CSV-JSON\\data.json";

        try
        {
            var lines = File.ReadAllLines(input);
            var data = (from line in lines.Skip(1)
                        let parts = line.Split(',')
                        let product = new Entry
                        {
                            Id = int.Parse(parts[0]),
                            Product = parts[1],
                            Category = parts[2],
                            Price = double.Parse(parts[3]),
                            Quantity = int.Parse(parts[4]),
                            Date = DateTime.Parse(parts[5])
                        }
                        where product.Category == "Electronics"
                        orderby product.Price descending
                        group product by product.Category into g
                        select new
                        {
                            Category = g.Key,
                            Products = from p in g
                                       select new { p.Product, p.Price, p.Quantity },
                            TotalValue = g.Sum(p => p.Price * p.Quantity)
                        }
                ).ToList();
            JsonSerializerOptions options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(data, options);
            File.WriteAllText(output, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

}

class Entry
{
    public int Id { get; set; }
    public string? Product { get; set; }
    public string? Category { get; set; }
    public double Price { get; set; }
    public int Quantity { get; set; }
    public DateTime Date { get; set; }
}