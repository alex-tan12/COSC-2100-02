// Author: Alex Tan (Lustrial)
// Updated: 2025-10-26
// Car model with camelCase private fields, PascalCase public properties,
// real-time UI updates via INotifyPropertyChanged, and a Display property.

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CarViewer
{
    public class Car : INotifyPropertyChanged
    {
        // ----- Class-level -----
        public static int Count { get; private set; } = 0;

        // ----- Identity -----
        public int IdentificationNumber { get; }

        // ----- Backing fields (camelCase) -----
        private string make = string.Empty;
        private string model = string.Empty;
        private int year;
        private decimal price;
        private bool isNew;

        // ----- Public properties (PascalCase) -----
        public string Make
        {
            get => make;
            set
            {
                if (make != value)
                {
                    make = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(Display));
                }
            }
        }

        public string Model
        {
            get => model;
            set
            {
                if (model != value)
                {
                    model = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(Display));
                }
            }
        }

        public int Year
        {
            get => year;
            set
            {
                if (year != value)
                {
                    year = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(Display));
                }
            }
        }

        public decimal Price
        {
            get => price;
            set
            {
                if (price != value)
                {
                    price = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(Display));
                }
            }
        }

        public bool IsNew
        {
            get => isNew;
            set
            {
                if (isNew != value)
                {
                    isNew = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(Display));
                }
            }
        }

        // What the ListBox shows (bind XAML to this)
        public string Display => $"#{IdentificationNumber}: {Year} {Make} {Model} — {Price:C} [{(IsNew ? "New" : "Used")}]";

        // Constructors
        public Car()
        {
            Count++;
            IdentificationNumber = Count;
        }

        public Car(string make, string model, int year, decimal price, bool isNew) : this()
        {
            this.make = make;
            this.model = model;
            this.year = year;
            this.price = price;
            this.isNew = isNew;
        }

        public override string ToString() => Display;

        // INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
