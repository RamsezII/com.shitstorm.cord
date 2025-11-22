using _ARK_;
using _SGUI_;
using Discord.Sdk;
using UnityEngine;
using UnityEngine.UI;

namespace _CORD_
{
    internal sealed partial class ShitcordSgui : SguiWindow1
    {
        public static ShitcordSgui instance;

        [SerializeField] Button button_login;
        [SerializeField] Traductable trad_status;
        [SerializeField] RectTransform layout_friends_prt;
        [SerializeField] VerticalLayoutGroup layout_friends;
        [SerializeField] CordFriendUI prefab_friendUI;
        CordFriendUI[] GetFriends() => prefab_friendUI.transform.parent.GetComponentsInChildren<CordFriendUI>(includeInactive: false);

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
            instance = this;

            button_login = transform.Find("rT/body/page_not-connection/panel1/rt/button_login").GetComponent<Button>();
            trad_status = transform.Find("rT/body/page_not-connection/panel1/rt/text_status").GetComponent<Traductable>();
            prefab_friendUI = GetComponentInChildren<CordFriendUI>(true);
            layout_friends = prefab_friendUI.GetComponentInParent<VerticalLayoutGroup>();
            layout_friends_prt = (RectTransform)layout_friends.transform.parent;

            base.Awake();
        }

        //--------------------------------------------------------------------------------------------------------------

        protected override void Start()
        {
            base.Start();

            ShitcordMachine.StartClient();

            button_login.onClick.AddListener(ShitcordMachine.TryLogin);

            prefab_friendUI.gameObject.SetActive(false);
        }

        //--------------------------------------------------------------------------------------------------------------

        internal void OnStatusChanged(in Client.Status status, in Client.Error error, in int errorCode)
        {
            trad_status.SetTrad(status);
            LoadFriends();
        }

        internal void LoadFriends()
        {
            RelationshipHandle[] relations = ShitcordMachine.client.GetRelationships();

            for (int i = 0; i < relations.Length; i++)
            {
                var clone = Instantiate(prefab_friendUI, prefab_friendUI.transform.parent);
                clone.gameObject.SetActive(true);
                clone.InitializeFriend(relations[i]);
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)layout_friends.transform);
            layout_friends_prt.sizeDelta = new(0, layout_friends.preferredHeight);

            SortFriends();
        }

        internal void UpdateFriends()
        {
            var friends = GetFriends();
            for (int i = 0; i < friends.Length; i++)
                friends[i].UpdateFriend();
        }

        internal void SortFriends()
        {
            var friends = GetFriends();

            System.Array.Sort(friends, (a, b) =>
            {
                return a.friend_handle.User().DisplayName().CompareTo(b.friend_handle.User().DisplayName());
            });

            for (int i = 0; i < friends.Length; i++)
                friends[i].transform.SetSiblingIndex(1 + i);
        }

        //--------------------------------------------------------------------------------------------------------------

        protected override void OnDestroy()
        {
            base.OnDestroy();

#if UNITY_EDITOR
            if (!Application.isEditor)
#endif
                ShitcordMachine.StopClient();
        }
    }
}