namespace DroneFactory.Model
{
    public class StockMovement
    {
        public StockMovement(string itemName, int delta, string reason)
        {
            ItemName = itemName;
            Delta = delta;
            Reason = reason;
        }

        public string ItemName { get; }
        public int Delta { get; }
        public string Reason { get; }
    }
}
