using Godot;
using System;
using System.Collections.Generic;


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

    #region ResourcePaths

    public static Dictionary<string, string> resourcePaths = new Dictionary<string, string>()
    {
        { "fireballThumbnail", "res://Sprites/SpellMenu/Fireball/fire_thumbnail.png" },
        { "fireballPageImage", "res://Sprites/SpellMenu/Fireball/fireball_thingy.png"},
        { "fireballObject", "res://Scenes/Spells/Fireball/fireball.tscn" }
    };

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

    /// <summary>
    /// Using the corresponding name in the dictionary, load a PackedScene from a path
    /// </summary>
    /// <param name="name">Name given to the resource</param>
    /// <returns>The PackedScene requested</returns>
    public static PackedScene GetPackedScene(string name)
    {
        return ResourceLoader.Load<PackedScene>(resourcePaths[name]);
    }

    /// <summary>
    /// Using the corresponding name in the dictionary, load a PackedScene from a path
    /// </summary>
    /// <param name="name">Name given to the resource</param>
    /// <returns>The requested Texture2D</returns>
    public static Texture2D GetTexture2D(string name)
    {
        return ResourceLoader.Load<Texture2D>(resourcePaths[name]);
    }

    /// <summary>
    /// Call this to reload the current scene.
    /// </summary>
    /// <param name="tree"></param>
    public static void ReloadScene(Node tree)
    {

    }
}
