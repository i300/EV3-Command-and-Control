using System;
using System.Collections.Generic;
using Gtk;
using EV3CommandAndControl;
using EV3MessengerLib;

public partial class MainWindow : Gtk.Window
{
	ScrollableView palleteView;
	ScrollableView queueView;

	List<CommandView> palleteViews;
	List<ProgramCommandView> commandViews;

	CommandModel model;

	public MainWindow() : base(Gtk.WindowType.Toplevel)
	{
		SetDefaultSize(900, 500);
		SetPosition(WindowPosition.Center);
		DeleteEvent += delegate { Application.Quit(); };

		model = CommandModel.Instance;
		model.CommandAddedEvent += OnCommandAdded;
		model.CommandRemovedEvent += OnCommandRemoved;
		model.ProgramCommandAddedEvent += OnProgramCommandAdded;
		model.ProgramCommandRemovedEvent += OnProgramCommandRemoved;

		palleteViews = new List<CommandView>();
		commandViews = new List<ProgramCommandView>();

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
		saveAs.Activated += saveCommands;
		fileMenu.Append(saveAs);

		MenuItem loadFile = new MenuItem("Load");
		loadFile.Activated += loadCommands;
		fileMenu.Append(loadFile);

		MenuItem simpleView = new MenuItem("Simple View");
		simpleView.Activated += switchView;
		viewMenu.Append(simpleView);

		MenuItem advancedView = new MenuItem("Advanced View");
		advancedView.Activated += switchView;
		viewMenu.Append(advancedView);

		MenuItem tryConnect = new MenuItem("Connect");
		tryConnect.Activated += connectionUI;
		connectionMenu.Append(tryConnect);

		mb.Append(file);
		mb.Append(view);
		mb.Append(connectionStatus);

		Statusbar statusbar = new Statusbar();
		statusbar.Push(1, "Ready");

		HBox hbox = new HBox(false, 2);

		hbox.BorderWidth = 5;

		VBox leftBox = new VBox(false, 2);

		Button addNewCommandButton = new Button();
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

		Button sendButton = new Button();
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
	}

	void switchView(object sender, EventArgs args)
	{

	}

	void loadCommands(object sender, EventArgs args)
	{

	}

	void saveCommands(object sender, EventArgs args)
	{
		
	}

	void connectionUI(object sender, EventArgs args)
	{
		ConnectionWindow w = new ConnectionWindow();
		w.Show();
	}

	void OnCommandAdded(object sender, CommandEventArgs e)
	{
		CommandView view = new CommandView(e.command);
		palleteView.AddWidget(view);
		palleteViews.Add(view);
	}

	void OnCommandRemoved(object sender, CommandEventArgs e)
	{
		foreach (CommandView view in palleteViews)
		{
			if (view.id == e.command.id)
			{
				palleteView.RemoveWidget(view);
			}
		}

		foreach (ProgramCommandView view in commandViews)
		{
			if (view.id == e.command.id)
			{
				queueView.RemoveWidget(view);
			}
		}
	}

	void OnProgramCommandAdded(object sender, ProgramCommandEventArgs e)
	{
		ProgramCommandView view = new ProgramCommandView(e.command.command.id, e.command.index);
		queueView.AddWidget(view);
		commandViews.Add(view);
	}

	void OnProgramCommandRemoved(object sender, ProgramCommandEventArgs e)
	{
		queueView.RemoveAllWidgets();

		foreach (ProgramCommand command in model.GetProgram())
		{
			ProgramCommandView view = new ProgramCommandView(command.command.id, command.index);
			queueView.AddWidget(view);
		}
	}

	protected void OnDeleteEvent(object sender, DeleteEventArgs a)
	{
		Application.Quit();
		a.RetVal = true;
	}
}
