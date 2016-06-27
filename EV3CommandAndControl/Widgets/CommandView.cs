using System;
using Gtk;

namespace EV3CommandAndControl
{
	public class CommandView : Gtk.Bin
	{
		Label nameLabel;
		Entry parameterEntry;
		Button moveUpButton;
		Button moveDownButton;
		Button removeButton;

		public CommandView()
		{
			HBox hbox = new HBox(false, 2);

			nameLabel = new Label();
			nameLabel.Text = "Name";

			parameterEntry = new Entry();
			parameterEntry.WidthRequest = 80;

			moveUpButton = new Button();
			moveUpButton.Label = "▲";

			moveDownButton = new Button();
			moveDownButton.Label = "▼";

			removeButton = new Button();
			removeButton.Label = "Remove";

			hbox.PackStart(nameLabel, true, true, 0);
			hbox.PackStart(parameterEntry, false, false, 0);
			hbox.PackStart(moveUpButton, false, false, 0);
			hbox.PackStart(moveDownButton, false, false, 0);
			hbox.PackEnd(removeButton, false, false, 0);

			Add(hbox);

			ShowAll();
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

