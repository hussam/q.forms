using System;
using System.Drawing;
using UIKit;
using CoreGraphics;

namespace CellSwipeGestureRecognition
{
	public static class FloatExtensions
	{
		/// <summary>
		/// Returns the -1.0F * this float
		/// </summary>
		public static nfloat Neg(this nfloat f)
		{
			return -1.0F * f;
		}
	}

	public enum SwipeTableCellMode {
		None,
		Exit,
		Switch
	};

	public enum SwipeTableViewCellState : byte {
		None = 0,
		State1 = (1 << 0),
		State2 = (1 << 1),
		State3 = (1 << 2),
		State4 = (1 << 3)
	};

	public class CellSwipeGestureRecognizer : UIPanGestureRecognizer
	{
		public delegate void SwipeCompletionBlock(UITableViewCell cell,  SwipeTableViewCellState state, SwipeTableCellMode mode);

		public delegate void SwipeHandler(UITableViewCell cell);
		public event SwipeHandler DidStartSwiping;
		public event SwipeHandler DidEndSwiping;

		public delegate void SwipePercentageHandler(UITableViewCell cell, nfloat percentage);
		public event SwipePercentageHandler SwipeWithPercentage;

		public nfloat Damping { get; set; }
		public nfloat Velocity { get; set; }
		public double AnimationDuration { get; set; }

		public UIColor DefaultColor { get; set; }
		public UIColor Color1 { get; set; }
		public UIColor Color2 { get; set; }
		public UIColor Color3 { get; set; }
		public UIColor Color4 { get; set; }

		public UIView View1 { get; set; }
		public UIView View2 { get; set; }
		public UIView View3 { get; set; }
		public UIView View4 { get; set; }

		public SwipeCompletionBlock CompletionBlock1 { get; set; }
		public SwipeCompletionBlock CompletionBlock2 { get; set; }
		public SwipeCompletionBlock CompletionBlock3 { get; set; }
		public SwipeCompletionBlock CompletionBlock4 { get; set; }

		public nfloat FirstTrigger { get; set; }
		public nfloat SecondTrigger { get; set; }

		public SwipeTableCellMode ModeForState1 { get; set; }
		public SwipeTableCellMode ModeForState2 { get; set; }
		public SwipeTableCellMode ModeForState3 { get; set; }
		public SwipeTableCellMode ModeForState4 { get; set; }

		public bool IsDragging {
			get { return dragging; }
		}

		public bool ShouldDrag { get; set; }
		public bool ShouldAnimateIcons { get; set; }

		const float kMCStop1                       	= 0.25F; // Percentage limit to trigger the first action
		const float kMCStop2                       	= 0.75F; // Percentage limit to trigger the second action
		const float kMCDamping                     	= 0.6F;  // Damping of the spring animation
		const float kMCVelocity                    	= 0.9F;  // Velocity of the spring animation
		const double kMCAnimationDuration           = 0.4;   // Duration of the animation
		const double kMCDurationLowLimit    	  	= 0.25;  // Lowest duration when swiping the cell because we try to simulate velocity
		const double kMCDurationHighLimit    		= 0.1;   // Highest duration when swiping the cell because we try to simulate velocity

		enum SwipeTableViewCellDirection {
			Left,
			Center,
			Right
		};



		UITableViewCell cell;


		SwipeTableViewCellDirection direction;
		double currentPercentage;
		bool isExited;
		bool dragging;

		UIImageView contentScreenshotView = null;
		UIView colorIndicatorView = null;
		UIView slidingView = null;
		UIView activeView = null;

		public CellSwipeGestureRecognizer(UITableViewCell cell)// : base(handlePanGestureRecognizer)
		{
			initDefaults(cell);
			AddTarget(handlePanGestureRecognizer);
			ShouldBegin = delegate(UIGestureRecognizer recognizer) {
				var point = VelocityInView (cell);

				if (Math.Abs (point.X) > Math.Abs (point.Y)) {
					if (point.X < 0 && ModeForState3 == SwipeTableCellMode.None && ModeForState4 == SwipeTableCellMode.None) {
						return false;
					}

					if (point.X > 0 && ModeForState1 == SwipeTableCellMode.None && ModeForState3 == SwipeTableCellMode.None) {
						return false;
					}

					// Notify the handler that we just started dragging
					if (DidStartSwiping != null) {
						DidStartSwiping (cell);
					}
					return true;
				}

				return false;
			};
		}

		void initDefaults(UITableViewCell cell)
		{
			this.cell = cell;

			isExited = false;
			dragging = false;
			ShouldDrag = true;
			ShouldAnimateIcons = true;

			FirstTrigger = kMCStop1;
			SecondTrigger = kMCStop2;

			Damping = kMCDamping;
			Velocity = kMCVelocity;
			AnimationDuration = kMCAnimationDuration;

			DefaultColor = UIColor.White;

			ModeForState1 = SwipeTableCellMode.None;
			ModeForState2 = SwipeTableCellMode.None;
			ModeForState3 = SwipeTableCellMode.None;
			ModeForState4 = SwipeTableCellMode.None;

			Color1 = null;
			Color2 = null;
			Color3 = null;
			Color4 = null;

			activeView = null;
			View1 = null;
			View2 = null;
			View3 = null;
			View4 = null;
		}

		public void PrepForReuse(UITableViewCell cell)
		{
			uninstallSwipingView ();
			initDefaults (cell);
		}


		#region View Manipulation
		void setupSwipingView()
		{
			if (contentScreenshotView != null) {
				return;
			}

			// If the content view background is transparent we get the background color.
			bool isContentViewBackgroundClear = (cell.ContentView.BackgroundColor != null);
			if (isContentViewBackgroundClear) {
				bool isBackgroundClear = (cell.BackgroundColor == UIColor.Clear);
				cell.ContentView.BackgroundColor = isBackgroundClear ? UIColor.White : cell.BackgroundColor;
			}

			UIImage contentViewScreenshotImage = imageWithView (cell);

			if (isContentViewBackgroundClear) {
				cell.ContentView.BackgroundColor = null;
			};

			colorIndicatorView = new UIView (cell.Bounds);
			colorIndicatorView.AutoresizingMask = UIViewAutoresizing.FlexibleHeight | UIViewAutoresizing.FlexibleWidth;
			colorIndicatorView.BackgroundColor = DefaultColor ?? UIColor.Clear;
			cell.AddSubview (colorIndicatorView);

			slidingView = new UIView ();
			slidingView.ContentMode = UIViewContentMode.Center;
			colorIndicatorView.AddSubview (slidingView);

			contentScreenshotView = new UIImageView (contentViewScreenshotImage);
			cell.AddSubview (contentScreenshotView);
		}

		void uninstallSwipingView() {
			if (contentScreenshotView == null) {
				return;
			}

			slidingView.RemoveFromSuperview ();
			slidingView = null;

			colorIndicatorView.RemoveFromSuperview ();
			colorIndicatorView = null;

			contentScreenshotView.RemoveFromSuperview ();
			contentScreenshotView = null;
		}

		void setViewofSlidingView(UIView view) {
			if (slidingView == null) {
				return;
			}

			foreach (var sv in slidingView.Subviews) {
				sv.RemoveFromSuperview ();
			}

			slidingView.AddSubview (view);
		}
		#endregion



		#region Swipe Configuration
		public void setSwipeGestureWithView(
			UIView view,
			UIColor color,
			SwipeTableCellMode mode,
			SwipeTableViewCellState state,
			SwipeCompletionBlock completionBlock)
		{

			if (view == null || color == null) {
				return;
			}

			// Depending on the state we assign the attributes
			if ((state & SwipeTableViewCellState.State1) == SwipeTableViewCellState.State1) {
				CompletionBlock1 = completionBlock;
				View1 = view;
				Color1 = color;
				ModeForState1 = mode;
			}

			if ((state & SwipeTableViewCellState.State2) == SwipeTableViewCellState.State2) {
				CompletionBlock2 = completionBlock;
				View2 = view;
				Color2 = color;
				ModeForState2 = mode;
			}

			if ((state & SwipeTableViewCellState.State3) == SwipeTableViewCellState.State3) {
				CompletionBlock3 = completionBlock;
				View3 = view;
				Color3 = color;
				ModeForState3 = mode;
			}

			if ((state & SwipeTableViewCellState.State4) == SwipeTableViewCellState.State4) {
				CompletionBlock4 = completionBlock;
				View4 = view;
				Color4 = color;
				ModeForState4 = mode;
			}

		}
		#endregion



		#region Handle Gestures
		void handlePanGestureRecognizer()
		{
			if (!ShouldDrag || isExited) {
				return;
			}

			var state = this.State;
			var translation = TranslationInView (cell);
			var velocity = VelocityInView (cell);

			nfloat percentage;
			if (contentScreenshotView == null) {
				percentage = percentageWithOffset (0, cell.Bounds.Width);
			} else {
				percentage = percentageWithOffset (contentScreenshotView.Frame.X, cell.Bounds.Width);
			}
			//var percentage = percentageWithOffset (contentScreenshotView.Frame.X, cell.Bounds.Width);

			var animationDuration = animationDurationWithVelocity (velocity);
			direction = directionWithPercentage (percentage);

			if (state == UIGestureRecognizerState.Began || state == UIGestureRecognizerState.Changed) {
				dragging = true;

				setupSwipingView ();

				var center = new CGPoint (contentScreenshotView.Center.X + translation.X, contentScreenshotView.Center.Y);
				contentScreenshotView.Center = center;
				animateWithOffset (contentScreenshotView.Frame.GetMinX ());
				SetTranslation (new CGPoint (0.0F, 0.0F), cell);

				// Notifying the handler that we are dragging with an offset percentage
				if (SwipeWithPercentage != null) {
					SwipeWithPercentage (cell, percentage);
				}
			} else if (state == UIGestureRecognizerState.Ended || state == UIGestureRecognizerState.Cancelled) {
				dragging = false;
				activeView = viewWithPercentage (percentage);
				currentPercentage = percentage;

				SwipeTableViewCellState cellState = stateWithPercentage (percentage);
				SwipeTableCellMode cellMode = SwipeTableCellMode.None;

				if (cellState == SwipeTableViewCellState.State1 && ModeForState1 != SwipeTableCellMode.None) {
					cellMode = ModeForState1;
				} else if (cellState == SwipeTableViewCellState.State2 && ModeForState2 != SwipeTableCellMode.None) {
					cellMode = ModeForState2;
				} else if (cellState == SwipeTableViewCellState.State3 && ModeForState3 != SwipeTableCellMode.None) {
					cellMode = ModeForState3;
				} else if (cellState == SwipeTableViewCellState.State4 && ModeForState4 != SwipeTableCellMode.None) {
					cellMode = ModeForState4;
				}

				if (cellMode == SwipeTableCellMode.Exit && direction != SwipeTableViewCellDirection.Center) {
					moveWithDuration (animationDuration, direction);
				} else {
					swipeToOriginWithCompletion (executeCompletionBlock);
				}

				// We notify the handler that we just ended swiping.
				if (DidEndSwiping != null) {
					DidEndSwiping (cell);
				}
			}
		}
		#endregion



		#region Percentage
		nfloat offsetWithPercentage (nfloat percentage, nfloat totalWidth)
		{
			var offset = percentage * totalWidth;

			if (offset < totalWidth.Neg()) {
				offset = totalWidth.Neg();
			} else if (offset > totalWidth) {
				offset = totalWidth;
			}

			return offset;
		}

		nfloat percentageWithOffset(nfloat offset, nfloat totalWidth) {
			nfloat percentage = offset / totalWidth;

			if (percentage < -1.0F) {
				percentage = -1.0F;
			} else if (percentage > 1.0F) {
				percentage = 1.0F;
			}

			return percentage;
		}

		double animationDurationWithVelocity(CGPoint velocity) {
			var width = cell.Bounds.Width;
			var animationDurationDiff = kMCDurationHighLimit - kMCDurationLowLimit;
			var horizontalVelocity = velocity.X;

			if (horizontalVelocity < width.Neg()) {
				horizontalVelocity = width.Neg();
			} else if (horizontalVelocity > width) {
				horizontalVelocity = width;
			}

			return (kMCDurationHighLimit + kMCDurationLowLimit) - Math.Abs ((horizontalVelocity / width) * animationDurationDiff);
		}

		SwipeTableViewCellDirection directionWithPercentage(nfloat percentage) {
			if (percentage < 0) {
				return SwipeTableViewCellDirection.Left;
			} else if (percentage > 0) {
				return SwipeTableViewCellDirection.Right;
			} else {
				return SwipeTableViewCellDirection.Center;
			}
		}

		UIView viewWithPercentage(nfloat percentage) {
			UIView view = null;

			if (percentage >= 0 && ModeForState1 != SwipeTableCellMode.None) {
				view = View1;
			}

			if (percentage >= SecondTrigger && ModeForState2 != SwipeTableCellMode.None) {
				view = View2;
			}

			if (percentage < 0 && ModeForState3 != SwipeTableCellMode.None) {
				view = View3;
			}

			if (percentage <= (SecondTrigger.Neg()) && ModeForState4 != SwipeTableCellMode.None) {
				view = View4;
			}

			return view;
		}

		nfloat alphaWithPercentage(nfloat percentage) {
			nfloat alpha;

			if (percentage >= 0 && percentage < FirstTrigger) {
				alpha = percentage / FirstTrigger;
			} else if (percentage < 0 && percentage > FirstTrigger.Neg()) {
				alpha = (nfloat)Math.Abs (percentage / FirstTrigger);
			} else {
				alpha = 1.0F;
			}

			return alpha;
		}

		UIColor colorWithPercentage(nfloat percentage) {
			UIColor color = DefaultColor ?? UIColor.Clear;

			if (percentage > FirstTrigger && ModeForState1 != SwipeTableCellMode.None) {
				color = Color1;
			} 

			if (percentage > SecondTrigger && ModeForState2 != SwipeTableCellMode.None) {
				color = Color2;
			} 

			if (percentage < FirstTrigger.Neg() && ModeForState3 != SwipeTableCellMode.None) {
				color = Color3;
			}

			if (percentage < SecondTrigger.Neg() && ModeForState4 != SwipeTableCellMode.None) {
				color = Color4;
			}

			return color;
		}

		SwipeTableViewCellState stateWithPercentage(nfloat percentage) {
			SwipeTableViewCellState state = SwipeTableViewCellState.None;

			if (percentage >= FirstTrigger && ModeForState1 != SwipeTableCellMode.None) {
				state = SwipeTableViewCellState.State1;
			}

			if (percentage >= SecondTrigger && ModeForState2 != SwipeTableCellMode.None) {
				state = SwipeTableViewCellState.State2;
			}

			if (percentage <= FirstTrigger.Neg() && ModeForState3 != SwipeTableCellMode.None) {
				state = SwipeTableViewCellState.State3;
			}

			if (percentage <= SecondTrigger.Neg() && ModeForState4 != SwipeTableCellMode.None) {
				state = SwipeTableViewCellState.State4;
			}

			return state;
		}
		#endregion



		#region Movement
		void animateWithOffset (nfloat offset)
		{
			var percentage = percentageWithOffset (offset, cell.Bounds.Width);
			var view = viewWithPercentage (percentage);

			// View Position.
			if (view != null) {
				setViewofSlidingView (view);
				slidingView.Alpha = alphaWithPercentage (percentage);
				slideViewWithPercentage (percentage, view, ShouldAnimateIcons);
			}

			// Color
			UIColor color = colorWithPercentage (percentage);
			if (color != null) {
				colorIndicatorView.BackgroundColor = color;
			}

		}

		void slideViewWithPercentage (nfloat percentage, UIView view, bool isDragging)
		{
			if (view == null) {
				return;
			}
				
			var position = new CGPoint (0.0F, 0.0F);
			position.Y = cell.Bounds.Height / 2.0F;

			var hft = FirstTrigger / 2.0F;		// half first trigger
			var width = cell.Bounds.Width;

			if (isDragging) {
				if (percentage >= 0 && percentage < FirstTrigger) {
					position.X = offsetWithPercentage (hft, width);
				} else if (percentage >= FirstTrigger) {
					position.X = offsetWithPercentage (percentage - hft, width);
				} else if (percentage < 0 && percentage >= FirstTrigger.Neg()) {
					position.X = width - offsetWithPercentage (hft, width);
				} else if (percentage < FirstTrigger.Neg()) {
					position.X = width + offsetWithPercentage (percentage + hft, width);
				}
			} else {
				if (direction == SwipeTableViewCellDirection.Right) {
					position.X = offsetWithPercentage (hft, width);
				} else if (direction == SwipeTableViewCellDirection.Left) {
					position.X = width - offsetWithPercentage (hft, width);
				} else {
					return;
				}
			}

			var activeViewSize = view.Bounds.Size;
			var activeViewFrame = new CGRect (
				position.X - (activeViewSize.Width / 2.0F),
				position.Y - (activeViewSize.Height / 2.0F),
				activeViewSize.Width,
				activeViewSize.Height);
			slidingView.Frame = activeViewFrame.Integral ();
		}

		void moveWithDuration (double duration, SwipeTableViewCellDirection direction)
		{
			isExited = true;
			nfloat origin = 0.0F;

			if (direction == SwipeTableViewCellDirection.Left) {
				origin = cell.Bounds.Width.Neg ();
			} else if (direction == SwipeTableViewCellDirection.Right) {
				origin = cell.Bounds.Width;
			}

			var percentage = percentageWithOffset (origin, cell.Bounds.Width);
			var frame = contentScreenshotView.Frame;
			frame.X = origin;

			// Color
			UIColor color = colorWithPercentage ((nfloat) currentPercentage);
			if (color != null) {
				colorIndicatorView.BackgroundColor = color;
			}

			UIView.Animate (
				duration,
				0,
				UIViewAnimationOptions.CurveEaseOut | UIViewAnimationOptions.AllowUserInteraction,
				() => {
					contentScreenshotView.Frame = frame;
					slidingView.Alpha = 0;
					slideViewWithPercentage (percentage, activeView, ShouldAnimateIcons);
				},
				executeCompletionBlock
			);
		}

		void swipeToOriginWithCompletion (Action completion)
		{
			// Assuming that we are in iOS 7+ then no need to check if the UIView can do spring dampining
			UIView.AnimateNotify (
				AnimationDuration,
				0,
				Damping,
				Velocity,
				UIViewAnimationOptions.CurveEaseInOut,
				() => {
					var frame = contentScreenshotView.Frame;
					frame.X = 0;
					contentScreenshotView.Frame = frame;

					// Clearing the indicatior view
					colorIndicatorView.BackgroundColor = DefaultColor;

					slidingView.Alpha = 0;
					slideViewWithPercentage (0, activeView, false);
				},
				(bool finished) => {
					isExited = false;
					uninstallSwipingView ();
					if (completion != null) {
						completion ();
					}
				}
			);

		}
		#endregion



		#region Utilities
		UIImage imageWithView (UIView view)
		{
			var scale = UIScreen.MainScreen.Scale;
			UIGraphics.BeginImageContextWithOptions (view.Bounds.Size, false, scale);

			view.Layer.RenderInContext (UIGraphics.GetCurrentContext ());
			var image = UIGraphics.GetImageFromCurrentImageContext ();

			UIGraphics.EndImageContext ();
			return image;
		}
		#endregion



		#region Completion Block
		void executeCompletionBlock ()
		{
			SwipeTableViewCellState state = stateWithPercentage ((float) currentPercentage);
			SwipeTableCellMode mode = SwipeTableCellMode.None;
			SwipeCompletionBlock completionBlock = null;

			switch (state) {
			case SwipeTableViewCellState.State1:
				mode = ModeForState1;
				completionBlock = CompletionBlock1;
				break;

			case SwipeTableViewCellState.State2:
				mode = ModeForState2;
				completionBlock = CompletionBlock2;
				break;

			case SwipeTableViewCellState.State3:
				mode = ModeForState3;
				completionBlock = CompletionBlock3;
				break;

			case SwipeTableViewCellState.State4:
				mode = ModeForState4;
				completionBlock = CompletionBlock4;
				break;

			default:
				break;
			}

			if (completionBlock != null) {
				completionBlock (cell, state, mode);
			}

		}
		#endregion
	}
}

