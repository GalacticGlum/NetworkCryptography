/*
 * Author: Shon Verch
 * File Name: ChatAutoScrollBehaviour.cs
 * Project Name: NetworkCryptography.cs
 * Creation Date: 10/20/2017
 * Modified Date: 10/20/2017
 * Description: Scrolls to new items as they are added in a control unless the scrollbar is scrolled up already.
 */

using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using System.Windows.Media;

namespace NetworkCryptography.Client
{
    /// <summary>
    /// Scrolls to new items as they are added in a control unless the scrollbar is scrolled up already.
    /// </summary>
    public sealed class ChatAutoScrollBehaviour : Behavior<ItemsControl>
    {
        private ScrollViewer scrollViewer;
        private bool shouldScroll;

        protected override void OnAttached()
        {
            AssociatedObject.Loaded += OnLoaded;
            AssociatedObject.Unloaded += OnUnloaded;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.Loaded -= OnLoaded;
            AssociatedObject.Unloaded -= OnUnloaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            INotifyCollectionChanged collectionChanged = (INotifyCollectionChanged)AssociatedObject.ItemsSource;
            if (collectionChanged == null) return;

            collectionChanged.CollectionChanged += OnCollectionChanged;

            // The ScrollViewer is a child of our ItemsControl.
            scrollViewer = (ScrollViewer)VisualTreeHelper.GetChild(AssociatedObject, 0);
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            INotifyCollectionChanged collectionChanged = (INotifyCollectionChanged)AssociatedObject.ItemsSource;
            if (collectionChanged == null) return;

            collectionChanged.CollectionChanged -= OnCollectionChanged;
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (scrollViewer == null) return;

            shouldScroll = scrollViewer.VerticalOffset == scrollViewer.ScrollableHeight;
            if (e.Action != NotifyCollectionChangedAction.Add || !shouldScroll) return;

            int count = AssociatedObject.Items.Count;
            if (count == 0) return;

            object item = AssociatedObject.Items[count - 1];
            FrameworkElement element = (FrameworkElement)AssociatedObject.ItemContainerGenerator.ContainerFromItem(item);
            element?.BringIntoView();
        }
    }
}
