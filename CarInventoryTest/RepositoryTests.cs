/*
    Name: Alex Tan
    Date: Dec 8th 2025
    Desc: Car Inventory Tests regarding repository
*/

using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CarViewer;

namespace CarViewer.Tests
{
    [TestFixture]
    public class VehicleRepositoryTests
    {
        private string _tempFilePath = null!;
        private VehicleRepository _repository = null!;

        [SetUp]
        public void SetUp()
        {
            // Create a unique temp file path for each test run
            _tempFilePath = Path.Combine(
                Path.GetTempPath(),
                $"vehicles_test_{Guid.NewGuid():N}.json");

            _repository = new VehicleRepository(_tempFilePath);
        }

        [TearDown]
        public void TearDown()
        {
            // Clean up the temp file after each test
            if (File.Exists(_tempFilePath))
            {
                File.Delete(_tempFilePath);
            }
        }

        [Test]
        public void LoadVehicles_WhenFileDoesNotExist_ReturnsEmptyList()
        {
            // Arrange: ensure the file does not exist
            if (File.Exists(_tempFilePath))
            {
                File.Delete(_tempFilePath);
            }

            // Act
            var result = _repository.LoadVehicles();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(0));
        }

        [Test]
        public void SaveAndLoadVehicles_RoundTripsDataCorrectly()
        {
            // Arrange
            var vehiclesToSave = new List<Vehicle>
            {
                new Car("Honda", "Civic", 2020, 20000m, true),
                new Motorcycle("Yamaha", "R3", 2019, 6500m, false)
            };

            // Act
            _repository.SaveVehicles(vehiclesToSave);
            var loaded = _repository.LoadVehicles();

            // Assert
            Assert.That(loaded.Count, Is.EqualTo(2));

            Assert.That(loaded[0].Type, Is.EqualTo("Car"));
            Assert.That(loaded[0].Make, Is.EqualTo("Honda"));
            Assert.That(loaded[0].Model, Is.EqualTo("Civic"));
            Assert.That(loaded[0].Year, Is.EqualTo(2020));
            Assert.That(loaded[0].Price, Is.EqualTo(20000m));
            Assert.That(loaded[0].IsNew, Is.True);

            Assert.That(loaded[1].Type, Is.EqualTo("Motorcycle"));
            Assert.That(loaded[1].Make, Is.EqualTo("Yamaha"));
            Assert.That(loaded[1].Model, Is.EqualTo("R3"));
            Assert.That(loaded[1].IsNew, Is.False);
        }

        [Test]
        public void DeleteVehicle_RemovesVehicleFromRepository()
        {
            // Arrange
            var vehicles = new List<Vehicle>
            {
                new Car("Toyota", "Corolla", 2021, 22000m, true),
                new Motorcycle("Honda", "CBR500R", 2020, 6500m, false)
            };

            // Save initial set
            _repository.SaveVehicles(vehicles);

            // Load to simulate app reading from disk
            var loaded = _repository.LoadVehicles();
            Assert.That(loaded.Count, Is.EqualTo(2), "Two vehicles should have been loaded before deletion.");

            // Simulate deletion of the first vehicle
            var vehicleToDelete = loaded[0];
            loaded.Remove(vehicleToDelete);

            // Save updated list
            _repository.SaveVehicles(loaded);

            // Reload after deletion
            var afterDelete = _repository.LoadVehicles();

            // Assert
            Assert.That(afterDelete.Count, Is.EqualTo(1), "Only one vehicle should remain after deletion.");

            // Ensure the deleted vehicle is gone
            bool deletedStillThere = afterDelete.Any(v =>
                v.Make == vehicleToDelete.Make &&
                v.Model == vehicleToDelete.Model &&
                v.Year == vehicleToDelete.Year);

            Assert.That(deletedStillThere, Is.False, "Deleted vehicle should not be present in the reloaded collection.");
        }
    }
}
