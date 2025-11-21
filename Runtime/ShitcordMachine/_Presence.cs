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
            string log_prefixe = $"{typeof(ShitcordMachine)}.TRY_UPDATE";

            if (client == null)
            {
                Debug.LogError($"{log_prefixe} null {nameof(client)} (\"{client}\")");
                return;
            }

            if (client_status.Value.value != Client.Status.Ready)
                Debug.LogWarning($"{log_prefixe} {nameof(client)} is not {Client.Status.Ready} ({client_status._value}).");

            RSettings r_settings = ShitcordMachine.r_settings.GetValue();

            if (r_settings.application_id == 0)
            {
                Debug.LogError($"{log_prefixe} {nameof(r_settings.application_id)}: {r_settings.application_id}");
                return;
            }

            Activity activity = new();

            activity.SetApplicationId(r_settings.application_id);
            activity.SetType(ActivityTypes.Playing);
            activity.SetDetails($"{(Application.isEditor ? "[E] " : string.Empty)}{scene.name}");
            activity.SetState($"net.v{_RUDP_.RudpSocket.r_settings.GetValue().VERSION}");

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
                    Debug.Log($"{log_prefixe} success");
                else
                    Debug.LogWarning($"{log_prefixe} failure: \"{result.Error()}\"");
            });
        }
    }
}