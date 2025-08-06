using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Demolyzer.Model
{
    public enum CommandIds : byte
    {
        svc_bad = 0,
        svc_nop = 1,
        svc_disconnect = 2,
        svc_updatestat = 3,	// [byte] [byte]
        nq_svc_version = 4,	// [long] server version
        svc_setview = 5,	// [short] entity number
        svc_sound = 6,	// <see code>
        nq_svc_time = 7,	// [float] server time
        svc_print = 8,	// [byte] id [string] null terminated string
        svc_stufftext = 9,	// [string] stuffed into client's console buffer
        // the string should be \n terminated
        svc_setangle = 10,	// [angle3] set the view angle to this absolute value

        svc_serverdata = 11,	// [long] protocol ...
        svc_lightstyle = 12,	// [byte] [string]
        nq_svc_updatename = 13,	// [byte] [string]
        svc_updatefrags = 14,	// [byte] [short]
        nq_svc_clientdata = 15,	// <shortbits + data>
        svc_stopsound = 16,	// <see code>
        nq_svc_updatecolors = 17,	// [byte] [byte] [byte]
        nq_svc_particle = 18,	// [vec3] <variable>
        svc_damage = 19,

        svc_spawnstatic = 20,
        //	svc_spawnbinary				21
        svc_spawnbaseline = 22,

        svc_temp_entity = 23,	// variable
        svc_setpause = 24,	// [byte] on / off
        nq_svc_signonnum = 25,	// [byte]  used for the signon sequence

        svc_centerprint = 26,	// [string] to put in center of the screen

        svc_killedmonster = 27,
        svc_foundsecret = 28,

        svc_spawnstaticsound = 29,	// [coord3] [byte] samp [byte] vol [byte] aten

        svc_intermission = 30,		// [vec3_t] origin [vec3_t] angle
        svc_finale = 31,		// [string] text

        svc_cdtrack = 32,		// [byte] track
        svc_sellscreen = 33,

        nq_svc_cutscene = 34,		// same as svc_smallkick

        svc_smallkick = 34,		// set client punchangle to 2
        svc_bigkick = 35,		// set client punchangle to 4

        svc_updateping = 36,		// [byte] [short]
        svc_updateentertime = 37,		// [byte] [float]

        svc_updatestatlong = 38,		// [byte] [long]

        svc_muzzleflash = 39,		// [short] entity

        svc_updateuserinfo = 40,		// [byte] slot [long] uid
        // [string] userinfo

        svc_download = 41,		// [short] size [size bytes]
        svc_playerinfo = 42,		// variable
        svc_nails = 43,		// [byte] num [48 bits] xyzpy 12 12 12 4 8 
        svc_chokecount = 44,		// [byte] packets choked
        svc_modellist = 45,		// [strings]
        svc_soundlist = 46,		// [strings]
        svc_packetentities = 47,		// [...]
        svc_deltapacketentities = 48,		// [...]
        svc_maxspeed = 49,		// maxspeed change, for prediction
        svc_entgravity = 50,		// gravity change, for prediction
        svc_setinfo = 51,		// setinfo on a client
        svc_serverinfo = 52,		// serverinfo
        svc_updatepl = 53,		// [byte] [byte]
        svc_nails2 = 54,
        svc_qizmovoice = 83,

        svc_spawnstatic2 = 21,
        svc_modellistshort = 60,
        svc_spawnbaseline2 = 66,
    }
}
