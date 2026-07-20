using System;
using System.Collections.Generic;
using DroneFactory.Model;

namespace DroneFactory.Data
{
    public class TemplateRepository
    {
        private readonly Dictionary<string, DroneTemplate> _templates =
            new Dictionary<string, DroneTemplate>(StringComparer.Ordinal);

        private readonly List<string> _order = new List<string>();
        private readonly StockRepository _stocks;

        public TemplateRepository(StockRepository stocks)
        {
            _stocks = stocks;
        }

        public IReadOnlyList<string> TemplateNames => _order;

        public bool Contains(string name)
        {
            return _templates.ContainsKey(name);
        }

        public DroneTemplate Get(string name)
        {
            return _templates[name];
        }

        public void Register(DroneTemplate template)
        {
            if (_templates.ContainsKey(template.Name))
            {
                throw new ArgumentException("template `" + template.Name + "` already exists");
            }

            _templates[template.Name] = template;
            _order.Add(template.Name);
            _stocks.RegisterDrone(template.Name);
        }
    }
}
