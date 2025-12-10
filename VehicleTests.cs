using NUnit.Framework;
using CarViewer; // your main project namespace

namespace CarViewer.Tests
{
    [TestFixture]
    public class VehicleTests
    {
        [Test]
        public void Car_Constructor_SetsPropertiesCorrectly()
        {
            // Arrange
            string make = "Honda";
            string model = "Civic";
            int year = 2020;
            decimal price = 19999.99m;
            bool isNew = true;

            // Act
            var car = new Car(make, model, year, price, isNew);

            // Assert
            Assert.That(car.Type, Is.EqualTo("Car"));
            Assert.That(car.Make, Is.EqualTo(make));
            Assert.That(car.Model, Is.EqualTo(model));
            Assert.That(car.Year, Is.EqualTo(year));
            Assert.That(car.Price, Is.EqualTo(price));
            Assert.That(car.IsNew, Is.EqualTo(isNew));
            Assert.That(car.Display, Does.Contain("Honda"));
            Assert.That(car.Display, Does.Contain("Civic"));
        }

        [Test]
        public void Motorcycle_Constructor_SetsPropertiesCorrectly()
        {
            // Arrange
            string make = "Yamaha";
            string model = "MT-07";
            int year = 2022;
            decimal price = 8999m;
            bool isNew = false;

            // Act
            var bike = new Motorcycle(make, model, year, price, isNew);

            // Assert
            Assert.That(bike.Type, Is.EqualTo("Motorcycle"));
            Assert.That(bike.Make, Is.EqualTo(make));
            Assert.That(bike.Model, Is.EqualTo(model));
            Assert.That(bike.Year, Is.EqualTo(year));
            Assert.That(bike.Price, Is.EqualTo(price));
            Assert.That(bike.IsNew, Is.EqualTo(isNew));
        }

        [Test]
        public void Vehicle_Display_IncludesKeyDetails()
        {
            // Arrange
            var car = new Car("Toyota", "Corolla", 2021, 22000m, true);

            // Act
            string display = car.Display;

            // Assert
            Assert.That(display, Does.Contain("Toyota"));
            Assert.That(display, Does.Contain("Corolla"));
            Assert.That(display, Does.Contain("2021"));
            Assert.That(display, Does.Contain("Car"));
        }

    }

}
