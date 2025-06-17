using Godot;
using System;


public static class GlobalData
{
    private static int coinCount = 0;

    #region Constants
    public const float gravityRate = 9.8f * 2;
    public const float maxGravity = 53;
    public const int hurtboxLayer = 16;
    public const int hitboxLayer = 32;
    public const float parryStunTime = 5;
    #endregion

    /// <summary>
    /// Get the current number of coins the player has.
    /// </summary>
    /// <returns>Integer amount of coins</returns>
    public static int GetCoinCount()
    {
        return coinCount;
    }

    /// <summary>
    /// Add or subtract some number of coins, but the number can't go below 0.
    /// </summary>
    /// <param name="mod">Amount to add or subtract coins by</param>
    public static void ModCoinCount(int mod)
    {
        coinCount += mod;

        if (coinCount < 0)
            coinCount = 0;
    }

    
}
