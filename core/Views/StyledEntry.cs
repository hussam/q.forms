using System;
using Xamarin.Forms;

namespace Q
{
	public class StyledEntry : Entry
	{
		public int MaxTextLength 			{ get; set; } = 20;
		public bool EnforceMaxTextLength	{ get; set; } = true;
		public int LeftSpacing 				{ get; set; } = 0;
		public Color TintColor 				{ get; set; } = Color.Default;
		public bool BottomBorder			{ get; set; } = false;
		public KeyboardReturnKey ReturnKey 	{ get; set; } = KeyboardReturnKey.Default;
		public int FontSize					{ get; set; }

		public enum KeyboardReturnKey
		{
			Default,
			Next,
			Done
		}

		public StyledEntry()
		{
			FontSize = (int) Device.GetNamedSize (NamedSize.Medium, typeof(Entry));

			TextChanged += (sender, e) => {
				if (EnforceMaxTextLength && Text.Length > MaxTextLength) {
					Text = e.OldTextValue;
				} else {
					Text = e.NewTextValue.ToUpper();
				}
			};
		}
	}
}

