using Congestion.Calculator.Enums;
using Congestion.Calculator.Models;
using Congestion.Calculator.Services.DayRuleCheckerService;
using Congestion.Calculator.Services.TaxCalculatorService;
using Congestion.Calculator.Types;

namespace CongestionTaxCalculator.UnitTests
{
    [TestFixture]
    public class TaxCalculatorTests
    {
        private static City GetGothenburgCity() => new()
        {
            Name = "Gothenburg",
            DailyFeeCap = 60,
            MoneyCurrency = Currency.SEK,
            SingleChargeRuleWindow = TimeSpan.FromMinutes(60)
        };

        private static List<TaxRule> GetGothenburgTaxRules()
        {
            return
            [
                new TaxRule { Start = new TimeSpan(6, 0, 0), End = new TimeSpan(6, 29, 59), Amount = 8 },
                new TaxRule { Start = new TimeSpan(6, 30, 0), End = new TimeSpan(6, 59, 59), Amount = 13 },
                new TaxRule { Start = new TimeSpan(7, 0, 0), End = new TimeSpan(7, 59, 59), Amount = 18 },
                new TaxRule { Start = new TimeSpan(8, 0, 0), End = new TimeSpan(8, 29, 59), Amount = 13 },
                new TaxRule { Start = new TimeSpan(8, 30, 0), End = new TimeSpan(14, 59, 59), Amount = 8 },
                new TaxRule { Start = new TimeSpan(15, 0, 0), End = new TimeSpan(15, 29, 59), Amount = 13 },
                new TaxRule { Start = new TimeSpan(15, 30, 0), End = new TimeSpan(16, 59, 59), Amount = 18 },
                new TaxRule { Start = new TimeSpan(17, 0, 0), End = new TimeSpan(17, 59, 59), Amount = 13 },
                new TaxRule { Start = new TimeSpan(18, 0, 0), End = new TimeSpan(18, 29, 59), Amount = 8 },
                new TaxRule { Start = new TimeSpan(18, 30, 0), End = new TimeSpan(23, 59, 59), Amount = 0 },
                new TaxRule { Start = new TimeSpan(0, 0, 0), End = new TimeSpan(5, 59, 59), Amount = 0 }
            ];
        }

        private static List<Holiday> Get2013Holidays()
        {
            return
            [
                new Holiday { Date = new DateTime(2013, 1, 1) },
                new Holiday { Date = new DateTime(2013, 3, 29) }
            ];
        }

        private static List<DateTime> GetColleagueTimestamps()
        {
            return
            [
                DateTime.Parse("2013-01-14 21:00:00"),
                DateTime.Parse("2013-01-15 21:00:00"),
                DateTime.Parse("2013-02-07 06:23:27"),
                DateTime.Parse("2013-02-07 15:27:00"),
                DateTime.Parse("2013-02-08 06:27:00"),
                DateTime.Parse("2013-02-08 06:20:27"),
                DateTime.Parse("2013-02-08 14:35:00"),
                DateTime.Parse("2013-02-08 15:29:00"),
                DateTime.Parse("2013-02-08 15:47:00"),
                DateTime.Parse("2013-02-08 16:01:00"),
                DateTime.Parse("2013-02-08 16:48:00"),
                DateTime.Parse("2013-02-08 17:49:00"),
                DateTime.Parse("2013-02-08 18:29:00"),
                DateTime.Parse("2013-02-08 18:35:00"),
                DateTime.Parse("2013-03-26 14:25:00"),
                DateTime.Parse("2013-03-28 14:07:27")
            ];
        }

        [Test]
        public void CalculateTax_AllAssignmentTimestamps_ReturnsExpectedTotal()
        {
            // Arrange
            City city = GetGothenburgCity();
            List<Holiday> holidays = Get2013Holidays();
            DayRuleChecker dayChecker = new(holidays);

            List<TaxRule> taxRules = GetGothenburgTaxRules();
            TaxCalculator calculator = new(city, taxRules, dayChecker);

            Vehicle vehicle = new() { PlateNumber = "ABC123", Type = VehicleType.Car };

            List<Passage> passages = GetColleagueTimestamps()
                .Select(ts => new Passage { Timestamp = ts, VehiclePlate = vehicle.PlateNumber })
                .ToList();

            // Expected calculation (manual derivation based on the assignment rules):
            // 2013-01-14 21:00 -> 0
            // 2013-01-15 21:00 -> 0
            // 2013-02-07 -> 06:23 (8) + 15:27 (13) = 21
            // 2013-02-08 -> windows -> sum 70 but capped to 60
            // 2013-03-26 -> 14:25 => 8
            // 2013-03-28 -> day before Good Friday => 0
            // Total expected = 0 + 0 + 21 + 60 + 8 + 0 = 89
            const int expectedTotal = 89;

            // Act
            Money totalTax = calculator.CalculateTax(vehicle, passages);

            // Assert
            Assert.That(totalTax.Amount, Is.EqualTo(expectedTotal),
                $"Expected total tax {expectedTotal} but was {totalTax.Amount}.");
        }

        [Test]
        public void CalculateTax_February7_Returns21()
        {
            // Arrange
            City city = GetGothenburgCity();
            List<Holiday> holidays = Get2013Holidays();
            DayRuleChecker dayChecker = new(holidays);
            List<TaxRule> taxRules = GetGothenburgTaxRules();
            ITaxCalculator calculator = new TaxCalculator(city, taxRules, dayChecker);

            Vehicle vehicle = new() { PlateNumber = "CAR1", Type = VehicleType.Car };

            List<Passage> passages = new()
            {
                new Passage { Timestamp = DateTime.Parse("2013-02-07 06:23:27"), VehiclePlate = vehicle.PlateNumber },
                new Passage { Timestamp = DateTime.Parse("2013-02-07 15:27:00"), VehiclePlate = vehicle.PlateNumber }
            };

            const int expected = 21;

            // Act
            Money tax = calculator.CalculateTax(vehicle, passages);

            // Assert
            Assert.That(tax.Amount, Is.EqualTo(expected));
        }

        [Test]
        public void CalculateTax_February8_CappedAt60()
        {
            // Arrange
            City city = GetGothenburgCity();
            List<Holiday> holidays = Get2013Holidays();
            DayRuleChecker dayChecker = new(holidays);
            List<TaxRule> taxRules = GetGothenburgTaxRules();
            TaxCalculator calculator = new(city, taxRules, dayChecker);

            Vehicle vehicle = new() { PlateNumber = "CAR2", Type = VehicleType.Car };

            List<Passage> passages =
            [
                new Passage { Timestamp = DateTime.Parse("2013-02-08 06:20:27"), VehiclePlate = vehicle.PlateNumber },
                new Passage { Timestamp = DateTime.Parse("2013-02-08 06:27:00"), VehiclePlate = vehicle.PlateNumber },
                new Passage { Timestamp = DateTime.Parse("2013-02-08 14:35:00"), VehiclePlate = vehicle.PlateNumber },
                new Passage { Timestamp = DateTime.Parse("2013-02-08 15:29:00"), VehiclePlate = vehicle.PlateNumber },
                new Passage { Timestamp = DateTime.Parse("2013-02-08 15:47:00"), VehiclePlate = vehicle.PlateNumber },
                new Passage { Timestamp = DateTime.Parse("2013-02-08 16:01:00"), VehiclePlate = vehicle.PlateNumber },
                new Passage { Timestamp = DateTime.Parse("2013-02-08 16:48:00"), VehiclePlate = vehicle.PlateNumber },
                new Passage { Timestamp = DateTime.Parse("2013-02-08 17:49:00"), VehiclePlate = vehicle.PlateNumber },
                new Passage { Timestamp = DateTime.Parse("2013-02-08 18:29:00"), VehiclePlate = vehicle.PlateNumber },
                new Passage { Timestamp = DateTime.Parse("2013-02-08 18:35:00"), VehiclePlate = vehicle.PlateNumber }
            ];

            // Act
            Money tax = calculator.CalculateTax(vehicle, passages);

            // Assert
            Assert.That(tax.Amount, Is.EqualTo(city.DailyFeeCap));
        }

        [Test]
        public void CalculateTax_March28_IsDayBeforeHoliday_Free()
        {
            // Arrange
            City city = GetGothenburgCity();
            List<Holiday> holidays = Get2013Holidays();
            DayRuleChecker dayChecker = new(holidays);
            List<TaxRule> taxRules = GetGothenburgTaxRules();
            TaxCalculator calculator = new(city, taxRules, dayChecker);

            Vehicle vehicle = new() { PlateNumber = "CAR3", Type = VehicleType.Car };

            List<Passage> passages =
                [new Passage { Timestamp = DateTime.Parse("2013-03-28 14:07:27"), VehiclePlate = vehicle.PlateNumber }];

            // Act
            Money tax = calculator.CalculateTax(vehicle, passages);

            // Assert
            Assert.That(tax.Amount, Is.EqualTo(0));
        }

        [Test]
        public void CalculateTax_Jan14AndJan15_NightPasses_Zero()
        {
            // Arrange
            City city = GetGothenburgCity();
            List<Holiday> holidays = Get2013Holidays();
            DayRuleChecker dayChecker = new(holidays);
            List<TaxRule> taxRules = GetGothenburgTaxRules();
            TaxCalculator calculator = new(city, taxRules, dayChecker);

            Vehicle vehicle = new() { PlateNumber = "CAR4", Type = VehicleType.Car };

            List<Passage> passages = new()
            {
                new Passage { Timestamp = DateTime.Parse("2013-01-14 21:00:00"), VehiclePlate = vehicle.PlateNumber },
                new Passage { Timestamp = DateTime.Parse("2013-01-15 21:00:00"), VehiclePlate = vehicle.PlateNumber }
            };

            // Act
            Money tax = calculator.CalculateTax(vehicle, passages);

            // Assert
            Assert.That(tax.Amount, Is.EqualTo(0));
        }

        [Test]
        public void CalculateTax_March26_14_25_Returns8()
        {
            // Arrange
            City city = GetGothenburgCity();
            List<Holiday> holidays = Get2013Holidays();
            DayRuleChecker dayChecker = new(holidays);
            List<TaxRule> taxRules = GetGothenburgTaxRules();
            TaxCalculator calculator = new(city, taxRules, dayChecker);

            Vehicle vehicle = new() { PlateNumber = "CAR5", Type = VehicleType.Car };

            List<Passage> passages =
                [new Passage { Timestamp = DateTime.Parse("2013-03-26 14:25:00"), VehiclePlate = vehicle.PlateNumber }];

            // Act
            Money tax = calculator.CalculateTax(vehicle, passages);

            // Assert
            Assert.That(tax.Amount, Is.EqualTo(8));
        }

        [Test]
        public void CalculateTax_TollFreeVehicle_ReturnsZero()
        {
            // Arrange
            City city = GetGothenburgCity();
            List<Holiday> holidays = Get2013Holidays();
            List<TaxRule> taxRules = GetGothenburgTaxRules();
            DayRuleChecker dayChecker = new(holidays);
            TaxCalculator calculator = new(city, taxRules, dayChecker);

            // Motorcycle is toll-free per assignment
            Vehicle vehicle = new() { PlateNumber = "MOTO1", Type = VehicleType.Motorcycle };

            List<Passage> passages = GetColleagueTimestamps()
                .Select(ts => new Passage { Timestamp = ts, VehiclePlate = vehicle.PlateNumber })
                .ToList();

            // Act
            Money tax = calculator.CalculateTax(vehicle, passages);

            // Assert
            Assert.That(tax.Amount, Is.EqualTo(0));
        }

        [Test]
        public void CalculateTax_July_IsFree()
        {
            // Arrange
            City city = GetGothenburgCity();
            List<Holiday> holidays = Get2013Holidays();
            List<TaxRule> taxRules = GetGothenburgTaxRules();
            DayRuleChecker dayChecker = new(holidays);
            TaxCalculator calculator = new(city, taxRules, dayChecker);

            Vehicle vehicle = new() { PlateNumber = "CAR6", Type = VehicleType.Car };

            List<Passage> passages = new()
            {
                new Passage
                {
                    Timestamp = DateTime.Parse("2013-07-10 08:10:00"), VehiclePlate = vehicle.PlateNumber
                }, // middle of July -> free
                new Passage { Timestamp = DateTime.Parse("2013-07-10 15:10:00"), VehiclePlate = vehicle.PlateNumber }
            };

            // Act
            Money tax = calculator.CalculateTax(vehicle, passages);

            // Assert
            Assert.That(tax.Amount, Is.EqualTo(0));
        }
    }
}