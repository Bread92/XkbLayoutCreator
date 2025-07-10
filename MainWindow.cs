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
using System.Text;
using System.Collections.Generic;
using Gtk;

namespace LayoutMaker
{
    class MainWindow : Window
    {
        private string preferredCharacter = string.Empty;
        private string filePath = string.Empty;

        public List<Button> buttons = new List<Button>();
        public List<Button> Row1 = new List<Button>();
        public List<Button> Row2 = new List<Button>();
        public List<Button> Row3 = new List<Button>();
        public List<Button> Row4 = new List<Button>();

        private const int Row1Length = 13;
        private const int Row2Length = 13;
        private const int Row3Length = 11;
        private const int Row4Length = 10;

        private LayoutBuilder lb = new LayoutBuilder();
        public Grid grid = new Grid();

        public MainWindow() : base("Keyboard Layout Creator")
        {
            SetDefaultSize(700, 200);
            SetPosition(WindowPosition.Center);

            // Menu Bar
            MenuBar mb = new MenuBar();

            Menu filemenu = new Menu();
            MenuItem file = new MenuItem("File");
            file.Submenu = filemenu;

            MenuItem save = new MenuItem("Save");
            MenuItem saveAs = new MenuItem("Save As");
            MenuItem load = new MenuItem("Load");
            MenuItem exit = new MenuItem("Exit");
            save.Activated += SaveFile;
            saveAs.Activated += SaveFileAs;
            load.Activated += LoadFile;
            exit.Activated += ExitMenu;
            filemenu.Append(save);
            filemenu.Append(load);
            filemenu.Append(exit);

            mb.Append(file);

            Box mainVbox = new Box(Orientation.Vertical, 2);
            mainVbox.PackStart(mb, false, false, 0);

            // Check Boxes
            CheckButton shiftCheck = new CheckButton("Shift");
            CheckButton altCheck = new CheckButton("AltGr");
            shiftCheck.Toggled += OnShiftToggled;
            altCheck.Toggled += OnAltToggled;

            // Keyboard Layout
            grid.RowSpacing = 5;
            grid.ColumnSpacing = 5;

            CreateButtonLayout();

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

        private void LoadFile(object sender, EventArgs a)
        {
            FileChooserDialog loadDialog = new FileChooserDialog("Select a file to load",
                    this,
                    FileChooserAction.Open,
                    "Cancel", ResponseType.Cancel,
                    "Open", ResponseType.Accept);

            FileFilter filter = new FileFilter();
            filter.Name = "KLC files";
            filter.AddPattern("*.klc");
            loadDialog.AddFilter(filter);

            if (loadDialog.Run() == (int)ResponseType.Accept)
            {
                filePath = loadDialog.Filename;
                lb.LoadLayout(filePath);
                Console.WriteLine($"Loaded file: {filePath}");
            }

            UpdateKeyLabels();

            loadDialog.Destroy();
        }

        private void SaveFile(object sender, EventArgs a)
        {
            if(filePath == string.Empty)
            {
                SaveFileAs(sender, a);
                return;
            }

            string fileText = CreateKlcFile();
            System.IO.File.WriteAllText(filePath, fileText);
            return;

        }

        private void SaveFileAs(object sender, EventArgs a)
        {
            FileChooserDialog saveDialog = new FileChooserDialog("Save file as...",
                    this,
                    FileChooserAction.Save,
                    "Cancel", ResponseType.Cancel,
                    "Save", ResponseType.Accept);

            FileFilter filter = new FileFilter();
            filter.Name = "KLC files";
            filter.AddPattern("*.klc");
            saveDialog.AddFilter(filter);

            saveDialog.CurrentName="my_layout.klc";

            if (saveDialog.Run() == (int)ResponseType.Accept)
            {
                string filePath = saveDialog.Filename;
                string fileText = CreateKlcFile();
                System.IO.File.WriteAllText(filePath, fileText);
                Console.WriteLine($"Saved file: {filePath}");
            }

            saveDialog.Destroy();
        }

        void OnShiftToggled(object sender, EventArgs args)
        {
            CheckButton cb = (CheckButton) sender;
            lb.IsShift = cb.Active;
            UpdateKeyLabels();
        }

        void OnAltToggled(object sender, EventArgs args)
        {
            CheckButton cb = (CheckButton) sender;
            lb.IsAlt = cb.Active;
            UpdateKeyLabels();
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            Dialog inputDialog = new Dialog("Preferred Character", this, DialogFlags.Modal);
            inputDialog.AddButton("Cancel", ResponseType.Cancel);
            inputDialog.AddButton("OK", ResponseType.Accept);

            Entry entry = new Entry();
            inputDialog.ContentArea.PackStart(entry, true, true, 0);
            inputDialog.ShowAll();

            if (inputDialog.Run() == (int)ResponseType.Accept)
            {
                preferredCharacter = entry.Text;

                var button = sender as Button;
                lb.SetKeyByIndex(int.Parse(button.Name), preferredCharacter);
                UpdateKeyLabels();
            }

            inputDialog.Destroy();
        }

        private void UpdateKeyLabels()
        {
            int keyIndex = 0;
            foreach(Button b in Row1)
            {
                b.Label = GetKeyLabel(lb, keyIndex);
                keyIndex++;
            }
            foreach(Button b in Row2)
            {
                b.Label = GetKeyLabel(lb, keyIndex);
                keyIndex++;
            }
            foreach(Button b in Row3)
            {
                b.Label = GetKeyLabel(lb, keyIndex);
                keyIndex++;
            }
            foreach(Button b in Row4)
            {
                b.Label = GetKeyLabel(lb, keyIndex);
                keyIndex++;
            }
        }

        private void CreateButtonLayout()
        {
            int keyIndex = 0;
            string keyLabel = "*";

            for (int col = 0; col < Row1Length; col++)
            {
                keyLabel = GetKeyLabel(lb, keyIndex);

                if (keyLabel != "None")
                {
                    Button newButton = new Button(keyLabel);
                    newButton.Name = keyIndex.ToString();
                    newButton.Clicked += (sender, e) => Button_Clicked(sender, e);

                    grid.Attach(newButton, col, 0, 1, 1);
                    Row1.Add(newButton);
                }

                keyIndex++;
            }
            for (int col = 0; col < Row2Length; col++)
            {
                keyLabel = GetKeyLabel(lb, keyIndex);

                if (keyLabel != "None")
                {
                    Button newButton = new Button(keyLabel);
                    newButton.Name = keyIndex.ToString();
                    newButton.Clicked += (sender, e) => Button_Clicked(sender, e);

                    grid.Attach(newButton, col, 1, 1, 1);
                    Row2.Add(newButton);
                }

                keyIndex++;
            }

            for (int col = 0; col < Row3Length; col++)
            {
                keyLabel = GetKeyLabel(lb, keyIndex);

                if (keyLabel != "None")
                {
                    Button newButton = new Button(keyLabel);
                    newButton.Name = keyIndex.ToString();
                    newButton.Clicked += (sender, e) => Button_Clicked(sender, e);

                    grid.Attach(newButton, col, 2, 1, 1);
                    Row3.Add(newButton);
                }

                keyIndex++;
            }

            for (int col = 0; col < Row4Length; col++)
            {
                keyLabel = GetKeyLabel(lb, keyIndex);

                if (keyLabel != "None")
                {
                    Button newButton = new Button(keyLabel);
                    newButton.Name = keyIndex.ToString();
                    newButton.Clicked += (sender, e) => Button_Clicked(sender, e);

                    grid.Attach(newButton, col, 3, 1, 1);
                    Row4.Add(newButton);
                }

                keyIndex++;
            }

            keyIndex = 20;
        }

        private string GetKeyLabel(LayoutBuilder lb, int index)
        {
            if(!lb.IsShift && !lb.IsAlt)
            {
                return lb.Keys[index].Normal;
            }
            else if(lb.IsShift && !lb.IsAlt)
            {
                return lb.Keys[index].Shift;
            }
            else if(!lb.IsShift && lb.IsAlt)
            {
                return lb.Keys[index].Alt;
            }
            else if(lb.IsShift && lb.IsAlt)
            {
                return lb.Keys[index].ShiftAlt;
            }
            else
            {
                return string.Empty;
            }
        }

        private string CreateKlcFile()
        {
            StringBuilder sb = new();

            foreach(var key in lb.Keys)
            {
                sb.AppendLine(key.ToString());
            }

            return sb.ToString();
        }
    }

}
