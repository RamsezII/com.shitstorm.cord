#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace _CORD_
{
    [InitializeOnLoad]
    partial class ShitcordMachine
    {
        static ShitcordMachine()
        {
            Debug.Log($"{typeof(ShitcordMachine)}.CONSTRUCTOR");

            bool ready = false;
            if (!ready)
                return;

            AssemblyReloadEvents.afterAssemblyReload += () =>
            {
                Debug.Log($"{typeof(ShitcordMachine)}.RELOAD_ASSEMBLY");
                StartClient();
            };

            EditorSceneManager.activeSceneChangedInEditMode += (scene0, scene1) =>
            {
                Debug.Log($"{typeof(ShitcordMachine)}.CHANGED_SCENE {{ {nameof(scene0)}: {scene0}, {nameof(scene1)}: {scene1} }}");
                TryUpdateRichPresence(scene1);
            };

            EditorApplication.quitting += () =>
            {
                Debug.Log($"{typeof(ShitcordMachine)}.QUIT_EDITOR");
                StopClient();
            };
        }
    }
}
#endif