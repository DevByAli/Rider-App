
namespace MyLibrary
{
    public class Passenger
    {
        string name;
        string phoneNo;

        public Passenger()
        {
            this.name = string.Empty;
            this.phoneNo = string.Empty;
        }
        public string PassengerName
        {
            get { return name; }
            set { name = value; }
        }
        public string PassengerPhone
        {
            get { return phoneNo; }
            set { phoneNo = value; }
        }

    }
}
