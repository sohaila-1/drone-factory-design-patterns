using DroneFactory.Data;
using DroneFactory.Model;

namespace DroneFactory.Categorization
{
    public class TerrestreRule : ICategoryRule
    {
        public DroneCategory Category => DroneCategory.Terrestre;

        public bool IsSatisfiedBy(DroneTemplate template, PieceCatalog catalog)
        {
            return catalog.Get(template.Move).HasTag(PieceTag.L)
                && catalog.Get(template.System).HasTag(PieceTag.TwoD);
        }
    }
}
