using Gtk;

namespace EV3CommandAndControl
{
	public class ScrollableView : Gtk.Bin
	{
		VBox listView;
		Alignment listViewAlignment;

		public ScrollableView()
		{
			ScrolledWindow scrollView = new ScrolledWindow();

			listView = new VBox(true, 2);

			listViewAlignment = new Alignment(0, 0, 1, 0);
			listViewAlignment.Add(listView);

			scrollView.AddWithViewport(listViewAlignment);

			Viewport scrollViewport = (Viewport)scrollView.Child;
			scrollViewport.ShadowType = ShadowType.None;

			Add(scrollView);

			ShowAll();
		}

		public void AddWidget(Widget w)
		{
			listView.PackStart(w, false, false, 0);
		}

		public void RemoveWidget(Widget w)
		{
			listView.Remove(w);
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

