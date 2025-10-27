// Authors: Kyle Chapman and Alex Tan
// Updated: October 26, 2025
// Description: WPF logic for the COSC2100 Assignment 2 "Car List" app

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace CarViewer
{
    public partial class MainWindow : Window
    {
        // Private fields in camelCase
        private readonly Dictionary<int, Car> carsById = new();
        private readonly ObservableCollection<Car> carsView = new();

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Populate makes (>=10)
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
            listCars.ItemsSource = carsView;

            // Defaults
            ResetInputs();

            // Load some demo cars (optional)
            LoadSampleCars();
        }

        private bool ValidateInputs(out List<string> errors)
        {
            errors = new List<string>();

            if (comboMake.SelectedItem is not string)
                errors.Add("Please select a Make.");

            if (string.IsNullOrWhiteSpace(textModel.Text))
                errors.Add("Model cannot be blank.");

            if (comboYear.SelectedItem is not int)
                errors.Add("Please select a Year.");

            if (!decimal.TryParse(textPrice.Text, NumberStyles.Number,
                CultureInfo.CurrentCulture, out var price) || price < 0)
                errors.Add("Price must be a non-negative number.");

            if (errors.Count > 0)
                labelResult.Text = "Please fix the following:\n• " + string.Join("\n• ", errors);

            return errors.Count == 0;
        }

        private void ResetInputs()
        {
            comboMake.SelectedIndex = -1;
            textModel.Clear();
            comboYear.SelectedIndex = -1;
            textPrice.Clear();
            checkIsNew.IsChecked = false;
            listCars.SelectedIndex = -1;
            labelResult.Text = "Ready.";
            comboMake.Focus();
        }

        private void Enter_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInputs(out _))
                return;

            var make = (string)comboMake.SelectedItem!;
            var model = textModel.Text.Trim();
            var year = (int)comboYear.SelectedItem!;
            var price = decimal.Parse(textPrice.Text, CultureInfo.CurrentCulture);
            var isNew = checkIsNew.IsChecked == true;

            var selectedCar = listCars.SelectedItem as Car;

            if (selectedCar is null)
            {
                var newCar = new Car(make, model, year, price, isNew);
                carsById[newCar.IdentificationNumber] = newCar;
                carsView.Add(newCar);
                labelResult.Text = $"Added: {newCar}";
            }
            else
            {
                var id = selectedCar.IdentificationNumber;
                if (carsById.TryGetValue(id, out var car))
                {
                    car.Make = make;
                    car.Model = model;
                    car.Year = year;
                    car.Price = price;
                    car.IsNew = isNew;

                    // Refresh ListBox
                    CollectionViewSource.GetDefaultView(carsView).Refresh();
                    labelResult.Text = $"Modified: {car}";
                }
            }

            ResetInputs();
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            ResetInputs();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void listCars_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (listCars.SelectedItem is not Car car)
                return;

            comboMake.SelectedItem = car.Make;
            textModel.Text = car.Model;
            comboYear.SelectedItem = car.Year;
            textPrice.Text = car.Price.ToString("0.##", CultureInfo.CurrentCulture);
            checkIsNew.IsChecked = car.IsNew;

            labelResult.Text = $"Loaded for edit: {car}";
        }

        private void LoadSampleCars()
        {
            var samples = new List<Car>
            {
                new Car("Toyota", "Corolla", 2021, 21950m, true),
                new Car("Honda", "Civic", 2020, 20400m, false),
                new Car("Ford", "F-150", 2022, 38900m, true),
                new Car("Hyundai", "Elantra", 2019, 18200m, false),
                new Car("BMW", "X5", 2023, 74900m, true)
            };

            foreach (var car in samples)
            {
                carsById[car.IdentificationNumber] = car;
                carsView.Add(car);
            }

            labelResult.Text = $"Loaded {samples.Count} demo cars.";
        }
    }
}
