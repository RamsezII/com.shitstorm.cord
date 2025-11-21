using _ARK_;
using _UTIL_;
using System;
using UnityEngine;

namespace _CORD_
{
    partial class ShitcordMachine
    {
        [Serializable]
        internal class RSettings : ResourcesJSon
        {
#if UNITY_EDITOR
            public bool rich_presence_in_editor;
#endif
            public ulong application_id;
        }

        [Serializable]
        internal class HSettings_infos : HomeJSon
        {
            public bool auto_login;
            public bool rich_presence;
        }

        [Serializable]
        internal class HSettings_codes : HomeJSon
        {
            public string refresh_token;
        }

        internal static readonly LazyValue<RSettings> r_settings = new(() =>
        {
            ResourcesJSon.TryReadResourcesJSon(true, out RSettings value);
            return value;
        });

        internal static readonly LazyValue<HSettings_codes> h_settings_codes = new(() =>
        {
            StaticJSon.ReadStaticJSon(out HSettings_codes value, true, true);
            return value;
        });

        internal static HSettings_infos h_settings_infos;

        //--------------------------------------------------------------------------------------------------------------

#if UNITY_EDITOR
        [UnityEditor.MenuItem(button_prefixe + nameof(OpenRSettings))]
        static void OpenRSettings()
        {
            Application.OpenURL(r_settings.GetValue().GetFilePath());
        }

        [UnityEditor.MenuItem(button_prefixe + nameof(SaveRSettings))]
        static void SaveRSettings()
        {
            r_settings.GetValue().SaveResourcesJSon();
        }

        [UnityEditor.MenuItem(button_prefixe + nameof(SaveHomeSettings))]
        static void SaveHomeSettings() => SaveHomeSettings(true);
#endif
        static void SaveHomeSettings(in bool log)
        {
            h_settings_infos.SaveStaticJSon(log);
        }

#if UNITY_EDITOR
        [UnityEditor.MenuItem(button_prefixe + nameof(LoadHomeSettings))]
        static void LoadHomeSettings() => LoadHomeSettings(true);
#endif
        static void LoadHomeSettings(in bool log)
        {
            StaticJSon.ReadStaticJSon(out h_settings_infos, true, log);
        }
    }
}