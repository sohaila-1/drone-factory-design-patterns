using System;
using System.Collections.Generic;
using DroneFactory.Model;

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
        private readonly List<StockMovement> _movements = new List<StockMovement>();

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
        public IReadOnlyList<StockMovement> Movements => _movements;

        public void RegisterDrone(string droneName)
        {
            if (!_droneStocks.ContainsKey(droneName))
            {
                _droneStocks[droneName] = 0;
                _droneOrder.Add(droneName);
            }
        }

        public bool IsKnownPiece(string pieceName)
        {
            return _pieceStocks.ContainsKey(pieceName);
        }

        public bool IsKnownDrone(string droneName)
        {
            return _droneStocks.ContainsKey(droneName);
        }

        public int GetPieceStock(string pieceName)
        {
            return _pieceStocks[pieceName];
        }

        public int GetDroneStock(string droneName)
        {
            return _droneStocks[droneName];
        }

        public void RemovePieces(IReadOnlyDictionary<string, int> quantities, string reason)
        {
            foreach (KeyValuePair<string, int> pair in quantities)
            {
                if (pair.Value == 0)
                {
                    continue;
                }

                _pieceStocks[pair.Key] -= pair.Value;
                _movements.Add(new StockMovement(pair.Key, -pair.Value, reason));
            }
        }

        public void AddPiece(string pieceName, int quantity, string reason)
        {
            _pieceStocks[pieceName] += quantity;
            _movements.Add(new StockMovement(pieceName, quantity, reason));
        }

        public void AddDrones(string droneName, int quantity, string reason)
        {
            _droneStocks[droneName] += quantity;
            _movements.Add(new StockMovement(droneName, quantity, reason));
        }
    }
}
