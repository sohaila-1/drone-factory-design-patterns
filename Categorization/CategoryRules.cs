using DroneFactory.Data;
using DroneFactory.Model;

namespace DroneFactory.Categorization
{
    public class AerienRule : ICategoryRule
    {
        public DroneCategory Category => DroneCategory.Aerien;

        public bool IsSatisfiedBy(DroneTemplate template, PieceCatalog catalog)
        {
            return catalog.Get(template.Move).HasTag(PieceTag.F)
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
                && catalog.Get(template.Move).HasTag(PieceTag.M);
        }
    }

    public class TerrestreRule : ICategoryRule
    {
        public DroneCategory Category => DroneCategory.Terrestre;

        public bool IsSatisfiedBy(DroneTemplate template, PieceCatalog catalog)
        {
            return catalog.Get(template.Move).HasTag(PieceTag.L)
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
            // la dimension 2D/3D), donc seules ces trois pieces sont verifiees ici.
            return catalog.Get(template.Hull).HasTag(PieceTag.S)
                && catalog.Get(template.Generator).HasTag(PieceTag.S)
                && catalog.Get(template.Move).HasTag(PieceTag.S)
                && catalog.Get(template.System).HasTag(PieceTag.ThreeD);
        }
    }
}
