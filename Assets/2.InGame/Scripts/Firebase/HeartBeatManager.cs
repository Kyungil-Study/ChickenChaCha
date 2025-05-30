using System.Collections;
using System.Collections.Generic;
using Firebase.Auth;
using Firebase.Extensions;
using Firebase.Firestore;
using UnityEngine;

public class HeartbeatManager : MonoBehaviour
{
    private const float HeartbeatInterval = 30f; // 30초마다 갱신
    private float mTimer = 0f;
    

    private void Update()
    {
        if (UserManager.Instance.Auth.CurrentUser == null) return;

        mTimer += Time.deltaTime;
        if (mTimer >= HeartbeatInterval)
        {
            mTimer = 0f;
            SendHeartbeat();
        }
    }

    private void SendHeartbeat()
    {
        string uid = UserManager.Instance.Auth.CurrentUser.UserId;

        DocumentReference userRef = UserManager.Instance.DB.Collection("users").Document(uid);
        userRef.UpdateAsync(new Dictionary<string, object>
        {
            { "lastHeartbeat", Timestamp.GetCurrentTimestamp() },
            { "isOnline", true }
        }).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompletedSuccessfully)
            {
                Debug.Log("❤️ 하트비트 전송 완료");
            }
            else
            {
                Debug.LogWarning("하트비트 실패: " + task.Exception?.Message);
            }
        });
    }
}