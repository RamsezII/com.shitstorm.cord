using _ARK_;
using _UTIL_;
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

        internal static readonly LazyValue<RSettings> r_settings = new(() =>
        {
            ResourcesJSon.TryReadResourcesJSon(true, out RSettings value);
            return value;
        });

        internal static HSettings h_settings;

        //--------------------------------------------------------------------------------------------------------------

        static void SaveHomeSettings(in bool log)
        {
            h_settings.SaveStaticJSon(log);
        }

        static void LoadHomeSettings(in bool log)
        {
            StaticJSon.ReadStaticJSon(out h_settings, true, log);
        }
    }
}