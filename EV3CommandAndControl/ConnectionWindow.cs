using System;
using Gtk;
using System.Linq;
using EV3MessengerLib;

namespace EV3CommandAndControl
{
	public class ConnectionEventArgs : EventArgs
	{
		public bool connected;

		public ConnectionEventArgs(bool connected)
		{
			this.connected = connected;	
		}
	}

	public class ConnectionWindow : Gtk.Window
	{
		TreeView portList;
		ListStore model;

		string selectedRow;

		public EventHandler<ConnectionEventArgs> ConnectionUpdatedEvent;
		public EventHandler<EventArgs> DisconnectedEvent;

		public ConnectionWindow():base(Gtk.WindowType.Toplevel)
		{
			//MainWindow.MessengerInstance;
			SetDefaultSize(400, 300);
			this.BorderWidth = 5;
			VBox mainBox = new VBox(false, 2);
			Label connectLabel = new Label("Connections");

			portList = new TreeView(model);
			portList.RulesHint = true;
			portList.Selection.Changed += delegate {
				TreeIter iter;
				if (portList.Selection.GetSelected(out iter))
				{
					selectedRow = (string)portList.Model.GetValue(iter, 0);
				}
			};

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
				try
				{
					if (MainWindow.MessengerInstance.IsConnected) MainWindow.MessengerInstance.Disconnect();
					OnRaiseConnectionUpdatedEvent(new ConnectionEventArgs(MainWindow.MessengerInstance.Connect(selectedRow)));
				}
				catch (Exception e)
				{
					Console.WriteLine("Caught exception while connecting: " + e.Message);
					Console.WriteLine(e.StackTrace);
				}
			};

			Button disconnectButton = new Button("Disconnect");
			disconnectButton.Clicked += delegate {
				if (MainWindow.MessengerInstance.IsConnected)
				{
					MainWindow.MessengerInstance.Disconnect();
					OnRaiseDisconnectedEvent(new EventArgs());
				}
			};

			HBox connectDisconnectBox = new HBox(false, 0);
			connectDisconnectBox.PackStart(connectButton, false, false, 0);
			connectDisconnectBox.PackEnd(disconnectButton, false, false, 0);

			Button okButton = new Button("Ok");
			okButton.Clicked += delegate {
				Destroy();
			};

			HBox topHbox = new HBox(false, 2); 
			HBox botHbox = new HBox(false, 2);

			topHbox.PackStart(connectLabel, false, false, 0);
			topHbox.PackEnd(reloadButton, false, false, 0);

			botHbox.PackStart(connectDisconnectBox, false, false, 0);
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
				foreach (string name in portNames)
				{
					model.AppendValues(name);
				}
			}

			portList.Model = model;
		}

		protected virtual void OnRaiseConnectionUpdatedEvent(ConnectionEventArgs e)
		{
			EventHandler<ConnectionEventArgs> handler = ConnectionUpdatedEvent;

			if (handler != null)
			{
				handler(this, e);
			}
		}

		protected virtual void OnRaiseDisconnectedEvent(EventArgs e)
		{
			EventHandler<EventArgs> handler = DisconnectedEvent;

			if (handler != null)
			{
				handler(this, e);
			}
		}
	}
}

