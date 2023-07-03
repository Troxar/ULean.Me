using System;
using System.Globalization;
using System.Linq;
using Ddd.Infrastructure;

namespace Ddd.Taxi.Domain
{
	public interface IDriversRepository
	{
		Driver GetDriverById(int id);
	}

    // In real application it would be the place where database is used to find driver by its Id.
    // But in this exercise it is just a mock to simulate database
    public class DriversRepository : IDriversRepository
	{
		public Driver GetDriverById(int id)
		{
			switch (id)
			{
				case 15:
					return new Driver(id,
						new PersonName("Drive", "Driverson"),
						new Car("Lada sedan", "A123BT 66", Color.Baklazhan));
				default:
					throw new DriverNotFoundException($"Driver id: {id}");
			}
		}
	}

	public class TaxiApi : ITaxiApi<TaxiOrder>
	{
		private readonly IDriversRepository _driversRepo;
		private readonly Func<DateTime> _currentTime;
        private int _orderIdCounter;

        public TaxiApi(IDriversRepository driversRepo, Func<DateTime> currentTime)
		{
			_driversRepo = driversRepo;
			_currentTime = currentTime;
		}

		public TaxiOrder CreateOrderWithoutDestination(string firstName, string lastName, string street, string building)
		{
			return new TaxiOrder(_orderIdCounter++,
				new PersonName(firstName, lastName),
				new Address(street, building),
				_currentTime()); ;
		}

		public void UpdateDestination(TaxiOrder order, string street, string building)
		{
			order.UpdateDestination(new Address(street, building));
		}

		public void AssignDriver(TaxiOrder order, int driverId)
		{
			order.AssignDriver(_driversRepo.GetDriverById(driverId), _currentTime());
		}

		public void UnassignDriver(TaxiOrder order)
		{
			order.UnassignDriver();
		}

		public string GetDriverFullInfo(TaxiOrder order)
		{
			return order.GetDriverFullInfo();
		}

		public string GetShortOrderInfo(TaxiOrder order)
		{
			return order.GetShortOrderInfo();
		}

		public void Cancel(TaxiOrder order)
		{
			order.Cancel(_currentTime());
		}

		public void StartRide(TaxiOrder order)
		{
			order.StartRide(_currentTime());
		}

		public void FinishRide(TaxiOrder order)
		{
			order.FinishRide(_currentTime());
		}
	}

	public class TaxiOrder : Entity<int>
	{
		public PersonName ClientName { get; }
		public Address Start { get; }
		public Address Destination { get; private set; }
		public Driver Driver { get; private set; }
		public Car Car => Driver?.Car;
		public TaxiOrderStatus Status { get; private set; }
        public DateTime CreationTime { get; }
		public DateTime DriverAssignmentTime { get; private set; }
		public DateTime CancelTime { get; private set; }
        public DateTime StartRideTime { get; private set; }
        public DateTime FinishRideTime { get; private set; }

		public TaxiOrder(int id, PersonName clientName, Address address, DateTime time)
			: base(id)
		{
			ClientName = clientName;
			Start = address;
			CreationTime = time;
		}

        public void AssignDriver(Driver driver, DateTime time)
        {
			if (Driver != null)
				throw new InvalidOperationException("Driver is assigned already");

            Status = TaxiOrderStatus.WaitingCarArrival;
            Driver = driver;
            DriverAssignmentTime = time;
        }

        public void UpdateDestination(Address address)
        {
            Destination = address;
        }

        public void UnassignDriver()
        {
			if (Driver is null)
				throw new InvalidOperationException("Driver is not assigned: " + GetShortOrderInfo());
            if (Status == TaxiOrderStatus.InProgress)
                throw new InvalidOperationException("Order is in progress");

            Status = TaxiOrderStatus.WaitingForDriver;
            Driver = null;
        }

        public void Cancel(DateTime time)
        {
			if (Status == TaxiOrderStatus.InProgress)
				throw new InvalidOperationException("Order is in progress");
			
            Status = TaxiOrderStatus.Canceled;
            CancelTime = time;
        }

        public void StartRide(DateTime time)
        {
            if (Driver is null)
                throw new InvalidOperationException("Driver is not assigned");

            Status = TaxiOrderStatus.InProgress;
            StartRideTime = time;
        }

        public void FinishRide(DateTime time)
        {
            if (Driver is null)
                throw new InvalidOperationException("Driver is not assigned");
            if (Status != TaxiOrderStatus.InProgress)
				throw new InvalidOperationException("Order is not in progress");

			Status = TaxiOrderStatus.Finished;
            FinishRideTime = time;
        }

        public DateTime GetLastProgressTime()
        {
			switch (Status)
			{
				case TaxiOrderStatus.WaitingForDriver:
					return CreationTime;
				case TaxiOrderStatus.WaitingCarArrival:
					return DriverAssignmentTime;
				case TaxiOrderStatus.InProgress:
					return StartRideTime;
                case TaxiOrderStatus.Finished:
                    return FinishRideTime;
                case TaxiOrderStatus.Canceled:
                    return CancelTime;
				default:
                    throw new NotSupportedException(Status.ToString());
            }
        }

        public string GetDriverFullInfo()
        {
            if (Status == TaxiOrderStatus.WaitingForDriver)
                return null;

            return string.Join(" ",
                Driver,
                Car);
        }

        public string GetShortOrderInfo()
        {
            return string.Join(" ",
                "OrderId: " + Id,
                "Status: " + Status,
                "Client: " + ClientName.FormatName(),
                "Driver: " + Driver?.Name.FormatName(),
                "From: " + Start.FormatAddress(),
                "To: " + Destination?.FormatAddress(),
                "LastProgressTime: " + GetLastProgressTime()
					.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture));
        }
    }

	public class Driver : Entity<int>
	{
		public readonly PersonName Name;
		public readonly Car Car;

		public Driver(int id, PersonName name, Car car)
			: base(id)
		{
			Name = name;
			Car = car;
		}

        public override string ToString()
        {
			return string.Join(" ",
				"Id: " + Id,
				"DriverName: " + Name.FormatName());
        }
    }

	public class Car : ValueType<Car>
	{
        public string Model { get; }
        public string PlateNumber { get; }
        public Color Color { get; }

        public Car(string model, string plateNumber, Color color)
        {
            Color = color;
            Model = model;
            PlateNumber = plateNumber;
        }

        public override string ToString()
        {
			return string.Join(" ",
				"Color: " + Color,
				"CarModel: " + Model,
				"PlateNumber: " + PlateNumber);
        }
    }

	public class DriverNotFoundException : ApplicationException
	{
		public DriverNotFoundException() { }

		public DriverNotFoundException(string message) : base(message) { }

		public DriverNotFoundException(string message, Exception inner) : base(message, inner) { }
	}

	public static class PersonNameExtensions
	{
		public static string FormatName(this PersonName name)
		{
            return string.Join(" ", new[] { name.FirstName, name.LastName }
                .Where(n => n != null));
        }
	}

    public static class AddressExtensions
    {
        public static string FormatAddress(this Address address)
        {
            return string.Join(" ", new[] { address.Street, address.Building }
                .Where(n => n != null));
        }
    }

    public enum Color
	{
        Baklazhan = 1
    }
}