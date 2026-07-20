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
}
