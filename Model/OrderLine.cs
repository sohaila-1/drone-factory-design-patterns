namespace DroneFactory.Model
{
    public class OrderLine
    {
        public OrderLine(string droneName, int quantity)
        {
            DroneName = droneName;
            Quantity = quantity;
        }

        public string DroneName;
        public int Quantity;
    }
}
