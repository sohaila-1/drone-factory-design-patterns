using System;
using System.Collections.Generic;

namespace DroneFactory.Commands
{
    public class CommandRegistry
    {
        private readonly Dictionary<string, ICommand> _commands =
            new Dictionary<string, ICommand>(StringComparer.Ordinal);

        public void Register(string instructionName, ICommand command)
        {
            _commands[instructionName] = command;
        }

        public void Dispatch(string instructionName, string arguments)
        {
            ICommand command;
            if (_commands.TryGetValue(instructionName, out command))
            {
                command.Execute(arguments);
                return;
            }

            Console.WriteLine("ERROR unknown instruction `" + instructionName + "`");
        }
    }
}
