class Key
{
    public KeyCode KeyCode  { get; set; }
    public string  Normal   { get; set; } = string.Empty;
    public string  Shift    { get; set; } = string.Empty;
    public string  Alt      { get; set; } = string.Empty;
    public string  ShiftAlt { get; set; } = string.Empty;

    public Key(KeyCode keyCode, string[] values)
    {
        KeyCode = keyCode;

        if(values.Length == 1)
        {
            Normal   = values[0];
            Shift    = "None";
            Alt      = "None";
            ShiftAlt = "None";
        }
        else if(values.Length == 2)
        {
            Normal   = values[0];
            Shift    = values[1];
            Alt      = "None";
            ShiftAlt = "None";
        }
        else if(values.Length == 3)
        {
            Normal   = values[0];
            Shift    = values[1];
            Alt      = values[2];
            ShiftAlt = "None";
        }
        else if(values.Length == 4)
        {
            Normal   = values[0];
            Shift    = values[1];
            Alt      = values[2];
            ShiftAlt = values[3];
        }
    }

    public override string ToString()
    {
        return $"{Normal} {Shift} {Alt} {ShiftAlt}";
    }
}

public enum KeyCode
{
    KeyBacktick,		// `
    Key1,      			// 1
    Key2,      			// 2
    Key3,      			// 3
    Key4,      			// 4
    Key5,      			// 5
    Key6,      			// 6
    Key7,      			// 7
    Key8,      			// 8
    Key9,      			// 9
    Key0,      			// 0
    KeyMinus,  			// -
    KeyEquals, 			// =
    KeyQ,      			// Q
    KeyW,      			// W
    KeyE,      			// E
    KeyR,      			// R
    KeyT,      			// T
    KeyY,      			// Y
    KeyU,      			// U
    KeyI,      			// I
    KeyO,      			// O
    KeyP,      			// P
    KeyLeftBracket,     // [
    KeyRightBracket,    // ]
    KeyBackSlash,       // \
    KeyA,      			// A
    KeyS,      			// S
    KeyD,      			// D
    KeyF,      			// F
    KeyG,      			// G
    KeyH,      			// H
    KeyJ,      			// J
    KeyK,      			// K
    KeyL,      			// L
    KeyColon,  			// :
    KeyQuote,  			// "
    KeyZ,      			// Z
    KeyX,      			// X
    KeyC,      			// C
    KeyV,      			// V
    KeyB,      			// B
    KeyN,      			// N
    KeyM,      			// M
    KeyPeriod, 			// .
    KeyComma,  			// ,
    KeySlash,  			// /
    KeySpace,  			// Space
}
