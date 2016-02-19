using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;

namespace GoG.Board.Extensions
{
    public static class PointerExtensions
    {
        private static readonly Point ORIGIN = new Point(0, 0);

        /// <summary>
        /// Returns the latest position of the pointer relative to the current window
        /// </summary>
        public static Point GetPosition(this Pointer pointer)
        {
            // Even though docs for the Position property say it is in screen-coordinates, 
            // it's actually in window coordinates.  
            // That's fine though... one less transform we need to do
            return PointerPoint.GetCurrentPoint(pointer.PointerId).Position;
        }

        /// <summary>
        /// Returns the latest position of the pointer relative to an element in the window
        /// </summary>
        public static Point GetPosition(this Pointer pointer, UIElement relativeTo)
        {
            var windowPos = pointer.GetPosition();
            var elementToWindowTransform = relativeTo.TransformToVisual(Window.Current.Content);
            var elementToWindowOffset = elementToWindowTransform.TransformPoint(ORIGIN);

            return new Point(windowPos.X - elementToWindowOffset.X,
                                windowPos.Y - elementToWindowOffset.Y);
        }
    }

}
