using System;
using System.Collections.Generic;
using DroneFactory.Data;
using DroneFactory.Model;

namespace DroneFactory.Commands
{
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
}
