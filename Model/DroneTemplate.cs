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
            string generator,
            string move,
            string processor)
        {
            Name = name;
            Hull = hull;
            Core = core;
            System = system;
            Generator = generator;
            Move = move;
            Processor = processor;
            Categories = new List<DroneCategory>();
        }

        public string Name { get; }
        public string Hull { get; }
        public string Core { get; }
        public string System { get; }
        public string Generator { get; }
        public string Move { get; }
        public string Processor { get; }

        public IReadOnlyList<DroneCategory> Categories { get; private set; }

        public void SetCategories(IReadOnlyList<DroneCategory> categories)
        {
            Categories = categories;
        }
    }
}
