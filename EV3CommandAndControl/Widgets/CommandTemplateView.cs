using System;
using Gtk;

namespace EV3CommandAndControl
{
	public class CommandTemplateView : Gtk.Bin
	{
		Label idLabel;
		Entry nameEntry;
		Button insertButton;
		Button removeButton;

		int commandID;

		public CommandTemplateView(ref Command c)
		{
			commandID = c.ID;

			HBox hbox = new HBox(false, 2);

			idLabel = new Label();
			idLabel.Text = c.ID.ToString();

			nameEntry = new Entry();
			nameEntry.Text = c.name;
			nameEntry.Changed += OnNameChanged;

			insertButton = new Button();
			insertButton.Label = "▶";
			insertButton.Clicked += OnInsertClicked;

			removeButton = new Button();
			removeButton.Label = "Delete";
			removeButton.Clicked += OnDeleteClicked;

			hbox.PackStart(idLabel, false, false, 0);
			hbox.PackStart(nameEntry, true, true, 0);
			hbox.PackStart(insertButton, false, false, 0);
			hbox.PackEnd(removeButton, false, false, 0);

			Add(hbox);

			ShowAll();
		}

		void OnNameChanged(object sender, EventArgs e)
		{

		}

		void OnInsertClicked(object sender, EventArgs e)
		{

		}

		void OnDeleteClicked(object sender, EventArgs e)
		{
			CommandModel.Instance.RemoveCommand(command);
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

