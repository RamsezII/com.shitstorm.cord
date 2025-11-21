using _ARK_;
using _UTIL_;
using Discord.Sdk;
using System;
using UnityEngine;

namespace _CORD_
{
    partial class ShitcordMachine
    {
        public struct ClientStatus
        {
            public Client.Status value;
            public Client.Error error;
            public int error_code;

            //--------------------------------------------------------------------------------------------------------------

            public ClientStatus(in Client.Status value, Client.Error error, int error_code)
            {
                this.value = value;
                this.error = error;
                this.error_code = error_code;
            }
        }

        static Client client;
        static string codeVerifier;

        public static Action<string, LoggingSeverity> logCallback;
        public static readonly ValueHandler<ClientStatus> client_status = new();

        //--------------------------------------------------------------------------------------------------------------

#if UNITY_EDITOR
        [UnityEditor.MenuItem("Assets/" + nameof(_CORD_) + "/" + nameof(ShitcordMachine) + "." + nameof(StartClient))]
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
                    logCallback?.Invoke(message, severity);
                },
                minSeverity: LoggingSeverity.Info
            );

            client.SetStatusChangedCallback((status, error, errorCode) =>
            {
                Debug.Log($"Status changed: \"{status}\".".ToSubLog());
                if (error != 0)
                    Debug.LogWarning($"Error: \"{error}\", code: \"{errorCode}\".");

                if (status == Client.Status.Ready)
                    NUCLEOR.instance.sequencer_parallel.AddRoutine(Util.EWaitForFrames(1, TryUpdateRichPresence));

                client_status.Value = new(status, error, errorCode);
            });
        }

        public static void TryLogin()
        {
            RSettings r_settings = ShitcordMachine.r_settings.GetValue();

            if (client == null)
                StartClient();

            var authorizationVerifier = client.CreateAuthorizationCodeVerifier();
            codeVerifier = authorizationVerifier.Verifier();

            var args = new AuthorizationArgs();

            args.SetClientId(r_settings.application_id);
            args.SetScopes(Client.GetDefaultCommunicationScopes());
            args.SetCodeChallenge(authorizationVerifier.Challenge());

            if (string.IsNullOrWhiteSpace(h_settings.refresh_token))
                client.Authorize(args, OnAuthorizeResult);
            else
                client.RefreshToken(r_settings.application_id, h_settings.refresh_token, OnRefreshToken);
        }

        static void OnAuthorizeResult(ClientResult result, string code, string redirectUri)
        {
            if (!result.Successful())
                Debug.LogWarning($"Authorization result: [{result.Error()}]");
            else
                client.GetToken(
                    applicationId: r_settings.GetValue().application_id,
                    code: code,
                    codeVerifier: codeVerifier,
                    redirectUri: redirectUri,
                    callback: OnRefreshToken
                );
        }

        static void OnRefreshToken(ClientResult result, string accessToken, string refreshToken, AuthorizationTokenType tokenType, int expiresIn, string scopes)
        {
            h_settings.refresh_token = refreshToken;
            h_settings.SaveStaticJSon(true);

            if (accessToken == null || accessToken == string.Empty)
                Debug.LogWarning("Failed to retrieve token");
            else
                client.UpdateToken(AuthorizationTokenType.Bearer, accessToken, OnUpdateToken);
        }

        static void OnUpdateToken(ClientResult result)
        {
            if (result.Successful())
                client.Connect();
            else
                Debug.LogWarning($"Failed to update token: {result.Error()}");
        }

        //--------------------------------------------------------------------------------------------------------------

#if UNITY_EDITOR
        [UnityEditor.MenuItem("Assets/" + nameof(_CORD_) + "/" + nameof(ShitcordMachine) + "." + nameof(StopClient))]
#endif
        public static void StopClient()
        {
            client?.Dispose();
            client = null;
        }
    }
}