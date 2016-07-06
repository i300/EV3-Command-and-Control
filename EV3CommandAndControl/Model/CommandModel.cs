using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Newtonsoft.Json;

namespace EV3CommandAndControl
{
	public class CommandEventArgs : EventArgs
	{
		public readonly Command command;

		public CommandEventArgs(Command c)
		{
			this.command = c;
		}
	}

	public class ProgramCommandEventArgs : EventArgs
	{
		public readonly ProgramCommand command;

		public ProgramCommandEventArgs(ProgramCommand c)
		{
			this.command = c;
		}
	}

	public class CommandModel
	{
		private static CommandModel instance;

		Dictionary<int, Command> commands;
		List<ProgramCommand> program;

		public bool deleteEnabled = true;

		public event EventHandler<CommandEventArgs> CommandAddedEvent;
		public event EventHandler<CommandEventArgs> CommandChangedEvent;
		public event EventHandler<CommandEventArgs> CommandRemovedEvent;

		public event EventHandler<ProgramCommandEventArgs> ProgramCommandAddedEvent;
		public event EventHandler<EventArgs> ProgramCommandRemovedEvent;
		
		public CommandModel()
		{
			commands = new Dictionary<int, Command>();
			program = new List<ProgramCommand>();
		}

		public static CommandModel Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new CommandModel();
				}
				return instance;
			}
		}

		public Dictionary<int, Command> GetCommands()
		{
			return commands;
		}

		public List<ProgramCommand> GetProgram()
		{
			return program;
		}

		public void SetCommands(Dictionary<int, Command> c)
		{
			RemoveAllCommands();

			foreach (int index in c.Keys)
			{
				NewCommand(c[index].name);
			}
		}

		public void SetProgram(List<ProgramCommand> p)
		{
			foreach (ProgramCommand c in p)
			{
				AddCommandToProgram(GetCommand(c.command.id), c.parameter);
			}
		}

		public Command NewCommand(string name="")
		{
			int newID = 0;

			for (;;)
			{
				if (!commands.ContainsKey(newID))
				{
					break;
				}

				newID += 1;
			}

			Command c = new Command(newID);
			c.name = name;

			commands.Add(newID, c);

			OnRaiseCommandAddedEvent(new CommandEventArgs(c));

			return c;
		}

		public Command GetCommand(int id)
		{
			if (commands.ContainsKey(id))
			{
				return commands[id];
			}
			else {
				return null;
			}
		}

		public void ChangeCommandName(int id, string name)
		{
			if (commands.ContainsKey(id))
			{
				Command c = commands[id];
				c.name = name;
				commands[id] = c;

				OnRaiseCommandChangedEvent(new CommandEventArgs(c));
			}
		}

		public void RemoveAllCommands()
		{
			if (commands.Keys.Count > 0)
			{
				// Loop through commands array backwards to avoid
				// out of bounds exceptions when removing from the array
				for (int i = commands.Keys.Count - 1; i >= 0; i--)
				{
					RemoveCommand(i);
				}
			}
		}

		public void RemoveCommand(int id)
		{
			if (commands.ContainsKey(id))
			{
				Command c = commands[id];
				commands.Remove(id);

				// Flag to check if a program was removed or not
				bool removedProgram = false;

				// Loop through program array backwards to avoid
				// out of bounds exceptions when removing from the array
				for (int i = program.Count - 1; i >= 0; i--)
				{
					if (program[i].command.id == id)
					{
						program.Remove(program[i]);

						removedProgram = true;
					}
				}

				if (removedProgram)
				{
					// Reassign indexes for program list
					for (int i = 0; i < program.Count; i++)
					{
						program[i].index = i;
					}

					// Raise program command removed
					OnRaiseProgramCommandRemovedEvent(new EventArgs());
				}

				OnRaiseCommandRemovedEvent(new CommandEventArgs(c));
			}
		}

		public void AddCommandToProgram(Command c, int param=0)
		{
			ProgramCommand command = new ProgramCommand(program.Count, c);
			command.parameter = param;
			program.Add(command);

			OnRaiseProgramCommandAddedEvent(new ProgramCommandEventArgs(command));
		}

		public void SetProgramCommandParameter(int index, int param)
		{
			program[index].parameter = param;
		}

		public void MoveCommandInProgram(int fromIndex, int toIndex)
		{
			ProgramCommand commandA, commandB;

			try
			{
				// Store values of from and to indexes
				commandA = program[fromIndex];
				commandB = program[toIndex];	
			}
			catch (Exception e)
			{
				// Indexes are out of range, break out
				System.Console.WriteLine(e.Message);
				return;
			}

			// Move command A to the to index
			program[toIndex] = commandA;
			commandA.index = toIndex;

			// Move command B to the from index
			program[fromIndex] = commandB;
			commandB.index = fromIndex;

			// Raise event
			OnRaiseProgramCommandRemovedEvent(new EventArgs());
		}

		public void RemoveCommandFromProgram(int index)
		{
			// Remove program from program list
			ProgramCommand command = program[index];
			program.RemoveAt(index);

			// Reassign indexes for program list
			for (int i = 0; i < program.Count; i++)
			{
				program[i].index = i;
			}

			// Raise program command removed event
			OnRaiseProgramCommandRemovedEvent(new EventArgs());
		}

		public string ProgramToJSON()
		{
			return JsonConvert.SerializeObject(program);
		}

		protected virtual void OnRaiseCommandAddedEvent(CommandEventArgs e)
		{
			EventHandler<CommandEventArgs> handler = CommandAddedEvent;

			if (handler != null)
			{
				handler(this, e);
			}
		}

		protected virtual void OnRaiseCommandChangedEvent(CommandEventArgs e)
		{
			EventHandler<CommandEventArgs> handler = CommandChangedEvent;

			if (handler != null)
			{
				handler(this, e);
			}
		}

		protected virtual void OnRaiseCommandRemovedEvent(CommandEventArgs e)
		{
			EventHandler<CommandEventArgs> handler = CommandRemovedEvent;

			if (handler != null)
			{
				handler(this, e);
			}
		}

		protected virtual void OnRaiseProgramCommandAddedEvent(ProgramCommandEventArgs e)
		{
			EventHandler<ProgramCommandEventArgs> handler = ProgramCommandAddedEvent;

			if (handler != null)
			{
				handler(this, e);
			}
		}

		protected virtual void OnRaiseProgramCommandRemovedEvent(EventArgs e)
		{
			EventHandler<EventArgs> handler = ProgramCommandRemovedEvent;

			if (handler != null)
			{
				handler(this, e);
			}
		}
	}
}

