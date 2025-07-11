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
        public Box mainVbox = new Box(Orientation.Vertical, 2);
        public Box rowBox1 = new Box(Orientation.Horizontal, 2);
        public Box rowBox2 = new Box(Orientation.Horizontal, 2);
        public Box rowBox3 = new Box(Orientation.Horizontal, 2);
        public Box rowBox4 = new Box(Orientation.Horizontal, 2);

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

            mainVbox.PackStart(rowBox1, false, false, 0);
            mainVbox.PackStart(rowBox2, false, false, 0);
            mainVbox.PackStart(rowBox3, false, false, 0);
            mainVbox.PackStart(rowBox4, false, false, 0);
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

        private void OnButtonClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            KeyCode keyCode = lb.GetKeyCodeByIndex(int.Parse(button.Name));
            Dialog inputDialog = new Dialog(keyCode.ToString(), this, DialogFlags.Modal);
            inputDialog.AddButton("Cancel", ResponseType.Cancel);
            inputDialog.AddButton("OK", ResponseType.Accept);

            Entry entry = new Entry();
            inputDialog.ContentArea.PackStart(entry, true, true, 0);
            inputDialog.ShowAll();

            if (inputDialog.Run() == (int)ResponseType.Accept)
            {
                preferredCharacter = entry.Text;

                lb.SetKey(keyCode, preferredCharacter);
                UpdateKeyLabels();
            }

            inputDialog.Destroy();
        }

        private void UpdateKeyLabels()
        {
            int keyIndex = 0;

            foreach(Button b in Row1)
            {
                if(b.Sensitive)
                {
                    b.Label = GetKeyLabel(lb, keyIndex);
                    keyIndex++;
                }
            }
            foreach(Button b in Row2)
            {
                if(b.Sensitive)
                {
                    b.Label = GetKeyLabel(lb, keyIndex);
                    keyIndex++;
                }
            }
            foreach(Button b in Row3)
            {
                if(b.Sensitive)
                {
                    b.Label = GetKeyLabel(lb, keyIndex);
                    keyIndex++;
                }
            }
            foreach(Button b in Row4)
            {
                if(b.Sensitive)
                {
                    b.Label = GetKeyLabel(lb, keyIndex);
                    keyIndex++;
                }
            }
        }

        private void CreateButtonLayout()
        {
            int keyIndex = 0;
            string keyLabel = "*";

           for(int i = 0; i < Row1Length; i++)
           {
               keyLabel = GetKeyLabel(lb, keyIndex);

               if (keyLabel != "None")
               {
                   Button newButton = new Button(keyLabel);
                   newButton.Name = keyIndex.ToString();
                   newButton.Clicked += (sender, e) => OnButtonClicked(sender, e);

                   rowBox1.PackStart(newButton, false, false, 0);
                   Row1.Add(newButton);
               }

               keyIndex++;

           }

           // BackSpace Key
           Button backspace = new Button("BackSpace");
           backspace.Sensitive = false;
           rowBox1.PackStart(backspace, false, false, 0);
           Row1.Add(backspace);

           // Tab
           Button tab = new Button("Tab");
           tab.Sensitive = false;
           rowBox2.PackStart(tab, false, false, 0);
           Row2.Add(tab);

           for(int i = 0; i < Row2Length; i++)
           {
               keyLabel = GetKeyLabel(lb, keyIndex);

               if (keyLabel != "None")
               {
                   Button newButton = new Button(keyLabel);
                   newButton.Name = keyIndex.ToString();
                   newButton.Clicked += (sender, e) => OnButtonClicked(sender, e);

                   rowBox2.PackStart(newButton, false, false, 0);
                   Row2.Add(newButton);
               }

               keyIndex++;
           }

           // CapsLock
           Button capsLock = new Button("CLock");
           capsLock.Sensitive = false;
           rowBox3.PackStart(capsLock, false, false, 0);
           Row3.Add(capsLock);

           for(int i = 0; i < Row3Length; i++)
           {
               keyLabel = GetKeyLabel(lb, keyIndex);

               if (keyLabel != "None")
               {
                   Button newButton = new Button(keyLabel);
                   newButton.Name = keyIndex.ToString();
                   newButton.Clicked += (sender, e) => OnButtonClicked(sender, e);

                   rowBox3.PackStart(newButton, false, false, 0);
                   Row3.Add(newButton);
               }

               keyIndex++;
           }

           // Enter
           Button enter = new Button("Enter");
           enter.Sensitive = false;
           rowBox3.PackStart(enter, false, false, 0);
           Row3.Add(enter);

           // LShift
           Button lShift = new Button("Left Shift");
           lShift.Sensitive = false;
           rowBox4.PackStart(lShift, false, false, 0);
           Row4.Add(lShift);

           for(int i = 0; i < Row4Length; i++)
           {
               keyLabel = GetKeyLabel(lb, keyIndex);

               if (keyLabel != "None")
               {
                   Button newButton = new Button(keyLabel);
                   newButton.Name = keyIndex.ToString();
                   newButton.Clicked += (sender, e) => OnButtonClicked(sender, e);

                   rowBox4.PackStart(newButton, false, false, 0);
                   Row4.Add(newButton);
               }

               keyIndex++;
           }

           // Enter
           Button rShift = new Button("Right Shift");
           rShift.Sensitive = false;
           rowBox4.PackStart(rShift, false, false, 0);
           Row4.Add(rShift);
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
