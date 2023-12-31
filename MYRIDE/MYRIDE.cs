using Microsoft.Data.SqlClient;
using MyLibrary;
using System.Globalization;

namespace MYRIDE
{
    internal class MYRIDE
    {
        static private char get_main_menu_choice()
        {
            string str;
            Console.Write("Press 1 to 3 to select an option: ");
            while (true)
            {
                str = (Console.ReadLine() ?? "");
                if (str.Length > 1 || string.IsNullOrEmpty(str))
                    Console.WriteLine("Invalid!\nPress 1 to 3 to select an option: ");
                else if (str.Length == 1 && (str[0] < '1' || str[0] > '4'))
                    Console.WriteLine("Invalid!\nPress 1 to 3 to select an option: ");
                else
                    break;
            }
            return str[0];
        }

        static private char Yes_No()
        {
            char choice = Char.ToLower((Console.ReadLine() ?? "")[0]);
            while (true)
            {
                if (choice != 'n' && choice != 'y')
                {
                    Console.Write("Invalid!\nEnter ‘Y’ or ‘N’: ");
                    choice = Char.ToLower((Console.ReadLine() ?? "")[0]);
                }
                else
                    return choice;
            }
        }

        private static int getId()
        {
            string id;
            bool valid = true;
            do
            {
                id = Console.ReadLine() ?? "";
                for (int i = 0; i < id.Length; i++)
                {
                    if (!Char.IsDigit(id[i]))
                    {
                        valid = false;
                        Console.Write("Id only contain digits\nEnter again: ");
                    }
                }
            } while (!valid);
            return int.Parse(id);
        }

        private static void mainMenu()
        {
            Console.WriteLine("\n1. Book a Ride");
            Console.WriteLine("2. Enter as Driver");
            Console.WriteLine("3. Enter as Admin");
            Console.WriteLine("4. Exit");
        }
        static void Main(string[] args)
        {
            Console.WriteLine("--------------------------------------------");
            Console.WriteLine("              WELCOME TO MYRIDE             ");
            Console.WriteLine("--------------------------------------------");

            // Used to back to main menu
            bool goBackToMainMenu;

            // Create "Admin"
            Admin admin = new Admin();
            do
            {
                mainMenu();
                int choice = get_main_menu_choice();

                switch (choice)
                {
                    // *********************************
                    // Enter as a rider
                    // *********************************
                    case '1':
                        Ride ride = new Ride();
                        ride.assignPassenger();
                        ride.setLocations();
                        ride.setRideType();
                        Console.WriteLine("-------------- THANK YOU -------------");
                        Console.WriteLine($"Price for this ride is: {ride.Price}");
                        Console.Write("Enter ‘Y’ if you want to Book the ride, enter ‘N’ if you want to cancel operation: Y: ");
                        char Y_N = Yes_No();

                        if (Y_N == 'y')
                        {
                            // Find the available and nearest driver 😶
                            Tuple<bool, Driver> rideDriver = admin.assignDriver(ref ride);

                            // Driver is found :)
                            if (rideDriver.Item1)
                            {
                                Console.WriteLine("Happy Travel...!");
                                ride.giveRating();

                                SqlConnection connection = admin.SqlConnection;
                                connection.Open();
                                string insertQuery = $"insert into rider values({rideDriver.Item2.ID},'{ride.Name}','{ride.phoneNo}'," +
                                                     $"{ride.Price},{ride.Rating},'{ride.StartLocation}','{ride.EndLocation}')";
                                SqlCommand command = new SqlCommand(insertQuery, connection);
                                command.ExecuteNonQuery();
                                connection.Close();
                            }
                            else
                                Console.WriteLine("Sorry! Right now, there is no driver available\nTry later!!!\n");

                        }
                        // Driver is find and assigned
                        break;


                    // *********************************
                    // Enter as a driver
                    // *********************************
                    case '2':
                        // use to check either driver mode is exit or not 😀
                        bool goBackToDriverMode;

                        goBackToDriverMode = true;
                        Console.Write("\nEnter ID: ");
                        int id = getId();
                        Console.Write("Enter Name: ");
                        string name = Console.ReadLine() ?? "";

                        // Admin is finding the driver !!!
                        Tuple<bool, Driver> tuple = admin.findDriver(id, name);

                        // If Driver is exist in Admin database
                        if (tuple.Item1)
                        {
                            Console.WriteLine($"\nHello! {CultureInfo.CurrentCulture.TextInfo.ToTitleCase(name.ToLower())}");
                            char driverChoice;
                            do
                            {
                                do
                                {
                                    Console.WriteLine("\n\n1. Change Availability");
                                    Console.WriteLine("2. Change Location");
                                    Console.WriteLine("3. Exit Driver");
                                    Console.Write("Choose: ");
                                    driverChoice = (Console.ReadLine() ?? "")[0];
                                } while (driverChoice < '1' || driverChoice > '3'); // Check validity

                                // Driver Menu
                                switch (driverChoice)
                                {
                                    case '1':
                                        tuple.Item2.updateAvailability();
                                        Console.WriteLine("Update...!\n");
                                        break;
                                    case '2':
                                        tuple.Item2.updateLocation();
                                        Console.WriteLine("Changed...!\n");
                                        break;
                                    case '3':
                                        // Exit driver
                                        goBackToDriverMode = false;
                                        break;
                                }

                                // Go back to driver mode :)
                            } while (goBackToDriverMode);
                        }
                        else
                            Console.WriteLine($"Driver of ID: {id} Name: {name} not found\n");
                        break;

                    // *********************************
                    // Admin mode
                    // *********************************
                    case '3':
                        bool goBackToAdminMode;
                        do
                        {
                            // use to check either to go back to admin mode or not
                            goBackToAdminMode = true;

                            Console.WriteLine("\n\n1. Add Driver");
                            Console.WriteLine("2. Remove Driver");
                            Console.WriteLine("3. Update Driver");
                            Console.WriteLine("4. Search Driver");
                            Console.WriteLine("5. Exit Admin");
                            Console.Write("Choose: ");
                            char adminChoice;
                            do
                            {
                                adminChoice = (Console.ReadLine() ?? "")[0];
                                if (adminChoice < '1' || adminChoice > '5')
                                    Console.WriteLine("\nInvalid!\nChoose 1-5: ");
                            } while (adminChoice < '1' || adminChoice > '5');

                            switch (adminChoice)
                            {
                                case '1':
                                    admin.addDriver();
                                    break;
                                case '2':
                                    admin.removeDriver();
                                    break;
                                case '3':
                                    admin.updateDriver();
                                    break;
                                case '4':
                                    admin.searchDriver();
                                    break;
                                case '5':
                                    // Exit Admin
                                    goBackToAdminMode = false;
                                    break;
                            }
                        } while (goBackToAdminMode);
                        break;

                    case '4':
                        admin.dispose();
                        System.Environment.Exit(0);
                        break;
                }
                Console.Write("\n\nDo you want to go back to Main menu?\nPress 'Y' or 'N': ");
                char mainMenuChoice = Yes_No();
                if (mainMenuChoice == 'y')
                    goBackToMainMenu = true;
                else
                    goBackToMainMenu = false;

            } while (goBackToMainMenu);
            admin.dispose();
        }
    }
}