using System;

namespace GoG.Client.Services.Popups
{
    [Flags]
    public enum PopupAnimation
    {
        ControlFlip = 0x1,
		ControlFlyoutRight = 0x2,
        OverlayFade = 0x1 << 16,
    }
}
