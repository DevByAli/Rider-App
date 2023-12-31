
using System.Text.RegularExpressions;

namespace MyLibrary
{
    public class Ride
    {
        Location start_loc;
        Location end_loc;
        int price;
        Driver driver;
        Passenger passenger;
        string rideType;
        int rating;
        const int FUEL_PRICE = 272;
        // Rate Table
        Dictionary<string, Tuple<int, int>> rates;
        public Ride()
        {
            start_loc = new Location();
            end_loc = new Location();
            driver = new Driver();
            passenger = new Passenger();
            rideType = "";

            // Initialize the rate table
            rates = new Dictionary<string, Tuple<int, int>>() { { "bike", Tuple.Create(50, 5) },
                                                                { "rickshaw", Tuple.Create(35, 10) },
                                                                { "car", Tuple.Create(15, 20) }};

        }

        public string RideType { get { return rideType; } }
        public Location Location { get { return start_loc; } }
        public Driver Driver { set { driver = value; } }
        public float Price
        {
            get
            {
                this.calculatePrice();
                return price;
            }
        }

        public string Name { get { return passenger.PassengerName; } }
        public string phoneNo { get { return passenger.PassengerPhone; } }
        public string StartLocation { get { return $"{start_loc.Loc_lat},{start_loc.Loc_lng}"; } }
        public string EndLocation { get { return $"{end_loc.Loc_lat},{end_loc.Loc_lng}"; } }
        public string driverName { get { return driver.Name; } }
        public int Rating { get { return rating; } }

        private void validateLocation(string str, ref bool isValid)
        {
            isValid = false;
            for (int i = 0; i < str.Length; ++i)
            {
                if (isValid && str[i] == ',')
                {
                    isValid = false;
                    break;
                }
                if (str[i] == ',' && isValid == false)
                    isValid = true;
            }
            if (!isValid)
                Console.WriteLine("\nInvalid!\n");
        }

        // Function find the longitude and latitude from string
        private void getLatitudeLongitude(string loc, ref float latitude, ref float longitude)
        {
            // getting the latitude
            latitude = float.Parse(Regex.Match(loc, @"\d+\.*\d*").Value);

            // Separate the input string value by ','
            int separator_index = loc.IndexOf(',');

            // getting the longitude
            longitude = float.Parse(Regex.Match(loc.Substring(separator_index + 1), @"\d+\.*\d*").Value);
        }

        // Check all the characters in phone number are 'digit or not'?
        private bool validatePhoneNo(string? phoneNo)
        {
            if (phoneNo == null)
                return false;
            for (int i = 0; i < phoneNo.Length; i++)
                if (!Char.IsDigit(phoneNo[i]))
                    return false;
            return true;
        }
        public void assignPassenger()
        {
            Console.Write("\nEnter Name: ");
            passenger.PassengerName = Console.ReadLine() ?? "";

            do
            {
                Console.Write("Enter Phone no: ");
                passenger.PassengerPhone = Console.ReadLine() ?? "";
            } while (!validatePhoneNo(passenger.PassengerPhone));
        }
        public void setLocations()
        {
            string loc;
            bool isValid = false;
            do
            {
                Console.Write("Enter start location: ");
                loc = Console.ReadLine() ?? "";
                validateLocation(loc, ref isValid);
            } while (!isValid);

            float latitude = 0, longitude = 0;
            getLatitudeLongitude(loc, ref latitude, ref longitude);
            start_loc.setLocation(latitude, longitude);

            do
            {
                Console.Write("Enter end location: ");
                loc = Console.ReadLine() ?? "";
                validateLocation(loc, ref isValid);
            } while (!isValid);

            getLatitudeLongitude(loc, ref latitude, ref longitude);
            end_loc.setLocation(latitude, longitude);

        }
        public void setRideType()
        {
            do
            {
                Console.Write("Enter Ride Type: ");
                rideType = Console.ReadLine() ?? "";
                rideType = rideType.ToLower();
                if (rideType != "car" && rideType != "rickshaw" && rideType != "bike")
                    Console.WriteLine("Ride Type can be: Car, Rickshaw, Bike\n");
            } while (rideType != "car" && rideType != "rickshaw" && rideType != "bike");

        }
        private void calculatePrice()
        {
            int fuel_average = rates[rideType].Item1;
            int commission = rates[rideType].Item2;
            float distance = (float)Math.Sqrt(Math.Pow(start_loc.Loc_lat - end_loc.Loc_lat, 2) +
                                              Math.Pow(start_loc.Loc_lng - end_loc.Loc_lng, 2));
            this.price = (int)(distance * FUEL_PRICE) / fuel_average + commission;
        }

        public void giveRating()
        {
            do
            {
                Console.Write("Enter rating out of 5: ");
                rating = int.Parse(Console.ReadLine() ?? "1");
                if (rating < 1 || rating > 5)
                    Console.WriteLine("Invalid!");
            } while (rating < 1 || rating > 5);
        }

        public override string ToString()
        {
            return $"Name: {passenger.PassengerName} Rating: {rating}\n";
        }
    }
}
