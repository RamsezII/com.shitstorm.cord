using _ARK_;
using System;
using UnityEngine;

namespace _CORD_
{
    internal static partial class ShitcordMachine
    {

        //--------------------------------------------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ResetStatics()
        {
            r_settings = null;
            h_settings = null;

            codeVerifier = null;

            logCallback = null;
            client_status.Reset();

            richp_start_tstamp = (ulong)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            client?.Dispose();
            client = null;

            ForceLoadSettings(true);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void OnAfterSceneLoad()
        {
            NUCLEOR.delegates.OnApplicationFocus += () => ForceLoadSettings(false);
#if UNITY_EDITOR
            NUCLEOR.delegates.OnEditorQuit += StopClient;
            LoadTimestamp();
#endif
        }
    }
}