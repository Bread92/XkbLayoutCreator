using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System;

class LayoutGenerator
{
    public void Generate(List<Key> keys, string lang, string variantName, string shortDesc)
    {
        GenerateXkb(keys, lang, variantName);
        GenerateXml(lang, variantName, shortDesc);
    }

    public void GenerateXkb(List<Key> keys, string lang, string variantName)
    {
        bool altGr = false;
        string fileName = $"{lang}_{variantName}.xkb";

        StreamWriter sw = new(fileName);

        sw.WriteLine("default partial alphanumeric_keys modifier_keys");
        sw.WriteLine($"xkb_symbols \"{lang}_{variantName}\" {{");
        sw.WriteLine($"  Name[Group1] = \"{lang}\";");

        foreach(var key in keys)
        {
            if(key.Alt != "NoSymbol" || key.ShiftAlt != "NoSymbol")
                altGr = true;

            string strKey = XkbKeys[key.KeyCode];
            string utf32str = string.Empty;
            sw.Write($"  key <{strKey}> {{ [ ");

            List<string> strKeys = new()
            {
                key.Normal,
                key.Shift,
                key.Alt,
                key.ShiftAlt
            };

            while(strKeys[^1] == "NoSymbol")
                strKeys.RemoveAt(strKeys.Count - 1);

            for(int i = 0; i < strKeys.Count; i++)
            {
                if(strKeys[i] != "NoSymbol")
                {
                    if(strKeys[i].Length == 1)
                    {
                        strKeys[i] = GetUtf32String(strKeys[i]);
                    }
                }
            }

            sw.Write(string.Join(", ", strKeys.ToArray()));
            sw.Write(" ] };");
            sw.WriteLine($" // {key.ToString()}\t{key.KeyCode}");
        }

        if(altGr)
            sw.WriteLine("\n  include \"level3(ralt_switch)\"");

        sw.WriteLine("};");

        sw.Close();

        Console.WriteLine($"Generated file {fileName}"); 
    }

    public void GenerateXml(string lang, string variantName, string shortDesc)
    {
        string fileName = $"{lang}_{variantName}.xml";

        StreamWriter sw = new(fileName);

        sw.WriteLine("<variant>");
        sw.WriteLine("  <configItem>");
        sw.WriteLine($"    <name>{lang}_{variantName}</name>");
        sw.WriteLine($"    <description>{shortDesc}</description>");
        sw.WriteLine("  </configItem>");
        sw.WriteLine("</variant>");

        sw.Close();

        Console.WriteLine($"Generated file {fileName}"); 
    }

    private Dictionary<KeyCode, string> XkbKeys = new()
    {
        { KeyCode.KeyBacktick,     "TLDE" },
        { KeyCode.Key1,            "AE01" },
        { KeyCode.Key2,            "AE02" },
        { KeyCode.Key3,            "AE03" },
        { KeyCode.Key4,            "AE04" },
        { KeyCode.Key5,            "AE05" },
        { KeyCode.Key6,            "AE06" },
        { KeyCode.Key7,            "AE07" },
        { KeyCode.Key8,            "AE08" },
        { KeyCode.Key9,            "AE09" },
        { KeyCode.Key0,            "AE10" },
        { KeyCode.KeyMinus,        "AE11" },
        { KeyCode.KeyEquals,       "AE12" },

        { KeyCode.KeyQ,            "AD01" },
        { KeyCode.KeyW,            "AD02" },
        { KeyCode.KeyE,            "AD03" },
        { KeyCode.KeyR,            "AD04" },
        { KeyCode.KeyT,            "AD05" },
        { KeyCode.KeyY,            "AD06" },
        { KeyCode.KeyU,            "AD07" },
        { KeyCode.KeyI,            "AD08" },
        { KeyCode.KeyO,            "AD09" },
        { KeyCode.KeyP,            "AD10" },
        { KeyCode.KeyLeftBracket,  "AD11" },
        { KeyCode.KeyRightBracket, "AD12" },

        { KeyCode.KeyBackSlash,    "BKSL" },
        { KeyCode.KeyA,            "AC01" },
        { KeyCode.KeyS,            "AC02" },
        { KeyCode.KeyD,            "AC03" },
        { KeyCode.KeyF,            "AC04" },
        { KeyCode.KeyG,            "AC05" },
        { KeyCode.KeyH,            "AC06" },
        { KeyCode.KeyJ,            "AC07" },
        { KeyCode.KeyK,            "AC08" },
        { KeyCode.KeyL,            "AC09" },
        { KeyCode.KeyColon,        "AC10" },
        { KeyCode.KeyQuote,        "AC11" },

        { KeyCode.KeyZ,            "AB01" },
        { KeyCode.KeyX,            "AB02" },
        { KeyCode.KeyC,            "AB03" },
        { KeyCode.KeyV,            "AB04" },
        { KeyCode.KeyB,            "AB05" },
        { KeyCode.KeyN,            "AB06" },
        { KeyCode.KeyM,            "AB07" },
        { KeyCode.KeyComma,        "AB08" },
        { KeyCode.KeyPeriod,       "AB09" },
        { KeyCode.KeySlash,        "AB10" },
        { KeyCode.KeySpace,        "SPCE" }
    };

    private string GetUtf32String(string str)
    {
        StringInfo stringInfo = new(str);
        int utf = 0;

        for (int i = 0; i < stringInfo.LengthInTextElements; i++)
        {
            string textElement = stringInfo.SubstringByTextElements(i, 1);
            utf = char.ConvertToUtf32(textElement, 0);
        }

        return $"U{utf:X4}";
    }
}

