using System;
using System.Collections.Generic;
using System.Linq;
using Firebase.Auth;
using Firebase.Firestore;
using Firebase.Extensions;
using UnityEngine;

public class FriendManager : MonoBehaviour
{
    public FirebaseAuth auth;
    public FirebaseFirestore db;

    
    private void Start()
    {
        auth = FirebaseAuth.DefaultInstance;
        db = FirebaseFirestore.DefaultInstance;
    }

    public static string MyUid;

    // 이메일로 UID 찾기
    private async void FindUidByEmail(string email, Action<string> callback)
    {
        if (db == null)
        {
            Debug.LogError("FirebaseFirestore가 초기화되지 않았습니다.");
            callback?.Invoke(null);
            return;
        }
        
        Debug.Log("CurrentUser: " + MyUid);
        
        try
        {
            var snapshot = await db.Collection("users")
                                   .WhereEqualTo("email", email)
                                   .Limit(1)
                                   .GetSnapshotAsync();
            
            if (snapshot.Count == 0)
            {
                Debug.LogWarning("해당 이메일을 가진 사용자를 찾을 수 없습니다.");
                callback?.Invoke(null);
                return;
            }

            string uid = snapshot.Documents.First().Id;
            callback?.Invoke(uid);
        }
        catch (Exception e)
        {
            Debug.LogError("UID 찾기 실패: " + e.Message);
            callback?.Invoke(null);
        }
    }

    // 📤 친구 추가
    public void AddFriendByEmail(string friendEmail, Action<bool> callback)
    {
        if (string.IsNullOrEmpty(MyUid))
        {
            Debug.LogError("로그인되지 않았습니다.");
            callback?.Invoke(false);
            return;
        }

        FindUidByEmail(friendEmail, friendUid =>
        {
            if (friendUid == null || friendUid == MyUid)
            {
                callback?.Invoke(false);
                return;
            }

            var myFriendRef = db.Collection("users").Document(MyUid)
                                .Collection("friends").Document(friendUid);
            var friendFriendRef = db.Collection("users").Document(friendUid)
                                     .Collection("friends").Document(MyUid);

            var data = new Dictionary<string, object>
            {
                { "status", "accepted" },
                { "timestamp", Timestamp.GetCurrentTimestamp() }
            };

            myFriendRef.SetAsync(data).ContinueWithOnMainThread(_ =>
            {
                friendFriendRef.SetAsync(data).ContinueWithOnMainThread(__ =>
                {
                    Debug.Log("친구 추가 완료");
                    callback?.Invoke(true);
                });
            });
        });
    }

    // ❌ 친구 삭제
    public void RemoveFriendByEmail(string friendEmail, Action<bool> callback)
    {
        if (string.IsNullOrEmpty(MyUid))
        {
            Debug.LogError("로그인되지 않았습니다.");
            callback?.Invoke(false);
            return;
        }

        FindUidByEmail(friendEmail, friendUid =>
        {
            if (friendUid == null)
            {
                callback?.Invoke(false);
                return;
            }

            var myFriendRef = db.Collection("users").Document(MyUid)
                                .Collection("friends").Document(friendUid);
            var friendFriendRef = db.Collection("users").Document(friendUid)
                                     .Collection("friends").Document(MyUid);

            myFriendRef.DeleteAsync().ContinueWithOnMainThread(_ =>
            {
                friendFriendRef.DeleteAsync().ContinueWithOnMainThread(__ =>
                {
                    Debug.Log("친구 삭제 완료");
                    callback?.Invoke(true);
                });
            });
        });
    }

    // 📋 친구 목록 가져오기 (이메일 포함)
    public void GetFriendList(Action<List<string>> callback)
    {
        db.Collection("users").Document(MyUid).Collection("friends")
            .WhereEqualTo("status", "accepted")
            .GetSnapshotAsync().ContinueWithOnMainThread(task =>
            {
                var result = new List<string>();

                if (task.IsCompletedSuccessfully)
                {
                    foreach (var doc in task.Result.Documents)
                    {
                        string friendUid = doc.Id;
                        db.Collection("users").Document(friendUid).GetSnapshotAsync().ContinueWithOnMainThread(userTask =>
                        {
                            if (userTask.IsCompletedSuccessfully && userTask.Result.Exists &&
                                userTask.Result.TryGetValue("email", out string email))
                            {
                                result.Add(email);

                                // 모든 친구 이메일 수집 완료 시 콜백 실행
                                if (result.Count == task.Result.Count)
                                    callback?.Invoke(result);
                            }
                        });
                    }
                }
                else
                {
                    Debug.LogError("친구 목록 가져오기 실패: " + task.Exception?.Message);
                    callback?.Invoke(result);
                }
            });
    }
}