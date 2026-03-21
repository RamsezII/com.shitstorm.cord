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
            h_settings = null;

            codeVerifier = null;

            client?.Dispose();
            client = null;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void OnBeforeSceneLoad()
        {
            LoadSettings(true);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void OnAfterSceneLoad()
        {
            NUCLEOR.delegates.OnApplicationUnfocus += () => SaveHomeSettings(false);
            NUCLEOR.delegates.OnApplicationFocus += () => LoadSettings(false);
        }
    }
}