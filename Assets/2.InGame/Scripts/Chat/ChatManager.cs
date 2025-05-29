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
    
    [Header("출력 UI")]
    [SerializeField] private Image mMessageBox;
    [SerializeField] private TextMeshProUGUI mChatText;
    
    // Start is called before the first frame update
    void Start()
    {
        mSendButton.onClick.AddListener(OnSendChatButtonClicked);
    }
    
    public void OnSendChatButtonClicked()
    {
        string message = mInputField.text;
        string nickname = UserManager.Instance.User.NickName;

        // 내가 직접 내 메시지를 전파
        RPC_BroadcastChat(nickname, message);

        mInputField.text = ""; // 입력창 초기화
    }
    
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_BroadcastChat(string senderName, string message)
    {
        string mMessage = $"{senderName} : {message}";
        AddChatLog(mMessage);
    }
    
    public void AddChatLog(string message)
    {
        mChatText.text += message;
    }
}
