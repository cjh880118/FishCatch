using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IO_PowerMsg : Message
{
    public bool ON = false;

    public IO_PowerMsg(bool on)
    {
        ON = on;
    }
}

public class IO_DamageMsg : Message
{
    public int Index = 0;
    public bool ON = false;

    public IO_DamageMsg(int index, bool on)
    {
        Index = index;
        ON = on;
    }
}

public class DamageMsg : Message
{
    public int Index = 0;
    public int BlowStrength = 0;

    public DamageMsg(int index, int blowStrength)
    {
        Index = index;
        BlowStrength = blowStrength;
    }
}

public class IO_LedMsg : Message
{
    public int Index = 0;
    public bool ON = false;

    public IO_LedMsg(int index, bool on)
    {
        Index = index;
        ON = on;
    }
}


public class IOStateMsg : Message
{
    public string Desc = "";

    public IOStateMsg(string desc)
    {
        Desc = desc;
    }
}


