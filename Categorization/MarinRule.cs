using DroneFactory.Data;
using DroneFactory.Model;

namespace DroneFactory.Categorization
{
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
}
