using Discord.Sdk;
using UnityEngine;

namespace _CORD_
{
    partial class ShitcordMachine
    {
        internal static Client client;
        static string codeVerifier;

#if UNITY_EDITOR
        const string button_prefixe = "Assets/" + nameof(_CORD_) + "/";

        //--------------------------------------------------------------------------------------------------------------

        [UnityEditor.MenuItem(button_prefixe + nameof(StartClient))]
#endif
        public static void StartClient()
        {
            client?.Dispose();

            client = new Client();

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
                minSeverity: LoggingSeverity.Warning
            );

            client.SetStatusChangedCallback((status, error, errorCode) =>
            {
                if (error != 0)
                    Debug.LogWarning($"Error: \"{error}\", code: \"{errorCode}\".");

                if (status == Client.Status.Ready)
                    if (r_settings.GetValue().rich_presence_in_editor)
                        TryUpdateRichPresence();

                if (ShitcordSgui.instance != null)
                    ShitcordSgui.instance.OnStatusChanged(status, error, errorCode);

                Debug.Log($"{typeof(ShitcordMachine)}.CHANGED_STATUS: \"{status}\".");
            });

            client.SetUserUpdatedCallback(userID =>
            {
                if (ShitcordSgui.instance != null)
                {
                    ShitcordSgui.instance.UpdateFriends();
                    ShitcordSgui.instance.SortFriends();
                }
            });
        }

#if UNITY_EDITOR
        [UnityEditor.MenuItem(button_prefixe + nameof(TryLogin))]
#endif
        public static void TryLogin()
        {
            if (client == null)
            {
                Debug.LogError($"null {nameof(client)} (\"{client}\")");
                return;
            }

            RSettings r_settings = ShitcordMachine.r_settings.GetValue();

            if (r_settings.application_id == 0)
            {
                Debug.LogError($"{typeof(ShitcordMachine)}.{nameof(RSettings.application_id)}: {r_settings.application_id}");
                return;
            }

            string refresh_token = h_settings_codes.GetValue(true).refresh_token;

            var authorizationVerifier = client.CreateAuthorizationCodeVerifier();
            codeVerifier = authorizationVerifier.Verifier();

            var args = new AuthorizationArgs();

            args.SetClientId(r_settings.application_id);
            args.SetScopes(Client.GetDefaultCommunicationScopes());
            args.SetCodeChallenge(authorizationVerifier.Challenge());

            if (string.IsNullOrWhiteSpace(refresh_token))
                client.Authorize(args, (ClientResult result, string code, string redirectUri) =>
                {
                    bool success = result.Successful();

                    if (!success)
                        Debug.LogWarning($"Authorization result: [{result.Error()}]");
                    else
                        client.GetToken(
                            applicationId: r_settings.application_id,
                            code: code,
                            codeVerifier: codeVerifier,
                            redirectUri: redirectUri,
                            callback: OnRefreshToken
                        );
                });
            else
                client.RefreshToken(r_settings.application_id, refresh_token, OnRefreshToken);
        }

        static void OnRefreshToken(ClientResult result, string accessToken, string refreshToken, AuthorizationTokenType tokenType, int expiresIn, string scopes)
        {
            bool success = result.Successful();

            h_settings_codes.GetValue().refresh_token = refreshToken;
            h_settings_codes._value.SaveStaticJSon(true);

            if (accessToken == null || accessToken == string.Empty)
                Debug.LogWarning($"Failed to retrieve token ({nameof(success)} was {success})");
            else
                client.UpdateToken(AuthorizationTokenType.Bearer, accessToken, (ClientResult result) =>
                {
                    if (result.Successful())
                        client.Connect();
                    else
                        Debug.LogWarning($"Failed to update token: {result.Error()}");
                });
        }

        //--------------------------------------------------------------------------------------------------------------

#if UNITY_EDITOR
        [UnityEditor.MenuItem(button_prefixe + nameof(StopClient))]
#endif
        public static void StopClient()
        {
            client?.Dispose();
            client = null;
        }
    }
}