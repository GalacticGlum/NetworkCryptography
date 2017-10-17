/*
 * Author: John Leidegren
 * File Name: BackendConsole.cs
 * Project Name: NetworkCryptography
 * Creation Date: 10/17/2017
 * Modified Date: 10/17/2017
 * Description: Console functionality using the backend Win32 API.
 * URL: https://stackoverflow.com/a/718505
 */

using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;

namespace NetworkCryptography.Core.Logging
{
    /// <summary>
    /// Console functionality using the backend Win32 API.
    /// </summary>
    [SuppressUnmanagedCodeSecurity]
    public static class BackendConsole
    {
        private const string Kernel32_DllName = "kernel32.dll";

        [DllImport(Kernel32_DllName)]
        private static extern bool AllocConsole();

        [DllImport(Kernel32_DllName)]
        private static extern bool FreeConsole();

        [DllImport(Kernel32_DllName)]
        private static extern IntPtr GetConsoleWindow();

        [DllImport(Kernel32_DllName)]
        private static extern int GetConsoleOutputCP();

        /// <summary>
        /// Indicates whether the application is hosting a console window.
        /// </summary>
        public static bool HasConsole => GetConsoleWindow() != IntPtr.Zero;

        /// <summary>
        /// Creates a new console instance if the process is not attached to a console already.
        /// </summary>
        public static void Show()
        {
            if (HasConsole) return;

            AllocConsole();
            InvalidateOutAndError();
        }

        /// <summary>
        /// If the process has a console attached to it, it will be detached and no longer visible. Writing to the System.Console is still possible, but no output will be shown.
        /// </summary>
        public static void Hide()
        {
            if (!HasConsole) return;

            FreeConsoleMemory();
            FreeConsole();
        }

        /// <summary>
        /// Toggle the console on and off.
        /// </summary>
        public static void Toggle()
        {
            if (HasConsole)
            {
                Hide();
            }
            else
            {
                Show();
            }
        }

        /// <summary>
        /// Clears the out, error, and standard error stream fields in the Console.
        /// </summary>
        private static void InvalidateOutAndError()
        {
            Type type = typeof(Console);

            FieldInfo outField = type.GetField("_out", BindingFlags.Static | BindingFlags.NonPublic);
            FieldInfo errorField = type.GetField("_error", BindingFlags.Static | BindingFlags.NonPublic);
            MethodInfo stdOutErrorMethod = type.GetMethod("InitializeStdOutError", BindingFlags.Static | BindingFlags.NonPublic);

            if (outField != null)
            {
                outField.SetValue(null, null);
            }

            if (errorField != null)
            {
                errorField.SetValue(null, null);
            }

            stdOutErrorMethod.Invoke(null, new object[] { true });
        }

        /// <summary>
        /// Frees memory from the console.
        /// </summary>
        private static void FreeConsoleMemory()
        {
            Console.SetOut(TextWriter.Null);
            Console.SetError(TextWriter.Null);
        }
    }
}
