using _ARK_;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using UnityEngine;

namespace _CORD_
{
    partial class ShitcordMachine
    {
        [Serializable]
        public class HSettings_infos : HomeJSon
        {
            public bool auto_login;
            public bool rich_presence;
            public string refresh_token;
        }

        public static HSettings_infos h_settings;

        static string GetSaveFName() => typeof(ShitcordMachine).GetJSonFileName();
#if UNITY_EDITOR
        static string GetSaveFPath() => Path.Combine(ArkPaths.dpath_ignore_resources, GetSaveFName());
        public static bool rich_presence_in_editor;
#endif
        public static ulong application_id;

        //--------------------------------------------------------------------------------------------------------------

#if UNITY_EDITOR
        [UnityEditor.MenuItem(button_prefixe + nameof(OpenRSettings))]
        static void OpenRSettings()
        {
            SaveRSettings();
            string fpath = GetSaveFPath();
            Application.OpenURL(fpath);
        }

        [UnityEditor.MenuItem(button_prefixe + nameof(SaveRSettings))]
        static void SaveRSettings()
        {
            string fpath = GetSaveFPath();
            var jobj = new JObject()
            {
                [nameof(rich_presence_in_editor)] = rich_presence_in_editor,
                [nameof(application_id)] = application_id,
            };
            jobj.NJSave(fpath);
            UnityEditor.AssetDatabase.Refresh();
        }
#endif
        static void SaveHomeSettings(in bool log)
        {
            h_settings.SaveStaticJSon(log);
        }

        static void LoadSettings(in bool log)
        {
            string rname = GetSaveFName()[..^4];
            var tasset = Resources.Load<TextAsset>(rname);

            if (tasset == null)
                Debug.LogWarning($"{typeof(ShitcordMachine)} config file ({rname}) not found in resources.");
            else
            {
                var jobj = JsonConvert.DeserializeObject<JObject>(tasset.text);
#if UNITY_EDITOR
                rich_presence_in_editor = jobj.Value<bool>(nameof(rich_presence_in_editor));
#endif
                application_id = jobj.Value<ulong>(nameof(application_id));
            }

            StaticJSon.ReadStaticJSon(out h_settings, true, log);
        }
    }
}