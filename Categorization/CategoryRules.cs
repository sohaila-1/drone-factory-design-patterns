using System.Collections.Generic;
using DroneFactory.Data;
using DroneFactory.Model;

namespace DroneFactory.Categorization
{
    internal static class PieceListTags
    {
        public static bool Any(IReadOnlyList<string> pieceNames, PieceTag tag, PieceCatalog catalog)
        {
            for (int i = 0; i < pieceNames.Count; i++)
            {
                if (catalog.Get(pieceNames[i]).HasTag(tag))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool All(IReadOnlyList<string> pieceNames, PieceTag tag, PieceCatalog catalog)
        {
            for (int i = 0; i < pieceNames.Count; i++)
            {
                if (!catalog.Get(pieceNames[i]).HasTag(tag))
                {
                    return false;
                }
            }

            return true;
        }
    }

    public class AerienRule : ICategoryRule
    {
        public DroneCategory Category => DroneCategory.Aerien;

        public bool IsSatisfiedBy(DroneTemplate template, PieceCatalog catalog)
        {
            return PieceListTags.Any(template.Moves, PieceTag.F, catalog)
                && catalog.Get(template.System).HasTag(PieceTag.ThreeD);
        }
    }

    public class MarinRule : ICategoryRule
    {
        public DroneCategory Category => DroneCategory.Marin;

        public bool IsSatisfiedBy(DroneTemplate template, PieceCatalog catalog)
        {
            return catalog.Get(template.Hull).HasTag(PieceTag.S)
                && catalog.Get(template.System).HasTag(PieceTag.TwoD)
                && PieceListTags.Any(template.Moves, PieceTag.M, catalog);
        }
    }

    public class TerrestreRule : ICategoryRule
    {
        public DroneCategory Category => DroneCategory.Terrestre;

        public bool IsSatisfiedBy(DroneTemplate template, PieceCatalog catalog)
        {
            return PieceListTags.Any(template.Moves, PieceTag.L, catalog)
                && catalog.Get(template.System).HasTag(PieceTag.TwoD);
        }
    }

    public class SubmersibleRule : ICategoryRule
    {
        public DroneCategory Category => DroneCategory.Submersible;

        public bool IsSatisfiedBy(DroneTemplate template, PieceCatalog catalog)
        {
            // "Toutes les pieces (S)" : seuls Hull/Generator/Move portent la
            // dimension F/M/L/S dans le catalogue (Core/Processor/System portent
            // la dimension 2D/3D), donc seules ces pieces sont verifiees ici.
            return catalog.Get(template.Hull).HasTag(PieceTag.S)
                && PieceListTags.All(template.Generators, PieceTag.S, catalog)
                && PieceListTags.All(template.Moves, PieceTag.S, catalog)
                && catalog.Get(template.System).HasTag(PieceTag.ThreeD);
        }
    }
}
