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

            StockProblem problem;
            try
            {
                problem = _calculator.FindFirstStockProblem(order.Items, _stocks);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine("ERROR " + ex.Message);
                return;
            }

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

            StockProblem problem;
            Dictionary<string, int> totalNeeds;
            try
            {
                problem = _calculator.FindFirstStockProblem(order.Items, _stocks);
                if (problem == null)
                {
                    totalNeeds = _calculator.CountTotalNeeds(order.Items);
                }
                else
                {
                    totalNeeds = null;
                }
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine("ERROR " + ex.Message);
                return;
            }

            if (problem != null)
            {
                Console.WriteLine("ERROR insufficient stock for " + problem.PieceName
                    + " (needed " + problem.NeededQuantity
                    + ", available " + problem.AvailableQuantity + ")");
                return;
            }

            _stocks.RemovePieces(totalNeeds, "PRODUCE");

            foreach (OrderLine item in order.Items)
            {
                _stocks.AddDrones(item.DroneName, item.Quantity, "PRODUCE");
            }

            Console.WriteLine("STOCK_UPDATED");
        }
    }
}
