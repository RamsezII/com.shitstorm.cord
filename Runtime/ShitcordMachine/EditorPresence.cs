#if UNITY_EDITOR
using _ARK_;
using Discord.Sdk;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _CORD_
{
    [InitializeOnLoad]
    static class EditorPresence
    {
        static Client client;

        //--------------------------------------------------------------------------------------------------------------

        static EditorPresence()
        {
            Debug.Log($"{typeof(EditorPresence)}.CONSTRUCTOR");

            bool block = true;
            if (block)
                return;

            AssemblyReloadEvents.afterAssemblyReload += () =>
            {
                Debug.Log($"{typeof(EditorPresence)}.RELOAD_ASSEMBLY");
                StartClient();
            };

            EditorSceneManager.activeSceneChangedInEditMode += (scene0, scene1) =>
            {
                Debug.Log($"{typeof(EditorPresence)}.CHANGED_SCENE {{ {nameof(scene0)}: {scene0}, {nameof(scene1)}: {scene1} }}");
                UpdatePresence(scene1);
            };

            EditorApplication.quitting += () =>
            {
                Debug.Log($"{typeof(EditorPresence)}.QUIT_EDITOR");
                StopClient();
            };
        }

        //--------------------------------------------------------------------------------------------------------------

        [MenuItem("Assets/" + nameof(_CORD_) + "/" + nameof(EditorPresence) + "." + nameof(StartClient))]
        static void StartClient()
        {
            client?.Dispose();

            client = new();

            ShitcordMachine.ForceLoadSettings(true);

            Debug.Log($"{typeof(EditorPresence)}.START_CLIENT (id: {ShitcordMachine.r_settings.application_id})");

            client.SetApplicationId(ShitcordMachine.r_settings.application_id);

            client.AddLogCallback(
                callback: (message, severity) =>
                {
                    switch (severity)
                    {
                        case LoggingSeverity.Verbose:
                            Debug.Log(message.ToSubLog());
                            break;

                        case LoggingSeverity.Warning:
                            Debug.LogWarning(message);
                            break;

                        case LoggingSeverity.Error:
                            Debug.LogError(message);
                            break;

                        default:
                            Debug.Log(message);
                            break;
                    }
                },
                minSeverity: LoggingSeverity.Verbose
            );

            UpdatePresence();
        }

        [MenuItem("Assets/" + nameof(_CORD_) + "/" + nameof(EditorPresence) + "." + nameof(UpdatePresence))]
        static void UpdatePresence() => UpdatePresence(SceneManager.GetActiveScene());
        static void UpdatePresence(Scene scene)
        {
            ShitcordMachine.ForceLoadSettings(true);

            Debug.Log($"{typeof(EditorPresence)}.UPDATE_CLIENT (id: {ShitcordMachine.r_settings.application_id})");

            Activity activity = new();

            activity.SetName(Application.productName);
            activity.SetApplicationId(ShitcordMachine.r_settings.application_id);
            activity.SetType(ActivityTypes.Playing);
            activity.SetDetails($"{(Application.isEditor ? "[E] " : string.Empty)}{scene.name}");
            activity.SetState($"net.v{_RUDP_.RudpSocket.version.GetValue().VERSION}");

            bool use_party = false;
            if (use_party)
            {
                ActivityParty party = new();
                party.SetId("test_lobby");
                party.SetCurrentSize(NUCLEOR.instance.party_count._value);
                party.SetMaxSize(byte.MaxValue - 1);
                activity.SetParty(party);
            }

            ActivityTimestamps timestamps = new();
            timestamps.SetStart(ShitcordMachine.richp_start_tstamp);
            activity.SetTimestamps(timestamps);

            client.UpdateRichPresence(activity, result =>
            {
                if (result.Successful())
                    Debug.Log($"{typeof(EditorPresence)} updated".ToSubLog());
                else
                    Debug.LogWarning($"{typeof(EditorPresence)} Failed to update: \"{result.Error()}\"");
            });
        }

        [MenuItem("Assets/" + nameof(_CORD_) + "/" + nameof(EditorPresence) + "." + nameof(StopClient))]
        static void StopClient()
        {
            Debug.Log($"{typeof(EditorPresence)}.STOP_CLIENT");
            client?.Dispose();
            client = null;
        }
    }
}
#endif