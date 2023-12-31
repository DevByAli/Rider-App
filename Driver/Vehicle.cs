namespace MyLibrary
{
    public class Vehicle
    {
        string type;
        string model;
        string license_plate;

        public Vehicle()
        {
            type = model = license_plate = string.Empty;
        }

        public string Type
        {
            get { return type; }
            set { type = value; }
        }

        public string Model
        {
            get { return model; }
            set { model = value; }
        }

        public string License_plate
        {
            get { return license_plate; }
            set { license_plate = value; }
        }
    }
}
