using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Windows.Data.Xml.Dom;
using Windows.Foundation;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Animation;
using GoG.Client.Services.Popups;
using GoG.Client.ViewModels;
using Microsoft.Practices.Prism.Mvvm.Interfaces;
using Microsoft.Practices.Prism.StoreApps.Interfaces;

namespace GoG.Client
{
    public static class Extensions
    {
        private static readonly Point Origin = new Point(0, 0);

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
            var elementToWindowOffset = elementToWindowTransform.TransformPoint(Origin);

            return new Point(windowPos.X - elementToWindowOffset.X,
                windowPos.Y - elementToWindowOffset.Y);
        }

        public static Task<bool> NavigateAsync<TPageViewModel>(this INavigationService me, object parameter = null)
            where TPageViewModel : IPageViewModelBase
        {
            var tcs = new TaskCompletionSource<bool>();

            var sc = SynchronizationContext.Current;
            sc.Post(o =>
            {
                try
                {
                    var success = me.Navigate<TPageViewModel>(parameter);
                    tcs.SetResult(success);
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                    throw;
                }
            }, null);

            return tcs.Task;
        }

        public static bool Navigate<TPage>(this INavigationService me, object parameter = null)
            where TPage : IPageViewModelBase
        {
            try
            {
                var ti = typeof (TPage).GetTypeInfo();
                if (!ti.IsInterface)
                    throw new Exception("Page types must be an interface.");
                var typename = typeof(TPage).Name;
                if (!typename.StartsWith("I"))
                    throw new Exception("Page interface types start with 'I'.");
                if (!typename.EndsWith("PageViewModel"))
                    throw new Exception("Page interface types must end in 'PageViewModel'.");
                var stub = typename.Substring(1, typename.Length - 14);
                bool success = me.Navigate(stub, parameter);
                return success;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static bool IsFlagOn(this PopupAnimation value, PopupAnimation flag)
        {
            return (value & flag) == flag;
        }

        public static Task<A> EventToTaskAsync<A>(Action<EventHandler<A>> adder, Action<EventHandler<A>> remover)
        {
            var tcs = new TaskCompletionSource<A>();
            EventHandler<A> onComplete = null;
            onComplete = (s, e) =>
            {
                remover(onComplete);
                tcs.SetResult(e);
            };
            adder(onComplete);
            return tcs.Task;
        }

        public static Task BeginAsync(this Storyboard storyboard)
        {
            return EventToTaskAsync<object>(
                e =>
                {
                    storyboard.Completed += e;
                    storyboard.Begin();
                },
                e => storyboard.Completed -= e);
        }

        public static string GetString(this IResourceLoader rl, ResourceStringKeys resourceKey)
        {
            var rval = rl.GetString(resourceKey.ToString());
            return rval;
        }

        /// <summary>
        /// Deserializes an object from an <see cref="INode"/>.
        /// </summary>
        /// <typeparam name="T">Type of the object to be deserialized.</typeparam>
        /// <param name="serializedObject">The object to be deserialized.</param>
        /// <returns>The deserialized object.</returns>
        public static T Deserialize<T>(IXmlNode serializedObject)
            where T : class
        {
            return (new XmlSerializer(typeof(T)).Deserialize(new StringReader(serializedObject.GetXml())) as T);
        }
    }
}
