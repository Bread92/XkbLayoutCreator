using System.Collections.Generic;
using System.IO;

class LayoutBuilder
{
    public bool IsShift { get; set; } = false;
    public bool IsAlt { get; set; } = false;

    public List<Key> Keys = new();

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
        { "Q", KeyCode.KeyQ },
        { "W", KeyCode.KeyW },
        { "E", KeyCode.KeyE },
        { "R", KeyCode.KeyR },
        { "T", KeyCode.KeyT },
        { "Y", KeyCode.KeyY },
        { "U", KeyCode.KeyU },
        { "I", KeyCode.KeyI },
        { "O", KeyCode.KeyO },
        { "P", KeyCode.KeyP },
        { "[", KeyCode.KeyLeftBracket },
        { "]", KeyCode.KeyRightBracket },
        { "\\", KeyCode.KeyBackSlash },
        { "A", KeyCode.KeyA },
        { "S", KeyCode.KeyS },
        { "D", KeyCode.KeyD },
        { "F", KeyCode.KeyF },
        { "G", KeyCode.KeyG },
        { "H", KeyCode.KeyH },
        { "J", KeyCode.KeyJ },
        { "K", KeyCode.KeyK },
        { "L", KeyCode.KeyL },
        { ";", KeyCode.KeyColon },
        { "'", KeyCode.KeyQuote },
        { "Z", KeyCode.KeyZ },
        { "X", KeyCode.KeyX },
        { "C", KeyCode.KeyC },
        { "V", KeyCode.KeyV },
        { "B", KeyCode.KeyB },
        { "N", KeyCode.KeyN },
        { "M", KeyCode.KeyM },
        { ",", KeyCode.KeyComma },
        { ".", KeyCode.KeyPeriod },
        { "/", KeyCode.KeySlash }
    };

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
        StreamReader sr = new("layout.txt");

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
}
