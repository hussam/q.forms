using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

using Q;
using Q.iOS;
using CoreAnimation;

[assembly: ExportRendererAttribute(typeof(StyledEntry), typeof(StyledEntryRenderer))]
namespace Q.iOS
{
	public class StyledEntryRenderer : EntryRenderer
	{
		protected override void OnElementChanged (ElementChangedEventArgs<Entry> e)
		{
			base.OnElementChanged (e);
			var se = (StyledEntry)Element;

			Control.Font = UIFont.FromName (Settings.FontNameBold, se.FontSize);
			Control.BorderStyle = UITextBorderStyle.None;
			Control.KeyboardType = UIKeyboardType.ASCIICapable;

			switch (se.ReturnKey) {
			case StyledEntry.KeyboardReturnKey.Done:
				Control.ReturnKeyType = UIReturnKeyType.Done;
				break;
			case StyledEntry.KeyboardReturnKey.Next:
				Control.ReturnKeyType = UIReturnKeyType.Next;
				break;
			default:
				break;
			}

			if (se.LeftSpacing > 0) {
				var rect = new CoreGraphics.CGRect (0, 0, se.LeftSpacing, Control.Bounds.Height);
				Control.LeftView = new UIView (rect);
				Control.LeftViewMode = UITextFieldViewMode.Always;
			}

			if (se.TintColor != Color.Default) {
				Control.TintColor = se.TintColor.ToUIColor ();
			}
		}
	}
}

