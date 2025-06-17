using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Tracing;

namespace MagicSystem
{
    public static class SpellManager
    {
        public enum SpellName
        {
            None,
            Fireball
        }

        public struct SpellData
        {
            public string path;
            public Type spellType;

            public SpellData(string path, Type spellType)
            {
                this.path = path;
                this.spellType = spellType;
            }
        }

        static Dictionary<SpellName, SpellData> SpellDictionary = new Dictionary<SpellName, SpellData>()
        {
            { SpellName.Fireball, new SpellData("res://Sprites/HUD/Spell_UI/thumbnails/fire_thumbnail.png", typeof(FireballSpell)) }
        };

        public struct EquippedSpell
        {
            public SpellName name;
            public Spell spell;

            public EquippedSpell(SpellName name)
            {
                this.name = name;
                spell = CreateSpellInstance(name);
            }

            public void CastState(double delta)
            {
                if (name == SpellName.None)
                {
                    //as a failsafe, just stun the player if a None spell is cast
                    //this will allow them to return to gameplay
                    Player.player.move.Stun(0.1f);
                    return;
                }

                spell.CastState(delta);
            }
        }

        public static EquippedSpell[] equippedSpells = {
            new EquippedSpell(SpellName.None),
            new EquippedSpell(SpellName.None),
            new EquippedSpell(SpellName.None)
        };

        /// <summary>
        /// Given a slot and a spell name, equip a spell in that slot.
        /// </summary>
        /// <param name="slot"></param>
        /// <param name="spellName"></param>
        public static void SelectSpell(int slot, SpellName spellName)
        {
            equippedSpells[slot] = new EquippedSpell(spellName);
        }

        public static SpellName GetSpellName(int slot)
        {
            return equippedSpells[slot].name;
        }

        public static void CastState(int slot, double delta)
        {
            equippedSpells[slot].CastState(delta);
        }

        public static void CastStart(int slot)
        {
            equippedSpells[slot].spell.Start();
        }

        /// <summary>
        /// Load a spell thumbnail image from disk. This is wildly
        /// inefficient, but it should be fine as long as it's not called
        /// very often. If it's a problem, yell at Kyle to implement some
        /// kinda cache or some shit idk.
        /// </summary>
        /// <param name="spell">The spell name to load.</param>
        /// <returns>Texture2D of the thumbnail.</returns>
        public static Texture2D GetSpellThumbnail(SpellName spell)
        {
            if (spell == SpellName.None)
                return null;

            if (SpellDictionary.ContainsKey(spell))
            {
                return GD.Load<Texture2D>(SpellDictionary[spell].path);
            }

            throw new Exception("Spell " + spell.ToString() + " not found!");
        }

        /// <summary>
        /// Creates an instance of the spell requested.
        /// </summary>
        /// <param name="spell">Name of Spell to instance.</param>
        /// <returns>Instance of Spell</returns>
        /// <exception cref="Exception"></exception>
        public static Spell CreateSpellInstance(SpellName spell)
        {
            if (spell == SpellName.None)
                return null;

            if (SpellDictionary.ContainsKey(spell))
                {
                    return (Spell)Activator.CreateInstance(SpellDictionary[spell].spellType);
                }

            throw new Exception("Spell " + spell.ToString() + " not found!");
        }
    }
}