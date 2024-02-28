using System;
using System.Collections.Generic;
using UnityEngine;

public enum Effects
{
    Wrath,
    PoisonCloud,
}

public class EffectManager : MonoBehaviour
{

    public static EffectManager S;
    
    // Factory Method Pattern
    Dictionary<byte, Func<Effect>> Data = new();

    private void Awake()
    {
        if (S == null)
        {
            S = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        Data.Add((byte)Effects.Wrath, () => new EffectWrath(Effects.Wrath));
        Data.Add((byte)Effects.PoisonCloud, () => new EffectPoisonCloud(Effects.PoisonCloud));
        //Data.Add((byte)Effects.HOT, () => new EffectHOT());
        
    }

    public Effect Get(Effects index)
    {
        return Data[(byte)index]();
    }
        
}
