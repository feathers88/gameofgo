using System;

namespace GoG.Client.Services.Popups
{
	public class PopupSettings
	{
		public const int DefaultDelayMs = 350;
		public static readonly PopupSettings CenterWideDialog = new PopupSettings(TimeSpan.FromMilliseconds(350), 0.7, 0.5, PopupAnimation.ControlFlip | PopupAnimation.OverlayFade, true);
		public static readonly PopupSettings Flyout = new PopupSettings(TimeSpan.FromMilliseconds(350), 1, 0, PopupAnimation.ControlFlyoutRight, true);

		public PopupSettings(TimeSpan duration, double overlayToControlRatio, double overlayOpacity, PopupAnimation animation, bool overlayDismissal)
		{
			AnimationDuration = duration;
			OverlayOpacity = overlayOpacity;
			OverlayToControlAnimationRatio = overlayToControlRatio;
			Animation = animation;
			OverlayDismissal = overlayDismissal;
		}

		public TimeSpan AnimationDuration { get; protected set; }
		public double OverlayToControlAnimationRatio { get; protected set; }
		public double OverlayOpacity { get; protected set; }
		public PopupAnimation Animation { get; protected set; }
		public bool OverlayDismissal { get; protected set; }
	}
}
