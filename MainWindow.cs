using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;
using Gtk;

namespace LayoutMaker
{
    class MainWindow : Window
    {
        string _filePath = string.Empty;
        string _xkbPath = "/usr/share/X11/xkb";
        string _symbolsPath = "/usr/share/X11/xkb/symbols";

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

        public Entry xkbPathEntry = new();
        public Button xkbBrowseButton = new("Browse");

        public KlcLayoutManager lm = new();

        public MainWindow() : base("Xkb Layout Creator")
        {
            Resizable = false;
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

            Menu managemenu = new();
            MenuItem manage = new("Manage");
            manage.Submenu = managemenu;

            MenuItem reset = new("New");
            MenuItem save = new("Save");
            MenuItem saveAs = new("Save As");
            MenuItem load = new("Load");
            MenuItem export = new("Export");
            MenuItem install = new("Install");
            MenuItem delete = new("Delete");
            MenuItem exit = new("Exit");

            reset.Activated += Reset;
            save.Activated += SaveFile;
            saveAs.Activated += SaveFileAs;
            load.Activated += LoadFile;
            export.Activated += ExportFile;
            install.Activated += Install;
            delete.Activated += Delete;
            exit.Activated += ExitMenu;

            filemenu.Append(reset);
            filemenu.Append(save);
            filemenu.Append(saveAs);
            filemenu.Append(load);
            filemenu.Append(exit);

            managemenu.Append(export);
            managemenu.Append(install);
            managemenu.Append(delete);

            mb.Append(file);
            mb.Append(manage);

            CheckButton shiftCheck = new("Shift");
            CheckButton altCheck = new("AltGr");
            shiftCheck.Toggled += OnShiftToggled;
            altCheck.Toggled += OnAltToggled;

            checkboxes.PackStart(shiftCheck, false, false, 0);
            checkboxes.PackStart(altCheck, false, false, 0);

            // XkbPath
            Box xkbBox = new(Orientation.Horizontal, 5);

            xkbPathEntry.Text = Directory.Exists(_xkbPath)
                ? _xkbPath
                : "Xkb directory not found. Provide the path to xkb directory";

            xkbPathEntry.IsEditable = false;
            xkbPathEntry.Sensitive = false;
            xkbPathEntry.CanFocus = false;
            xkbPathEntry.Expand = true;

            xkbBrowseButton.Clicked += OnBrowseXkbPath;

            xkbBox.PackStart(xkbPathEntry, true, true, 5);
            xkbBox.PackStart(xkbBrowseButton, false, false, 5);

            CreateButtonLayout();
            UpdateKeyLabels();

            mainVbox.PackStart(mb, false, false, 0);
            mainVbox.PackStart(rowBox1, false, false, 0);
            mainVbox.PackStart(rowBox2, false, false, 0);
            mainVbox.PackStart(rowBox3, false, false, 0);
            mainVbox.PackStart(rowBox4, false, false, 0);
            mainVbox.PackStart(rowBox5, false, false, 0);
            mainVbox.PackStart(checkboxes, false, false, 0);
            mainVbox.PackEnd(xkbBox, false, false, 5);

            Add(mainVbox);
            ShowAll();
            DeleteEvent += Exit;
        }

        void OnBrowseXkbPath(object sender, EventArgs e)
        {
            using FileChooserDialog dialog = new(
                    "Select xkb symbols folder",
                    this,
                    FileChooserAction.SelectFolder,
                    "Cancel", ResponseType.Cancel,
                    "Select", ResponseType.Accept
                    );

            if (dialog.Run() == (int)ResponseType.Accept)
            {
                xkbPathEntry.Text = dialog.Filename;
                _xkbPath = xkbPathEntry.Text;
                _symbolsPath = _xkbPath + "/symbols";
            }
        }

        private void Exit(object sender, DeleteEventArgs a) => Application.Quit();

        private void ExitMenu(object sender, EventArgs a) => Application.Quit();

        private void Reset(object sender, EventArgs a)
        {
            lm = new KlcLayoutManager();

            _filePath = string.Empty;
            UpdateKeyLabels();
        }

        private void LoadFile(object sender, EventArgs a)
        {
            using FileChooserDialog loadDialog = new("Select a file to load",
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
                lm.LoadLayout(_filePath);
            }

            UpdateKeyLabels();
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
            using FileChooserDialog saveDialog = new("Save file as...",
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
            }
        }

        private void SaveFileAs(object sender, EventArgs a) => SaveFileAs();

        private void ExportFile(object sender, EventArgs a)
        {
            if(_filePath == string.Empty)
            {
                SaveFile();
            }

            if(_filePath == string.Empty)
                return;

            ShowExportDialog();
        }

        private void ShowExportDialog()
        {
            using Dialog exportDialog = new("Export Layout", this, DialogFlags.Modal);
            exportDialog.AddButton("Cancel", ResponseType.Cancel);
            exportDialog.AddButton("OK", ResponseType.Accept);

            Box exportBox = new(Orientation.Vertical, 5);

            Label langLabel = new("Language Code [us, ru, de, cz]:");
            Entry langEntry = new();

            Label variantLabel = new("Your variant's Code [Shavian -> shvn]:");
            Entry variantEntry = new();

            Label layoutDescLabel = new("Short description [English (Shavian)]:");
            Entry layoutDescEntry = new();

            exportBox.PackStart(langLabel, false, false, 0);
            exportBox.PackStart(langEntry, false, false, 5);
            exportBox.PackStart(variantLabel, false, false, 0);
            exportBox.PackStart(variantEntry, false, false, 5);
            exportBox.PackStart(layoutDescLabel, false, false, 0);
            exportBox.PackStart(layoutDescEntry, false, false, 5);

            exportDialog.ContentArea.PackStart(exportBox, true, true, 0);
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
                    if(!IsValidLang(lang))
                    {
                        ShowDialog(MessageType.Warning, "This Language doesn't exist!");
                        continue;
                    }

                    validInput = true;
                    LayoutGenerator lg = new();
                    Layout layout = new(lm.Keys, lang, variantCode, layoutDesc);
                    lg.Generate(layout);

                    ShowDialog(MessageType.Info, "Files generated successfully!\nProceed to README file for further instructions");
                }
                else
                    return;
            }
        }

        bool IsValidLang(string lang)
        {
            if (string.IsNullOrWhiteSpace(lang))
                return false;

            // Collision with Widget.Path
            string path = System.IO.Path.Combine(_symbolsPath, lang);
            Console.WriteLine(path);

            return File.Exists(path);
        }

        void OnShiftToggled(object sender, EventArgs args)
        {
            CheckButton cb = (CheckButton) sender;
            lm.IsShift = cb.Active;
            UpdateKeyLabels();
        }

        void OnAltToggled(object sender, EventArgs args)
        {
            CheckButton cb = (CheckButton) sender;
            lm.IsAlt = cb.Active;
            UpdateKeyLabels();
        }

        private void OnButtonClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            KeyCode keyCode = lm.GetKeyCodeByIndex(int.Parse(button.Name));
            using Dialog inputDialog = new(keyCode.ToString(), this, DialogFlags.Modal);
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
                        lm.SetKey(keyCode, preferredCharacter);
                        validInput = true;
                    }
                    else if ((userInput.StartsWith("U+") || userInput.StartsWith("U")) && userInput.Length > 1)
                    {
                        userInput = userInput.Replace("+", "");

                        string converted = ConvertUnicodeInput(userInput);

                        if (converted != null && converted != "?")
                        {
                            preferredCharacter = converted;
                            lm.SetKey(keyCode, userInput);
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
                        lm.SetKey(keyCode, preferredCharacter);
                        validInput = true;
                    }
                    else
                    {
                        ShowDialog(MessageType.Warning, "Please enter exactly one character or a valid unicode codepoint (U#####).");
                        entry.Text = string.Empty;
                    }
                }
                else
                    return;
            }

            UpdateKeyLabels();
        }

        private void ShowDialog(MessageType type, string text)
        {
            using var dialog = new MessageDialog(this,
                    DialogFlags.Modal,
                    type,
                    ButtonsType.Ok,
                    text);
            dialog.Run();
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
            if(!lm.IsShift && !lm.IsAlt)     return lm.Keys[index].Normal;
            else if(lm.IsShift && !lm.IsAlt) return lm.Keys[index].Shift;
            else if(!lm.IsShift && lm.IsAlt) return lm.Keys[index].Alt;
            else if(lm.IsShift && lm.IsAlt)  return lm.Keys[index].ShiftAlt;
            else                             return "[]";
        }

        private string CreateKlcFile()
        {
            StringBuilder sb = new();

            foreach(var key in lm.Keys)
                sb.AppendLine(key.ToKlcString());

            return sb.ToString();
        }

        private void Install(object sender, EventArgs a)
        {
            ShowInstallDialog();
        }

        void ShowInstallDialog()
        {
            using Dialog installDialog = new("Install Layout", this, DialogFlags.Modal);
            installDialog.AddButton("Cancel", ResponseType.Cancel);
            installDialog.AddButton("OK", ResponseType.Accept);

            Box installBox = new(Orientation.Vertical, 5);

            Label langLabel = new("Language Code [us, ru, de, cz]:");
            Entry langEntry = new();
            Label variantLabel = new("Your Variant's Code [Shavian -> shvn]:");
            Entry variantEntry = new();
            Label descLabel = new("Short description [English (Shavian)]:");
            Entry layoutDescEntry = new();

            installBox.PackStart(langLabel, false, false, 0);
            installBox.PackStart(langEntry, false, false, 5);
            installBox.PackStart(variantLabel, false, false, 0);
            installBox.PackStart(variantEntry, false, false, 5);
            installBox.PackStart(descLabel, false, false, 0);
            installBox.PackStart(layoutDescEntry, false, false, 5);

            installDialog.ContentArea.PackStart(installBox, true, true, 0);
            installDialog.ShowAll();

            bool validInput = false;

            while(!validInput)
            {
                if(installDialog.Run() == (int)ResponseType.Accept)
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

                    if (!IsValidLang(lang))
                    {
                        ShowDialog(MessageType.Warning, "This Language doesn't exist!");
                        continue;
                    }

                    validInput = true;

                    LayoutManager manager = new(_xkbPath);

                    if (manager.IsVariantPresent(lang, variantCode))
                    {
                        using Dialog rewriteDialog = new("Install", this, DialogFlags.Modal);
                        rewriteDialog.AddButton("Cancel", ResponseType.Cancel);
                        rewriteDialog.AddButton("OK", ResponseType.Accept);

                        rewriteDialog.ContentArea.PackStart(
                                new Label("This variant already exists. Do you want to rewrite it?"),
                                true, true, 0);

                        rewriteDialog.ShowAll();

                        if ((ResponseType)rewriteDialog.Run() == ResponseType.Cancel)
                            return;
                        else
                            manager.Delete(lang, variantCode);
                    }

                    Console.WriteLine("Installing...");
                    Layout layout = new(lm.Keys, lang, variantCode, layoutDesc);
                    manager.Install(layout);

                    Console.WriteLine("Finished!");
                    ShowDialog(MessageType.Info, "Layout installed successfully! Logout to apply changes");
                }
                else
                    return;
            }
        }

        public void Delete(object sender, EventArgs a)
        {
            using Dialog deleteDialog = new("Delete", this, DialogFlags.Modal);
            deleteDialog.AddButton("Cancel", ResponseType.Cancel);
            deleteDialog.AddButton("OK", ResponseType.Accept);

            Box deleteBox = new(Orientation.Vertical, 5);

            Label langLabel = new("Language Code [us, ru, de, cz]:");
            Entry langEntry = new();

            Label variantLabel = new("Variant's Code [Shavian -> shvn]:");
            Entry variantEntry = new();

            deleteBox.PackStart(langLabel, false, false, 0);
            deleteBox.PackStart(langEntry, false, false, 5);
            deleteBox.PackStart(variantLabel, false, false, 0);
            deleteBox.PackStart(variantEntry, false, false, 5);

            deleteDialog.ContentArea.PackStart(deleteBox, true, true, 0);
            deleteDialog.ShowAll();

            bool validInput = false;

            while(!validInput)
            {
                if (deleteDialog.Run() == (int)ResponseType.Accept)
                {
                    string lang = langEntry.Text;
                    string variantCode = variantEntry.Text;

                    if (string.IsNullOrWhiteSpace(lang) ||
                            string.IsNullOrWhiteSpace(variantCode))
                    {
                        ShowDialog(MessageType.Warning, "One of the fields is empty!");
                        continue;
                    }
                    if(!IsValidLang(lang))
                    {
                        ShowDialog(MessageType.Warning, "This Language doesn't exist!");
                        continue;
                    }
                    // Check if it's generated by XKBLC
                    string text = File.ReadAllText($"{_symbolsPath}{lang}");
                    if(!text.Contains($"XKBLC {lang} {variantCode}"))
                    {
                        ShowDialog(MessageType.Warning, "This layout either a system layout or does not exist. Can't delete");
                        continue;
                    }

                    validInput = true;

                    LayoutManager manager = new(_xkbPath);
                    manager.Delete(lang, variantCode);
                    ShowDialog(MessageType.Info, "Layout deleted successfully!");
                }
                else
                    return;
            }
        }
    }
}
