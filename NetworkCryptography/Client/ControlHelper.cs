/*
 * Author: Shon Verch
 * File Name: ControlHelper.cs
 * Project Name: NetworkCryptography
 * Creation Date: 10/21/2017
 * Modified Date: 10/21/2017
 * Description: Collection of useful controls-related functionality.
 */

using System.Windows.Controls;

namespace NetworkCryptography.Client
{
    /// <summary>
    /// Collection of useful controls-related functionality.
    /// </summary>
    public static class ControlHelper
    {
        /// <summary>
        /// Returns whether a <see cref="ScrollViewer"/> is scrolled to the bottom. 
        /// </summary>
        /// <param name="scrollViewer">The scroll viewer.</param>
        /// <param name="leeway">The threshold of extra space recognized as the bottom.</param>
        public static bool IsScrolledToBottom(this ScrollViewer scrollViewer, int leeway = 80) =>
            scrollViewer.VerticalOffset >= scrollViewer.ScrollableHeight - leeway;
    }
}
