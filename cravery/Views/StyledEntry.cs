﻿using System;
using Xamarin.Forms;

namespace cravery
{
	public class StyledEntry : Entry
	{
		public int MaxTextLength 			{ get; set; } = 20;
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
				if (Text.Length > MaxTextLength) {
					Text = e.OldTextValue;
				} else {
					Text = e.NewTextValue.ToUpper();
				}
			};
		}
	}
}

