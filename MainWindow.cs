/*
 * Project Name: Xkb Keyboard Layout Creator (XKLC)
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

// TODO:
// - Combobox to choose which language's variant (load from xkb/symbols dir)
// - Option for allowing changing system files
// - Backing up system files before changing (once! check for <name>.bak
//   before rewriting!)
// ? Allow making your own languages instead of only variants(?)
namespace LayoutMaker
{
    class MainWindow : Window
    {
        private string preferredCharacter = string.Empty;
        private string filePath = string.Empty;

        public List<List<Button>> buttons = new List<List<Button>>();
        public List<Button> Row1 = new List<Button>();
        public List<Button> Row2 = new List<Button>();
        public List<Button> Row3 = new List<Button>();
        public List<Button> Row4 = new List<Button>();
        public List<Button> Row5 = new List<Button>();

        private const int Row1Length = 13;
        private const int Row2Length = 13;
        private const int Row3Length = 11;
        private const int Row4Length = 10;
        private const int Row5Length = 1;

        private LayoutBuilder lb = new LayoutBuilder();
        public Box mainVbox = new Box(Orientation.Vertical, 2);
        public Box rowBox1 = new Box(Orientation.Horizontal, 2);
        public Box rowBox2 = new Box(Orientation.Horizontal, 2);
        public Box rowBox3 = new Box(Orientation.Horizontal, 2);
        public Box rowBox4 = new Box(Orientation.Horizontal, 2);
        public Box rowBox5 = new Box(Orientation.Horizontal, 2);

        public MainWindow() : base("Keyboard Layout Creator")
        {
            SetDefaultSize(700, 200);
            SetPosition(WindowPosition.Center);

            buttons.Add(Row1);
            buttons.Add(Row2);
            buttons.Add(Row3);
            buttons.Add(Row4);
            buttons.Add(Row5);

            // Menu Bar
            MenuBar mb = new MenuBar();

            Menu filemenu = new Menu();
            MenuItem file = new MenuItem("File");
            file.Submenu = filemenu;

            MenuItem save = new MenuItem("Save");
            MenuItem saveAs = new MenuItem("Save As");
            MenuItem export = new MenuItem("Export");
            MenuItem load = new MenuItem("Load");
            MenuItem exit = new MenuItem("Exit");

            save.Activated += SaveFile;
            saveAs.Activated += SaveFileAs;
            export.Activated += ExportFile;
            load.Activated += LoadFile;
            exit.Activated += ExitMenu;

            filemenu.Append(save);
            filemenu.Append(saveAs);
            filemenu.Append(export);
            filemenu.Append(load);
            filemenu.Append(exit);

            mb.Append(file);

            mainVbox.PackStart(mb, false, false, 0);

            // Check Boxes
            CheckButton shiftCheck = new CheckButton("Shift");
            CheckButton altCheck = new CheckButton("AltGr");
            shiftCheck.Toggled += OnShiftToggled;
            altCheck.Toggled += OnAltToggled;

            CreateButtonLayout();
            UpdateKeyLabels();

            // Keyboard Layout
            mainVbox.PackStart(rowBox1, false, false, 0);
            mainVbox.PackStart(rowBox2, false, false, 0);
            mainVbox.PackStart(rowBox3, false, false, 0);
            mainVbox.PackStart(rowBox4, false, false, 0);
            mainVbox.PackStart(rowBox5, false, false, 0);
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

        private void SaveFile()
        {
            if(filePath == string.Empty)
            {
                SaveFileAs();
                return;
            }

            string fileText = CreateKlcFile();
            System.IO.File.WriteAllText(filePath, fileText);
        }

        private void SaveFile(object sender, EventArgs a)
        {
            SaveFile();
        }

        private void SaveFileAs()
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

            if(filePath == string.Empty)
            {
                saveDialog.CurrentName="my_layout.klc";
            }
            else
            {
                // Collision with Widget.Path
                string fileName = System.IO.Path.GetFileName(filePath);
                saveDialog.CurrentName=fileName;
            }

            if (saveDialog.Run() == (int)ResponseType.Accept)
            {
                string filePath = saveDialog.Filename;
                this.filePath = filePath;
                string fileText = CreateKlcFile();
                System.IO.File.WriteAllText(filePath, fileText);
                Console.WriteLine($"Saved file: {filePath}");
            }

            saveDialog.Destroy();

        }

        private void SaveFileAs(object sender, EventArgs a)
        {
            SaveFileAs();
        }

        private void ExportFile(object sender, EventArgs a)
        {
            SaveFile();
            ShowExportDialog();
        }

        private void ShowExportDialog()
        {
            Dialog exportDialog = new Dialog("Export Layout", this, DialogFlags.Modal);
            exportDialog.AddButton("Cancel", ResponseType.Cancel);
            exportDialog.AddButton("OK", ResponseType.Accept);

            // Create a Box to hold the labels and entries
            Box vbox = new Box(Orientation.Vertical, 5);

            // Create labels and entries
            Label languageLabel = new Label("Language Code [us, ru, de]:");
            Entry languageEntry = new Entry();

            Label variantLabel = new Label("Variant's Code [Shavian -> shvn]:");
            Entry variantEntry = new Entry();

            Label layoutNameLabel = new Label("Layout Description [English (Shavian)]:");
            Entry layoutNameEntry = new Entry();

            // Pack the labels and entries into the Box
            vbox.PackStart(languageLabel, false, false, 0);
            vbox.PackStart(languageEntry, false, false, 5);
            vbox.PackStart(variantLabel, false, false, 0);
            vbox.PackStart(variantEntry, false, false, 5);
            vbox.PackStart(layoutNameLabel, false, false, 0);
            vbox.PackStart(layoutNameEntry, false, false, 5);

            exportDialog.ContentArea.PackStart(vbox, true, true, 0);
            exportDialog.ShowAll();

            // Run the dialog and check the response
            if (exportDialog.Run() == (int)ResponseType.Accept)
            {
                string languageCode = languageEntry.Text;
                string variantCode = variantEntry.Text;
                string layoutName = layoutNameEntry.Text;

                // Call the LayoutGenerator with the provided information
                LayoutGenerator lg = new LayoutGenerator();
                lg.Generate(lb.Keys, languageCode, variantCode, layoutName);
            }

            exportDialog.Destroy();
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

                if(entry.Text == string.Empty)
                {
                    lb.SetKey(keyCode, "NoSymbol");
                    inputDialog.Destroy();
                    UpdateKeyLabels();
                    return;
                }

                lb.SetKey(keyCode, preferredCharacter);
                UpdateKeyLabels();
            }

            inputDialog.Destroy();
        }

        private void UpdateKeyLabels()
        {
            int keyIndex = 0;

            foreach(List<Button> buttonList in buttons)
            {
                foreach(Button b in buttonList)
                {
                    if(b.Sensitive)
                    {
                        string label = GetKeyLabel(lb, keyIndex);

                        if(label == " ")
                        {
                            b.Label = "[Space]";
                        }
                        else if(label == "NoSymbol")
                        {
                            b.Label = "[]";
                        }
                        else
                        {
                            b.Label = label;
                        }

                        keyIndex++;
                    }
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

               if (keyLabel != "NoSymbol")
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

               if (keyLabel != "NoSymbol")
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
           Button capsLock = new Button("Caps");
           capsLock.Sensitive = false;
           rowBox3.PackStart(capsLock, false, false, 0);
           Row3.Add(capsLock);

           for(int i = 0; i < Row3Length; i++)
           {
               keyLabel = GetKeyLabel(lb, keyIndex);

               if (keyLabel != "NoSymbol")
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
           Button lShift = new Button("LShift");
           lShift.Sensitive = false;
           rowBox4.PackStart(lShift, false, false, 0);
           Row4.Add(lShift);

           for(int i = 0; i < Row4Length; i++)
           {
               keyLabel = GetKeyLabel(lb, keyIndex);

               if (keyLabel != "NoSymbol")
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
           Button rShift = new Button("RShift");
           rShift.Sensitive = false;
           rowBox4.PackStart(rShift, false, false, 0);
           Row4.Add(rShift);


           Button lCtrl = new Button("Ctrl");
           lCtrl.Sensitive = false;
           rowBox5.PackStart(lCtrl, false, false, 0);
           Row5.Add(lCtrl);

           Button super = new Button("X");
           super.Sensitive = false;
           rowBox5.PackStart(super, false, false, 0);
           Row5.Add(super);

           Button lAlt = new Button("Alt");
           lAlt.Sensitive = false;
           rowBox5.PackStart(lAlt, false, false, 0);
           Row5.Add(lAlt);


           for(int i = 0; i < Row5Length; i++)
           {
               keyLabel = GetKeyLabel(lb, keyIndex);

               if (keyLabel != "NoSymbol")
               {
                   Button newButton = new Button(keyLabel);
                   newButton.Name = keyIndex.ToString();
                   newButton.Clicked += (sender, e) => OnButtonClicked(sender, e);

                   int minWidth = 350;
                   newButton.SetSizeRequest(minWidth, -1);

                   rowBox5.PackStart(newButton, false, false, 0);
                   Row5.Add(newButton);
               }

               keyIndex++;
           }

           Button rAlt = new Button("Alt");
           rAlt.Sensitive = false;
           rowBox5.PackStart(rAlt, false, false, 0);
           Row5.Add(rAlt);

           Button rCtrl = new Button("Ctrl");
           rCtrl.Sensitive = false;
           rowBox5.PackStart(rCtrl, false, false, 0);
           Row5.Add(rCtrl);
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
                return "[]";
            }
        }

        private string CreateKlcFile()
        {
            StringBuilder sb = new();

            foreach(var key in lb.Keys)
            {
                sb.AppendLine(key.ToKlcString());
            }

            return sb.ToString();
        }
    }

}
