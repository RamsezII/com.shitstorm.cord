using _ARK_;
using System;

namespace _CORD_
{
    partial class ShitcordMachine
    {
        [Serializable]
        internal class RSettings : ResourcesJSon
        {
            public ulong application_id;
        }

        [Serializable]
        internal class HSettings : HomeJSon
        {
            public bool auto_login;
            public bool rich_presence;
            public string refresh_token;
        }

        internal static RSettings r_settings;
        internal static HSettings h_settings;

        //--------------------------------------------------------------------------------------------------------------

        internal static void ForceLoadSettings(in bool log)
        {
            ResourcesJSon.TryReadResourcesJSon(log, out r_settings);
            StaticJSon.ReadStaticJSon(ref h_settings, true, log);
        }
    }
}