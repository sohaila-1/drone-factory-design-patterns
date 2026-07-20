using DroneFactory.Data;
using DroneFactory.Model;

namespace DroneFactory.Categorization
{
    public interface ICategoryRule
    {
        DroneCategory Category { get; }
        bool IsSatisfiedBy(DroneTemplate template, PieceCatalog catalog);
    }
}
