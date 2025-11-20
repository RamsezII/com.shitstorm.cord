using _ARK_;
using System;
using UnityEngine;

namespace _CORD_
{
    internal static partial class ShitcordMachine
    {
        [Serializable]
        internal class RSettings : ResourcesJSon
        {
            // 1029411129065222204
            public ulong application_id;
        }

        [Serializable]
        internal class HSettings : HomeJSon
        {
            public bool auto_login;
            public bool rich_presence;
        }

        internal static RSettings r_settings;
        internal static HSettings h_settings;

        //--------------------------------------------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ResetStatics()
        {
            r_settings = null;
            h_settings = null;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void OnBeforeSceneLoad()
        {
            ForceLoadSettings(true);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void OnAfterSceneLoad()
        {
            NUCLEOR.delegates.OnApplicationFocus += () => ForceLoadSettings(false);
        }

        //--------------------------------------------------------------------------------------------------------------

        internal static ulong GetOrLoadApplicationID()
        {
            if (r_settings == null || r_settings.application_id == 0)
                ForceLoadSettings(true);
            return r_settings.application_id;
        }

        static void ForceLoadSettings(in bool log)
        {
            ResourcesJSon.TryReadResourcesJSon(log, out r_settings);
            StaticJSon.ReadStaticJSon(ref h_settings, true, log);
        }
    }
}