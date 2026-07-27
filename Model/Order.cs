using System.Collections.Generic;

namespace DroneFactory.Model
{
    public enum PieceModificationKind
    {
        Add,
        Remove
    }

    public class PieceModification
    {
        public PieceModification(PieceModificationKind kind, string pieceName, int quantity)
        {
            Kind = kind;
            PieceName = pieceName;
            Quantity = quantity;
        }

        public PieceModificationKind Kind { get; }
        public string PieceName { get; }
        public int Quantity { get; }
    }

    public class OrderLine
    {
        public OrderLine(string droneName, int quantity)
        {
            DroneName = droneName;
            Quantity = quantity;
            Modifications = new List<PieceModification>();
        }

        public string DroneName;
        public int Quantity;
        public List<PieceModification> Modifications;
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
