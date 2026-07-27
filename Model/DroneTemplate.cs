using System.Collections.Generic;

namespace DroneFactory.Model
{
    public class DroneTemplate
    {
        public DroneTemplate(
            string name,
            string hull,
            string core,
            string system,
            IReadOnlyList<string> generators,
            IReadOnlyList<string> moves,
            string processor)
        {
            Name = name;
            Hull = hull;
            Core = core;
            System = system;
            Generators = generators;
            Moves = moves;
            Processor = processor;
            Categories = new List<DroneCategory>();
        }

        public string Name { get; }
        public string Hull { get; }
        public string Core { get; }
        public string System { get; }
        public IReadOnlyList<string> Generators { get; }
        public IReadOnlyList<string> Moves { get; }
        public string Processor { get; }

        public IReadOnlyList<DroneCategory> Categories { get; private set; }

        public void SetCategories(IReadOnlyList<DroneCategory> categories)
        {
            Categories = categories;
        }
    }
}
