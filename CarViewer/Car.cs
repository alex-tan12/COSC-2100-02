// Description: 
// Represents a car object with make, model, year, price, and availability status.
// Includes a constructor and getter/setter properties.

namespace CarViewer
{
    public class Car
    {
        // Auto-implemented properties for a car's basic information
        public string Make { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public decimal Price { get; set; }
        public bool IsAvailable { get; set; }

        /// 
        /// Parameterized constructor to initialize all fields.
        /// 
        public Car(string make, string model, int year, decimal price, bool isAvailable)
        {
            Make = make;
            Model = model;
            Year = year;
            Price = price;
            IsAvailable = isAvailable;
        }

        /// 
        /// Default constructor (optional) in case you need to create an empty Car object.
        /// 
        public Car() { }

        /// 
        /// Returns a formatted string with the car's key details using ternary operator.
        /// 
        public override string ToString()
        {
            return $"{Year} {Make} {Model} - {Price:C} {(IsAvailable ? "(Available)" : "(Unavailable)")}";
        }
    }
}
