using _ARK_;
using Discord.Sdk;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace _CORD_
{
    internal sealed class CordFriendUI : MonoBehaviour
    {
        public CanvasGroup canvasGroup;
        public Button button;
        public TextMeshProUGUI text_dname, text_uname;
        public RawImage rimg_avatar, rimg_status;
        public RelationshipHandle friend_handle;
        [SerializeField] Texture2D tex_avatar;

        //--------------------------------------------------------------------------------------------------------------

        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            button = GetComponent<Button>();
            text_dname = transform.Find("text_displayname").GetComponent<TextMeshProUGUI>();
            text_uname = transform.Find("text_username").GetComponent<TextMeshProUGUI>();
            rimg_avatar = transform.Find("avatar").GetComponent<RawImage>();
            rimg_status = transform.Find("status_color").GetComponent<RawImage>();
        }

        //--------------------------------------------------------------------------------------------------------------

        public void InitializeFriend(in RelationshipHandle handle)
        {
            friend_handle = handle;
            UpdateFriend();
        }

        //--------------------------------------------------------------------------------------------------------------

        public void UpdateFriend()
        {
            UserHandle user = friend_handle.User();
            StatusType status = user.Status();

            text_dname.text = user.DisplayName();
            text_uname.text = user.Username();

            rimg_status.color = status switch
            {
                StatusType.Online => Color.green,
                StatusType.Offline => Color.gray,
                StatusType.Blocked => Color.purple,
                StatusType.Idle => Color.yellow,
                StatusType.Dnd => Color.red,
                StatusType.Invisible => .5f * Color.green,
                StatusType.Streaming => Color.magenta,
                StatusType.Unknown => .5f * Color.yellow,
                _ => new(.5f, .5f, .5f, .5f)
            };

            NUCLEOR.instance.sequencer_parallel.AddRoutine(ELoadAvatar(
                user.AvatarUrl(
                    animatedType: UserHandle.AvatarType.Png,
                    staticType: UserHandle.AvatarType.Png
                )
            ));
        }

        IEnumerator<float> ELoadAvatar(string url)
        {
            using UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
            request.SendWebRequest();

            while (!request.isDone)
                yield return request.downloadProgress;

            if (request.result == UnityWebRequest.Result.Success)
            {
                if (tex_avatar != null)
                    Destroy(tex_avatar);

                tex_avatar = DownloadHandlerTexture.GetContent(request);
                rimg_avatar.texture = tex_avatar;
            }
            else
                Debug.LogError($"Failed to load profile image from URL: {url}. Error: {request.error}");
        }

        //--------------------------------------------------------------------------------------------------------------

        private void OnDestroy()
        {
            if (tex_avatar != null)
                Destroy(tex_avatar);
        }
    }
}