using System;
using System.Collections.Generic;
using DroneFactory.Data;
using DroneFactory.Model;

namespace DroneFactory.Commands
{
    public class AddTemplateCommand : ICommand
    {
        private readonly DroneTemplateFactory _factory;
        private readonly TemplateRepository _templates;

        public AddTemplateCommand(DroneTemplateFactory factory, TemplateRepository templates)
        {
            _factory = factory;
            _templates = templates;
        }

        public void Execute(string arguments)
        {
            if (arguments == null || arguments.Trim().Length == 0)
            {
                Console.WriteLine("ERROR missing template name and pieces");
                return;
            }

            string[] parts = arguments.Split(',');
            if (parts.Length < 2)
            {
                Console.WriteLine("ERROR ADD_TEMPLATE requires a name followed by at least one piece");
                return;
            }

            string name = parts[0].Trim();
            if (name.Length == 0)
            {
                Console.WriteLine("ERROR missing template name");
                return;
            }

            if (_templates.Contains(name))
            {
                Console.WriteLine("ERROR template `" + name + "` already exists");
                return;
            }

            List<string> pieceNames = new List<string>();
            for (int i = 1; i < parts.Length; i++)
            {
                string pieceName = parts[i].Trim();
                if (pieceName.Length == 0)
                {
                    Console.WriteLine("ERROR empty piece name in template definition");
                    return;
                }

                pieceNames.Add(pieceName);
            }

            try
            {
                DroneTemplate template = _factory.Create(name, pieceNames);
                _templates.Register(template);
                Console.WriteLine("TEMPLATE_ADDED " + name);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine("ERROR " + ex.Message);
            }
        }
    }
}
