// Author: Alex Tan (Lustrial)
// Updated: 2025-11-19
// Description: Abstract base class for all vehicles in the inventory.

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CarViewer
{
    public abstract class Vehicle : INotifyPropertyChanged
    {
        // ----- Class-level -----
        public static int Count { get; private set; } = 0;

        // ----- Identity -----
        public int IdentificationNumber { get; }

        // ----- Backing fields -----
        private string _make = string.Empty;
        private string _model = string.Empty;
        private int _year;
        private decimal _price;
        private bool _isNew;

        // ----- Public properties -----
        public string Make
        {
            get => _make;
            set
            {
                if (_make != value)
                {
                    _make = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(Display));
                }
            }
        }

        public string Model
        {
            get => _model;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentNullException(nameof(Model), "Model cannot be blank.");
                }

                if (_model != value)
                {
                    _model = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(Display));
                }
            }
        }

        public int Year
        {
            get => _year;
            set
            {
                if (_year != value)
                {
                    _year = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(Display));
                }
            }
        }

        public decimal Price
        {
            get => _price;
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(Price), "Price cannot be negative.");
                }

                if (_price != value)
                {
                    _price = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(Display));
                }
            }
        }

        public bool IsNew
        {
            get => _isNew;
            set
            {
                if (_isNew != value)
                {
                    _isNew = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(Display));
                }
            }
        }

        // Type of vehicle (Car, Motorcycle, etc.)
        public abstract string Type { get; }

        // What the ListBox shows
        public virtual string Display =>
            $"#{IdentificationNumber}: {Year} {Make} {Model} — {Price:C} [{(IsNew ? "New" : "Used")}] ({Type})";

        // ----- Constructors -----
        protected Vehicle()
        {
            Count++;
            IdentificationNumber = Count;
        }

        protected Vehicle(string make, string model, int year, decimal price, bool isNew) : this()
        {
            Make = make;
            Model = model;
            Year = year;
            Price = price;
            IsNew = isNew;
        }

        public override string ToString() => Display;

        // ----- INotifyPropertyChanged -----
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
