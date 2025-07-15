using System.Collections.Generic;
using System.IO;
using System;

class LayoutBuilder
{
    public bool IsShift { get; set; } = false;
    public bool IsAlt { get; set; } = false;

    public List<Key> Keys = new();

    public LayoutBuilder()
    {
        foreach(string[] keyValues in DefaultLayout)
        {
            KeyCode keyCode = GetKeyCode(keyValues[0]);
            Keys.Add(new Key(keyCode, keyValues));
        }
    }

    public void ShiftOff() => IsShift = false;
    public void ShiftOn() => IsShift = true;
    public void AltOff() => IsAlt = false;
    public void AltOn() => IsAlt = true;

    public void SetKey(KeyCode keyCode, string value)
    {
        var index = Keys.FindIndex(x => x.KeyCode == keyCode);

        if(!IsShift && !IsAlt) Keys[index].Normal = value;
        else if(IsShift && !IsAlt) Keys[index].Shift = value;
        else if(!IsShift && IsAlt) Keys[index].Alt = value;
        else if(IsShift && IsAlt) Keys[index].ShiftAlt = value;
    }

    public KeyCode GetKeyCodeByIndex(int index) => Keys[index].KeyCode;

    public KeyCode GetKeyCode(string keyCode) => KeyMap[keyCode];

    public void LoadLayout(string filepath)
    {
        Keys = new List<Key>();
        StreamReader sr = new(filepath);

        int keyCode = 0;

        while(sr.Peek() != -1)
        {
            string[] values = sr.ReadLine()!.Split(" // ");

            Keys.Add(new Key((KeyCode) keyCode, values));
            keyCode++;
        }

        sr.Close();
    }

    public string GetKey(KeyCode keyCode)
    {
        int index = Keys.FindIndex(x => x.KeyCode == keyCode);

        return Keys[index].ToString();
    }

    public string GetKey(string strKeyCode)
    {
        KeyCode keyCode = KeyMap[strKeyCode];

        int index = Keys.FindIndex(x => x.KeyCode == keyCode);

        return Keys[index].ToString();
    }

    public void SaveLayout(string filepath)
    {
        StreamWriter sw = new(filepath);

        foreach(var key in Keys)
            sw.WriteLine(key.ToString());

        sw.Close();
    }

    private static readonly string[][] DefaultLayout =
    {
        new[] { "`", "~" },
        new[] { "1", "!" },
        new[] { "2", "@" },
        new[] { "3", "#" },
        new[] { "4", "$" },
        new[] { "5", "%" },
        new[] { "6", "^" },
        new[] { "7", "&" },
        new[] { "8", "*" },
        new[] { "9", "(" },
        new[] { "0", ")" },
        new[] { "-", "_" },
        new[] { "=", "+" },
        new[] { "q", "Q" },
        new[] { "w", "W" },
        new[] { "e", "E" },
        new[] { "r", "R" },
        new[] { "t", "T" },
        new[] { "y", "Y" },
        new[] { "u", "U" },
        new[] { "i", "I" },
        new[] { "o", "O" },
        new[] { "p", "P" },
        new[] { "[", "{" },
        new[] { "]", "}" },
        new[] { "\\", "|" },
        new[] { "a", "A" },
        new[] { "s", "S" },
        new[] { "d", "D" },
        new[] { "f", "F" },
        new[] { "g", "G" },
        new[] { "h", "H" },
        new[] { "j", "J" },
        new[] { "k", "K" },
        new[] { "l", "L" },
        new[] { ";", ":" },
        new[] { "'", "\"" },
        new[] { "z", "Z" },
        new[] { "x", "X" },
        new[] { "c", "C" },
        new[] { "v", "V" },
        new[] { "b", "B" },
        new[] { "n", "N" },
        new[] { "m", "M" },
        new[] { ",", "<" },
        new[] { ".", ">" },
        new[] { "/", "?" },
        new[] { " ", " " },
    };

    Dictionary<string, KeyCode> KeyMap = new() 
    {
        { "`", KeyCode.KeyBacktick },
        { "1", KeyCode.Key1 },
        { "2", KeyCode.Key2 },
        { "3", KeyCode.Key3 },
        { "4", KeyCode.Key4 },
        { "5", KeyCode.Key5 },
        { "6", KeyCode.Key6 },
        { "7", KeyCode.Key7 },
        { "8", KeyCode.Key8 },
        { "9", KeyCode.Key9 },
        { "0", KeyCode.Key0 },
        { "-", KeyCode.KeyMinus },
        { "=", KeyCode.KeyEquals },
        { "q", KeyCode.KeyQ },
        { "w", KeyCode.KeyW },
        { "e", KeyCode.KeyE },
        { "r", KeyCode.KeyR },
        { "t", KeyCode.KeyT },
        { "y", KeyCode.KeyY },
        { "u", KeyCode.KeyU },
        { "i", KeyCode.KeyI },
        { "o", KeyCode.KeyO },
        { "p", KeyCode.KeyP },
        { "[", KeyCode.KeyLeftBracket },
        { "]", KeyCode.KeyRightBracket },
        { "\\", KeyCode.KeyBackSlash },
        { "a", KeyCode.KeyA },
        { "s", KeyCode.KeyS },
        { "d", KeyCode.KeyD },
        { "f", KeyCode.KeyF },
        { "g", KeyCode.KeyG },
        { "h", KeyCode.KeyH },
        { "j", KeyCode.KeyJ },
        { "k", KeyCode.KeyK },
        { "l", KeyCode.KeyL },
        { ";", KeyCode.KeyColon },
        { "'", KeyCode.KeyQuote },
        { "z", KeyCode.KeyZ },
        { "x", KeyCode.KeyX },
        { "c", KeyCode.KeyC },
        { "v", KeyCode.KeyV },
        { "b", KeyCode.KeyB },
        { "n", KeyCode.KeyN },
        { "m", KeyCode.KeyM },
        { ",", KeyCode.KeyComma },
        { ".", KeyCode.KeyPeriod },
        { "/", KeyCode.KeySlash },
        { " ", KeyCode.KeySpace }
    };
}
