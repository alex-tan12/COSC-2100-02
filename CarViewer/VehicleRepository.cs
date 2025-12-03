/*
 * Name: Alex Tan
 * Date: Dec 3rd 2025
 * Description: Handles saving and loading Vehicle objects to/from a JSON file.
*/
// Handles saving and loading Vehicle objects to/from a JSON file.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace CarViewer
{
    public class VehicleRepository
    {
        private readonly string _filePath;

        public VehicleRepository(string filePath)
        {
            _filePath = filePath;
        }

        // DTO for serialization
        private class VehicleDto
        {
            public string Type { get; set; } = string.Empty;
            public string Make { get; set; } = string.Empty;
            public string Model { get; set; } = string.Empty;
            public int Year { get; set; }
            public decimal Price { get; set; }
            public bool IsNew { get; set; }
        }

        public List<Vehicle> LoadVehicles()
        {
            var vehicles = new List<Vehicle>();

            if (!File.Exists(_filePath))
            {
                return vehicles;
            }

            using var stream = File.Open(_filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

            if (stream.Length == 0)
            {
                return vehicles;
            }

            using var reader = new StreamReader(stream);
            var json = reader.ReadToEnd();

            if (string.IsNullOrWhiteSpace(json))
            {
                return vehicles;
            }

            var dtos = JsonSerializer.Deserialize<List<VehicleDto>>(json);

            if (dtos == null)
            {
                return vehicles;
            }

            foreach (var dto in dtos)
            {
                Vehicle vehicle = dto.Type == "Motorcycle"
                    ? new Motorcycle(dto.Make, dto.Model, dto.Year, dto.Price, dto.IsNew)
                    : new Car(dto.Make, dto.Model, dto.Year, dto.Price, dto.IsNew);

                vehicles.Add(vehicle);
            }

            return vehicles;
        }

        public void SaveVehicles(IEnumerable<Vehicle> vehicles)
        {
            var dtos = vehicles.Select(v => new VehicleDto
            {
                Type = v.Type,
                Make = v.Make,
                Model = v.Model,
                Year = v.Year,
                Price = v.Price,
                IsNew = v.IsNew
            }).ToList();

            var json = JsonSerializer.Serialize(
                dtos,
                new JsonSerializerOptions
                {
                    WriteIndented = true
                });

            var directory = Path.GetDirectoryName(_filePath);
            if (!string.IsNullOrWhiteSpace(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            File.WriteAllText(_filePath, json);
        }
    }
}
