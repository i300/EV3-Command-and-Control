using System;
using Gtk;
using System.Linq;
using EV3MessengerLib;

namespace EV3CommandAndControl
{
	public class ConnectionWindow : Gtk.Window
	{
		ListStore model;

		public ConnectionWindow():base(Gtk.WindowType.Toplevel)
		{
			//MainWindow.MessengerInstance;
			SetDefaultSize(900, 500);
			this.BorderWidth = 5;
			VBox mainBox = new VBox(false, 2);
			Label connectLabel = new Label("Connections");
			Button reloadPorts = new Button("Reload");

			TreeView portList = new TreeView(model);
			portList.RulesHint = true;
			portList.RowActivated += RowActivated;

			TreeViewColumn ports = new TreeViewColumn();
			ports.Title = "Ports";
			Gtk.CellRendererText portNameCell = new Gtk.CellRendererText();
			ports.PackStart(portNameCell, true);
			portList.AppendColumn(ports);

			ports.AddAttribute(portNameCell, "text", 0);

			string[] listOfPortNames = System.IO.Ports.SerialPort.GetPortNames();

			ListStore portListStore = new ListStore(typeof(String));
			portList.Model = portListStore;

			var sortedPortNames = from x in listOfPortNames where x.Contains("tty.") select x;

			foreach (string name in sortedPortNames)
			{
				portListStore.AppendValues(name);
			}

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

		void AddColumns(TreeView treeview)
		{
		}
		void RowActivated(object sender, RowActivatedArgs args)
		{
			TreeIter iter;
			TreeView view = (TreeView)sender;
			if (view.Model.GetIter(out iter, args.Path))
			{
				string row = (string)view.Model.GetValue(iter, 0);
				MainWindow.MessengerInstance.Connect(row);
			}
		}
	}
}

