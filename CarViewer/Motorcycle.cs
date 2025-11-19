// Author: Alex Tan (Lustrial)
// Updated: 2025-11-19
// Description: Motorcycle class inheriting from Vehicle.

using System;

namespace CarViewer
{
    public class Motorcycle : Vehicle
    {
        public override string Type => "Motorcycle";

        public Motorcycle() : base()
        {
        }

        public Motorcycle(string make, string model, int year, decimal price, bool isNew)
            : base(make, model, year, price, isNew)
        {
        }
    }
}
