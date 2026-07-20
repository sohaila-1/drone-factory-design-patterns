using System;
using System.Collections.Generic;
using DroneFactory.Data;
using DroneFactory.Model;

namespace DroneFactory.Commands
{
    public class VerifyCommand : ICommand
    {
        private readonly OrderParser _parser;
        private readonly OrderCalculator _calculator;
        private readonly StockRepository _stocks;

        public VerifyCommand(OrderParser parser, OrderCalculator calculator, StockRepository stocks)
        {
            _parser = parser;
            _calculator = calculator;
            _stocks = stocks;
        }

        public void Execute(string arguments)
        {
            OrderParseResult order = _parser.Parse(arguments);
            if (order.HasError)
            {
                Console.WriteLine("ERROR " + order.ErrorMessage);
                return;
            }

            StockProblem problem = _calculator.FindFirstStockProblem(order.Items, _stocks);
            Console.WriteLine(problem == null ? "AVAILABLE" : "UNAVAILABLE");
        }
    }

    public class ProduceCommand : ICommand
    {
        private readonly OrderParser _parser;
        private readonly OrderCalculator _calculator;
        private readonly StockRepository _stocks;

        public ProduceCommand(OrderParser parser, OrderCalculator calculator, StockRepository stocks)
        {
            _parser = parser;
            _calculator = calculator;
            _stocks = stocks;
        }

        public void Execute(string arguments)
        {
            OrderParseResult order = _parser.Parse(arguments);
            if (order.HasError)
            {
                Console.WriteLine("ERROR " + order.ErrorMessage);
                return;
            }

            StockProblem problem = _calculator.FindFirstStockProblem(order.Items, _stocks);
            if (problem != null)
            {
                Console.WriteLine("ERROR insufficient stock for " + problem.PieceName
                    + " (needed " + problem.NeededQuantity
                    + ", available " + problem.AvailableQuantity + ")");
                return;
            }

            Dictionary<string, int> totalNeeds = _calculator.CountTotalNeeds(order.Items);
            _stocks.RemovePieces(totalNeeds);

            foreach (OrderLine item in order.Items)
            {
                _stocks.AddDrones(item.DroneName, item.Quantity);
            }

            Console.WriteLine("STOCK_UPDATED");
        }
    }
}
