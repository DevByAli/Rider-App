
using Microsoft.Data.SqlClient;
using System.Globalization;

namespace MyLibrary
{
    public class Admin
    {
        List<Driver> driversList;
        string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Driver;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";
        SqlConnection connection;


        private void setup()
        {

            string selectQuery = "select * from driver;";

            SqlCommand command = new SqlCommand(selectQuery, connection);
            SqlDataReader reader = command.ExecuteReader();
            if (!reader.HasRows)
                return;

            while (reader.Read())
            {
                Driver driver = new Driver();

                driver.ID = (int)reader[0];
                driver.Name = (string)reader[1];
                driver.Age = (int)reader[2];
                driver.Gender = (string)reader[3];
                driver.PhoneNo = (string)reader[4];
                driver.Address = (string)reader[5];
                driver.VehicleType = (string)reader[6];
                driver.VehicleModel = (string)reader[7];
                driver.VehicleLicensePlate = (string)reader[8];
                driver.Availability = Convert.ToInt32((bool)reader[9]);
                if (reader[9] != null && reader[10] != null)
                    driver.Location.setLocation(Convert.ToSingle(reader[10]), Convert.ToSingle(reader[11]));
                
                // Add driver in driversList
                driversList.Add(driver);
            }
            reader.Close();
        }

        // Before closing the program this function will write the changes in file
        public void dispose()
        {
            connection.Open();
            string deleteQuery = "DELETE FROM DRIVER;";
            SqlCommand command = new SqlCommand(deleteQuery, connection);
            command.ExecuteNonQuery();
            command.Dispose();
            string resetPrimaryKey = "DBCC CHECKIDENT('DRIVER', RESEED, 0);";
            command = new SqlCommand(resetPrimaryKey, connection);
            command.ExecuteNonQuery();


            for (int i = 0; i < driversList.Count; i++)
            {
                string insertQuery = $"insert into driver values('{driversList[i].Name}',{driversList[i].Age}," +
                    $"'{driversList[i].Gender.ToLower()}','{driversList[i].PhoneNo}','{driversList[i].Address}'," +
                    $"'{driversList[i].VehicleType}','{driversList[i].VehicleModel}'," +
                    $"'{driversList[i].VehicleLicensePlate}',{driversList[i].Availability}," +
                    $"{driversList[i].Location.Loc_lat},{driversList[i].Location.Loc_lng});";

                command.Dispose();
                command = new SqlCommand(insertQuery, connection);
                command.ExecuteNonQuery();
            }
            connection.Close();
        }
        public Admin()
        {
            driversList = new List<Driver>();
            connection = new SqlConnection(connectionString);
            connection.Open();
            setup();
            connection.Close();
        }

        public SqlConnection SqlConnection { get { return connection; } }

        public Tuple<bool, Driver> findDriver(int id, string name)
        {
            for (int i = 0; i < driversList.Count; i++)
            {
                if (driversList[i].ID == id && driversList[i].Name.Equals(name, StringComparison.CurrentCultureIgnoreCase))
                    return Tuple.Create(true, driversList[i]);
            }
            // driversList[0] is use as a dummy driver object
            return Tuple.Create(false, new Driver());
        }
        private string getValue()
        {
            string value;
            bool valid;
            do
            {
                valid = true;
                value = (Console.ReadLine() ?? "").Trim();
                if (isNull(value))
                    return string.Empty;
                for (int i = 0; i < value.Length; i++)
                {
                    if (!Char.IsDigit(value[i]))
                    {
                        valid = false;
                        Console.Write("Only contain digits\nEnter again: ");
                        break;
                    }
                }
            } while (!valid);
            return value;
        }

        private string getVehicleType()
        {
            string type;
            List<string> vehicleTypeList = new List<string>() { "car", "bike", "rickshaw" };
            do
            {
                type = (Console.ReadLine() ?? "").Trim();
                if (isNull(type))
                    return string.Empty;
                foreach (string str in vehicleTypeList)
                {
                    if (str.Equals(type, StringComparison.CurrentCultureIgnoreCase))
                        return type.ToLower();
                }
                Console.WriteLine("Vehicle should be:  \"car\", \"bike\", \"rickshaw\"");
                Console.Write("Enter Vehicle Type: ");
            } while (true);
        }

        private bool isExist(int id)
        {
            for (int i = 0; i < driversList.Count; ++i)
                if (driversList[i].ID == id)
                    return true;
            return false;
        }

        private bool validatePhoneNo(string? phoneNo)
        {
            if (phoneNo == null)
                return false;
            for (int i = 0; i < phoneNo.Length; i++)
                if (!Char.IsDigit(phoneNo[i]))
                    return false;
            return true;
        }

        private bool validateGender(string gender)
        {
            gender = gender.ToLower();
            return gender == "male" || gender == "female";
        }

        public void addDriver()
        {
            Driver newDriver = new Driver();

            newDriver.ID = driversList.Count + 1;
            Console.Write("Enter Name: ");
            string name = Console.ReadLine() ?? "";
            newDriver.Name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(name.ToLower());

            Console.Write("Enter Age: ");
            string age = getValue();
            while (isNull(age))
            {
                Console.Write("Enter Age: ");
                age = getValue();
            }
            newDriver.Age = int.Parse(age);

            string gender;
            do
            {
                Console.Write("Enter Gender: ");
                gender = Console.ReadLine() ?? "";
            } while (!validateGender(gender));
            newDriver.Gender = gender;

            string phoneno;
            do
            {
                Console.Write("Enter Phone No.: ");
                phoneno = Console.ReadLine() ?? "";
            } while (!validatePhoneNo(phoneno));
            newDriver.PhoneNo = phoneno;

            Console.Write("Enter Address: ");
            newDriver.Address = Console.ReadLine() ?? "";

            Console.Write("Enter Vehicle Type: ");
            string type = getVehicleType();
            while (isNull(type))
            {
                Console.Write("Enter Vehicle Type: ");
                type = getVehicleType();
            }
            newDriver.VehicleType = type;

            Console.Write("Enter Vehicle Model: ");
            newDriver.VehicleModel = Console.ReadLine() ?? "";

            Console.Write("Enter Vehicle License Plate: ");
            newDriver.VehicleLicensePlate = Console.ReadLine() ?? "";

            // Driver is add to driver list
            driversList.Add(newDriver);
        }
        public void removeDriver()
        {
            Console.Write("Enter ID: ");
            string idString = getValue();
            while (isNull(idString))
            {
                Console.Write("Enter ID: ");
                idString = getValue();
            }
            int id = int.Parse(idString);

            Console.Write("Enter Name: ");
            string name = Console.ReadLine() ?? "";

            // Find and remove the driver from drivers list
            for (int i = 0; i < driversList.Count; ++i)
            {
                if (driversList[i].ID == id && string.Equals(driversList[i].Name, name, StringComparison.OrdinalIgnoreCase))
                {
                    driversList.RemoveAt(i);
                    Console.WriteLine($"Driver of ID: {id} removed");

                    connection.Open();
                    string query = $"delete from rider where driver_id={id};";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.ExecuteNonQuery();
                    command.Dispose();
                    connection.Close();
                    return;
                }
            }
            Console.WriteLine($"Driver of ID: {id}  Name: {name} not found\n");
        }
        public void updateDriver()
        {
            Console.Write("Enter ID: ");
            string idString = getValue();
            while (isNull(idString))
            {
                Console.Write("Enter ID: ");
                idString = getValue();
            }
            int id = int.Parse(idString);

            // Searching for a desire driver
            for (int i = 0; i < driversList.Count; ++i)
            {
                // Driver is found
                if (driversList[i].ID == id)
                {
                    Console.WriteLine($"\n-------------Driver with ID {id} exists-------------\n");
                    Console.Write("Enter Name: ");
                    string name = (Console.ReadLine() ?? "").Trim();
                    if (!string.IsNullOrEmpty(name))
                    {
                        name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(name.ToLower());
                        driversList[i].Name = name;
                    }

                    Console.Write("Enter Age: ");
                    string ageString = getValue();
                    if (!isNull(ageString))
                        driversList[i].Age = int.Parse(ageString);

                    string gender;
                    do
                    {
                        Console.Write("Enter Gender: ");
                        gender = Console.ReadLine() ?? "".Trim();
                        if (string.IsNullOrEmpty(gender))
                            break;
                    } while (!validateGender(gender));
                    if (!string.IsNullOrEmpty(gender))
                        driversList[i].Gender = gender;

                    string phoneno;
                    do
                    {
                        Console.Write("Enter Phone No.: ");
                        phoneno = Console.ReadLine() ?? "".Trim();
                        if (string.IsNullOrEmpty(phoneno))
                            break;
                    } while (!validatePhoneNo(phoneno));
                    if (!string.IsNullOrEmpty(phoneno))
                        driversList[i].PhoneNo = phoneno;

                    Console.Write("Enter Address: ");
                    string address = (Console.ReadLine() ?? "").Trim();
                    if (!string.IsNullOrEmpty(address))
                        driversList[i].Address = address;

                    Console.Write("Enter Vehicle Type: ");
                    string vehicleType = getVehicleType();
                    if (!isNull(vehicleType))
                        driversList[i].VehicleType = vehicleType;

                    Console.Write("Enter Vehicle Model: ");
                    string vehicleModel = (Console.ReadLine() ?? "").Trim();
                    if (!string.IsNullOrEmpty(vehicleModel))
                        driversList[i].VehicleModel = vehicleModel;

                    Console.Write("Enter Vehicle License Plate: ");
                    string licensePlate = (Console.ReadLine() ?? "").Trim();
                    if (!string.IsNullOrEmpty(licensePlate))
                        driversList[i].VehicleLicensePlate = licensePlate;

                    return;
                }
            }
            Console.WriteLine($"Driver of id {id} not found\n");
        }

        // Function return true if driver found else return false
        public Tuple<bool, Driver> assignDriver(ref Ride ride)
        {
            float nearestDriverDistance = Int32.MaxValue;
            int driversListIndex = -1;

            // Check the driver which is available and nearest
            for (int i = 0; i < driversList.Count; ++i)
            {
                float driverToRiderDistance = (float)Math.Sqrt(Math.Pow(ride.Location.Loc_lat - driversList[i].Location.Loc_lat, 2) +
                                                      Math.Pow(ride.Location.Loc_lng - driversList[i].Location.Loc_lng, 2));

                // Is nearest, having same vehicle type driver is available ???
                if (driversList[i].Availability == 1 && driverToRiderDistance < nearestDriverDistance &&
                    ride.RideType.Equals(driversList[i].VehicleType, StringComparison.CurrentCultureIgnoreCase))
                {
                    driversListIndex = i;
                    nearestDriverDistance = driverToRiderDistance;
                }
            }
            if (driversListIndex != -1)
            {
                // found driver
                ride.Driver = driversList[driversListIndex];
                return new Tuple<bool, Driver>(true, driversList[driversListIndex]);
            }
            else
            {
                Console.WriteLine("No driver is available\n");
                return new Tuple<bool, Driver>(false, new Driver());
            }
        }
        //********************************************
        // Utility Functions for searchDriver Function
        //********************************************
        const int tableWidth = 73;
        static void PrintLine()
        {
            Console.WriteLine(new string('-', tableWidth));
        }
        private void PrintRow(params string[] columns)
        {
            int width = (tableWidth - columns.Length) / columns.Length;
            string row = "|";

            foreach (string column in columns)
                row += AlignCentre(column, width) + "|";

            Console.WriteLine(row);
        }
        private string AlignCentre(string text, int width)
        {
            text = text.Length > width ? text.Substring(0, width - 3) + "..." : text;
            if (string.IsNullOrEmpty(text))
                return new string(' ', width);
            else
                return text.PadRight(width - (width - text.Length) / 2).PadLeft(width);
        }
        private bool isNull(string value)
        {
            return string.IsNullOrEmpty(value);
        }
        public void searchDriver()
        {
            int null_count = 0;
            Console.Write("Enter ID: ");
            string id = getValue();
            if (!isNull(id)) { null_count++; }

            Console.Write("Enter Name: ");
            string name = (Console.ReadLine() ?? "").Trim();
            if (!isNull(name)) { null_count++; }

            Console.Write("Enter Age: ");
            string age = (Console.ReadLine() ?? "").Trim();
            if (!isNull(age)) { null_count++; }

            Console.Write("Enter Gender: ");
            string gender = (Console.ReadLine() ?? "").Trim();
            if (!isNull(gender)) { null_count++; }

            Console.Write("Enter Address: ");
            string address = (Console.ReadLine() ?? "").Trim();
            if (!isNull(address)) { null_count++; }

            Console.Write("Enter Vehicle Type: ");
            string vehicleType = getVehicleType();
            if (!isNull(vehicleType)) { null_count++; }

            Console.Write("Enter Vehicle Model: ");
            string vehicleModel = (Console.ReadLine() ?? "").Trim();
            if (!isNull(vehicleModel)) { null_count++; }

            Console.Write("Enter Vehicle License Plate: ");
            string licensePlate = (Console.ReadLine() ?? "").Trim();
            if (!isNull(licensePlate)) { null_count++; }

            if (null_count == 0)
            {
                PrintLine();
                PrintRow("Name", "Age", "Gender", "V.Type", "V.Model", "V.License");
                PrintLine();
                Console.WriteLine("\n\t\t\t\"Table has no such record\"");
                return;
            }
            PrintLine();
            PrintRow("Name", "Age", "Gender", "V.Type", "V.Model", "V.License");
            PrintLine();
            bool isAnyRecord = false;
            for (int i = 0; i < driversList.Count; ++i)
            {
                int count = 0;
                if ((!isNull(id) && (int.Parse(id) == driversList[i].ID)))
                    count++;
                if ((!isNull(name) && (name.Equals(driversList[i].Name, StringComparison.CurrentCultureIgnoreCase))))
                    count++;
                if ((!isNull(age) && (int.Parse(age) == driversList[i].Age)))
                    count++;
                if ((!isNull(address) && (address.Equals(driversList[i].Address, StringComparison.CurrentCultureIgnoreCase))))
                    count++;
                if ((!isNull(gender) && (gender.Equals(driversList[i].Gender, StringComparison.CurrentCultureIgnoreCase))))
                    count++;
                if ((!isNull(vehicleType) && (vehicleType.Equals(driversList[i].VehicleType, StringComparison.CurrentCultureIgnoreCase))))
                    count++;
                if ((!isNull(vehicleModel) && (vehicleModel.Equals(driversList[i].VehicleModel, StringComparison.CurrentCultureIgnoreCase))))
                    count++;
                if ((!isNull(licensePlate) && (licensePlate.Equals(driversList[i].VehicleLicensePlate, StringComparison.CurrentCultureIgnoreCase))))
                    count++;
                if (count == null_count)
                {
                    PrintRow(driversList[i].Name, "" + driversList[i].Age, driversList[i].Gender, driversList[i].VehicleType,
                             driversList[i].VehicleModel, driversList[i].VehicleLicensePlate);
                    isAnyRecord = true;
                }
            }
            if (!isAnyRecord)
                Console.WriteLine("\n\t\t\t\"Table has no such record\"");
            else
                PrintLine();
        }
    }
}
