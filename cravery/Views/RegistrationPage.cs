using System;
using Xamarin.Forms;
using Parse;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace cravery
{
	public class RegistrationPage : ContentPage
	{
		Entry numbersEntry;
		bool atVerification = false;
		string phoneNumber;

		const int PhoneNumberLength = 10;
		const int VerificationCodeLength = 4;

		public RegistrationPage ()
		{
			var title = new Label {
				Text = "Cravery",
				FontAttributes = FontAttributes.Bold | FontAttributes.Italic,
				FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)) * 1.5,
				XAlign = TextAlignment.Center,
				HeightRequest = 100
			};

			var blurb = new Label {
				Text = "What's your phone number?",
				XAlign = TextAlignment.Center
			};

			var button = new Button {
				Text = "Next",
				IsEnabled = false
			};
					
			numbersEntry = new Entry {
				Placeholder = "(555) 123-4567",
				Keyboard = Keyboard.Telephone
			};
			numbersEntry.TextChanged += (sender, e) => {
				var maxLen = atVerification ? VerificationCodeLength : PhoneNumberLength;
				if (numbersEntry.Text.Length == maxLen) {
					button.IsEnabled = true;
				} else if (numbersEntry.Text.Length > maxLen) {
					numbersEntry.Text = e.OldTextValue;
				}
			};

			var body = new StackLayout {
				HorizontalOptions = LayoutOptions.Fill,
				Children = {
					blurb,
					new StackLayout {
						Orientation = StackOrientation.Horizontal,
						HorizontalOptions = LayoutOptions.Center,
						Children = { numbersEntry, button }
					}
				}
			};
				
			var resendBtn = new Button { Text = "Resend Verification Code" };
			resendBtn.Clicked += async (sender, e) => {
				await ParseCloud.CallFunctionAsync<bool>("RegisterAndSendVerificationCode",
					new Dictionary<string, object> {
						{"number", phoneNumber}
					}
				);
			};

			button.Clicked += async (sender, e) => {
				if (!atVerification) {
					phoneNumber = numbersEntry.Text;

					var funcParams = new Dictionary<string, object> {
						{"number", phoneNumber}
					};

					ParseCloud.CallFunctionAsync<bool>("RegisterAndSendVerificationCode", funcParams).Start();
					await body.FadeTo(0);

					numbersEntry.Text = "";
					numbersEntry.Placeholder = "_ _ _ _";
					blurb.Text = "Nice! What's the verification code?";
					button.Text = "Let's do this!";
					button.IsEnabled = false;
					body.Children.Add(resendBtn);

					await body.FadeTo(1);

					atVerification = true;
				} else {
					var loginSuccess = await AccountManager.LoginUser(phoneNumber, numbersEntry.Text);
					if (loginSuccess) {
						await Navigation.PopModalAsync();
					}
				}
			};

			Padding = new Thickness (10, Device.OnPlatform (20, 0, 0), 10, 15);
			Content = new StackLayout {
				HorizontalOptions = LayoutOptions.Fill,
				Padding = new Thickness(0, 50, 0, 0),
				Children = { title, body }
			};
		}

		protected override void OnAppearing ()
		{
			base.OnAppearing ();
			numbersEntry.Focus ();
		}
	}
}