using System;
using System.Collections.Generic;
using DroneFactory.Assembly;
using DroneFactory.Data;
using DroneFactory.Model;

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

    public class NeededStocksCommand : ICommand
    {
        private readonly OrderParser _parser;
        private readonly OrderCalculator _calculator;
        private readonly TemplateRepository _templates;

        public NeededStocksCommand(OrderParser parser, OrderCalculator calculator, TemplateRepository templates)
        {
            _parser = parser;
            _calculator = calculator;
            _templates = templates;
        }

        public void Execute(string arguments)
        {
            OrderParseResult order = _parser.Parse(arguments);
            if (order.HasError)
            {
                Console.WriteLine("ERROR " + order.ErrorMessage);
                return;
            }

            Dictionary<string, int> totalNeeds = _calculator.EmptyPieceQuantities();

            foreach (OrderLine item in order.Items)
            {
                DroneTemplate drone = _templates.Get(item.DroneName);
                Dictionary<string, int> droneNeeds = _calculator.CountPiecesForDrone(drone, item.Quantity);

                Console.WriteLine(item.Quantity + " " + drone.Name + " :");
                _calculator.PrintNonZeroPieceQuantities(droneNeeds);

                _calculator.AddPieceQuantities(totalNeeds, droneNeeds);
            }

            Console.WriteLine("Total :");
            _calculator.PrintNonZeroPieceQuantities(totalNeeds);
        }
    }

    public class InstructionsCommand : ICommand
    {
        private readonly OrderParser _parser;
        private readonly TemplateRepository _templates;

        public InstructionsCommand(OrderParser parser, TemplateRepository templates)
        {
            _parser = parser;
            _templates = templates;
        }

        public void Execute(string arguments)
        {
            OrderParseResult order = _parser.Parse(arguments);
            if (order.HasError)
            {
                Console.WriteLine("ERROR " + order.ErrorMessage);
                return;
            }

            foreach (OrderLine item in order.Items)
            {
                DroneTemplate drone = _templates.Get(item.DroneName);

                for (int produced = 0; produced < item.Quantity; produced++)
                {
                    foreach (string line in DroneAssemblyBuilder.BuildInstructions(drone))
                    {
                        Console.WriteLine(line);
                    }
                }
            }
        }
    }
}
