using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using DroneFactory.Data;
using DroneFactory.Model;

namespace DroneFactory.Commands
{
    public class OrderParser
    {
        private static readonly Regex ModifierSplitter =
            new Regex(@"\b(WITH|WITHOUT|REPLACE)\b", RegexOptions.Compiled);

        private readonly TemplateRepository _templates;
        private readonly PieceCatalog _catalog;

        public OrderParser(TemplateRepository templates, PieceCatalog catalog)
        {
            _templates = templates;
            _catalog = catalog;
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

            char separator = ModifierSplitter.IsMatch(arguments) ? ';' : ',';
            string[] parts = arguments.Split(separator);

            for (int i = 0; i < parts.Length; i++)
            {
                string part = parts[i].Trim();
                if (part.Length == 0)
                {
                    result.HasError = true;
                    result.ErrorMessage = "empty command element";
                    return result;
                }

                OrderLine line;
                string error = ParseElement(part, out line);
                if (error != null)
                {
                    result.HasError = true;
                    result.ErrorMessage = error;
                    return result;
                }

                OrderLine existingLine = FindOrderLine(result.Items, line.DroneName);
                if (existingLine == null)
                {
                    result.Items.Add(line);
                }
                else
                {
                    existingLine.Quantity += line.Quantity;
                    existingLine.Modifications.AddRange(line.Modifications);
                }
            }

            return result;
        }

        private string ParseElement(string part, out OrderLine line)
        {
            line = null;
            string[] segments = ModifierSplitter.Split(part);

            string basePart = segments[0].Trim();
            string[] baseTokens = basePart.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            if (baseTokens.Length != 2)
            {
                return "invalid command element `" + basePart + "` (expected: quantity drone)";
            }

            int quantity;
            if (!int.TryParse(baseTokens[0], out quantity) || quantity <= 0)
            {
                return "`" + baseTokens[0] + "` is not a valid positive quantity";
            }

            string droneName = baseTokens[1];
            if (!_templates.Contains(droneName))
            {
                return "`" + droneName + "` is not a recognized drone";
            }

            line = new OrderLine(droneName, quantity);

            for (int i = 1; i < segments.Length; i += 2)
            {
                string keyword = segments[i];
                string segmentText = segments[i + 1].Trim();

                List<KeyValuePair<int, string>> pairs;
                string pairError = ParsePairs(segmentText, out pairs);
                if (pairError != null)
                {
                    return pairError;
                }

                if (keyword == "WITH")
                {
                    foreach (KeyValuePair<int, string> pair in pairs)
                    {
                        line.Modifications.Add(new PieceModification(PieceModificationKind.Add, pair.Value, pair.Key));
                    }
                }
                else if (keyword == "WITHOUT")
                {
                    foreach (KeyValuePair<int, string> pair in pairs)
                    {
                        line.Modifications.Add(new PieceModification(PieceModificationKind.Remove, pair.Value, pair.Key));
                    }
                }
                else
                {
                    if (pairs.Count != 2)
                    {
                        return "REPLACE expects exactly 2 pieces (quantity to remove, quantity to add), got " + pairs.Count;
                    }

                    line.Modifications.Add(new PieceModification(PieceModificationKind.Remove, pairs[0].Value, pairs[0].Key));
                    line.Modifications.Add(new PieceModification(PieceModificationKind.Add, pairs[1].Value, pairs[1].Key));
                }
            }

            return null;
        }

        private string ParsePairs(string segmentText, out List<KeyValuePair<int, string>> pairs)
        {
            pairs = new List<KeyValuePair<int, string>>();

            string[] rawPairs = segmentText.Split(',');
            for (int i = 0; i < rawPairs.Length; i++)
            {
                string rawPair = rawPairs[i].Trim();
                if (rawPair.Length == 0)
                {
                    return "empty piece element in modifier";
                }

                string[] tokens = rawPair.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                if (tokens.Length != 2)
                {
                    return "invalid piece element `" + rawPair + "` (expected: quantity piece)";
                }

                int quantity;
                if (!int.TryParse(tokens[0], out quantity) || quantity <= 0)
                {
                    return "`" + tokens[0] + "` is not a valid positive quantity";
                }

                string pieceName = tokens[1];
                PieceDefinition piece;
                if (!_catalog.TryGet(pieceName, out piece))
                {
                    return "`" + pieceName + "` is not a recognized piece";
                }

                pairs.Add(new KeyValuePair<int, string>(quantity, pieceName));
            }

            return null;
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
