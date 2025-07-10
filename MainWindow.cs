using System;
using System.Collections.Generic;
using Gtk;

namespace LayoutMaker
{
    class MainWindow : Window
    {
        public List<Button> keys = new List<Button>();
        private LayoutBuilder lb = new LayoutBuilder();

        public MainWindow() : base("Keyboard Layout Creator")
        {
            SetDefaultSize(700, 200);
            SetPosition(WindowPosition.Center);

            const int Rows = 4;
            const int Columns = 13;

            // Menu Bar
            MenuBar mb = new MenuBar();

            Menu filemenu = new Menu();
            MenuItem file = new MenuItem("File");
            file.Submenu = filemenu;

            MenuItem exit = new MenuItem("Exit");
            exit.Activated += ExitMenu;
            filemenu.Append(exit);

            mb.Append(file);

            Box mainVbox = new Box(Orientation.Vertical, 2);
            mainVbox.PackStart(mb, false, false, 0);

            CheckButton shiftCheck = new CheckButton("Shift");
            CheckButton altCheck = new CheckButton("AltGr");

            // Keyboard Layout
            var grid = new Grid();
            grid.RowSpacing = 5;
            grid.ColumnSpacing = 5;

            string[,] layout = new string[4, 13]
            {
                { "`", "1", "2", "3", "4", "5", "6", "7", "8", "9", "0", "-", "=" },
                { "Q", "W", "E", "R", "T", "Y", "U", "I", "O", "P", "[", "]", "\\" },
                { "A", "S", "D", "F", "G", "H", "J", "K", "L", ";", "'", "", "" },
                { "Z", "X", "C", "V", "B", "N", "M", ",", ".", "/", "", "", "" }
            };

            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Columns; col++)
                {
                    string keyLabel = layout[row, col];
                    if (!string.IsNullOrEmpty(keyLabel))
                    {
                        keys.Add(new Button(keyLabel));
                        keys[keys.Count - 1].Clicked += Button_Clicked;
                        grid.Attach(keys[keys.Count - 1], col, row, 1, 1);
                    }
                }
            }
            mainVbox.PackStart(grid, true, true, 0);
            mainVbox.PackStart(shiftCheck, false, false, 0);
            mainVbox.PackStart(altCheck, false, false, 0);

            Add(mainVbox);
            ShowAll();
            DeleteEvent += Exit;
        }

        private void Exit(object sender, DeleteEventArgs a)
        {
            Application.Quit();
        }

        private void ExitMenu(object sender, EventArgs a)
        {
            Application.Quit();
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            Button button = sender as Button;

            if (button != null)
            {
                Console.WriteLine($"{lb.GetKeyCode(button.Label)} clicked!");
            }
        }
    }
}
