using System.Collections.Generic;
using DroneFactory.Data;
using DroneFactory.Model;

namespace DroneFactory.Categorization
{
    public class DroneCategorizer
    {
        private readonly List<ICategoryRule> _rules;
        private readonly PieceCatalog _catalog;

        public DroneCategorizer(PieceCatalog catalog)
        {
            _catalog = catalog;
            _rules = new List<ICategoryRule>
            {
                new AerienRule(),
                new MarinRule(),
                new TerrestreRule(),
                new SubmersibleRule()
            };
        }

        public List<DroneCategory> Categorize(DroneTemplate template)
        {
            List<DroneCategory> categories = new List<DroneCategory>();

            for (int i = 0; i < _rules.Count; i++)
            {
                if (_rules[i].IsSatisfiedBy(template, _catalog))
                {
                    categories.Add(_rules[i].Category);
                }
            }

            return categories;
        }
    }
}
