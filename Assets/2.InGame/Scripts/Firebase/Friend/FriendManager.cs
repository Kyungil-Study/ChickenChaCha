// using System;
// using System.Collections;
// using System.Collections.Generic;
// using Firebase.Auth;
// using Firebase.Firestore;
// using UnityEngine;
//
// public class FriendManager : MonoBehaviour
// {
//     //public static FriendManager Instance;
//     
//     private FirebaseAuth mAuth;
//     private FirebaseFirestore mDB;
//
//     private string MyUid => mAuth.CurrentUser?.UserId;
//
//     private void Awake()
//     {
//         // if(Instance == null)
//         // {
//         //     Instance = this;
//         // }
//         mAuth = FirebaseAuth.DefaultInstance;
//         mDB = FirebaseFirestore.DefaultInstance;
//     }
//
//     // 친구 요청 보내기
//     public void SendFriendRequest(string targetUid, Action<bool> callback)
//     {
//         if (MyUid == null || targetUid == null || MyUid == targetUid)
//         {
//             callback?.Invoke(false);
//             return;
//         }
//
//         var myRef = mDB.Collection("users").Document(MyUid).Collection("friends").Document(targetUid);
//         var targetRef = mDB.Collection("users").Document(targetUid).Collection("friends").Document(MyUid);
//
//         var requestData = new Dictionary<string, object>
//         {
//             { "status", "pending" },
//             { "timestamp", Timestamp.GetCurrentTimestamp() }
//         };
//         var reverseData = new Dictionary<string, object>
//         {
//             { "status", "requested" },
//             { "timestamp", Timestamp.GetCurrentTimestamp() }
//         };
//
//         myRef.SetAsync(requestData).ContinueWith(_ =>
//         {
//             targetRef.SetAsync(reverseData).ContinueWith(__ =>
//             {
//                 callback?.Invoke(true);
//             });
//         });
//     }
//
//     // 친구 요청 수락
//     public void AcceptFriendRequest(string fromUid, Action<bool> callback)
//     {
//         if (MyUid == null || fromUid == null)
//         {
//             callback?.Invoke(false);
//             return;
//         }
//
//         var myRef = mDB.Collection("users").Document(MyUid).Collection("friends").Document(fromUid);
//         var targetRef = mDB.Collection("users").Document(fromUid).Collection("friends").Document(MyUid);
//
//         var data = new Dictionary<string, object>
//         {
//             { "status", "accepted" },
//             { "timestamp", Timestamp.GetCurrentTimestamp() }
//         };
//
//         myRef.SetAsync(data).ContinueWith(_ =>
//         {
//             targetRef.SetAsync(data).ContinueWith(__ =>
//             {
//                 callback?.Invoke(true);
//             });
//         });
//     }
//
//     // 친구 삭제
//     public void RemoveFriend(string friendUid, Action<bool> callback)
//     {
//         if (MyUid == null || friendUid == null)
//         {
//             callback?.Invoke(false);
//             return;
//         }
//
//         var myRef = mDB.Collection("users").Document(MyUid).Collection("friends").Document(friendUid);
//         var targetRef = mDB.Collection("users").Document(friendUid).Collection("friends").Document(MyUid);
//
//         myRef.DeleteAsync().ContinueWith(_ =>
//         {
//             targetRef.DeleteAsync().ContinueWith(__ =>
//             {
//                 callback?.Invoke(true);
//             });
//         });
//     }
//
//     // 친구 목록 가져오기
//     public void GetFriendList(Action<List<string>> callback)
//     {
//         if (MyUid == null)
//         {
//             callback?.Invoke(new List<string>());
//             return;
//         }
//
//         mDB.Collection("users").Document(MyUid).Collection("friends")
//             .WhereEqualTo("status", "accepted")
//             .GetSnapshotAsync().ContinueWith(task =>
//             {
//                 var friendList = new List<string>();
//                 if (task.IsCompletedSuccessfully)
//                 {
//                     foreach (var doc in task.Result.Documents)
//                     {
//                         friendList.Add(doc.Id);
//                     }
//                 }
//                 callback?.Invoke(friendList);
//             });
//     }
//     
//     // public void SendFriendRequest(string myUid, string targetUid)
//     // {
//     //     var db = FirebaseFirestore.DefaultInstance;
//     //
//     //     var data = new Dictionary<string, object>
//     //     {
//     //         { "status", "pending" },
//     //         { "timestamp", Timestamp.GetCurrentTimestamp() }
//     //     };
//     //
//     //     // 내 friends → 대상
//     //     var myRef = db.Collection("users").Document(myUid).Collection("friends").Document(targetUid);
//     //     myRef.SetAsync(data);
//     //
//     //     // 대상 friends → 나 (역방향)
//     //     var reverseData = new Dictionary<string, object>
//     //     {
//     //         { "status", "requested" },
//     //         { "timestamp", Timestamp.GetCurrentTimestamp() }
//     //     };
//     //     var targetRef = db.Collection("users").Document(targetUid).Collection("friends").Document(myUid);
//     //     targetRef.SetAsync(reverseData);
//     // }
//     //
//     // public void AcceptFriendRequest(string myUid, string fromUid)
//     // {
//     //     var db = FirebaseFirestore.DefaultInstance;
//     //
//     //     var data = new Dictionary<string, object>
//     //     {
//     //         { "status", "accepted" },
//     //         { "timestamp", Timestamp.GetCurrentTimestamp() }
//     //     };
//     //
//     //     db.Collection("users").Document(myUid).Collection("friends").Document(fromUid).SetAsync(data);
//     //     db.Collection("users").Document(fromUid).Collection("friends").Document(myUid).SetAsync(data);
//     // }
//     //
//     // public void RemoveFriend(string myUid, string otherUid)
//     // {
//     //     var db = FirebaseFirestore.DefaultInstance;
//     //
//     //     db.Collection("users").Document(myUid).Collection("friends").Document(otherUid).DeleteAsync();
//     //     db.Collection("users").Document(otherUid).Collection("friends").Document(myUid).DeleteAsync();
//     // }
//     //
//     // public void GetFriendList(string myUid, Action<List<string>> onComplete)
//     // {
//     //     var db = FirebaseFirestore.DefaultInstance;
//     //
//     //     db.Collection("users").Document(myUid)
//     //         .Collection("friends")
//     //         .WhereEqualTo("status", "accepted")
//     //         .GetSnapshotAsync()
//     //         .ContinueWith(task =>
//     //         {
//     //             if (task.IsCompletedSuccessfully)
//     //             {
//     //                 var result = new List<string>();
//     //                 foreach (var doc in task.Result.Documents)
//     //                 {
//     //                     result.Add(doc.Id); // 친구 UID
//     //                 }
//     //                 onComplete(result);
//     //             }
//     //         });
//     // }
// }

using System;
using System.Collections.Generic;
using System.Linq;
using Firebase.Auth;
using Firebase.Firestore;
using UnityEngine;

public class FriendManager : MonoBehaviour
{
    private FirebaseAuth auth;
    private FirebaseFirestore db;

    private string myUid => auth.CurrentUser?.UserId;
    private string myEmail => auth.CurrentUser?.Email;

    private void Awake()
    {
        auth = FirebaseAuth.DefaultInstance;
        db = FirebaseFirestore.DefaultInstance;
    }

    // 🔍 이메일로 UID 찾아서 콜백
    // private void FindUidByEmail(string email, Action<string> callback)
    // {
    //     db.Collection("users").WhereEqualTo("email", email).Limit(1).GetSnapshotAsync().ContinueWith(task =>
    //     {
    //         if (task.Result.Count == 0)
    //         {
    //             callback(null);
    //             return;
    //         }
    //
    //         var uid = task.Result.Documents[0].Id;
    //         callback(uid);
    //     });
    // }
    
    public async void FindUidByEmail(string email, Action<string> callback)
    {
        try
        {
            QuerySnapshot snapshot = await db.Collection("users")
                .WhereEqualTo("email", email)
                .Limit(1)
                .GetSnapshotAsync();

            if (snapshot.Count == 0)
            {
                Debug.Log("이메일로 등록된 유저 없음");
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

    // 📤 친구 요청 보내기 (이메일 기반)
    public void SendFriendRequestByEmail(string targetEmail, Action<bool> callback)
    {
        FindUidByEmail(targetEmail, targetUid =>
        {
            if (targetUid == null || targetUid == myUid)
            {
                callback?.Invoke(false);
                return;
            }

            var myRef = db.Collection("users").Document(myUid).Collection("friends").Document(targetUid);
            var targetRef = db.Collection("users").Document(targetUid).Collection("friends").Document(myUid);

            var requestData = new Dictionary<string, object>
            {
                { "status", "pending" },
                { "timestamp", Timestamp.GetCurrentTimestamp() }
            };
            var reverseData = new Dictionary<string, object>
            {
                { "status", "requested" },
                { "timestamp", Timestamp.GetCurrentTimestamp() }
            };

            myRef.SetAsync(requestData).ContinueWith(_ =>
            {
                targetRef.SetAsync(reverseData).ContinueWith(__ =>
                {
                    callback?.Invoke(true);
                });
            });
        });
    }

    // ✅ 친구 요청 수락 (상대 이메일 기준)
    public void AcceptFriendRequestByEmail(string targetEmail, Action<bool> callback)
    {
        FindUidByEmail(targetEmail, targetUid =>
        {
            if (targetUid == null)
            {
                callback?.Invoke(false);
                return;
            }

            var myRef = db.Collection("users").Document(myUid).Collection("friends").Document(targetUid);
            var targetRef = db.Collection("users").Document(targetUid).Collection("friends").Document(myUid);

            var data = new Dictionary<string, object>
            {
                { "status", "accepted" },
                { "timestamp", Timestamp.GetCurrentTimestamp() }
            };

            myRef.SetAsync(data).ContinueWith(_ =>
            {
                targetRef.SetAsync(data).ContinueWith(__ =>
                {
                    callback?.Invoke(true);
                });
            });
        });
    }

    // ❌ 친구 삭제 (이메일 기준)
    public void RemoveFriendByEmail(string targetEmail, Action<bool> callback)
    {
        FindUidByEmail(targetEmail, targetUid =>
        {
            if (targetUid == null)
            {
                callback?.Invoke(false);
                return;
            }

            var myRef = db.Collection("users").Document(myUid).Collection("friends").Document(targetUid);
            var targetRef = db.Collection("users").Document(targetUid).Collection("friends").Document(myUid);

            myRef.DeleteAsync().ContinueWith(_ =>
            {
                targetRef.DeleteAsync().ContinueWith(__ =>
                {
                    callback?.Invoke(true);
                });
            });
        });
    }

    // 📋 친구 목록 가져오기 (이메일 포함)
    public void GetFriendList(Action<List<string>> onComplete)
    {
        db.Collection("users").Document(myUid)
            .Collection("friends")
            .WhereEqualTo("status", "accepted")
            .GetSnapshotAsync()
            .ContinueWith(task =>
            {
                var result = new List<string>();
                if (task.IsCompletedSuccessfully)
                {
                    foreach (var doc in task.Result.Documents)
                    {
                        string friendUid = doc.Id;
                        // 이메일은 users/{friendUid}/email 필드로 다시 불러올 수 있음
                        db.Collection("users").Document(friendUid).GetSnapshotAsync().ContinueWith(emailTask =>
                        {
                            if (emailTask.Result.Exists && emailTask.Result.TryGetValue("email", out string email))
                            {
                                result.Add(email); // 이메일 기준으로 출력
                                if (result.Count == task.Result.Count)
                                    onComplete(result);
                            }
                        });
                    }
                }
            });
    }
}