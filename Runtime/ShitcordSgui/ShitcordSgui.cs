using _ARK_;
using _SGUI_;
using UnityEngine;
using UnityEngine.UI;

namespace _CORD_
{
    public sealed partial class ShitcordSgui : SguiWindow1
    {
        [SerializeField] Button button_login;
        [SerializeField] Traductable trad_status;

        class Settings : HomeJSon
        {
            public bool auto_connection;
        }

        [SerializeField] Settings settings;

        //--------------------------------------------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void OnAfterSceneLoad()
        {
            if (ShitcordMachine.r_settings.GetValue().application_id > 0)
                OSView.instance.GetSoftwareButton<ShitcordSgui>(force: true);
            else
            {
                var windows = ShowAlert(
                    type: SguiDialogs.Error,
                    alert: out _,
                    traductions: new($"SHITCORD ID NOT SET.")
                );
#if UNITY_EDITOR
                windows.onOblivion += () => Application.OpenURL(ShitcordMachine.r_settings.GetValue().GetFilePath());
#endif
            }
        }

        //--------------------------------------------------------------------------------------------------------------

        protected override void Awake()
        {
            button_login = transform.Find("rT/body/page_not-connected/panel1/rt/button_login").GetComponent<Button>();
            trad_status = transform.Find("rT/body/page_not-connected/panel1/rt/text_status").GetComponent<Traductable>();

            base.Awake();
        }

        //--------------------------------------------------------------------------------------------------------------

        protected override void Start()
        {
            base.Start();

            ShitcordMachine.client_status.AddListener(OnStatusChanged);

            ShitcordMachine.StartClient();

            button_login.onClick.AddListener(ShitcordMachine.TryLogin);
        }

        //--------------------------------------------------------------------------------------------------------------

        void OnStatusChanged(ShitcordMachine.ClientStatus status)
        {
            trad_status.SetTrad(status.value);
        }

        //--------------------------------------------------------------------------------------------------------------

        protected override void OnDestroy()
        {
            base.OnDestroy();

            ShitcordMachine.client_status.RemoveListener(OnStatusChanged);

            return;
            ShitcordMachine.StopClient();
        }
    }
}