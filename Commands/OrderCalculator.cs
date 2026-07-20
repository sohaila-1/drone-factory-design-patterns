using System;
using System.Collections.Generic;
using DroneFactory.Data;
using DroneFactory.Model;

namespace DroneFactory.Commands
{
    public class OrderCalculator
    {
        private readonly PieceCatalog _catalog;
        private readonly TemplateRepository _templates;

        public OrderCalculator(PieceCatalog catalog, TemplateRepository templates)
        {
            _catalog = catalog;
            _templates = templates;
        }

        public Dictionary<string, int> EmptyPieceQuantities()
        {
            Dictionary<string, int> quantities = new Dictionary<string, int>(StringComparer.Ordinal);
            foreach (string pieceName in _catalog.StockablePieceNames)
            {
                quantities[pieceName] = 0;
            }

            return quantities;
        }

        public Dictionary<string, int> CountPiecesForDrone(DroneTemplate drone, int quantity)
        {
            Dictionary<string, int> quantities = EmptyPieceQuantities();
            quantities[drone.Hull] += quantity;
            quantities[drone.Core] += quantity;
            quantities[drone.Generator] += quantity;
            quantities[drone.Move] += quantity;
            quantities[drone.Processor] += quantity;
            return quantities;
        }

        public Dictionary<string, int> CountTotalNeeds(List<OrderLine> order)
        {
            Dictionary<string, int> totalNeeds = EmptyPieceQuantities();

            foreach (OrderLine item in order)
            {
                DroneTemplate drone = _templates.Get(item.DroneName);
                Dictionary<string, int> droneNeeds = CountPiecesForDrone(drone, item.Quantity);
                AddPieceQuantities(totalNeeds, droneNeeds);
            }

            return totalNeeds;
        }

        public void AddPieceQuantities(Dictionary<string, int> target, Dictionary<string, int> source)
        {
            foreach (string pieceName in _catalog.StockablePieceNames)
            {
                target[pieceName] += source[pieceName];
            }
        }

        public StockProblem FindFirstStockProblem(List<OrderLine> order, StockRepository stocks)
        {
            Dictionary<string, int> totalNeeds = CountTotalNeeds(order);

            foreach (string pieceName in _catalog.StockablePieceNames)
            {
                int needed = totalNeeds[pieceName];
                int available = stocks.GetPieceStock(pieceName);

                if (needed > available)
                {
                    return new StockProblem(pieceName, needed, available);
                }
            }

            return null;
        }

        public void PrintNonZeroPieceQuantities(Dictionary<string, int> quantities)
        {
            foreach (string pieceName in _catalog.StockablePieceNames)
            {
                int quantity = quantities[pieceName];
                if (quantity > 0)
                {
                    Console.WriteLine(quantity + " " + pieceName);
                }
            }
        }
    }
}
