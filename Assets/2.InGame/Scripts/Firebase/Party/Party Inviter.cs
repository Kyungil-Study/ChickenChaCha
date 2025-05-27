// PartyInviter.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Firebase.Auth;
using Firebase.Firestore;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PartyInviter : MonoBehaviour
{
    private FirebaseAuth mAuth;
    private FirebaseFirestore mDB;
    
    public TMP_InputField mInputField;

    private void Awake()
    {
        mAuth = UserManager.Instance.Auth;
        mDB = UserManager.Instance.DB;
    }

    public async void InviteFriend(string roomName)
    {
        try
        {
            string myUid = mAuth.CurrentUser?.UserId;
            string myEmail = mAuth.CurrentUser?.Email;
            string friendEmail = mInputField.text;

            if (myUid == null || string.IsNullOrEmpty(roomName)) return;

            string friendUid = await FindUidByEmail(friendEmail);
            if (friendUid == null) return;

            var invitationRef = mDB.Collection("users").Document(friendUid)
                .Collection("invitations").Document(roomName);

            var data = new Dictionary<string, object>
            {
                { "roomName", roomName },
                { "from", myEmail },
                { "timestamp", Timestamp.GetCurrentTimestamp() }
            };

            await invitationRef.SetAsync(data);
            Debug.Log($"✅ {friendEmail}에게 파티 초대 전송 완료");
        }
        catch (Exception ex)
        {
            Debug.LogError("❌ 초대 전송 실패: " + ex.Message);
        }
    }

    private async Task<string> FindUidByEmail(string email)
    {
        try
        {
            var snapshot = await mDB.Collection("users")
                .WhereEqualTo("email", email).Limit(1)
                .GetSnapshotAsync();

            return snapshot.Count > 0 ? snapshot.Documents.First().Id : null;
        }
        catch (Exception e)
        {
            Debug.LogError("UID 조회 실패: " + e.Message);
            return null;
        }
    }
}