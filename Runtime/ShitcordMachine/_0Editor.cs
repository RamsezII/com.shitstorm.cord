//#if UNITY_EDITOR
//using UnityEditor;
//using UnityEditor.SceneManagement;
//using UnityEngine;

//namespace _CORD_
//{
//    [InitializeOnLoad]
//    partial class ShitcordMachine
//    {
//        static ShitcordMachine()
//        {
//            Debug.Log($"{typeof(ShitcordMachine)}.CONSTRUCTOR");

//            RSettings r_settings = ShitcordMachine.r_settings.GetValue(true);
//            r_settings.rich_presence_in_editor = true;

//            LoadHomeSettings(true);

//            if (!r_settings.rich_presence_in_editor)
//                return;

//            AssemblyReloadEvents.afterAssemblyReload += () =>
//            {
//                StartClient();
//                TryLogin();
//            };

//            EditorSceneManager.activeSceneChangedInEditMode += (arg0, arg1) =>
//            {
//                TryUpdateRichPresence(arg1);
//            };

//            EditorApplication.quitting += StopClient;
//        }
//    }
//}
//#endif