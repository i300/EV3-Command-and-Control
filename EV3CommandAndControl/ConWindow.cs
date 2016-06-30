using System;
using Gtk;

namespace EV3CommandAndControl
{
	public partial class Window : Gtk.Window
	{
		public Window() :
				base(Gtk.WindowType.Toplevel)
		{
			SetDefaultSize(900, 500);
			SetPosition(WindowPosition.Center);
			DeleteEvent += delegate { Application.Quit(); };
			ShowAll();
			this.Build();
		}
	}
}

