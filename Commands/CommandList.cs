﻿using System;
using System.Collections;
using System.Collections.Generic;

namespace Gash.Commands
{
    internal class CommandList : IEnumerable<ICommand>
    {
        private List<ICommand> KnownCommands = new List<ICommand>();
        private Dictionary<string, ICommand> KnownCommandsMap = new Dictionary<string, ICommand>();

        public void RegisterCommand(ICommand command)
        {
            KnownCommands.Add(command);
            KnownCommandsMap.Add(command.Name(), command);
        }

        internal void ParseLine(string line)
        {
            ParsingResult result = new ParsingResult();

            foreach (var command in KnownCommands)
            {
                result = command.Parse(line);
                if (result.Type == ParsingResultType.Success || result.Type == ParsingResultType.SuccessReachedEnd ||
                    result.Type == ParsingResultType.ParsingFailure || result.Type == ParsingResultType.MissingParam)
                {
                    break;
                }
            }

            if (result.Type == ParsingResultType.WrongCommand)
            {
                string command = line;
                var parsingResult = ParsingHelpers.TryAnyCommandBody(line);
                if (parsingResult.WasSuccessful && parsingResult.Value.Length > 0)
                {
                    command = parsingResult.Value;
                }

                GConsole.WriteLine(GConsole.ColorifyText(1,String.Format(Resources.text.UnknownCommand, command)));
            }
        }

        internal bool FindMan(string commandName)
        {
            ICommand result = null;
            if (KnownCommandsMap.TryGetValue(commandName, out result))
            {
                result.PrintManPage();
                return true;
            }

            return false;
        }

        public IEnumerator<ICommand> GetEnumerator()
        {
            return KnownCommands.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return KnownCommands.GetEnumerator();
        }
    }
}
