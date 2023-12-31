
namespace MyLibrary
{
    public class Location
    {
        float latitude;
        float longitude;

        public Location()
        {
            latitude = 0;
            longitude = 0;
        }

        public float Loc_lat
        { get { return latitude; } }
        public float Loc_lng
        { get { return longitude; } }

        public void setLocation(float latitude, float longitude)
        {
            this.latitude = Math.Abs(latitude);
            this.longitude = Math.Abs(longitude);
        }
    }
}
