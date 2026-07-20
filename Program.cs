using System;
using System.Collections.Generic;
using System.Text;

namespace DroneFactoryNaive
{
    class Program
    {
        private const int InitialPieceStock = 10;

        private static readonly string[] DroneOrder = new string[]
        {
            "DXF-1",
            "RDL-1",
            "WDS-1",
            "DYM-1"
        };

        private static readonly string[] PieceOrder = new string[]
        {
            "Hull_HG1",
            "Hull_HF1",
            "Hull_HS1",
            "Core_CG1",
            "Core_C3D1",
            "Generator_GG1",
            "Generator_GF1",
            "Generator_GS1",
            "Move_MF1",
            "Move_ML1",
            "Move_MS1",
            "Move_MM1",
            "Move_MU1",
            "Move_MS2",
            "Processor_PG1",
            "Processor_P3D1",
            "Processor_PU1"
        };

        private static readonly Dictionary<string, DroneTemplate> Drones =
            new Dictionary<string, DroneTemplate>(StringComparer.Ordinal);

        private static readonly Dictionary<string, int> PieceStocks =
            new Dictionary<string, int>(StringComparer.Ordinal);

        private static readonly Dictionary<string, int> DroneStocks =
            new Dictionary<string, int>(StringComparer.Ordinal);

        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            InitializeData();

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
                    ExecuteUserInstruction(line);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("ERROR unexpected problem: " + ex.Message);
                }
            }
        }

        private static void PrintStartupMessage()
        {
            Console.WriteLine("DroneFactoryNaive pret.");
            Console.WriteLine("Tape une commande, par exemple : STOCKS");
            Console.WriteLine("Commandes : STOCKS, NEEDED_STOCKS, INSTRUCTIONS, VERIFY, PRODUCE");
            Console.WriteLine("Quitter : EXIT ou QUIT");
        }

        private static void InitializeData()
        {
            AddDrone(new DroneTemplate(
                "DXF-1",
                "Hull_HF1",
                "Core_C3D1",
                "System_S3D1",
                "Generator_GF1",
                "Move_MF1",
                "Processor_P3D1"));

            AddDrone(new DroneTemplate(
                "RDL-1",
                "Hull_HG1",
                "Core_CG1",
                "System_SG1",
                "Generator_GG1",
                "Move_ML1",
                "Processor_PG1"));

            AddDrone(new DroneTemplate(
                "WDS-1",
                "Hull_HS1",
                "Core_C3D1",
                "System_S3D1",
                "Generator_GS1",
                "Move_MS1",
                "Processor_P3D1"));

            AddDrone(new DroneTemplate(
                "DYM-1",
                "Hull_HG1",
                "Core_CG1",
                "System_SG1",
                "Generator_GG1",
                "Move_MM1",
                "Processor_PG1"));

            for (int i = 0; i < PieceOrder.Length; i++)
            {
                PieceStocks[PieceOrder[i]] = InitialPieceStock;
            }
        }

        private static void AddDrone(DroneTemplate drone)
        {
            Drones[drone.Name] = drone;
            DroneStocks[drone.Name] = 0;
        }

        private static void ExecuteUserInstruction(string line)
        {
            string instruction;
            string arguments;
            SplitInstruction(line, out instruction, out arguments);

            string normalizedInstruction = instruction.ToUpperInvariant();

            if (normalizedInstruction == "STOCKS")
            {
                if (arguments.Length > 0)
                {
                    Console.WriteLine("ERROR STOCKS does not take arguments");
                    return;
                }

                PrintStocks();
                return;
            }

            if (normalizedInstruction == "NEEDED_STOCKS")
            {
                OrderParseResult order = ParseOrder(arguments);
                if (order.HasError)
                {
                    Console.WriteLine("ERROR " + order.ErrorMessage);
                    return;
                }

                PrintNeededStocks(order.Items);
                return;
            }

            if (normalizedInstruction == "INSTRUCTIONS")
            {
                OrderParseResult order = ParseOrder(arguments);
                if (order.HasError)
                {
                    Console.WriteLine("ERROR " + order.ErrorMessage);
                    return;
                }

                PrintAssemblyInstructions(order.Items);
                return;
            }

            if (normalizedInstruction == "VERIFY")
            {
                OrderParseResult order = ParseOrder(arguments);
                if (order.HasError)
                {
                    Console.WriteLine("ERROR " + order.ErrorMessage);
                    return;
                }

                StockProblem problem = FindFirstStockProblem(order.Items);
                Console.WriteLine(problem == null ? "AVAILABLE" : "UNAVAILABLE");
                return;
            }

            if (normalizedInstruction == "PRODUCE")
            {
                OrderParseResult order = ParseOrder(arguments);
                if (order.HasError)
                {
                    Console.WriteLine("ERROR " + order.ErrorMessage);
                    return;
                }

                StockProblem problem = FindFirstStockProblem(order.Items);
                if (problem != null)
                {
                    Console.WriteLine("ERROR insufficient stock for " + problem.PieceName
                        + " (needed " + problem.NeededQuantity
                        + ", available " + problem.AvailableQuantity + ")");
                    return;
                }

                ProduceOrder(order.Items);
                Console.WriteLine("STOCK_UPDATED");
                return;
            }

            if (normalizedInstruction == "ADD_TEMPLATE")
            {
                Console.WriteLine("ERROR ADD_TEMPLATE is part of the next project iteration");
                return;
            }

            Console.WriteLine("ERROR unknown instruction `" + instruction + "`");
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

        private static OrderParseResult ParseOrder(string arguments)
        {
            OrderParseResult result = new OrderParseResult();
            result.Items = new List<OrderLine>();

            if (arguments == null || arguments.Trim().Length == 0)
            {
                result.HasError = true;
                result.ErrorMessage = "missing command arguments";
                return result;
            }

            string[] parts = arguments.Split(',');
            for (int i = 0; i < parts.Length; i++)
            {
                string part = parts[i].Trim();
                if (part.Length == 0)
                {
                    result.HasError = true;
                    result.ErrorMessage = "empty command element";
                    return result;
                }

                string[] tokens = part.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                if (tokens.Length != 2)
                {
                    result.HasError = true;
                    result.ErrorMessage = "invalid command element `" + part
                        + "` (expected: quantity drone)";
                    return result;
                }

                int quantity;
                if (!int.TryParse(tokens[0], out quantity) || quantity <= 0)
                {
                    result.HasError = true;
                    result.ErrorMessage = "`" + tokens[0] + "` is not a valid positive quantity";
                    return result;
                }

                string droneName = tokens[1];
                if (!Drones.ContainsKey(droneName))
                {
                    result.HasError = true;
                    result.ErrorMessage = "`" + droneName + "` is not a recognized drone";
                    return result;
                }

                OrderLine existingLine = FindOrderLine(result.Items, droneName);
                if (existingLine == null)
                {
                    result.Items.Add(new OrderLine(droneName, quantity));
                }
                else
                {
                    existingLine.Quantity += quantity;
                }
            }

            return result;
        }

        private static OrderLine FindOrderLine(List<OrderLine> items, string droneName)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].DroneName == droneName)
                {
                    return items[i];
                }
            }

            return null;
        }

        private static void PrintStocks()
        {
            for (int i = 0; i < DroneOrder.Length; i++)
            {
                string droneName = DroneOrder[i];
                Console.WriteLine(DroneStocks[droneName] + " " + droneName);
            }

            for (int i = 0; i < PieceOrder.Length; i++)
            {
                string pieceName = PieceOrder[i];
                Console.WriteLine(PieceStocks[pieceName] + " " + pieceName);
            }
        }

        private static void PrintNeededStocks(List<OrderLine> order)
        {
            Dictionary<string, int> totalNeeds = EmptyPieceQuantities();

            for (int i = 0; i < order.Count; i++)
            {
                OrderLine item = order[i];
                DroneTemplate drone = Drones[item.DroneName];
                Dictionary<string, int> droneNeeds = CountPiecesForDrone(drone, item.Quantity);

                Console.WriteLine(item.Quantity + " " + drone.Name + " :");
                PrintNonZeroPieceQuantities(droneNeeds);

                AddPieceQuantities(totalNeeds, droneNeeds);
            }

            Console.WriteLine("Total :");
            PrintNonZeroPieceQuantities(totalNeeds);
        }

        private static void PrintAssemblyInstructions(List<OrderLine> order)
        {
            for (int i = 0; i < order.Count; i++)
            {
                OrderLine item = order[i];
                DroneTemplate drone = Drones[item.DroneName];

                for (int produced = 0; produced < item.Quantity; produced++)
                {
                    PrintInstructionsForOneDrone(drone);
                }
            }
        }

        private static void PrintInstructionsForOneDrone(DroneTemplate drone)
        {
            string installedCore = drone.Core + "{" + drone.System + "}";

            Console.WriteLine("PRODUCING " + drone.Name);
            Console.WriteLine("GET_OUT_STOCK 1 " + drone.Hull);
            Console.WriteLine("GET_OUT_STOCK 1 " + drone.Core);
            Console.WriteLine("GET_OUT_STOCK 1 " + drone.Generator);
            Console.WriteLine("GET_OUT_STOCK 1 " + drone.Move);
            Console.WriteLine("GET_OUT_STOCK 1 " + drone.Processor);
            Console.WriteLine("INSTALL " + drone.System + " " + drone.Core);
            Console.WriteLine("ASSEMBLE TMP1 " + drone.Hull + " " + drone.Generator);
            Console.WriteLine("ASSEMBLE TMP2 TMP1 " + drone.Move);
            Console.WriteLine("ASSEMBLE TMP2 " + installedCore);
            Console.WriteLine("ASSEMBLE [TMP2, " + installedCore + "] " + drone.Processor);
            Console.WriteLine("FINISHED " + drone.Name);
        }

        private static StockProblem FindFirstStockProblem(List<OrderLine> order)
        {
            Dictionary<string, int> totalNeeds = EmptyPieceQuantities();

            for (int i = 0; i < order.Count; i++)
            {
                OrderLine item = order[i];
                DroneTemplate drone = Drones[item.DroneName];
                Dictionary<string, int> droneNeeds = CountPiecesForDrone(drone, item.Quantity);
                AddPieceQuantities(totalNeeds, droneNeeds);
            }

            for (int i = 0; i < PieceOrder.Length; i++)
            {
                string pieceName = PieceOrder[i];
                int needed = totalNeeds[pieceName];
                int available = PieceStocks[pieceName];

                if (needed > available)
                {
                    return new StockProblem(pieceName, needed, available);
                }
            }

            return null;
        }

        private static void ProduceOrder(List<OrderLine> order)
        {
            Dictionary<string, int> totalNeeds = EmptyPieceQuantities();

            for (int i = 0; i < order.Count; i++)
            {
                OrderLine item = order[i];
                DroneTemplate drone = Drones[item.DroneName];
                Dictionary<string, int> droneNeeds = CountPiecesForDrone(drone, item.Quantity);
                AddPieceQuantities(totalNeeds, droneNeeds);
            }

            for (int i = 0; i < PieceOrder.Length; i++)
            {
                string pieceName = PieceOrder[i];
                PieceStocks[pieceName] -= totalNeeds[pieceName];
            }

            for (int i = 0; i < order.Count; i++)
            {
                OrderLine item = order[i];
                DroneStocks[item.DroneName] += item.Quantity;
            }
        }

        private static Dictionary<string, int> CountPiecesForDrone(DroneTemplate drone, int quantity)
        {
            Dictionary<string, int> quantities = EmptyPieceQuantities();
            quantities[drone.Hull] += quantity;
            quantities[drone.Core] += quantity;
            quantities[drone.Generator] += quantity;
            quantities[drone.Move] += quantity;
            quantities[drone.Processor] += quantity;
            return quantities;
        }

        private static Dictionary<string, int> EmptyPieceQuantities()
        {
            Dictionary<string, int> quantities = new Dictionary<string, int>(StringComparer.Ordinal);
            for (int i = 0; i < PieceOrder.Length; i++)
            {
                quantities[PieceOrder[i]] = 0;
            }

            return quantities;
        }

        private static void AddPieceQuantities(
            Dictionary<string, int> target,
            Dictionary<string, int> source)
        {
            for (int i = 0; i < PieceOrder.Length; i++)
            {
                string pieceName = PieceOrder[i];
                target[pieceName] += source[pieceName];
            }
        }

        private static void PrintNonZeroPieceQuantities(Dictionary<string, int> quantities)
        {
            for (int i = 0; i < PieceOrder.Length; i++)
            {
                string pieceName = PieceOrder[i];
                int quantity = quantities[pieceName];

                if (quantity > 0)
                {
                    Console.WriteLine(quantity + " " + pieceName);
                }
            }
        }
    }

    class DroneTemplate
    {
        public DroneTemplate(
            string name,
            string hull,
            string core,
            string system,
            string generator,
            string move,
            string processor)
        {
            Name = name;
            Hull = hull;
            Core = core;
            System = system;
            Generator = generator;
            Move = move;
            Processor = processor;
        }

        public string Name;
        public string Hull;
        public string Core;
        public string System;
        public string Generator;
        public string Move;
        public string Processor;
    }

    class OrderLine
    {
        public OrderLine(string droneName, int quantity)
        {
            DroneName = droneName;
            Quantity = quantity;
        }

        public string DroneName;
        public int Quantity;
    }

    class OrderParseResult
    {
        public bool HasError;
        public string ErrorMessage;
        public List<OrderLine> Items;
    }

    class StockProblem
    {
        public StockProblem(string pieceName, int neededQuantity, int availableQuantity)
        {
            PieceName = pieceName;
            NeededQuantity = neededQuantity;
            AvailableQuantity = availableQuantity;
        }

        public string PieceName;
        public int NeededQuantity;
        public int AvailableQuantity;
    }
}
