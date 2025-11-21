using _ARK_;
using Discord.Sdk;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _CORD_
{
    partial class ShitcordMachine
    {
#if UNITY_EDITOR
        [UnityEditor.MenuItem("Assets/" + nameof(_CORD_) + "/" + nameof(TryUpdateRichPresence))]
#endif
        public static void TryUpdateRichPresence() => TryUpdateRichPresence(SceneManager.GetActiveScene());
        public static void TryUpdateRichPresence(in Scene scene)
        {
            if (client == null || client_status.Value.value != Client.Status.Ready)
            {
                Debug.LogWarning($"tried updating Shitcord presence with no client ready.");
                return;
            }

            ForceLoadSettings(true);

            Activity activity = new();

            activity.SetApplicationId(r_settings.application_id);
            activity.SetType(ActivityTypes.Playing);
            activity.SetDetails($"{(Application.isEditor ? "[E] " : string.Empty)}{scene.name}");
            activity.SetState($"net.v{_RUDP_.RudpSocket.version.GetValue().VERSION}");

            bool use_party = false;
            if (use_party)
            {
                ActivityParty party = new();
                party.SetCurrentSize(NUCLEOR.instance.party_count._value);
                party.SetMaxSize(byte.MaxValue - 1);
                activity.SetParty(party);
            }

            ActivityTimestamps timestamps = new();
            timestamps.SetStart((ulong)NUCLEOR.timestamp_appstart.ToUnixTimeMilliseconds());
            activity.SetTimestamps(timestamps);

            client.UpdateRichPresence(activity, result =>
            {
                if (result.Successful())
                    Debug.Log($"Shitcord rich presence updated".ToSubLog());
                else
                    Debug.LogWarning($"Failed to update Shitcord rich presence: \"{result.Error()}\"");
            });
        }
    }
}