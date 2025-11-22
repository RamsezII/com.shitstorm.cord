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
        public TextMeshProUGUI text_name, text_status;
        public RawImage rimg_avatar;
        public RelationshipHandle friend_handle;
        [SerializeField] Texture2D tex_avatar;

        //--------------------------------------------------------------------------------------------------------------

        private void Awake()
        {
            text_name = transform.Find("text_name").GetComponent<TextMeshProUGUI>();
            text_status = transform.Find("text_status").GetComponent<TextMeshProUGUI>();
            rimg_avatar = transform.Find("avatar").GetComponent<RawImage>();
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

            text_name.text = user.DisplayName();
            text_status.text = user.Status().ToString();

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