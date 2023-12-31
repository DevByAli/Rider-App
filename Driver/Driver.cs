
using System.Text.RegularExpressions;

namespace MyLibrary
{
    public class Driver
    {
        int Id;
        string name;
        int age;
        string gender;
        string address;
        string phoneNo;
        Location currLocation;
        Vehicle vehicle;
        List<string> rating;
        int availability;

        public Driver()
        {
            Id = 0;
            name = "";
            age = 0;
            gender = "";
            address = "";
            phoneNo = "";
            currLocation = new Location();
            vehicle = new Vehicle();
            rating = new List<string>();
            availability = 1;
        }

        public int ID
        { get { return Id; } set { this.Id = value; } }
        public string Name
        { get { return name; } set { name = value; } }
        public int Age
        { get { return age; } set { age = value; } }
        public string Gender
        { get { return gender; } set { gender = value; } }
        public string Address
        { get { return address; } set { address = value; } }
        public string PhoneNo
        { get { return phoneNo; } set { phoneNo = value; } }
        public string VehicleType { get { return vehicle.Type; } set { vehicle.Type = value; } }
        public string VehicleModel { get { return vehicle.Model; } set { vehicle.Model = value; } }
        public string VehicleLicensePlate { get { return vehicle.License_plate; } set { vehicle.License_plate = value; } }
        public Location Location { get { return currLocation; } }
        public Vehicle Vehicle { get { return vehicle; } }
        public string RatingList
        {
            set
            {
                string str = value;
                rating.Add(str);
            }
        }
        public int Availability { get { return availability; } set { availability = value; } }

        private void getLatitudeLongitude(string loc, ref float lat, ref float lng)// Utility function
        {
            // getting the latitude
            lat = float.Parse(Regex.Match(loc, @"\d+\.*\d*").Value);

            // Separate the input string value by ','
            int separator_index = loc.IndexOf(',');

            // getting the longitude
            lng = float.Parse(Regex.Match(loc.Substring(separator_index + 1), @"\d+\.*\d*").Value);
        }
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
        public void updateAvailability()
        {
            Console.WriteLine("Mark Driver as: \n1. Available\n2. Offline");
            Console.Write("Choose: ");
            char driverAvailabilityChoice;
            do
            {
                driverAvailabilityChoice = Char.ToLower(Console.ReadLine()[0]);
                if (driverAvailabilityChoice == '1')
                    availability = 1;
                else if (driverAvailabilityChoice == '2')
                    availability = 0;
                else
                    Console.Write("Choose 1-2: ");
            } while (driverAvailabilityChoice != '1' && driverAvailabilityChoice != '2');
        }

        public void getRating()
        {
            for (int i = 0; i < rating.Count; i++)
                Console.WriteLine(rating[i]);
        }

        public void updateLocation()
        {
            string loc;
            bool isValid = false;
            do
            {
                Console.Write("Enter new location: ");
                loc = Console.ReadLine() ?? "";
                validateLocation(loc, ref isValid);
            } while (!isValid);

            float latitude = 0, longitude = 0;
            getLatitudeLongitude(loc, ref latitude, ref longitude);
            currLocation.setLocation(latitude, longitude);
        }
    }
}