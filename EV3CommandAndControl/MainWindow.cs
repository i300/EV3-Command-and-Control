using System;
using System.Collections.Generic;
using Gtk;
using EV3CommandAndControl;
using EV3MessengerLib;
using System.Threading;


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

	private static EV3Messenger messenger;

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
		statusbar.Push(1, "Ready");

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

		sendButton = new Button();
		sendButton.Clicked += OnSendButtonClicked;
		sendButton.Label = "Send";

		Alignment sendButtonAlign = new Alignment(1, 0, 0, 0);
		sendButtonAlign.Add(sendButton);

		queueView = new ScrollableView();

		rightBox.PackStart(commandQueueLabel, false, false, 0);
		rightBox.PackStart(queueView, true, true, 0);
		rightBox.PackEnd(sendButtonAlign, false, false, 0);

		hbox.PackStart(leftBox, true, true, 0);
		hbox.PackStart(new VSeparator(), false, false, 0);
		hbox.PackEnd(rightBox, true, true, 0);

		mainBox.PackStart(mb, false, false, 0);
		mainBox.PackStart(hbox, true, true, 0);
		mainBox.PackEnd(statusbar, false, false, 0);

		Add(mainBox);

		ShowAll();

		simpleView.Activate();

		// TODO Remove this code-- for debugging only
		/*if (messenger.Connect("/dev/tty.EV3-SerialPort"))
		{
			statusbar.Push(1, "Connected to EV3");
		}
		else {
			statusbar.Push(1, "Connection Failed");
		}*/
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

	}

	void SaveCommands(object sender, EventArgs args)
	{
		
	}

	void ShowConnectionUI(object sender, EventArgs args)
	{
		ConnectionWindow w = new ConnectionWindow();
		w.ConnectionUpdatedEvent += OnConnectionUpdated;
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

	void OnSendButtonClicked(object sender, EventArgs e)
	{
		if (messenger.IsConnected)
		{
			sendButton.Sensitive = false;
			statusbar.Push(1, "Sending to EV3...");

			List<ProgramCommand> program = model.GetProgram();
			foreach (ProgramCommand c in program)
			{
				Console.WriteLine("id: " + c.command.id + " param: " + c.parameter);
				messenger.SendMessage("abc", c.command.id);
				messenger.SendMessage("abc", c.parameter);

				EV3Message message;
				Console.WriteLine("Waiting for handshake...");
				for (;;)
				{
					message = messenger.ReadMessage();
					if (message != null)
					{
						break;
					}
					Thread.Sleep(10);
				}
				Console.WriteLine("Handshake Recieved");
			}

			statusbar.Pop(1);
			statusbar.Push(1, "Sent Program to EV3");
			sendButton.Sensitive = true;
		}
		else {
			statusbar.Push(1, "Sending Failed");
		}
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
