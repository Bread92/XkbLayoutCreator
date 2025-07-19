/*
 * Project Name: Xkb Layout Creator (XLC)
 * Author: Bread92 <vaneck.van2019@gmail.com>
 * Date: July 15, 2025
 * Description: A simple UI app for creating your own keyboard layout.
 * Check out README.md for more information.
 *
 * This project is open source and available under the MIT License.
*/

using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Gtk;

namespace LayoutMaker
{
    class MainWindow : Window
    {
        private string _filePath = string.Empty;
        private bool _exportCancelled = true;

        public List<List<Button>> buttons = new();
        public List<Button> Row1 = new();
        public List<Button> Row2 = new();
        public List<Button> Row3 = new();
        public List<Button> Row4 = new();
        public List<Button> Row5 = new();

        private const int Row1Length = 13;
        private const int Row2Length = 13;
        private const int Row3Length = 11;
        private const int Row4Length = 10;
        private const int Row5Length = 1;

        public Box mainVbox = new(Orientation.Vertical, 2);
        public Box rowBox1 = new(Orientation.Horizontal, 2);
        public Box rowBox2 = new(Orientation.Horizontal, 2);
        public Box rowBox3 = new(Orientation.Horizontal, 2);
        public Box rowBox4 = new(Orientation.Horizontal, 2);
        public Box rowBox5 = new(Orientation.Horizontal, 2);
        public Box checkboxes = new(Orientation.Horizontal, 2);

        public LayoutBuilder lb = new();

        public MainWindow() : base("Xkb Layout Creator")
        {
            SetDefaultSize(700, 200);
            SetPosition(WindowPosition.Center);

            buttons.Add(Row1);
            buttons.Add(Row2);
            buttons.Add(Row3);
            buttons.Add(Row4);
            buttons.Add(Row5);

            MenuBar mb = new();

            Menu filemenu = new();
            MenuItem file = new("File");
            file.Submenu = filemenu;

            MenuItem reset = new("New");
            MenuItem save = new("Save");
            MenuItem saveAs = new("Save As");
            MenuItem export = new("Export");
            MenuItem load = new("Load");
            MenuItem exit = new("Exit");

            reset.Activated += Reset;
            save.Activated += SaveFile;
            saveAs.Activated += SaveFileAs;
            export.Activated += ExportFile;
            load.Activated += LoadFile;
            exit.Activated += ExitMenu;

            filemenu.Append(reset);
            filemenu.Append(save);
            filemenu.Append(saveAs);
            filemenu.Append(export);
            filemenu.Append(load);
            filemenu.Append(exit);

            mb.Append(file);

            CheckButton shiftCheck = new("Shift");
            CheckButton altCheck = new("AltGr");
            shiftCheck.Toggled += OnShiftToggled;
            altCheck.Toggled += OnAltToggled;

            checkboxes.PackStart(shiftCheck, false, false, 0);
            checkboxes.PackStart(altCheck, false, false, 0);

            CreateButtonLayout();
            UpdateKeyLabels();

            mainVbox.PackStart(mb, false, false, 0);
            mainVbox.PackStart(rowBox1, false, false, 0);
            mainVbox.PackStart(rowBox2, false, false, 0);
            mainVbox.PackStart(rowBox3, false, false, 0);
            mainVbox.PackStart(rowBox4, false, false, 0);
            mainVbox.PackStart(rowBox5, false, false, 0);
            mainVbox.PackStart(checkboxes, false, false, 0);

            Add(mainVbox);
            ShowAll();
            DeleteEvent += Exit;
        }

        private void Exit(object sender, DeleteEventArgs a) => Application.Quit();

        private void ExitMenu(object sender, EventArgs a) => Application.Quit();

        private void Reset(object sender, EventArgs a)
        {
            lb = new LayoutBuilder();

            _filePath = string.Empty;
            UpdateKeyLabels();
        }

        private void LoadFile(object sender, EventArgs a)
        {
            FileChooserDialog loadDialog = new("Select a file to load",
                    this,
                    FileChooserAction.Open,
                    "Cancel", ResponseType.Cancel,
                    "Open", ResponseType.Accept);

            FileFilter filter = new();
            filter.Name = "KLC files";
            filter.AddPattern("*.klc");
            loadDialog.AddFilter(filter);

            if (loadDialog.Run() == (int)ResponseType.Accept)
            {
                _filePath = loadDialog.Filename;
                lb.LoadLayout(_filePath);
                Console.WriteLine($"Loaded file: {_filePath}");
            }

            UpdateKeyLabels();

            loadDialog.Destroy();
        }

        private void SaveFile()
        {
            if(_filePath == string.Empty)
            {
                SaveFileAs();
                return;
            }

            string fileText = CreateKlcFile();
            System.IO.File.WriteAllText(_filePath, fileText);
        }

        private void SaveFile(object sender, EventArgs a)
        {
            SaveFile();
        }

        private void SaveFileAs()
        {
            FileChooserDialog saveDialog = new("Save file as...",
                    this,
                    FileChooserAction.Save,
                    "Cancel", ResponseType.Cancel,
                    "Save", ResponseType.Accept);

            FileFilter filter = new();
            filter.Name = "KLC files";
            filter.AddPattern("*.klc");
            saveDialog.AddFilter(filter);

            if(_filePath == string.Empty)
            {
                saveDialog.CurrentName="my_layout.klc";
            }
            else
            {
                // Collision with Widget.Path
                string fileName = System.IO.Path.GetFileName(_filePath);
                saveDialog.CurrentName=fileName;
            }

            if (saveDialog.Run() == (int)ResponseType.Accept)
            {
                _filePath = saveDialog.Filename;
                string fileText = CreateKlcFile();
                System.IO.File.WriteAllText(_filePath, fileText);
                Console.WriteLine($"Saved file: {_filePath}");
                _exportCancelled = false;
            }
            else
            {
                _exportCancelled = true;
            }

            saveDialog.Destroy();
        }

        private void SaveFileAs(object sender, EventArgs a) => SaveFileAs();

        private void ExportFile(object sender, EventArgs a)
        {
            SaveFile();

            if(!_exportCancelled)
                ShowExportDialog();
        }

        private void ShowExportDialog()
        {
            Dialog exportDialog = new("Export Layout", this, DialogFlags.Modal);
            exportDialog.AddButton("Cancel", ResponseType.Cancel);
            exportDialog.AddButton("OK", ResponseType.Accept);

            Box vbox = new(Orientation.Vertical, 5);

            Label langLabel = new("Language Code [us, ru, de, cz]:");
            Entry langEntry = new();

            Label variantLabel = new("Your variant's Code [Shavian -> shvn]:");
            Entry variantEntry = new();

            Label layoutDescLabel = new("Short description [English (Shavian)]:");
            Entry layoutDescEntry = new();

            vbox.PackStart(langLabel, false, false, 0);
            vbox.PackStart(langEntry, false, false, 5);
            vbox.PackStart(variantLabel, false, false, 0);
            vbox.PackStart(variantEntry, false, false, 5);
            vbox.PackStart(layoutDescLabel, false, false, 0);
            vbox.PackStart(layoutDescEntry, false, false, 5);

            exportDialog.ContentArea.PackStart(vbox, true, true, 0);
            exportDialog.ShowAll();

            bool validInput = false;

            while(!validInput)
            {
                if (exportDialog.Run() == (int)ResponseType.Accept)
                {
                    string lang = langEntry.Text;
                    string variantCode = variantEntry.Text;
                    string layoutDesc = layoutDescEntry.Text;

                    if (string.IsNullOrWhiteSpace(lang) || 
                            string.IsNullOrWhiteSpace(variantCode) || 
                            string.IsNullOrWhiteSpace(layoutDesc))
                    {
                        ShowDialog(MessageType.Warning, "One of the fields is empty!");
                        continue;
                    }

                    validInput = true;
                    LayoutGenerator lg = new();
                    lg.Generate(lb.Keys, lang, variantCode, layoutDesc);

                    ShowDialog(MessageType.Info, "Files generated successfully!\nProceed to README file for further instructions");
                }
                else
                {
                    exportDialog.Destroy();
                    return;
                }
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
            Dialog inputDialog = new(keyCode.ToString(), this, DialogFlags.Modal);
            inputDialog.AddButton("Cancel", ResponseType.Cancel);
            inputDialog.AddButton("OK", ResponseType.Accept);

            Label charLabel = new("New character or unicode value (U####):");
            Entry entry = new();
            inputDialog.ContentArea.PackStart(charLabel, false, false, 5);
            inputDialog.ContentArea.PackStart(entry, false, false, 5);
            inputDialog.ShowAll();

            bool validInput = false;

            string preferredCharacter = string.Empty;

            while (!validInput)
            {
                if (inputDialog.Run() == (int)ResponseType.Accept)
                {
                    string userInput = entry.Text;

                    if (string.IsNullOrEmpty(userInput))
                    {
                        preferredCharacter = "NoSymbol";
                        lb.SetKey(keyCode, preferredCharacter);
                        validInput = true;
                    }
                    else if ((userInput.StartsWith("U+") || userInput.StartsWith("U")) && userInput.Length > 1)
                    {
                        string converted = ConvertUnicodeInput(userInput);

                        if (converted != null && converted != "?")
                        {
                            preferredCharacter = converted;
                            lb.SetKey(keyCode, userInput);
                            validInput = true;
                        }
                        else
                        {
                            ShowDialog(MessageType.Warning, "Invalid Unicode codepoint.");
                        }
                    }
                    else if (userInput.Length == 1)
                    {
                        preferredCharacter = userInput;
                        lb.SetKey(keyCode, preferredCharacter);
                        validInput = true;
                    }
                    else
                    {
                        ShowDialog(MessageType.Warning, "Please enter exactly one character or a valid unicode codepoint (U#####).");
                        entry.Text = string.Empty;
                    }
                }
                else
                {
                    inputDialog.Destroy();
                    return;
                }
            }

            UpdateKeyLabels();
            inputDialog.Destroy();
        }

        private void ShowDialog(MessageType type, string text)
        {
            var dialog = new MessageDialog(this,
                    DialogFlags.Modal,
                    type,
                    ButtonsType.Ok,
                    text);
            dialog.Run();
            dialog.Destroy();

        }

        private string ConvertUnicodeInput(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return input;

            input = input.Trim().ToUpperInvariant();

            if (input.StartsWith("U+"))
                input = input.Substring(2);
            else if (input.StartsWith("U"))
                input = input.Substring(1);
            else
                return input;

            if (int.TryParse(input, System.Globalization.NumberStyles.HexNumber, null, out int codepoint))
            {
                try { return char.ConvertFromUtf32(codepoint); }
                catch { return "?"; }
            }

            return "?";
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
                        string label = GetKeyLabel(keyIndex);

                        if(label == " ")
                            b.Label = "[Space]";
                        else if(label == "NoSymbol")
                            b.Label = "";
                        else if ((label.StartsWith("U") || label.StartsWith("U+")) && label.Length > 1)
                        {
                            b.Label = ConvertUnicodeInput(label);
                        }
                        else
                            b.Label = label;

                        keyIndex++;
                    }
                }
            }
        }

        private void CreateButtonLayout()
        {
            int keyIndex = 0;

           AddActiveButtons(ref keyIndex, Row1Length, Row1, rowBox1);

           AddInactiveButton("BackSpace", Row1, rowBox1);
           AddInactiveButton("Tab", Row2, rowBox2);

           AddActiveButtons(ref keyIndex, Row2Length, Row2, rowBox2);

           AddInactiveButton("Caps", Row3, rowBox3);

           AddActiveButtons(ref keyIndex, Row3Length, Row3, rowBox3);

           AddInactiveButton("Enter", Row3, rowBox3);
           AddInactiveButton("LShift", Row4, rowBox4);

           AddActiveButtons(ref keyIndex, Row4Length, Row4, rowBox4);

           AddInactiveButton("RShift", Row4, rowBox4);
           AddInactiveButton("Ctrl", Row5, rowBox5);
           AddInactiveButton("X", Row5, rowBox5);
           AddInactiveButton("Alt", Row5, rowBox5);

           for(int i = 0; i < Row5Length; i++)
           {
               string keyLabel = GetKeyLabel(keyIndex);

               if (keyLabel != "NoSymbol")
               {
                   Button newButton = new(keyLabel);
                   newButton.Name = keyIndex.ToString();
                   newButton.Clicked += (sender, e) => OnButtonClicked(sender, e);

                   int minWidth = 350;
                   newButton.SetSizeRequest(minWidth, -1);

                   rowBox5.PackStart(newButton, false, false, 0);
                   Row5.Add(newButton);
               }
           }

           AddInactiveButton("Alt", Row5, rowBox5);
           AddInactiveButton("Ctrl", Row5, rowBox5);
        }

        private void AddInactiveButton(string name, List<Button> buttons, Box rowBox)
        {
           Button button = new(name);
           button.Sensitive = false;
           rowBox.PackStart(button, false, false, 0);
           buttons.Add(button);
        }

        private void AddActiveButtons(ref int keyIndex, int rowLength, List<Button> buttons, Box rowBox)
        {
           for(int i = 0; i < rowLength; i++)
           {
               string keyLabel = GetKeyLabel(keyIndex);

               if (keyLabel != "NoSymbol")
               {
                   Button newButton = new(keyLabel);
                   newButton.Name = keyIndex.ToString();
                   newButton.Clicked += (sender, e) => OnButtonClicked(sender, e);

                   rowBox.PackStart(newButton, false, false, 0);
                   buttons.Add(newButton);
               }

               keyIndex++;
           }

        }

        private string GetKeyLabel(int index)
        {
            if(!lb.IsShift && !lb.IsAlt)     return lb.Keys[index].Normal;
            else if(lb.IsShift && !lb.IsAlt) return lb.Keys[index].Shift;
            else if(!lb.IsShift && lb.IsAlt) return lb.Keys[index].Alt;
            else if(lb.IsShift && lb.IsAlt)  return lb.Keys[index].ShiftAlt;
            else                             return "[]";
        }

        private string CreateKlcFile()
        {
            StringBuilder sb = new();

            foreach(var key in lb.Keys)
                sb.AppendLine(key.ToKlcString());

            return sb.ToString();
        }
    }
}
