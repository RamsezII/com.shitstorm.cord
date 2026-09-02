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

#if UNITY_EDITOR
        static string GetSaveFPath() => Path.Combine(NUCLEOR.DFResources.FullName, typeof(ShitcordMachine).GetJSonFileName());
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
            string rname = typeof(ShitcordMachine).GetJSonFileName_noTXT();
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
                if (application_id == 0)
#if UNITY_EDITOR
                    if (Application.isEditor)
                        Debug.LogWarning($"{typeof(ShitcordMachine)} application_id is 0, please set it in the config file. ({GetSaveFPath()})");
                    else
#endif
                        Debug.LogWarning($"{typeof(ShitcordMachine)} application_id is 0, please set it in the config file.");
            }

            StaticJSon.ReadStaticJSon(out h_settings, true, log);
        }
    }
}