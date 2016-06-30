using System;
using Gtk;

namespace EV3CommandAndControl
{
	public class ProgramCommandView : Gtk.Bin
	{
		Label nameLabel;
		Entry parameterEntry;
		Button moveUpButton;
		Button moveDownButton;
		Button removeButton;

		public readonly int id;
		public readonly int index;

		public ProgramCommandView(ProgramCommand c)
		{
			HBox hbox = new HBox(false, 2);

			this.id = c.command.id;
			this.index = c.index;

			nameLabel = new Label();
			nameLabel.Text = CommandModel.Instance.GetCommand(id).name;

			parameterEntry = new Entry();
			parameterEntry.Text = c.parameter.ToString();
			parameterEntry.Changed += delegate {
				int newParameter;
				int.TryParse(parameterEntry.Text, out newParameter);
				CommandModel.Instance.SetProgramCommandParameter(this.index, newParameter);
			};
			parameterEntry.WidthRequest = 80;

			moveUpButton = new Button();
			moveUpButton.Clicked += delegate {
				CommandModel.Instance.MoveCommandInProgram(index, index - 1);
			};
			moveUpButton.Label = "▲";

			moveDownButton = new Button();
			moveDownButton.Clicked += delegate
			{
				CommandModel.Instance.MoveCommandInProgram(index, index + 1);
			};
			moveDownButton.Label = "▼";

			removeButton = new Button();
			removeButton.Clicked += delegate {
				CommandModel.Instance.RemoveCommandFromProgram(index);
			};
			removeButton.Label = "Remove";

			hbox.PackStart(nameLabel, true, true, 0);
			hbox.PackStart(parameterEntry, false, false, 0);
			hbox.PackStart(moveUpButton, false, false, 0);
			hbox.PackStart(moveDownButton, false, false, 0);
			hbox.PackEnd(removeButton, false, false, 0);

			Add(hbox);

			ShowAll();

			CommandModel.Instance.CommandChangedEvent += CommandNameChanged;
		}

		void CommandNameChanged(object sender, CommandEventArgs e)
		{
			if (e.command.id == id)
			{
				nameLabel.Text = e.command.name;
			}
		}

		protected override void OnSizeAllocated(Gdk.Rectangle allocation)
		{
			if (this.Child != null)
			{
				this.Child.Allocation = allocation;
			}
		}

		protected override void OnSizeRequested(ref Requisition requisition)
		{
			if (this.Child != null)
			{
				requisition = this.Child.SizeRequest();
			}
		}

		public override void Destroy()
		{
			CommandModel.Instance.CommandChangedEvent -= CommandNameChanged;

			base.Destroy();
		}
	}
}

