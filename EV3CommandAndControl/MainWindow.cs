﻿using System;
using System.Collections.Generic;
using Gtk;
using EV3CommandAndControl;

public partial class MainWindow : Gtk.Window
{
	ScrollableView palleteView;
	ScrollableView queueView;

	List<CommandPalleteView> palleteViews;
	List<CommandView> commandViews;

	CommandModel model;

	public MainWindow() : base(Gtk.WindowType.Toplevel)
	{
		SetDefaultSize(900, 500);
		SetPosition(WindowPosition.Center);
		DeleteEvent += delegate { Application.Quit(); };

		model = CommandModel.Instance;
		model.CommandAddedEvent += OnCommandAdded;
		model.CommandRemovedEvent += OnCommandRemoved;

		palleteViews = new List<CommandPalleteView>();
		commandViews = new List<CommandView>();

		VBox mainBox = new VBox(false, 2);

		MenuBar mb = new MenuBar();

		MenuItem file = new MenuItem("File");
		Menu fileMenu = new Menu();
		file.Submenu = fileMenu;
		mb.Append(file);

		MenuItem view = new MenuItem("View");
		Menu viewMenu = new Menu();
		file.Submenu = viewMenu;
		mb.Append(view);

		MenuItem connectionStatus = new MenuItem("Connection Status");
		Menu connectionMenu = new Menu();
		file.Submenu = connectionMenu;
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

	void OnCommandAdded(object sender, CommandEventArgs e)
	{
		CommandPalleteView view = new CommandPalleteView(e.command);
		palleteView.AddWidget(view);
		palleteViews.Add(view);
	}

	void OnCommandRemoved(object sender, CommandEventArgs e)
	{
		foreach (CommandPalleteView view in palleteViews)
		{
			if (view.id == e.command.id)
			{
				palleteView.RemoveWidget(view);
			}
		}
	}

	protected void OnDeleteEvent(object sender, DeleteEventArgs a)
	{
		Application.Quit();
		a.RetVal = true;
	}
}
