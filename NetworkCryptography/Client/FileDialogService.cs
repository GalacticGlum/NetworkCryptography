/*
 * Author: Shon Verch
 * File Name: FileDialogService.cs
 * Project Name: NetworkCryptography
 * Creation Date: 10/21/2017
 * Modified Date: 10/21/2017
 * Description: A service class for opening file dialogs.
 */

using System;
using System.Windows;
using Microsoft.Win32;

namespace NetworkCryptography.Client
{
    /// <summary>
    /// A service class for opening file dialogs.
    /// </summary>
    public class FileDialogService
    {
        /// <summary>
        /// Open a file dialog with a caption and filter.
        /// </summary>
        /// <param name="caption">The caption of the file dialog.</param>
        /// <param name="filter">The file filter; file selection will be restricted to these file extensions.</param>
        /// <returns>A string containing the path of the selected file.</returns>
        public string OpenDialog(string caption, string filter = "All files (*.*)|*.*")
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
                Title = caption,
                Filter = filter,
                CheckFileExists = true,
                CheckPathExists = true,
                RestoreDirectory = true
            };

            return dialog.ShowDialog() == true ? dialog.FileName : string.Empty;
        }
    }
}
