/*
 * Project Name: Linux Keyboard Layout Creator (LKLC)
 * Author: Bread92 <vaneck.van2019@gmail.com>
 * Date: July 10, 2025
 * Description: A simple UI app for creating your own keyboard layout.
 * Check out README.md for more information.
 *
 * This project is open source and available under the MIT License.
*/

using System;
using Gtk;

namespace LayoutMaker
{
    class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            Application.Init();

            var app = new Application("org.LayoutMaker.LayoutMaker", GLib.ApplicationFlags.None);
            app.Register(GLib.Cancellable.Current);

            var win = new MainWindow();
            app.AddWindow(win);

            win.Show();
            Application.Run();
        }
    }
}
