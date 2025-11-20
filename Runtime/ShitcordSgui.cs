using _ARK_;
using _SGUI_;
using Discord.Sdk;
using UnityEngine;
using UnityEngine.UI;

namespace _CORD_
{
    public sealed partial class ShitcordSgui : SguiWindow1
    {
        [SerializeField] Button button_login;
        [SerializeField] Traductable trad_text2;
        [SerializeField] Traductable trad_status;
        [SerializeField] Client client;
        [SerializeField] string codeVerifier;

        class Settings : HomeJSon
        {
            public bool auto_connection;
        }

        [SerializeField] Settings settings;

        //--------------------------------------------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void OnAfterSceneLoad()
        {
            if (ShitcordMachine.GetOrLoadApplicationID() > 0)
                OSView.instance.GetSoftwareButton<ShitcordSgui>(force: true);
            else
            {
                var windows = ShowAlert(
                    type: SguiDialogs.Error,
                    alert: out _,
                    traductions: new($"SHITCORD ID NOT SET.")
                );
#if UNITY_EDITOR
                windows.onOblivion += () => Application.OpenURL(ShitcordMachine.r_settings.GetFilePath());
#endif
            }

            NUCLEOR.delegates.OnApplicationFocus += () =>
            {
                bool is_valid = ShitcordMachine.GetOrLoadApplicationID() > 0;
                SoftwareButton button = OSView.instance.GetSoftwareButton<ShitcordSgui>(force: is_valid);

                if (!is_valid && button != null)
                    Destroy(button.gameObject);
            };
        }

        //--------------------------------------------------------------------------------------------------------------

        protected override void Awake()
        {
            button_login = transform.Find("rT/body/page_not-connected/panel1/rt/button_login").GetComponent<Button>();
            trad_text2 = transform.Find("rT/body/page_not-connected/panel1/rt/text2").GetComponent<Traductable>();
            trad_status = transform.Find("rT/body/page_not-connected/panel1/rt/text_status").GetComponent<Traductable>();

            base.Awake();
        }

        //--------------------------------------------------------------------------------------------------------------

        protected override void Start()
        {
            base.Start();

            client = new Client();

            client.AddLogCallback(
                callback: (message, severity) =>
                {
                    if (oblivionized)
                        return;

                    if (severity >= LoggingSeverity.Info)
                        trad_text2.SetTrad(message);

                    switch (severity)
                    {
                        case LoggingSeverity.Verbose:
                            Debug.Log(message.ToSubLog(), this);
                            break;

                        case LoggingSeverity.Warning:
                            Debug.LogWarning(message, this);
                            break;

                        case LoggingSeverity.Error:
                            Debug.LogError(message, this);
                            break;

                        default:
                            Debug.Log(message, this);
                            break;
                    }
                },
                minSeverity: LoggingSeverity.Info
            );

            client.SetStatusChangedCallback((status, error, errorCode) =>
            {
                if (oblivionized)
                    return;

                trad_status.SetTrad(status);

                Debug.Log($"Status changed: \"{status}\".".ToSubLog(), this);
                if (error != 0)
                    Debug.LogWarning($"Error: \"{error}\", code: \"{errorCode}\".");

                if (status == Client.Status.Ready)
                {
                    Util.InstantiateOrCreateIfAbsent<ShitcordPresence>();
                    ShitcordPresence.instance.UpdateRichPresence(client);
                }
            });

            button_login.onClick.AddListener(StartOAuthFlow);
        }

        //--------------------------------------------------------------------------------------------------------------

        [ContextMenu(nameof(StartOAuthFlow))]
        public void StartOAuthFlow()
        {
            var authorizationVerifier = client.CreateAuthorizationCodeVerifier();
            codeVerifier = authorizationVerifier.Verifier();

            var args = new AuthorizationArgs();

            args.SetClientId(ShitcordMachine.GetOrLoadApplicationID());
            args.SetScopes(Client.GetDefaultCommunicationScopes());
            args.SetCodeChallenge(authorizationVerifier.Challenge());

            client.Authorize(args, OnAuthorizeResult);
        }

        private void OnAuthorizeResult(ClientResult result, string code, string redirectUri)
        {
            if (!result.Successful())
            {
                Debug.LogWarning($"Authorization result: [{result.Error()}]", this);
                return;
            }
            GetTokenFromCode(code, redirectUri);
        }

        private void GetTokenFromCode(string code, string redirectUri)
        {
            client.GetToken(ShitcordMachine.GetOrLoadApplicationID(), code, codeVerifier, redirectUri, OnGetToken);
        }

        private void OnGetToken(ClientResult result, string token, string refreshToken, AuthorizationTokenType tokenType, int expiresIn, string scope)
        {
            if (token == null || token == string.Empty)
            {
                Debug.LogWarning("Failed to retrieve token", this);
            }
            else
            {
                client.UpdateToken(AuthorizationTokenType.Bearer, token, OnUpdateToken);
            }
        }

        private void OnUpdateToken(ClientResult result)
        {
            if (result.Successful())
            {
                client.Connect();
            }
            else
            {
                Debug.LogWarning($"Failed to update token: {result.Error()}", this);
            }
        }

        //--------------------------------------------------------------------------------------------------------------

        protected override void OnDestroy()
        {
            base.OnDestroy();

            client.Disconnect();
            client.Dispose();
        }
    }
}