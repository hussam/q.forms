using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using UIKit;

using cravery;
using cravery.iOS;
using CellSwipeGestureRecognition;

[assembly: ExportRenderer(typeof(InspirationCell), typeof(InspirationCellRenderer))]
namespace cravery.iOS
{
	public class InspirationCellRenderer : ViewCellRenderer
	{
		public override UITableViewCell GetCell (Cell item, UITableViewCell reusableCell, UITableView tv)
		{
			var cell = base.GetCell (item, reusableCell, tv);
			CellSwipeGestureRecognizer gr;

			if (cell.GestureRecognizers == null) {
				gr = new CellSwipeGestureRecognizer (cell);
				cell.AddGestureRecognizer (gr);
			} else {
				gr = ((CellSwipeGestureRecognizer)cell.GestureRecognizers [0]);
				gr.PrepForReuse(cell);
			}
				
			var checkView = new UIImageView (UIImage.FromBundle ("check"));
			checkView.ContentMode = UIViewContentMode.Center;

			var crossView = new UIImageView (UIImage.FromBundle ("cross"));
			crossView.ContentMode = UIViewContentMode.Center;

			gr.FirstTrigger = 0.1F;

			gr.setSwipeGestureWithView (
				checkView,
				UIColor.Green,
				SwipeTableCellMode.Exit,
				SwipeTableViewCellState.State1,
				(_cell, state, mode) => {
					((InspirationCell) item).InterestedInActivity();
				}
			);

			gr.setSwipeGestureWithView (
				crossView,
				UIColor.Red,
				SwipeTableCellMode.Exit,
				SwipeTableViewCellState.State3,
				(_cell, state, mode) => {
					((InspirationCell) item).NotInterestedInActivity();
				}
			);

			return cell;
		}



	}
}

