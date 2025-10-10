// Author:  Kyle Chapman AND Alex Tan
// (please don't leave the previous line saying "AND YOU".)
// Created: October 1, 2025
// Updated: October 9th, 2025
// Description:
// Code for a WPF form to display car objects. Currently, it is purely for
// display, and shows a car's make, model, year and price. Creating the car
// class to support the function of this program is meant as an exercise.
// Functionality should exist to move through the list of car objects.
// See the Car class (which you will create in a separate file!) for more details.

using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CarViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // "Global" or "class-level" variables, including a list for cars.
        // If there's an issue on this line, it means you haven't defined a Car class (which is the point of the exercise).
        List<Car> listOfCars = new List<Car>();
        int currentIndex = 0;

        /// <summary>
        /// Constructor for the form class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// When the form loads, initialize the list of cars.
        /// </summary>
        private void FormLoad(object sender, RoutedEventArgs e)
        {
            // Create some default car objects.
            // You'll be encouraged to add a few more!
            // If these lines are in error, either the Car class is missing or there's an issue with its parametrized constructor.
            var carOne = new Car("Hyundai", "Tucson", 2020, 17500m, true);
            var carTwo = new Car("Dodge", "Caliber", 2012, 11499m, true);
            var carThree = new Car("Volkswagen", "Beetle", 1979, 5999m, false);

            // Add the car objects into the list.
            // It's also technically possible to manage the list within the class.
            // Can you think of any positives or negatives of this?
            listOfCars.Add(carOne);
            listOfCars.Add(carTwo);
            listOfCars.Add(carThree);

            // Show the first car.
            DisplayCar(listOfCars[0]);
        }

        /// <summary>
        /// Displays a car object in the form.
        /// </summary>
        /// <param name="currentCar">A valid car object.</param>
        private void DisplayCar(Car currentCar)
        {
            // If the method signature above has an error in it, the Car class doesn't exist.
            // If the lines below cause errors, then the Car class' properties aren't set up correctly.

            // Set form control properties based on values of the car passed in.
            textMake.Text = currentCar.Make;
            textModel.Text = currentCar.Model;
            textYear.Text = currentCar.Year.ToString();
            textPrice.Text = currentCar.Price.ToString("c");
        }

        /// <summary>
        /// Display the first car from the list in the form.
        /// </summary>
        private void ShowFirstCar(object sender, RoutedEventArgs e)
        {
            // Set the index to 0, and show it.
            currentIndex = 0;
            DisplayCar(listOfCars[currentIndex]);
            // Previous is disabled, Next is enabled.
            buttonPrevious.IsEnabled = false;
            buttonNext.IsEnabled = true;
        }

        /// <summary>
        /// Display the last car in the list in the form.
        /// </summary>
        private void ShowLastCar(object sender, RoutedEventArgs e)
        {
            // Set the index to the end of the list, and show it.
            currentIndex = listOfCars.Count - 1;
            DisplayCar(listOfCars[currentIndex]);
            // Next is disabled, Previous is enabled.
            buttonPrevious.IsEnabled = true;
            buttonNext.IsEnabled = false;
        }

        /// <summary>
        /// Display the previous car in the list in the form.
        /// </summary>
        private void ShowPreviousCar(object sender, RoutedEventArgs e)
        {
            // Reduce the current index by 1.
            currentIndex -= 1;
            // If the index is 0 (or less?), set it to 0 and disable Previous.
            if (currentIndex <= 0)
            {
                currentIndex = 0;
                buttonPrevious.IsEnabled = false;
            }
            // Display the car at this index and enable the Next button.
            DisplayCar(listOfCars[currentIndex]);
            buttonNext.IsEnabled = true;
        }

        /// <summary>
        /// Display the next car from the list in the form.
        /// </summary>
        private void ShowNextCar(object sender, RoutedEventArgs e)
        {
            // Increase the current index by 1.
            currentIndex += 1;
            // If at the end of the list, set the index to the maximum and disable Next.
            if (currentIndex >= listOfCars.Count - 1)
            {
                currentIndex = listOfCars.Count - 1;
                buttonNext.IsEnabled = false;
            }
            // Display the car at this index and enable the Previous button.
            DisplayCar(listOfCars[currentIndex]);
            buttonPrevious.IsEnabled = true;
        }
    }
}