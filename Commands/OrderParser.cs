using System;
using System.Collections.Generic;
using DroneFactory.Data;
using DroneFactory.Model;

namespace DroneFactory.Commands
{
    public class OrderParser
    {
        private readonly TemplateRepository _templates;

        public OrderParser(TemplateRepository templates)
        {
            _templates = templates;
        }

        public OrderParseResult Parse(string arguments)
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
                if (!_templates.Contains(droneName))
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
    }
}
