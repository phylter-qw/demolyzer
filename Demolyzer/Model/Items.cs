using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Demolyzer.Model
{
    public enum Stat : uint
    {
        STAT_HEALTH = 0,
        //define STAT_FRAGS			1
        STAT_WEAPON = 2,
        STAT_AMMO = 3,
        STAT_ARMOR = 4,
        STAT_WEAPONFRAME = 5,
        STAT_SHELLS = 6,
        STAT_NAILS = 7,
        STAT_ROCKETS = 8,
        STAT_CELLS = 9,
        STAT_ACTIVEWEAPON = 10,
        STAT_TOTALSECRETS = 11,
        STAT_TOTALMONSTERS = 12,
        STAT_SECRETS = 13,// bumped on client side by svc_foundsecret
        STAT_MONSTERS = 14,// bumped by svc_killedmonster
        STAT_ITEMS = 15,
        STAT_VIEWHEIGHT = 16,	// Z_EXT_VIEWHEIGHT protocol extension
        STAT_TIME = 17,	// Z_EXT_TIME extension
    };

    // Item flags.
    [Flags]
    public enum StatValue
    {
        IT_SHOTGUN = 1,
        IT_SUPER_SHOTGUN = 2,
        IT_NAILGUN = 4,
        IT_SUPER_NAILGUN = 8,
        IT_GRENADE_LAUNCHER = 16,
        IT_ROCKET_LAUNCHER = 32,
        IT_LIGHTNING = 64,
        IT_SUPER_LIGHTNING = 128,

        IT_SHELLS = 256,
        IT_NAILS = 512,
        IT_ROCKETS = 1024,
        IT_CELLS = 2048,

        IT_AXE = 4096,

        IT_ARMOR1 = 8192,
        IT_ARMOR2 = 16384,
        IT_ARMOR3 = 32768,

        IT_SUPERHEALTH = 65536,

        IT_KEY1 = 131072,
        IT_KEY2 = 262144,

        IT_INVISIBILITY = 524288,

        IT_INVULNERABILITY = 1048576,
        IT_SUIT = 2097152,
        IT_QUAD = 4194304,
    };

    [Flags]
    public enum Weapon
    {
        None = 0,
        IT_SHOTGUN = 1,
        IT_SUPER_SHOTGUN = 2,
        IT_NAILGUN = 4,
        IT_SUPER_NAILGUN = 8,
        IT_GRENADE_LAUNCHER = 16,
        IT_ROCKET_LAUNCHER = 32,
        IT_LIGHTNING = 64,
        IT_SUPER_LIGHTNING = 128,
        IT_AXE = 4096,
        All = IT_SHOTGUN | IT_SUPER_SHOTGUN | IT_NAILGUN | IT_SUPER_NAILGUN | IT_GRENADE_LAUNCHER | IT_ROCKET_LAUNCHER | IT_LIGHTNING | IT_SUPER_LIGHTNING | IT_AXE,
    }

    [Flags]
    public enum Armor
    {
        None = 0,
        IT_ARMOR1 = 8192,
        IT_ARMOR2 = 16384,
        IT_ARMOR3 = 32768,
        All = IT_ARMOR1 | IT_ARMOR2 | IT_ARMOR3,
    }

    [Flags]
    public enum Powerup
    {
        None = 0,
        IT_INVISIBILITY = 524288,
        IT_INVULNERABILITY = 1048576,
        IT_SUIT = 2097152,
        IT_QUAD = 4194304,
        All = IT_INVISIBILITY | IT_INVULNERABILITY | IT_QUAD | IT_SUIT,
    }
}
