using System.Collections.Generic;
using System.IO;

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

        if(!IsShift && !IsAlt)
        {
            Keys[index].Normal = value;
        }
        else if(IsShift && !IsAlt)
        {
            Keys[index].Shift = value;
        }
        else if(!IsShift && IsAlt)
        {
            Keys[index].Alt = value;
        }
        else if(IsShift && IsAlt)
        {
            Keys[index].ShiftAlt = value;
        }
    }

    public void SetKey(string strKeyCode, string value)
    {
        KeyCode keyCode = KeyMap[strKeyCode];

        var index = Keys.FindIndex(x => x.KeyCode == keyCode);

        if(!IsShift && !IsAlt)
        {
            Keys[index].Normal = value;
        }
    }

    public KeyCode GetKeyCode(string keyCode)
    {
        return KeyMap[keyCode];
    }

    public void LoadLayout(string filepath)
    {
        StreamReader sr = new(filepath);

        int keyCode = 0;

        while(sr.Peek() != -1)
        {
            string[] values = sr.ReadLine()!.Split(' ');

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
        {
            sw.WriteLine(key.ToString());
        }

        sw.Close();
    }

    private static readonly string[][] DefaultLayout = new string[][]
    {
        new string[] { "`", "~" },
        new string[] { "1", "!" },
        new string[] { "2", "@" },
        new string[] { "3", "#" },
        new string[] { "4", "$" },
        new string[] { "5", "%" },
        new string[] { "6", "^" },
        new string[] { "7", "&" },
        new string[] { "8", "*" },
        new string[] { "9", "(" },
        new string[] { "0", ")" },
        new string[] { "-", "_" },
        new string[] { "=", "+" },
        new string[] { "q", "Q" },
        new string[] { "w", "W" },
        new string[] { "e", "E" },
        new string[] { "r", "R" },
        new string[] { "t", "T" },
        new string[] { "y", "Y" },
        new string[] { "u", "U" },
        new string[] { "i", "I" },
        new string[] { "o", "O" },
        new string[] { "p", "P" },
        new string[] { "[", "{" },
        new string[] { "]", "}" },
        new string[] { "\\", "|" },
        new string[] { "a", "A" },
        new string[] { "s", "S" },
        new string[] { "d", "D" },
        new string[] { "f", "F" },
        new string[] { "g", "G" },
        new string[] { "h", "H" },
        new string[] { "j", "J" },
        new string[] { "k", "K" },
        new string[] { "l", "L" },
        new string[] { ";", ":" },
        new string[] { "'", "\"" },
        new string[] { "z", "Z" },
        new string[] { "x", "X" },
        new string[] { "c", "C" },
        new string[] { "v", "V" },
        new string[] { "b", "B" },
        new string[] { "n", "N" },
        new string[] { "m", "M" },
        new string[] { ",", "<" },
        new string[] { ".", ">" },
        new string[] { "/", "?" },
    };

    Dictionary<string, KeyCode> KeyMap = new Dictionary<string, KeyCode>
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
        { "/", KeyCode.KeySlash }
    };
}
