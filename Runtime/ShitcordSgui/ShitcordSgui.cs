using _ARK_;
using _SGUI_.composer;
using _SGUI_;
using Discord.Sdk;
using UnityEngine;
using UnityEngine.UI;
using Unity.Scripting.LifecycleManagement;

namespace _CORD_
{
    internal sealed partial class ShitcordSgui : SguiFrame
    {
        [AutoStaticsCleanup] public static ShitcordSgui instance;

        [SerializeField] Button button_login;
        [SerializeField] Traductable trad_status;
        [SerializeField] ScrollRect scrollview_friends;
        [SerializeField] VerticalLayoutGroup vlayout_friends;
        [SerializeField] CordFriendUI prefab_friendUI;
        CordFriendUI[] GetFriends() => prefab_friendUI.transform.parent.GetComponentsInChildren<CordFriendUI>(includeInactive: false);

        //--------------------------------------------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void OnAfterSceneLoad()
        {
            if (ShitcordMachine.application_id > 0)
                OSView.instance.AddSoftwareButton<ShitcordSgui>(new("Shitcord"));
            else
                ShowAlert(
                    type: SguiDialogs.Error,
                    alert: out _,
                    traductions: new($"SHITCORD ID NOT SET.")
                );
        }

        //--------------------------------------------------------------------------------------------------------------

        protected override void Awake()
        {
            instance = this;

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
            trad_status.SetText(status.ToString());
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

            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)vlayout_friends.transform);
            scrollview_friends.content.sizeDelta = new(0, vlayout_friends.preferredHeight);

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
            ShitcordMachine.StopClient();
        }
    }
}