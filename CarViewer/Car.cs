// Author: Alex Tan (Lustrial)
// Updated: 2025-11-19
// Description: Car class inheriting from Vehicle.

using System;

namespace CarViewer
{
    public class Car : Vehicle
    {
        public override string Type => "Car";

        public Car() : base()
        {
        }

        public Car(string make, string model, int year, decimal price, bool isNew)
            : base(make, model, year, price, isNew)
        {
        }
    }
}
