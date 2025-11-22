using _ARK_;
using UnityEngine;

namespace _CORD_
{
    public static partial class ShitcordMachine
    {

        //--------------------------------------------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ResetStatics()
        {
            h_settings_infos = null;
            h_settings_codes.Reset();

            codeVerifier = null;

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
#endif
        }
    }
}