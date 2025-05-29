using System.Collections;
using System.Collections.Generic;
using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChatManager : NetworkBehaviour
{
    [Header("입력 UI")]
    [SerializeField] private TMP_InputField mInputField;
    [SerializeField] private Button mSendButton;

    [Header("메시지 Prefab")]
    [SerializeField] private GameObject mMessageBoxPrefab;

    [Header("출력 ContentUI")]
    [SerializeField] private Transform mChatContentParent; // ScrollView 아래 Content 오브젝트

    void Start()
    {
        mSendButton.onClick.AddListener(OnSendChatButtonClicked);
    }

    public void OnSendChatButtonClicked()
    {
        string message = mInputField.text;
        string nickname = UserManager.Instance.User.NickName;

        if (string.IsNullOrWhiteSpace(message)) return;

        RPC_BroadcastChat(nickname, message);
        mInputField.text = "";
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_BroadcastChat(string senderName, string message)
    {
        AddChatLog($"{senderName} : {message}");
    }

    public void AddChatLog(string message)
    {
        GameObject msgObj = Instantiate(mMessageBoxPrefab, mChatContentParent);
        TextMeshProUGUI text = msgObj.GetComponentInChildren<TextMeshProUGUI>();
        text.text = message;

        // 다음 프레임에 Scroll 강제 이동
        StartCoroutine(ScrollToBottomNextFrame());
    }

    private IEnumerator ScrollToBottomNextFrame()
    {
        yield return null; // 한 프레임 기다림 (UI 업데이트 후)
        Canvas.ForceUpdateCanvases();
        ScrollRect scroll = mChatContentParent.GetComponentInParent<ScrollRect>();
        if (scroll != null)
        {
            scroll.verticalNormalizedPosition = 0f;
            
                // 만약 Scrollbar를 따로 연결해놨다면:
                if (scroll.verticalScrollbar != null)
                {
                    scroll.verticalScrollbar.value = 0f;
                }
        }
    }
}