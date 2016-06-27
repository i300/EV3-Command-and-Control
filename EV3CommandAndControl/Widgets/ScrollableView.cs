using Gtk;

namespace EV3CommandAndControl
{
	public class ScrollableView : Gtk.Bin
	{
		protected VBox listView;

		public ScrollableView()
		{
			ScrolledWindow scrollView = new ScrolledWindow();

			listView = new VBox(true, 2);

			Alignment align = new Alignment(0, 0, 1, 0);
			align.Add(listView);

			scrollView.AddWithViewport(align);

			Viewport scrollViewport = (Viewport)scrollView.Child;
			scrollViewport.ShadowType = ShadowType.None;

			Add(scrollView);

			ShowAll();
		}

		public void AddWidget(Widget w)
		{
			listView.PackStart(w, false, false, 0);
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

