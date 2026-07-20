namespace DroneFactory.Model
{
    public class StockProblem
    {
        public StockProblem(string pieceName, int neededQuantity, int availableQuantity)
        {
            PieceName = pieceName;
            NeededQuantity = neededQuantity;
            AvailableQuantity = availableQuantity;
        }

        public string PieceName;
        public int NeededQuantity;
        public int AvailableQuantity;
    }
}
