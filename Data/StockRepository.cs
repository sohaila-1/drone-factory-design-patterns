using System;
using System.Collections.Generic;

namespace DroneFactory.Data
{
    public class StockRepository
    {
        private const int InitialPieceStock = 10;

        private readonly PieceCatalog _catalog;

        private readonly Dictionary<string, int> _pieceStocks =
            new Dictionary<string, int>(StringComparer.Ordinal);

        private readonly Dictionary<string, int> _droneStocks =
            new Dictionary<string, int>(StringComparer.Ordinal);

        private readonly List<string> _droneOrder = new List<string>();

        public StockRepository(PieceCatalog catalog)
        {
            _catalog = catalog;
            foreach (string pieceName in catalog.StockablePieceNames)
            {
                _pieceStocks[pieceName] = InitialPieceStock;
            }
        }

        public IReadOnlyList<string> PieceNames => _catalog.StockablePieceNames;
        public IReadOnlyList<string> DroneNames => _droneOrder;

        public void RegisterDrone(string droneName)
        {
            if (!_droneStocks.ContainsKey(droneName))
            {
                _droneStocks[droneName] = 0;
                _droneOrder.Add(droneName);
            }
        }

        public int GetPieceStock(string pieceName)
        {
            return _pieceStocks[pieceName];
        }

        public int GetDroneStock(string droneName)
        {
            return _droneStocks[droneName];
        }

        public void RemovePieces(IReadOnlyDictionary<string, int> quantities)
        {
            foreach (KeyValuePair<string, int> pair in quantities)
            {
                if (pair.Value == 0)
                {
                    continue;
                }

                _pieceStocks[pair.Key] -= pair.Value;
            }
        }

        public void AddDrones(string droneName, int quantity)
        {
            _droneStocks[droneName] += quantity;
        }
    }
}
