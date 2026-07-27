using System;
using System.Collections.Generic;
using DroneFactory.Data;
using DroneFactory.Model;

namespace DroneFactory.Commands
{
    public class GetMovementsCommand : ICommand
    {
        private readonly StockRepository _stocks;

        public GetMovementsCommand(StockRepository stocks)
        {
            _stocks = stocks;
        }

        public void Execute(string arguments)
        {
            HashSet<string> filter = null;

            if (arguments != null && arguments.Trim().Length > 0)
            {
                filter = new HashSet<string>(StringComparer.Ordinal);
                string[] names = arguments.Split(',');
                for (int i = 0; i < names.Length; i++)
                {
                    string name = names[i].Trim();
                    if (name.Length == 0)
                    {
                        Console.WriteLine("ERROR empty item name");
                        return;
                    }

                    filter.Add(name);
                }
            }

            foreach (StockMovement movement in _stocks.Movements)
            {
                if (filter != null && !filter.Contains(movement.ItemName))
                {
                    continue;
                }

                string sign = movement.Delta >= 0 ? "+" : "";
                Console.WriteLine(sign + movement.Delta + " " + movement.ItemName + " (" + movement.Reason + ")");
            }
        }
    }
}
