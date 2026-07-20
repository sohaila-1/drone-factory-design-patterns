using System;
using System.Collections.Generic;
using System.Text;
using DroneFactory.Categorization;
using DroneFactory.Commands;
using DroneFactory.Data;

namespace DroneFactory
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            PieceCatalog catalog = new PieceCatalog();
            StockRepository stocks = new StockRepository(catalog);
            TemplateRepository templates = new TemplateRepository(stocks);
            DroneCategorizer categorizer = new DroneCategorizer(catalog);
            DroneTemplateFactory templateFactory = new DroneTemplateFactory(catalog, categorizer);

            RegisterBuiltinDrones(templateFactory, templates);

            CommandRegistry registry = BuildCommandRegistry(catalog, templates, stocks, templateFactory);

            bool interactiveMode = !Console.IsInputRedirected;
            if (interactiveMode)
            {
                PrintStartupMessage();
            }

            string line;
            while (true)
            {
                if (interactiveMode)
                {
                    Console.Write("> ");
                }

                line = Console.ReadLine();
                if (line == null)
                {
                    break;
                }

                line = line.Trim();

                if (line.Length == 0)
                {
                    continue;
                }

                if (string.Equals(line, "EXIT", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(line, "QUIT", StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }

                try
                {
                    string instruction;
                    string arguments;
                    SplitInstruction(line, out instruction, out arguments);
                    registry.Dispatch(instruction.ToUpperInvariant(), arguments);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("ERROR unexpected problem: " + ex.Message);
                }
            }
        }

        private static CommandRegistry BuildCommandRegistry(
            PieceCatalog catalog,
            TemplateRepository templates,
            StockRepository stocks,
            DroneTemplateFactory templateFactory)
        {
            OrderParser orderParser = new OrderParser(templates);
            OrderCalculator calculator = new OrderCalculator(catalog, templates);

            CommandRegistry registry = new CommandRegistry();
            registry.Register("STOCKS", new StocksCommand(stocks));
            registry.Register("NEEDED_STOCKS", new NeededStocksCommand(orderParser, calculator, templates));
            registry.Register("INSTRUCTIONS", new InstructionsCommand(orderParser, templates));
            registry.Register("VERIFY", new VerifyCommand(orderParser, calculator, stocks));
            registry.Register("PRODUCE", new ProduceCommand(orderParser, calculator, stocks));
            registry.Register("ADD_TEMPLATE", new AddTemplateCommand(templateFactory, templates));
            return registry;
        }

        private static void RegisterBuiltinDrones(DroneTemplateFactory factory, TemplateRepository templates)
        {
            templates.Register(factory.Create("DXF-1", new List<string>
            {
                "Hull_HF1", "Core_C3D1", "System_S3D1", "Generator_GF1", "Move_MF1", "Processor_P3D1"
            }));

            templates.Register(factory.Create("RDL-1", new List<string>
            {
                "Hull_HG1", "Core_CG1", "System_SG1", "Generator_GG1", "Move_ML1", "Processor_PG1"
            }));

            templates.Register(factory.Create("WDS-1", new List<string>
            {
                "Hull_HS1", "Core_C3D1", "System_S3D1", "Generator_GS1", "Move_MS1", "Processor_P3D1"
            }));

            templates.Register(factory.Create("DYM-1", new List<string>
            {
                "Hull_HG1", "Core_CG1", "System_SG1", "Generator_GG1", "Move_MM1", "Processor_PG1"
            }));
        }

        private static void PrintStartupMessage()
        {
            Console.WriteLine("DroneFactory pret.");
            Console.WriteLine("Tape une commande, par exemple : STOCKS");
            Console.WriteLine("Commandes : STOCKS, NEEDED_STOCKS, INSTRUCTIONS, VERIFY, PRODUCE, ADD_TEMPLATE");
            Console.WriteLine("Quitter : EXIT ou QUIT");
        }

        private static void SplitInstruction(string line, out string instruction, out string arguments)
        {
            int firstSpaceIndex = line.IndexOf(' ');

            if (firstSpaceIndex < 0)
            {
                instruction = line;
                arguments = "";
                return;
            }

            instruction = line.Substring(0, firstSpaceIndex).Trim();
            arguments = line.Substring(firstSpaceIndex + 1).Trim();
        }
    }
}
