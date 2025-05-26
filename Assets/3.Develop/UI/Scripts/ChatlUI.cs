using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Firestore;
using Firebase.Auth;
using Firebase.Extensions;

public class ChatUI : MonoBehaviour
{
    [Header("Chat UI")]
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private Button sendButton;
    [SerializeField] private Transform contentRoot;   // ScrollView > Content
    [SerializeField] private GameObject messagePrefab; // 말풍선 프리팹

    private ListenerRegistration listener;
    private string mUserId;
    private string mNickname;

    private void Start()
    {
        // 1) 버튼 클릭 리스너만 등록
        sendButton.onClick.AddListener(OnSendClicked);
        // 2) ListenToChatRoom() 은 InitializeUser() 호출 시에만 실행
    }

    // 외부에서 로그인된 유저 정보를 주입합니다.
    // AccountManagement.cs 를 건드리지 않고, 이 메서드만 호출해 주면 됩니다.
    public void InitializeUser(string userId, string nickname)
    {
        mUserId = userId;
        mNickname = nickname;
        Debug.Log($"[ChatUI] Initialized: {mNickname} ({mUserId})");

        StartListening("room_001");
    }

    private void OnSendClicked()
    {
        string msg = inputField.text;
        if (string.IsNullOrWhiteSpace(msg)) return;

        SendMessageToFirebase(msg);
        inputField.text = "";
    }

    private void SendMessageToFirebase(string message)
    {
        if (string.IsNullOrEmpty(mUserId))
        {
            Debug.LogWarning("[ChatUI] User not initialized. Cannot send message.");
            return;
        }

        var data = new Dictionary<string, object>
        {
            { "senderId", mUserId },
            { "nickname",  mNickname },
            { "message",   message },
            { "timestamp", FieldValue.ServerTimestamp }
        };

        FirebaseFirestore.DefaultInstance
            .Collection("ChatRooms")
            .Document("room_001")
            .Collection("Messages")
            .AddAsync(data)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCompletedSuccessfully)
                    Debug.Log("[ChatUI] Message sent.");
                else
                    Debug.LogError("[ChatUI] Send failed: " + task.Exception);
            });
    }

    private void StartListening(string roomId)
    {
        // 중복 등록 방지
        if (listener != null) return;

        listener = FirebaseFirestore.DefaultInstance
            .Collection("ChatRooms").Document(roomId)
            .Collection("Messages")
            .OrderBy("timestamp")
            .Listen(snapshot =>
            {
                foreach (var change in snapshot.GetChanges())
                {
                    if (change.ChangeType == DocumentChange.Type.Added)
                    {
                        var doc = change.Document;
                        string nick = doc.GetValue<string>("nickname");
                        string msg  = doc.GetValue<string>("message");
                        AddMessageToUI($"{nick}: {msg}");
                    }
                }
            });
    }

    private void AddMessageToUI(string fullMessage)
    {
        var go = Instantiate(messagePrefab, contentRoot);
        var textComp = go.GetComponentInChildren<TMP_Text>();
        if (textComp != null) textComp.text = fullMessage;
    }

    private void OnDestroy()
    {
        listener?.Stop();
    }
}
