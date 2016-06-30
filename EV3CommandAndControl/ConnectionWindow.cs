using System;
using Gtk;
namespace EV3CommandAndControl
{
	public class ConnectionWindow : Gtk.Window
	{
		ListStore model;

		public ConnectionWindow():base(Gtk.WindowType.Toplevel)
		{
			SetDefaultSize(900, 500);
			this.BorderWidth = 5;
			VBox mainBox = new VBox(false, 2);
			Label connectLabel = new Label("Connections");
			Button reloadPorts = new Button("Reload");
			TreeView portList = new TreeView(model);
			Button connectToPort = new Button("Connect");
			Button okButton = new Button("Ok");
			HBox topHbox = new HBox(false, 2); 
			HBox botHbox = new HBox(false, 2);



			topHbox.PackStart(connectLabel, false, false, 0);
			topHbox.PackEnd(reloadPorts, false, false, 0);

			botHbox.PackStart(connectToPort, false, false, 0);
			botHbox.PackEnd(okButton, false, false, 0);

			mainBox.PackStart(topHbox, false, false, 0);
			mainBox.PackStart(portList, true, true, 0);
			mainBox.PackStart(botHbox, false, false, 0);




			Add(mainBox);
			 

			ShowAll();

		}
	}
}

