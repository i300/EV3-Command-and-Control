using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using Gtk;
using EV3CommandAndControl;
using EV3MessengerLib;
using System.Threading;

class SendProgramWorker
{
	EV3Messenger messenger;
	List<ProgramCommand> program;

	public EventHandler<EventArgs> OnFinishedEvent;

	public SendProgramWorker(EV3Messenger m, List<ProgramCommand> p)
	{
		this.messenger = m;
		this.program = p;
	}

	public void Send()
	{
		int _commandSent = 0;
		foreach (ProgramCommand c in program)
		{
			messenger.SendMessage("abc", c.command.id);
			Thread.Sleep(1000);
			messenger.SendMessage("abc", c.parameter);
			Thread.Sleep(500);

			Console.WriteLine("Waiting for handshake...");
			EV3Message message;
			for (;;)
			{
				message = messenger.ReadMessage();
				if (message != null)
				{
					if (message.ValueAsText == c.command.id.ToString() + "+" + c.parameter.ToString())
					{
						Console.WriteLine("Handshake `" + message.ValueAsText + "` Recieved");
						break;
					}
				}
				Thread.Sleep(10);

				if (_shouldStop)
				{
					break;
				}
			}

			_commandSent++;

			if (_shouldStop)
			{
				Console.WriteLine("CANCELLING THREAD");
				Console.WriteLine("CANCELLING THREAD");
				Console.WriteLine("CANCELLING THREAD");

				break;
			}
		}

		Console.WriteLine("Sent " + _commandSent.ToString() + " commands.");

		OnRaiseOnFinishedEvent(new EventArgs());
	}

	void OnRaiseOnFinishedEvent(EventArgs e)
	{
		EventHandler<EventArgs> handler = OnFinishedEvent;

		if (handler != null)
		{
			handler(this, e);
		}
	}

	public void RequestStop()
	{
		_shouldStop = true;
	}

	private volatile bool _shouldStop;
}

public partial class MainWindow : Gtk.Window
{
	ScrollableView palleteView;
	ScrollableView queueView;

	Statusbar statusbar;

	RadioMenuItem simpleView;
	RadioMenuItem advancedView;

	Button addNewCommandButton;
	Button sendButton;

	List<CommandView> commandViews;
	List<ProgramCommandView> programCommandViews;

	CommandModel model;

	private static volatile EV3Messenger messenger;

	public static EV3Messenger MessengerInstance
	{
		get
		{
			if (messenger == null)
			{
				messenger = new EV3Messenger();
			}
			return messenger;
		}
	}

	public MainWindow() : base(Gtk.WindowType.Toplevel)
	{
		SetDefaultSize(900, 500);
		SetPosition(WindowPosition.Center);
		DeleteEvent += OnDeleteEvent;

		model = CommandModel.Instance;
		model.CommandAddedEvent += OnCommandAdded;
		model.CommandRemovedEvent += OnCommandRemoved;
		model.ProgramCommandAddedEvent += OnProgramCommandAdded;
		model.ProgramCommandRemovedEvent += OnProgramCommandRemoved;

		messenger = MainWindow.MessengerInstance;

		commandViews = new List<CommandView>();
		programCommandViews = new List<ProgramCommandView>();

		VBox mainBox = new VBox(false, 2);
		MenuBar mb = new MenuBar();

		Menu fileMenu = new Menu();
		MenuItem file = new MenuItem("File");
		file.Submenu = fileMenu;

		Menu viewMenu = new Menu();
		MenuItem view = new MenuItem("View");
		view.Submenu = viewMenu;

		Menu connectionMenu = new Menu();
		MenuItem connectionStatus = new MenuItem("Connection Status");
		connectionStatus.Submenu = connectionMenu;

		MenuItem saveAs = new MenuItem("Save as...");
		saveAs.Activated += SaveCommands;
		fileMenu.Append(saveAs);

		MenuItem loadFile = new MenuItem("Load");
		loadFile.Activated += LoadCommands;
		fileMenu.Append(loadFile);

		simpleView = new RadioMenuItem("Simple");
		simpleView.Activated += SwitchView;
		viewMenu.Append(simpleView);

		advancedView = new RadioMenuItem(simpleView, "Advanced");
		advancedView.Activated += SwitchView;
		viewMenu.Append(advancedView);

		MenuItem tryConnect = new MenuItem("Connect");
		tryConnect.Activated += ShowConnectionUI;
		connectionMenu.Append(tryConnect);

		mb.Append(file);
		mb.Append(view);
		mb.Append(connectionStatus);

		statusbar = new Statusbar();
		statusbar.Push(1, "Disconnected");

		HBox hbox = new HBox(false, 2);

		hbox.BorderWidth = 5;

		VBox leftBox = new VBox(false, 2);

		addNewCommandButton = new Button();
		addNewCommandButton.Label = "Create New Command";
		addNewCommandButton.Clicked += delegate { model.NewCommand(); };

		Alignment addNewCommandButtonAlign = new Alignment(0, 0, 0, 0);
		addNewCommandButtonAlign.Add(addNewCommandButton);

		Label commandPalleteLabel = new Label();
		commandPalleteLabel.Text = "Command Pallete";
		commandPalleteLabel.SetAlignment(0, 0);

		palleteView = new ScrollableView();

		leftBox.PackStart(commandPalleteLabel, false, false, 0);
		leftBox.PackStart(palleteView, true, true, 0);
		leftBox.PackEnd(addNewCommandButtonAlign, false, false, 0);

		VBox rightBox = new VBox(false, 2);

		Label commandQueueLabel = new Label();
		commandQueueLabel.Text = "Command Queue";
		commandQueueLabel.SetAlignment(0, 0);

		HBox sendHBox = new HBox(false, 2);

		sendButton = new Button();
		sendButton.Clicked += OnSendButtonClicked;
		sendButton.Label = "Send";

		Alignment sendButtonAlign = new Alignment(1, 0, 0, 0);
		sendButtonAlign.Add(sendButton);

		//sendHBox.PackStart();
		sendHBox.PackEnd(sendButtonAlign, true, true, 0);

		queueView = new ScrollableView();

		rightBox.PackStart(commandQueueLabel, false, false, 0);
		rightBox.PackStart(queueView, true, true, 0);
		rightBox.PackEnd(sendHBox, false, false, 0);

		hbox.PackStart(leftBox, true, true, 0);
		hbox.PackStart(new VSeparator(), false, false, 0);
		hbox.PackEnd(rightBox, true, true, 0);

		mainBox.PackStart(mb, false, false, 0);
		mainBox.PackStart(hbox, true, true, 0);
		mainBox.PackEnd(statusbar, false, false, 0);

		Add(mainBox);

		ShowAll();

		simpleView.Activate();
	}

	void SwitchView(object sender, EventArgs args)
	{
		if (simpleView.Active)
		{
			model.RemoveAllCommands();
			model.deleteEnabled = false;

			model.NewCommand("Forward");
			model.NewCommand("Reverse");
			model.NewCommand("Left");
			model.NewCommand("Right");

			addNewCommandButton.Sensitive = false;
		}
		else if (advancedView.Active)
		{
			model.RemoveAllCommands();
			model.deleteEnabled = true;

			addNewCommandButton.Sensitive = true;
		}
	}

	void LoadCommands(object sender, EventArgs args)
	{
		FileChooserDialog filechooser = new FileChooserDialog("Choose the file to open",
			this,
			FileChooserAction.Open,
			"Cancel", ResponseType.Cancel,
			"Open", ResponseType.Accept);

		if (filechooser.Run() == (int)ResponseType.Accept)
		{
			String loadedJson = "";
			// Open the text file using a stream reader.
			using (StreamReader sr = new StreamReader(filechooser.Filename))
			{
				// Read the stream to a string, and write the string to the console.
				String line = sr.ReadToEnd();
				loadedJson += line;
			}

			List<string> payload = JsonConvert.DeserializeObject<List<string>>(loadedJson);

			Dictionary<int, Command> commands = JsonConvert.DeserializeObject<Dictionary<int, Command>>(payload[0]);
			List<ProgramCommand> program = JsonConvert.DeserializeObject<List<ProgramCommand>>(payload[1]);

			advancedView.Activate();

			model.SetCommands(commands);
			model.SetProgram(program);
		}

		filechooser.Destroy();
	}

	void SaveCommands(object sender, EventArgs args)
	{
		FileChooserDialog filechooser = new FileChooserDialog("Choose a location to save",
			this,
	 		FileChooserAction.Save,
			"Cancel", ResponseType.Cancel,
			"Save", ResponseType.Accept);

		string commandsJSON = JsonConvert.SerializeObject(model.GetCommands());
		string programJSON = JsonConvert.SerializeObject(model.GetProgram());

		List<string> payload = new List<string>();
		payload.Add(commandsJSON);
		payload.Add(programJSON);

		string payloadJSON = JsonConvert.SerializeObject(payload);

		if (filechooser.Run() == (int)ResponseType.Accept)
		{
			File.WriteAllText(filechooser.Filename, payloadJSON);
		}

		filechooser.Destroy();
	}

	void ShowConnectionUI(object sender, EventArgs args)
	{
		ConnectionWindow w = new ConnectionWindow();
		w.ConnectionUpdatedEvent += OnConnectionUpdated;
		w.DisconnectedEvent += delegate {
			statusbar.Push(1, "Disconnected");
		};
		w.Show();
	}

	void OnConnectionUpdated(object sender, ConnectionEventArgs e)
	{
		if (e.connected)
		{
			statusbar.Push(1, "Connected to EV3");
		}
		else {
			statusbar.Push(1, "Connection Failed");
		}
	}


	SendProgramWorker worker;
	Thread sendThread;
	void OnSendButtonClicked(object sender, EventArgs e)
	{
		Button self = (Button)sender;
		if (self.Label == "Cancel")
		{
			if (worker != null)
			{
				worker.RequestStop();
				while (sendThread.IsAlive) ;

			}
		}
		else {
			if (messenger.IsConnected)
			{
				worker = new SendProgramWorker(messenger, model.GetProgram());
				worker.OnFinishedEvent += OnFinishedSendingProgram;

				sendThread = new Thread(worker.Send);
				sendThread.Start();

				self.Label = "Cancel";

				statusbar.Push(1, "Sending Program...");
			}
		}
	}

	void OnFinishedSendingProgram(object sender, EventArgs e)
	{
		sendButton.Label = "Send";

		statusbar.Push(1, "Finished Sending Program");
	}

	void OnCommandAdded(object sender, CommandEventArgs e)
	{
		CommandView view = new CommandView(e.command);
		palleteView.AddWidget(view);
		commandViews.Add(view);
	}

	void OnCommandRemoved(object sender, CommandEventArgs e)
	{
		foreach (CommandView view in commandViews)
		{
			if (view.id == e.command.id)
			{
				palleteView.RemoveWidget(view);
			}
		}
	}

	void OnProgramCommandAdded(object sender, ProgramCommandEventArgs e)
	{
		ProgramCommandView view = new ProgramCommandView(e.command);
		queueView.AddWidget(view);
		programCommandViews.Add(view);
	}

	void OnProgramCommandRemoved(object sender, EventArgs e)
	{
		foreach (ProgramCommandView view in programCommandViews)
		{
			view.Destroy();
		}

		programCommandViews.Clear();

		List<ProgramCommand> program = model.GetProgram();
		foreach (ProgramCommand command in program)
		{
			ProgramCommandView view = new ProgramCommandView(command);
			queueView.AddWidget(view);
			programCommandViews.Add(view);
		}
	}

	protected void OnDeleteEvent(object sender, DeleteEventArgs a)
	{
		MainWindow.MessengerInstance.Disconnect();

		Application.Quit();
		a.RetVal = true;
	}
}
