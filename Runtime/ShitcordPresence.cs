using _ARK_;
using Discord.Sdk;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _CORD_
{
    sealed class ShitcordPresence : MonoBehaviour
    {
        public static ShitcordPresence instance;

        [SerializeField] ulong startTimestamp;

        //--------------------------------------------------------------------------------------------------------------

        private void Awake()
        {
            instance = this;
            startTimestamp = (ulong)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }

        //--------------------------------------------------------------------------------------------------------------

        public void UpdateRichPresence(Client client)
        {
            Activity activity = new();

            activity.SetApplicationId(ShitcordMachine.r_settings.application_id);
            activity.SetType(ActivityTypes.Playing);
            activity.SetDetails($"{(Application.isEditor ? "[E] " : string.Empty)}{SceneManager.GetActiveScene().name}");
            activity.SetState($"net.v{_RUDP_.RudpSocket.version.VERSION}");

            if (false)
            {
                ActivityParty party = new();
                party.SetCurrentSize(NUCLEOR.instance.party_count._value);
                party.SetMaxSize(byte.MaxValue - 1);
                activity.SetParty(party);
            }

            ActivityTimestamps timestamps = new();
            timestamps.SetStart(startTimestamp);
            activity.SetTimestamps(timestamps);

            client.UpdateRichPresence(activity, OnUpdateRichPresence);
        }

        void OnUpdateRichPresence(ClientResult result)
        {
            if (result.Successful())
                Debug.Log($"Rich presence updated!", this);
            else
                Debug.LogWarning($"Failed to update rich presence: \"{result.Error()}\"", this);
        }
    }
}