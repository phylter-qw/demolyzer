using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Demolyzer.Model
{
    public enum DemoDeltaType
    {
        PlayerMovement = 0,
        Damage,
        Health,
        Kill, //do kill before death so we can record who kills quad
        Death,
        Score,
        //DroppedPack,
        WeaponPickup,
        StatChanged,
        ArmorAcquired,
        MatchStarted,
        ActiveWeaponChanged,
        PowerupPickup,
        UserInfo,
        SetInfo,
        Suicide,
        MatchComplete,
        PlayerPing,
        PlayerPL,
        ServerInfo,
        PlayerEndGameStats,
    }
}
