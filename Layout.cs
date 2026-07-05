using System.Collections.Generic;

class Layout
{
    public List<Key> Keys { get; set; }
    public string Lang { get; set; }
    public string Variant { get; set; }
    public string Desc { get; set; }

    public Layout(List<Key> keys, string lang, string variant, string desc)
    {
        Keys = keys;
        Lang = lang;
        Variant = variant;
        Desc = desc;
    }
}
