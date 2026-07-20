using System;
using DroneFactory.Assembly;
using DroneFactory.Data;
using DroneFactory.Model;

namespace DroneFactory.Commands
{
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
