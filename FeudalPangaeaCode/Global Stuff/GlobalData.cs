using Godot;
using System;

public static class GlobalData
{
    private static int coinCount = 0;
    public const float gravityRate = 9.8f * 2;
    public const float maxGravity = 53;

    /// <summary>
    /// Get the current number of coins the player has.
    /// </summary>
    /// <returns>Integer amount of coins</returns>
    public static int getCoinCount()
    {
        return coinCount;
    }

    /// <summary>
    /// Add or subtract some number of coins, but the number can't go below 0.
    /// </summary>
    /// <param name="mod">Amount to add or subtract coins by</param>
    public static void modCoinCount(int mod){
        coinCount += mod;

        if(coinCount < 0)
            coinCount = 0;
    }
}
