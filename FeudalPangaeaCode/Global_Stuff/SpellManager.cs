using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Tracing;
using System.Runtime.CompilerServices;

namespace MagicSystem
{
    public static class SpellManager
    {
        //names of every spell in the game
        public enum SpellName
        {
            None,
            Fireball
        }

        //all the information needed to instance and display a spell
        public struct SpellData
        {
            public string name;
            public string description;
            public Type spellType;
            public Texture2D thumbnail;
            public Texture2D pageImage;

            public SpellData(Type spellType, string name, string desc, string thumbnailPath, string pageImagePath)
            {
                this.spellType = spellType;
                this.name = name;
                this.description = desc;

                if(thumbnailPath != "")
                    this.thumbnail = GD.Load<Texture2D>(thumbnailPath);
                if(pageImagePath != "")
                    this.pageImage = GD.Load<Texture2D>(pageImagePath);
            }
        }

        //dictionary of all spells that exist in the game
        static Dictionary<SpellName, SpellData> spellDictionary = new Dictionary<SpellName, SpellData>()
        {
            {SpellName.None, new SpellData(null, "", "", "", "")},

            { SpellName.Fireball, new SpellData(typeof(FireballSpell), "Fireball",
            "Take aim and fire this spell to destroy your targets!",
            GlobalData.resourcePaths["fireballThumbnail"], GlobalData.resourcePaths["fireballPageImage"] ) }
        };

        public static List<SpellName> spellInventory = new List<SpellName>() {  };

        //used to store information about currently equipped spells
        //stores the name of the spell (by the enum) and the current instance
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

        public class SpellChangeEventArgs
        {
            public int slot { get; }
            public SpellName name { get; }

            public SpellChangeEventArgs(int slot, SpellName spellName)
            {
                this.slot = slot;
                this.name = spellName;       
            }
        }

        public static event EventHandler<SpellChangeEventArgs> SpellChange;

        /// <summary>
        /// Removes all subscribers from the SpellChange event.
        /// This should always be done when changing scenes, as we don't want to
        /// accidentally keep track of disposed nodes.
        /// </summary>
        public static void ResetSpellChange() => SpellChange = null;

        /// <summary>
        /// Given a slot and a spell name, equip a spell in that slot.
        /// </summary>
        /// <param name="slot"></param>
        /// <param name="spellName"></param>
        public static void SelectSpell(int slot, SpellName spellName)
        {
            equippedSpells[slot] = new EquippedSpell(spellName);
            SpellChange?.Invoke(equippedSpells[slot], new SpellChangeEventArgs(slot, spellName));

            for (int i = 1; i < 3; i++)
            {
                int otherSlot = (slot + i) % 3;
                if (equippedSpells[otherSlot].name == spellName)
                {
                    equippedSpells[otherSlot] = new EquippedSpell(SpellName.None);
                    SpellChange?.Invoke(equippedSpells[otherSlot], new SpellChangeEventArgs(otherSlot, SpellName.None));
                }
            }
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

        public static SpellData GetSpellData(SpellName spell)
        {
            return spellDictionary[spell];
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

            if (spellDictionary.ContainsKey(spell))
            {
                return (Spell)Activator.CreateInstance(spellDictionary[spell].spellType);
            }

            throw new Exception("Spell " + spell.ToString() + " not found!");
        }

        /// <summary>
        /// Returns an integer from 0 - 2 depending on which 
        /// spell button was pressed. Returns -1 if none
        /// were pressed. Favors smaller slot if multiple were
        /// presssed.
        /// </summary>
        /// <returns>Integer slot 0 - 2</returns>
        public static int GetSpellButton()
        {
            if (Input.IsActionJustPressed("CAST1"))
            {
                return 0;
            }

            if (Input.IsActionJustPressed("CAST2"))
            {
                return 1;
            }

            if (Input.IsActionJustPressed("CAST3"))
            {
                return 2;
            }

            return -1;
        }
    }
}