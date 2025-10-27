// Author: Alex Tan (Lustrial)
// Date: 2025-10-25
// Description: A WPF Car Inventory application to view and add car models.


using System;

namespace CarViewer
{
    public class Car
    {
        // ----- Class-level (static) -----
        // Read-only count of all created Car objects.
        public static int Count { get; private set; } = 0;

        // ----- Instance-level -----
        // Read-only unique identifier per assignment requirements.
        public int IdentificationNumber { get; }

        public string Make { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public int Year { get; set; }
        public decimal Price { get; set; }
        public bool IsNew { get; set; }

        // Default constructor:
        //  - Assigns IdentificationNumber based on updated Count
        public Car()
        {
            Count++;
            IdentificationNumber = Count;
        }

        // Parameterized constructor:
        //  - sets all properties from parameters
        public Car(string make, string model, int year, decimal price, bool isNew)
            : this()
        {
            Make = make;
            Model = model;
            Year = year;
            Price = price;
            IsNew = isNew;
        }

        // Summary for list display/result label
        public override string ToString()
        {
            var condition = IsNew ? "New" : "Used";
            return $"#{IdentificationNumber}: {Year} {Make} {Model} — {Price:C} [{condition}]";
        }
    }
}
