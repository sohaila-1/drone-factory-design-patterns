using System.Collections.Generic;
using DroneFactory.Model;

namespace DroneFactory.Assembly
{
    public class DroneAssemblyBuilder
    {
        private readonly List<string> _lines = new List<string>();
        private string _installedCoreNotation;

        public DroneAssemblyBuilder Begin(string droneName)
        {
            _lines.Add("PRODUCING " + droneName);
            return this;
        }

        public DroneAssemblyBuilder GetOutOfStock(string pieceName)
        {
            _lines.Add("GET_OUT_STOCK 1 " + pieceName);
            return this;
        }

        public DroneAssemblyBuilder InstallSystem(string systemName, string coreName)
        {
            _lines.Add("INSTALL " + systemName + " " + coreName);
            _installedCoreNotation = coreName + "{" + systemName + "}";
            return this;
        }

        public DroneAssemblyBuilder AssembleHullWithGenerator(string hull, string generator)
        {
            _lines.Add("ASSEMBLE TMP1 " + hull + " " + generator);
            return this;
        }

        public DroneAssemblyBuilder AttachMoveModule(string move)
        {
            _lines.Add("ASSEMBLE TMP2 TMP1 " + move);
            return this;
        }

        public DroneAssemblyBuilder AttachCore()
        {
            _lines.Add("ASSEMBLE TMP2 " + _installedCoreNotation);
            return this;
        }

        public DroneAssemblyBuilder AttachProcessor(string processor)
        {
            _lines.Add("ASSEMBLE [TMP2, " + _installedCoreNotation + "] " + processor);
            return this;
        }

        public DroneAssemblyBuilder Finish(string droneName)
        {
            _lines.Add("FINISHED " + droneName);
            return this;
        }

        public List<string> Build()
        {
            return _lines;
        }

        public static List<string> BuildInstructions(DroneTemplate drone)
        {
            return new DroneAssemblyBuilder()
                .Begin(drone.Name)
                .GetOutOfStock(drone.Hull)
                .GetOutOfStock(drone.Core)
                .GetOutOfStock(drone.Generator)
                .GetOutOfStock(drone.Move)
                .GetOutOfStock(drone.Processor)
                .InstallSystem(drone.System, drone.Core)
                .AssembleHullWithGenerator(drone.Hull, drone.Generator)
                .AttachMoveModule(drone.Move)
                .AttachCore()
                .AttachProcessor(drone.Processor)
                .Finish(drone.Name)
                .Build();
        }
    }
}
