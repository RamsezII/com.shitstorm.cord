using _ARK_;
using UnityEngine;

namespace _CORD_
{
#if UNITY_EDITOR
    [UnityEditor.InitializeOnLoad]
#endif
    public static partial class ShitcordMachine
    {

        //--------------------------------------------------------------------------------------------------------------

        static ShitcordMachine()
        {
        }

        //--------------------------------------------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ResetStatics()
        {
            h_settings = null;

            codeVerifier = null;

            logCallback = null;
            client_status.Reset();

            client?.Dispose();
            client = null;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void OnBeforeSceneLoad()
        {
            r_settings.GetValue(true);
            LoadHomeSettings(true);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void OnAfterSceneLoad()
        {
            NUCLEOR.delegates.OnApplicationUnfocus += () => SaveHomeSettings(false);
            NUCLEOR.delegates.OnApplicationFocus += () => LoadHomeSettings(false);

#if UNITY_EDITOR
            NUCLEOR.delegates.OnApplicationFocus += () => r_settings.GetValue(true);
            NUCLEOR.delegates.OnEditorQuit += StopClient;
#endif
        }
    }
}