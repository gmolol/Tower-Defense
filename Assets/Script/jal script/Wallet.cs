using System;
using UnityEngine;

// Coins that persist between runs and game launches, spent on unlocks
public static class Wallet
{
    const string Key = "Coins";

    public static event Action Changed; // UI listens to this to update coin text

    public static int Coins => PlayerPrefs.GetInt(Key, 0);

    public static void Add(int amount) => Set(Coins + amount);

    public static bool TrySpend(int amount)
    {
        if (Coins < amount) return false;
        Set(Coins - amount);
        return true;
    }

    public static void Reset() => Set(0);

    static void Set(int value)
    {
        PlayerPrefs.SetInt(Key, value);
        PlayerPrefs.Save(); // write now, in case Unity crashes
        Changed?.Invoke();
    }
}