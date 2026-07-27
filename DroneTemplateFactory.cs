using System;
using System.Collections.Generic;
using DroneFactory.Categorization;
using DroneFactory.Data;
using DroneFactory.Model;

namespace DroneFactory
{
    public class DroneTemplateFactory
    {
        private readonly PieceCatalog _catalog;
        private readonly DroneCategorizer _categorizer;

        public DroneTemplateFactory(PieceCatalog catalog, DroneCategorizer categorizer)
        {
            _catalog = catalog;
            _categorizer = categorizer;
        }

        public DroneTemplate Create(string name, List<string> pieceNames)
        {
            string hull = null;
            string core = null;
            string system = null;
            string processor = null;
            List<string> generators = new List<string>();
            List<string> moves = new List<string>();

            foreach (string pieceName in pieceNames)
            {
                PieceDefinition piece;
                if (!_catalog.TryGet(pieceName, out piece))
                {
                    throw new ArgumentException("`" + pieceName + "` is not a recognized piece");
                }

                switch (piece.Kind)
                {
                    case PieceKind.Hull:
                        AssignOnce(ref hull, piece.Name, "coque (Hull)");
                        break;
                    case PieceKind.Core:
                        AssignOnce(ref core, piece.Name, "module principal (Core)");
                        break;
                    case PieceKind.System:
                        AssignOnce(ref system, piece.Name, "systeme");
                        break;
                    case PieceKind.Generator:
                        generators.Add(piece.Name);
                        break;
                    case PieceKind.Move:
                        moves.Add(piece.Name);
                        break;
                    case PieceKind.Processor:
                        AssignOnce(ref processor, piece.Name, "module de controle");
                        break;
                }
            }

            RequirePresent(hull, "coque (Hull)");
            RequirePresent(core, "module principal (Core)");
            RequirePresent(system, "systeme");
            RequirePresent(processor, "module de controle");

            string constructionError = ConstructionRules.Validate(generators.Count, moves.Count);
            if (constructionError != null)
            {
                throw new ArgumentException(constructionError);
            }

            DroneTemplate template = new DroneTemplate(name, hull, core, system, generators, moves, processor);
            List<DroneCategory> categories = _categorizer.Categorize(template);

            if (categories.Count == 0)
            {
                throw new ArgumentException(
                    "template `" + name + "` does not match any drone category (Aerien/Marin/Terrestre/Submersible)");
            }

            template.SetCategories(categories);
            return template;
        }

        private static void AssignOnce(ref string slot, string pieceName, string roleLabel)
        {
            if (slot != null)
            {
                throw new ArgumentException(
                    "template already has a " + roleLabel + " (`" + slot + "`), got a second one (`" + pieceName + "`)");
            }

            slot = pieceName;
        }

        private static void RequirePresent(string slot, string roleLabel)
        {
            if (slot == null)
            {
                throw new ArgumentException("template is missing a " + roleLabel);
            }
        }
    }
}
