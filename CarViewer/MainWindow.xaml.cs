// Authors: Kyle Chapman and Alex Tan
// Updated: November 19, 2025
// Description: A WPF vehicle inventory application to view, add, and analyze vehicles.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace CarViewer
{
    public partial class MainWindow : Window
    {
        private readonly Dictionary<int, Vehicle> _vehiclesById = new();
        private readonly ObservableCollection<Vehicle> _vehiclesView = new();

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Populate makes
            comboMake.ItemsSource = new[]
            {
                "Toyota", "Honda", "Ford", "Chevrolet", "Hyundai",
                "Nissan", "Kia", "Volkswagen", "BMW", "Mercedes-Benz",
                "Subaru", "Mazda", "Dodge"
            };

            // Populate last 50 years
            var currentYear = DateTime.Now.Year;
            var years = Enumerable.Range(currentYear - 49, 50).Reverse().ToList();
            comboYear.ItemsSource = years;

            // Bind the list
            listCars.ItemsSource = _vehiclesView;

            // Default selection
            radioCar.IsChecked = true;

            ResetInputsInternal(false);
            LoadSampleVehicles();
            UpdateStatistics();
            UpdateStatus("Application started. Ready to add vehicles.");
        }

        private bool ValidateInputs(out decimal parsedPrice)
        {
            var errors = new List<string>();
            parsedPrice = 0m;

            if (comboMake.SelectedItem is not string)
            {
                errors.Add("Please select a Make.");
            }

            // Let the Vehicle.Model property handle blank model via an ArgumentNullException.

            if (comboYear.SelectedItem is not int)
            {
                errors.Add("Please select a Year.");
            }

            if (!decimal.TryParse(textPrice.Text, NumberStyles.Number,
                                  CultureInfo.CurrentCulture, out parsedPrice))
            {
                errors.Add("Price must be a valid number.");
            }

            if (errors.Count > 0)
            {
                UpdateStatus("Please fix the following:\n• " + string.Join("\n• ", errors));
                return false;
            }

            return true;
        }

        private void ResetInputs()
        {
            ResetInputsInternal(true);
        }

        private void ResetInputsInternal(bool updateStatus)
        {
            comboMake.SelectedIndex = -1;
            textModel.Clear();
            comboYear.SelectedIndex = -1;
            textPrice.Clear();
            checkIsNew.IsChecked = false;
            listCars.SelectedIndex = -1;
            radioCar.IsChecked = true;

            if (updateStatus)
            {
                UpdateStatus("Inputs cleared.");
            }

            comboMake.Focus();
        }

        private void Enter_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInputs(out var price))
            {
                return;
            }

            var make = (string)comboMake.SelectedItem!;
            var model = textModel.Text;
            var year = (int)comboYear.SelectedItem!;
            var isNew = checkIsNew.IsChecked == true;

            var selectedVehicle = listCars.SelectedItem as Vehicle;

            try
            {
                if (selectedVehicle is null)
                {
                    Vehicle newVehicle = CreateVehicleFromSelection(make, model, year, price, isNew);

                    _vehiclesById[newVehicle.IdentificationNumber] = newVehicle;
                    _vehiclesView.Add(newVehicle);

                    // I should feel bad. :(
                    UpdateStatus($"Added: {newVehicle.Display}");
                }
                else
                {
                    var id = selectedVehicle.IdentificationNumber;
                    if (_vehiclesById.TryGetValue(id, out var vehicle))
                    {
                        vehicle.Make = make;
                        vehicle.Model = model;
                        vehicle.Year = year;
                        vehicle.Price = price;
                        vehicle.IsNew = isNew;

                        CollectionViewSource.GetDefaultView(_vehiclesView).Refresh();
                        // I should feel bad. :(
                        UpdateStatus($"Modified: {vehicle.Display}");
                    }
                }

                UpdateStatistics();
                ResetInputsInternal(false);
            }
            catch (ArgumentNullException)
            {
                UpdateStatus("Model cannot be blank.");
                textModel.Focus();
            }
            catch (ArgumentOutOfRangeException)
            {
                UpdateStatus("Price cannot be negative.");
                textPrice.Focus();
            }
            catch (Exception)
            {
                // I should feel bad. :(
                UpdateStatus("An unexpected error occurred while adding the vehicle.");
            }
        }

        private Vehicle CreateVehicleFromSelection(string make, string model, int year, decimal price, bool isNew)
        {
            // Even if things feel fishy, this factory method ensures
            // the right type of vehicle swims into our collection.

            if (radioMotorcycle.IsChecked == true)
            {
                return new Motorcycle(make, model, year, price, isNew);
            }

            // Default to Car
            return new Car(make, model, year, price, isNew);
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            ResetInputs();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void listCars_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listCars.SelectedItem is not Vehicle vehicle)
            {
                return;
            }

            comboMake.SelectedItem = vehicle.Make;
            textModel.Text = vehicle.Model;
            comboYear.SelectedItem = vehicle.Year;
            textPrice.Text = vehicle.Price.ToString("0.##", CultureInfo.CurrentCulture);
            checkIsNew.IsChecked = vehicle.IsNew;

            if (vehicle.Type == "Motorcycle")
            {
                radioMotorcycle.IsChecked = true;
            }
            else
            {
                radioCar.IsChecked = true;
            }

            UpdateStatus($"Loaded for edit: {vehicle.Display}");
        }

        private void LoadSampleVehicles()
        {
            var samples = new List<Vehicle>
            {
                new Car("Toyota", "Corolla", 2021, 21950m, true),
                new Car("Honda", "Civic", 2020, 20400m, false),
                new Motorcycle("Yamaha", "MT-07", 2022, 9500m, true)
            };

            foreach (var vehicle in samples)
            {
                _vehiclesById[vehicle.IdentificationNumber] = vehicle;
                _vehiclesView.Add(vehicle);
            }

            UpdateStatus($"Loaded {samples.Count} demo vehicles.");
        }

        private void UpdateStatistics()
        {
            int count = _vehiclesView.Count;
            decimal total = _vehiclesView.Sum(v => v.Price);
            decimal average = count > 0 ? total / count : 0m;

            textVehicleCount.Text = count.ToString();
            textTotalPrice.Text = total.ToString("C", CultureInfo.CurrentCulture);
            textAveragePrice.Text = count > 0
                ? average.ToString("C", CultureInfo.CurrentCulture)
                : "N/A";
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tabAdd.IsSelected)
            {
                UpdateStatus("Adding vehicles.");
            }
            else if (tabList.IsSelected)
            {
                UpdateStatus("Viewing vehicle list.");
            }
            else if (tabStats.IsSelected)
            {
                UpdateStatistics();
                UpdateStatus("Viewing inventory statistics.");
            }
        }

        private void UpdateStatus(string message)
        {
            labelResult.Text = $"{DateTime.Now:T} — {message}";
        }
    }
}
