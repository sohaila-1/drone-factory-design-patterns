using System;
using System.Collections.Generic;
using DroneFactory.Data;
using DroneFactory.Model;

namespace DroneFactory.Commands
{
    public class ReceiveCommand : ICommand
    {
        private readonly PieceCatalog _catalog;
        private readonly TemplateRepository _templates;
        private readonly StockRepository _stocks;

        public ReceiveCommand(PieceCatalog catalog, TemplateRepository templates, StockRepository stocks)
        {
            _catalog = catalog;
            _templates = templates;
            _stocks = stocks;
        }

        public void Execute(string arguments)
        {
            if (arguments == null || arguments.Trim().Length == 0)
            {
                Console.WriteLine("ERROR missing command arguments");
                return;
            }

            List<KeyValuePair<int, string>> pieceDeltas = new List<KeyValuePair<int, string>>();
            List<KeyValuePair<int, string>> droneDeltas = new List<KeyValuePair<int, string>>();

            string[] parts = arguments.Split(',');
            for (int i = 0; i < parts.Length; i++)
            {
                string part = parts[i].Trim();
                if (part.Length == 0)
                {
                    Console.WriteLine("ERROR empty command element");
                    return;
                }

                string[] tokens = part.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                if (tokens.Length != 2)
                {
                    Console.WriteLine("ERROR invalid command element `" + part + "` (expected: quantity item)");
                    return;
                }

                int quantity;
                if (!int.TryParse(tokens[0], out quantity) || quantity <= 0)
                {
                    Console.WriteLine("ERROR `" + tokens[0] + "` is not a valid positive quantity");
                    return;
                }

                string itemName = tokens[1];
                PieceDefinition piece;
                if (_catalog.TryGet(itemName, out piece))
                {
                    if (piece.Kind == PieceKind.System)
                    {
                        Console.WriteLine("ERROR `" + itemName + "` is a system, systems are not tracked in piece stock");
                        return;
                    }

                    pieceDeltas.Add(new KeyValuePair<int, string>(quantity, itemName));
                }
                else if (_templates.Contains(itemName))
                {
                    droneDeltas.Add(new KeyValuePair<int, string>(quantity, itemName));
                }
                else
                {
                    Console.WriteLine("ERROR `" + itemName + "` is not a recognized piece or drone");
                    return;
                }
            }

            foreach (KeyValuePair<int, string> delta in pieceDeltas)
            {
                _stocks.AddPiece(delta.Value, delta.Key, "RECEIVE");
            }

            foreach (KeyValuePair<int, string> delta in droneDeltas)
            {
                _stocks.AddDrones(delta.Value, delta.Key, "RECEIVE");
            }

            Console.WriteLine("STOCK_UPDATED");
        }
    }
}
