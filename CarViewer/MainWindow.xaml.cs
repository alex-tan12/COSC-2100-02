/*
 * Name: Alex Tan
 * Date: Dec 3rd 2025
 * Description: Car Inventory Management System
*/

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace CarViewer
{
    public partial class MainWindow : Window
    {
        // Dictionary keyed by vehicle ID for quick lookup when modifying/removing.
        private readonly Dictionary<int, Vehicle> _vehiclesById = new();

        // Observable collection used as the ItemsSource for the ListBox.
        // Changes to this collection are automatically reflected in the UI.
        private readonly ObservableCollection<Vehicle> _vehiclesView = new();

        // Repository responsible for loading / saving vehicles to a JSON file.
        private readonly VehicleRepository _repository;

        public MainWindow()
        {
            InitializeComponent();

            // Build the path for the data file (vehicles.json) in the same folder as the executable.
            var dataFilePath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "vehicles.json");

            // Initialize the repository with the data file path.
            _repository = new VehicleRepository(dataFilePath);

            // Register the Loaded event so we can finish setup once the window is ready.
            Loaded += MainWindow_Loaded;
        }

        /// <summary>
        /// Runs once when the window has finished loading.
        /// Used to populate combo boxes, bind the ListBox, and load vehicles.
        /// </summary>
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Populate the "Make" combo box with a selection of vehicle brands.
            comboMake.ItemsSource = new[]
            {
                "Toyota", "Honda", "Ford", "Chevrolet", "Hyundai",
                "Nissan", "Kia", "Volkswagen", "BMW", "Mercedes-Benz",
                "Subaru", "Mazda", "Dodge", "Yamaha", "Harley-Davidson"
            };

            // Populate the "Year" combo box with the last 50 years (including current year).
            var currentYear = DateTime.Now.Year;
            var years = Enumerable.Range(currentYear - 49, 50).Reverse().ToList();
            comboYear.ItemsSource = years;

            // Bind the ListBox to the observable collection.
            listCars.ItemsSource = _vehiclesView;

            // Default the radio button to "Car".
            radioCar.IsChecked = true;

            // Clear all input fields and status text.
            ResetInputsInternal(false);

            // Attempt to load vehicles from JSON file.
            // If none exist, a demo set is loaded instead.
            LoadVehiclesFromRepository();

            // Recalculate counts and update status bar.
            UpdateStatistics();
            UpdateStatus("Application started. Ready to add vehicles.");
        }

        // --------------------------------------------------------------------
        // PERSISTENCE HELPERS
        // --------------------------------------------------------------------

        /// <summary>
        /// Load vehicles from the repository into the in-memory collections.
        /// If file is missing or empty, demo vehicles are loaded and saved.
        /// </summary>
        private void LoadVehiclesFromRepository()
        {
            try
            {
                // Ask the repository to get vehicles from the JSON file.
                var loadedVehicles = _repository.LoadVehicles();

                // Clear any existing data before re-populating.
                _vehiclesById.Clear();
                _vehiclesView.Clear();

                // If no vehicles were loaded (file missing/empty),
                // use demo vehicles and save them for next time.
                if (loadedVehicles.Count == 0)
                {
                    LoadSampleVehicles();
                    SaveVehiclesToRepository();
                    UpdateStatus("No saved inventory found. Loaded demo vehicles.");
                    return;
                }

                // Load each vehicle into both our dictionary and observable collection.
                foreach (var vehicle in loadedVehicles)
                {
                    _vehiclesById[vehicle.IdentificationNumber] = vehicle;
                    _vehiclesView.Add(vehicle);
                }

                UpdateStatus($"Loaded {loadedVehicles.Count} vehicles from storage.");
            }
            catch (Exception ex)
            {
                // If anything goes wrong (file missing, corrupted JSON, etc.),
                // fall back to demo data but do not crash the program.
                _vehiclesById.Clear();
                _vehiclesView.Clear();
                LoadSampleVehicles();
                UpdateStatus($"Could not load saved vehicles. Demo data loaded instead. Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Save the current observable collection of vehicles to JSON using the repository.
        /// Any IO or serialization errors are reported via the status bar.
        /// </summary>
        private void SaveVehiclesToRepository()
        {
            try
            {
                _repository.SaveVehicles(_vehiclesView);
                UpdateStatus("Inventory saved to persistent storage.");
            }
            catch (Exception ex)
            {
                // Display error in status bar but do not crash.
                UpdateStatus($"Unable to save vehicles: {ex.Message}");
            }
        }

        /// <summary>
        /// Load a fixed set of sample vehicles into the in-memory collections.
        /// Used if there is no data file or if loading fails.
        /// </summary>
        private void LoadSampleVehicles()
        {
            _vehiclesById.Clear();
            _vehiclesView.Clear();

            var demoVehicles = new List<Vehicle>
            {
                /*
                new Car("Toyota", "Corolla", 2021, 22500m, true),
                new Car("Honda", "Civic", 2019, 18999m, false),
                new Car("Ford", "F-150", 2020, 38999m, false),
                new Motorcycle("Yamaha", "MT-07", 2022, 8999m, true),
                new Motorcycle("Harley-Davidson", "Street 750", 2018, 10499m, false)
                */
            };

            foreach (var v in demoVehicles)
            {
                _vehiclesById[v.IdentificationNumber] = v;
                _vehiclesView.Add(v);
            }
        }

        // --------------------------------------------------------------------
        // GENERAL UI HELPERS
        // --------------------------------------------------------------------

        /// <summary>
        /// Update the text in the status bar.
        /// </summary>
        private void UpdateStatus(string message)
        {
            if (labelStatus != null)
            {
                labelStatus.Text = message;
            }
        }

        /// <summary>
        /// Recalculate and display basic statistics:
        /// total vehicles, number of cars, and number of motorcycles.
        /// </summary>
        private void UpdateStatistics()
        {
            int total = _vehiclesView.Count;
            int cars = _vehiclesView.OfType<Car>().Count();
            int bikes = _vehiclesView.OfType<Motorcycle>().Count();

            UpdateStatus($"Vehicles: {total} (Cars: {cars}, Motorcycles: {bikes})");
        }

        /// <summary>
        /// Reset all input fields on the Add/Edit tab to their default state.
        /// Optionally sets keyboard focus to the Make combo box.
        /// </summary>
        private void ResetInputsInternal(bool setFocus)
        {
            // Reset basic fields.
            comboMake.SelectedIndex = -1;
            textModel.Text = string.Empty;
            comboYear.SelectedIndex = -1;
            textPrice.Text = string.Empty;
            checkIsNew.IsChecked = false;

            // Default type to Car.
            radioCar.IsChecked = true;

            // Clear any list selection.
            listCars.SelectedItem = null;

            // Optionally give focus back to the "Make" combo box.
            if (setFocus)
            {
                comboMake.Focus();
            }

            UpdateStatus("Input fields cleared.");
        }

        // --------------------------------------------------------------------
        // VALIDATION & CREATION
        // --------------------------------------------------------------------

        /// <summary>
        /// Validates user input from the Add/Edit tab.
        /// Returns true if all fields are valid and populates out parameters;
        /// otherwise shows a message box with errors and returns false.
        /// </summary>
        private bool ValidateInputs(out string make, out string model, out int year, out decimal price, out bool isNew)
        {
            var errors = new List<string>();

            // Read current values from controls.
            make = comboMake.SelectedItem as string ?? string.Empty;
            model = textModel.Text.Trim();
            isNew = checkIsNew.IsChecked == true;

            // Validate Make.
            if (string.IsNullOrWhiteSpace(make))
            {
                errors.Add("Make is required.");
            }

            // Validate Model.
            if (string.IsNullOrWhiteSpace(model))
            {
                errors.Add("Model is required.");
            }

            // Validate Year combo selection.
            if (comboYear.SelectedItem is int selectedYear)
            {
                year = selectedYear;
            }
            else
            {
                errors.Add("Year is required.");
                year = 0;
            }

            // Validate Price: must parse and must be non-negative.
            if (!decimal.TryParse(textPrice.Text, NumberStyles.Number, CultureInfo.CurrentCulture, out price) ||
                price < 0)
            {
                errors.Add("Price must be a non-negative number.");
            }

            // If any errors were collected, show them and return false.
            if (errors.Count > 0)
            {
                MessageBox.Show(
                    string.Join(Environment.NewLine, errors),
                    "Validation Errors",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                UpdateStatus("Validation errors occurred. Please correct the fields.");
                return false;
            }

            // All good.
            return true;
        }

        /// <summary>
        /// Creates a specific Vehicle instance (Car or Motorcycle)
        /// based on the currently selected radio button.
        /// </summary>
        private Vehicle CreateVehicleFromSelection(string make, string model, int year, decimal price, bool isNew)
        {
            // If the Motorcycle radio button is checked, create a Motorcycle.
            if (radioMotorcycle.IsChecked == true)
            {
                return new Motorcycle(make, model, year, price, isNew);
            }

            // Otherwise default to creating a Car.
            return new Car(make, model, year, price, isNew);
        }

        // --------------------------------------------------------------------
        // BUTTON & CONTROL EVENT HANDLERS
        // --------------------------------------------------------------------

        /// <summary>
        /// Handles the Enter button click:
        /// - If nothing is selected in the list, adds a new vehicle.
        /// - If a vehicle is selected, modifies that existing vehicle.
        /// Afterwards, saves to disk and updates statistics.
        /// </summary>
        private void Enter_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validate the input fields and collect values.
                if (!ValidateInputs(out var make, out var model, out var year, out var price, out var isNew))
                {
                    // Validation failed; user has already been notified.
                    return;
                }

                // If no item is selected in the list, we are adding a new vehicle.
                if (listCars.SelectedItem is not Vehicle selectedVehicle)
                {
                    // Create a Car or Motorcycle based on the radio buttons.
                    Vehicle newVehicle = CreateVehicleFromSelection(make, model, year, price, isNew);

                    // Store in both the dictionary and observable collection.
                    _vehiclesById[newVehicle.IdentificationNumber] = newVehicle;
                    _vehiclesView.Add(newVehicle);

                    UpdateStatus($"Added: {newVehicle.Display}");
                }
                else
                {
                    // We are modifying an existing vehicle.
                    var id = selectedVehicle.IdentificationNumber;

                    // Find the vehicle by its ID in our dictionary.
                    if (_vehiclesById.TryGetValue(id, out var vehicle))
                    {
                        // Update properties.
                        vehicle.Make = make;
                        vehicle.Model = model;
                        vehicle.Year = year;
                        vehicle.Price = price;
                        vehicle.IsNew = isNew;

                        // Force the ListBox to refresh its display.
                        CollectionViewSource.GetDefaultView(_vehiclesView).Refresh();

                        UpdateStatus($"Modified: {vehicle.Display}");
                    }
                    else
                    {
                        // If we cannot find it, something went out of sync.
                        UpdateStatus("Unable to locate the selected vehicle in the data source.");
                    }
                }

                // Persist the updated collection to the JSON file.
                SaveVehiclesToRepository();

                // Recalculate counts and clear input fields.
                UpdateStatistics();
                ResetInputsInternal(false);
            }
            catch (Exception ex)
            {
                // Catch any unexpected error and show a message box.
                MessageBox.Show(
                    $"An unexpected error occurred: {ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                UpdateStatus("An unexpected error occurred while saving the vehicle.");
            }
        }

        /// <summary>
        /// Handles Reset button click on the Add/Edit tab.
        /// Clears all input fields and refocuses the Make combo box.
        /// </summary>
        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            ResetInputsInternal(true);
        }

        /// <summary>
        /// Handles the Exit button and File &gt; Exit menu click.
        /// Closes the application window.
        /// </summary>
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// When the selection in the ListBox changes, load that vehicle's
        /// details into the Add/Edit tab for editing.
        /// </summary>
        private void listCars_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // If nothing is selected, do nothing.
            if (listCars.SelectedItem is not Vehicle selected)
            {
                return;
            }

            // Populate the fields with the selected vehicle's data.
            comboMake.SelectedItem = selected.Make;
            textModel.Text = selected.Model;
            comboYear.SelectedItem = selected.Year;
            textPrice.Text = selected.Price.ToString("0.##", CultureInfo.CurrentCulture);
            checkIsNew.IsChecked = selected.IsNew;

            // Switch the type radio button based on the runtime type.
            if (selected is Motorcycle)
            {
                radioMotorcycle.IsChecked = true;
            }
            else
            {
                radioCar.IsChecked = true;
            }

            // Move to the Add/Edit tab so the user can see and change details.
            tabControl.SelectedItem = tabAdd;

            UpdateStatus($"Loaded vehicle #{selected.IdentificationNumber} into the Add Vehicles tab.");
        }

        /// <summary>
        /// Handles the Remove Selected button click on the Vehicle List tab.
        /// Removes the selected vehicle from both the list and persistent storage.
        /// </summary>
        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            // Make sure a vehicle is selected before attempting removal.
            if (listCars.SelectedItem is not Vehicle selected)
            {
                UpdateStatus("Please select a vehicle to remove.");
                return;
            }

            // Ask the user to confirm deletion.
            var result = MessageBox.Show(
                $"Are you sure you want to permanently remove vehicle #{selected.IdentificationNumber}?\n\n{selected.Display}",
                "Confirm Remove",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
            {
                // User chose not to delete.
                return;
            }

            var id = selected.IdentificationNumber;

            // Remove from the dictionary first.
            if (_vehiclesById.Remove(id))
            {
                // Then remove from the observable collection (which updates the ListBox).
                _vehiclesView.Remove(selected);

                // Save the updated list and refresh stats / inputs.
                SaveVehiclesToRepository();
                UpdateStatistics();
                ResetInputsInternal(false);
                UpdateStatus($"Removed vehicle #{id} from the inventory.");
            }
            else
            {
                // If we couldn't find the vehicle by ID, collections are out of sync.
                UpdateStatus("Unable to find the selected vehicle in the data source.");
            }
        }

        /// <summary>
        /// Keeps the status bar text in sync with the selected tab.
        /// </summary>
        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tabControl.SelectedItem == tabAdd)
            {
                UpdateStatus("Ready to add or edit a vehicle.");
            }
            else if (tabControl.SelectedItem == tabList)
            {
                UpdateStatus("Viewing vehicle inventory.");
            }
        }

        // --------------------------------------------------------------------
        // MENU HANDLERS
        // --------------------------------------------------------------------

        /// <summary>
        /// File &gt; Save Inventory menu item.
        /// Manually persists the current inventory to the JSON file.
        /// </summary>
        private void MenuSave_Click(object sender, RoutedEventArgs e)
        {
            SaveVehiclesToRepository();
        }

        /// <summary>
        /// File &gt; Reload Inventory menu item.
        /// Reloads vehicles from the JSON file (or demo data if needed).
        /// </summary>
        private void MenuReload_Click(object sender, RoutedEventArgs e)
        {
            LoadVehiclesFromRepository();
            UpdateStatistics();
        }

        /// <summary>
        /// Tools &gt; Clear Inputs menu item.
        /// Clears only the Add/Edit page controls.
        /// </summary>
        private void MenuClearInputs_Click(object sender, RoutedEventArgs e)
        {
            ResetInputsInternal(true);
        }

        /// <summary>
        /// Tools &gt; Clear Inventory menu item.
        /// Completely wipes the in-memory list and saves an empty collection.
        /// </summary>
        private void MenuClearInventory_Click(object sender, RoutedEventArgs e)
        {
            // No vehicles? Nothing to clear.
            if (_vehiclesView.Count == 0)
            {
                UpdateStatus("There are no vehicles to clear.");
                return;
            }

            // Confirm that the user wants to remove everything.
            var result = MessageBox.Show(
                "This will remove all vehicles from the inventory and from persistent storage. Continue?",
                "Clear Inventory",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes)
            {
                return;
            }

            // Clear both the dictionary and observable collection.
            _vehiclesById.Clear();
            _vehiclesView.Clear();

            // Persist the now-empty collection and reset UI.
            SaveVehiclesToRepository();
            UpdateStatistics();
            ResetInputsInternal(false);
            UpdateStatus("All vehicles were removed from the inventory.");
        }

        /// <summary>
        /// Help &gt; About menu item.
        /// Shows a basic About dialog with program information.
        /// </summary>
        private void MenuAbout_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "Vehicle Inventory\nCOSC2100 – Assignment 5\n\nAuthor: Alex Tan (Lustrial)\nA Car Inventory Management System.",
                "About Vehicle Inventory",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
    }
}
