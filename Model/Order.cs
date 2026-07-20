using System.Collections.Generic;

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

    public class OrderParseResult
    {
        public bool HasError;
        public string ErrorMessage;
        public List<OrderLine> Items;
    }

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
