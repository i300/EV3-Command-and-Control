using System;
using Gtk;

namespace EV3CommandAndControl
{
	public class CommandView : Gtk.Bin
	{
		Label idLabel;
		Entry nameEntry;
		Button insertButton;
		Button removeButton;

		public readonly int id;

		public CommandView(Command c)
		{
			id = c.id;

			HBox hbox = new HBox(false, 2);

			idLabel = new Label();
			idLabel.Text = c.id.ToString();

			nameEntry = new Entry();
			nameEntry.Text = c.name;
			nameEntry.Changed += OnNameChanged;

			insertButton = new Button();
			insertButton.Label = "▶";
			insertButton.Clicked += OnInsertClicked;

			removeButton = new Button();
			removeButton.Label = "Delete";
			removeButton.Clicked += OnDeleteClicked;
			removeButton.Sensitive = CommandModel.Instance.deleteEnabled;

			hbox.PackStart(idLabel, false, false, 0);
			hbox.PackStart(nameEntry, true, true, 0);
			hbox.PackStart(insertButton, false, false, 0);
			hbox.PackEnd(removeButton, false, false, 0);

			Add(hbox);

			ShowAll();
		}

		void OnNameChanged(object sender, EventArgs e)
		{
			CommandModel.Instance.ChangeCommandName(id, nameEntry.Text);
		}

		void OnInsertClicked(object sender, EventArgs e)
		{
			CommandModel model = CommandModel.Instance;

			model.AddCommandToProgram(model.GetCommand(id));
		}

		void OnDeleteClicked(object sender, EventArgs e)
		{
			CommandModel.Instance.RemoveCommand(id);
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
	}
}

