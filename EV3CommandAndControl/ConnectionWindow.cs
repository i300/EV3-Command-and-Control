using System;
using Gtk;
using System.Linq;
using EV3MessengerLib;

namespace EV3CommandAndControl
{
	public class ConnectionWindow : Gtk.Window
	{
		TreeView portList;
		ListStore model;

		string selectedRow;

		public ConnectionWindow():base(Gtk.WindowType.Toplevel)
		{
			//MainWindow.MessengerInstance;
			SetDefaultSize(900, 500);
			this.BorderWidth = 5;
			VBox mainBox = new VBox(false, 2);
			Label connectLabel = new Label("Connections");

			portList = new TreeView(model);
			portList.RulesHint = true;
			portList.RowActivated += RowActivated;

			CellRendererText rendererText = new CellRendererText();
			TreeViewColumn column = new TreeViewColumn("Port", rendererText, "text", 0);
			column.SortColumnId = 0;
			portList.AppendColumn(column);

			model = new ListStore(typeof(String));
			portList.Model = model;

			Button reloadButton = new Button("Reload");
			reloadButton.Clicked += delegate
			{
				ReloadPorts();
			};

			Button connectButton = new Button("Connect");
			connectButton.Clicked += delegate {
				MainWindow.MessengerInstance.Connect(selectedRow);
			};

			Button okButton = new Button("Ok");
			okButton.Clicked += delegate {
				Destroy();
			};

			HBox topHbox = new HBox(false, 2); 
			HBox botHbox = new HBox(false, 2);

			topHbox.PackStart(connectLabel, false, false, 0);
			topHbox.PackEnd(reloadButton, false, false, 0);

			botHbox.PackStart(connectButton, false, false, 0);
			botHbox.PackEnd(okButton, false, false, 0);

			mainBox.PackStart(topHbox, false, false, 0);
			mainBox.PackStart(portList, true, true, 0);
			mainBox.PackStart(botHbox, false, false, 0);

			Add(mainBox);
			ShowAll();

			ReloadPorts();
		}

		void ReloadPorts()
		{
			model.Clear();

			string[] portNames = System.IO.Ports.SerialPort.GetPortNames();

			string os_platform = System.Environment.OSVersion.Platform.ToString();
			if (os_platform == "Unix")
			{
				var sortedPortNames = from x in portNames where x.Contains("tty.") select x;

				foreach (string name in sortedPortNames)
				{
					model.AppendValues(name);
				}
			}
			else {
				model.AppendValues(portNames);
			}


			portList.Model = model;
		}

		void RowActivated(object sender, RowActivatedArgs args)
		{
			TreeIter iter;
			TreeView view = (TreeView)sender;
			if (view.Model.GetIter(out iter, args.Path))
			{
				selectedRow = (string)view.Model.GetValue(iter, 0);
			}
		}
	}
}

