using System;
using DroneFactory.Data;

namespace DroneFactory.Commands
{
    public class StocksCommand : ICommand
    {
        private readonly StockRepository _stocks;

        public StocksCommand(StockRepository stocks)
        {
            _stocks = stocks;
        }

        public void Execute(string arguments)
        {
            if (arguments.Length > 0)
            {
                Console.WriteLine("ERROR STOCKS does not take arguments");
                return;
            }

            foreach (string droneName in _stocks.DroneNames)
            {
                Console.WriteLine(_stocks.GetDroneStock(droneName) + " " + droneName);
            }

            foreach (string pieceName in _stocks.PieceNames)
            {
                Console.WriteLine(_stocks.GetPieceStock(pieceName) + " " + pieceName);
            }
        }
    }
}
