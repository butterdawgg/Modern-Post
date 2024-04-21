using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public enum FloatType
{
    MasterVolume = 0,
    SfxVolume = 1,
    MusicVolume = 2,
    HighestStreak = 4
}

public enum BoolType
{
    FirstPlay = 0
}

public class SerializeManager
{
    public static SerializeManager Instance { get; }

    static SerializeManager()
    {
        Instance = new SerializeManager();
    }

    private SerializeManager() { }

    public void SetFloat(FloatType type, float value) 
    { 
        PlayerPrefs.SetFloat(type.ToString(), value);
    }

    public float GetFloat(FloatType type) 
    { 
        if (PlayerPrefs.HasKey(type.ToString())) 
            return PlayerPrefs.GetFloat(type.ToString()); 
        else
            return 1f;
    }

    public void SetBool(BoolType type, bool value) 
    {
        PlayerPrefs.SetInt(type.ToString(), Convert.ToInt32(value)); 
    }

    public bool GetBool(BoolType type) 
    { 
        if (PlayerPrefs.HasKey(type.ToString()))
            return Convert.ToBoolean(PlayerPrefs.GetInt(type.ToString()));
        else 
            return true;
    }
}
