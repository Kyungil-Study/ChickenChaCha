using System;
using System.Collections.Generic;
using System.Linq;
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using Firebase.Firestore;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UserManager : MonoBehaviour
{
    private FirebaseAuth mAuth;
    private FirebaseFirestore mDB;

    [FormerlySerializedAs("inputSignUpEmail")]
    [Header("회원가입 UI")]
    [SerializeField] private TMP_InputField mInputSignUpEmail;
    [FormerlySerializedAs("inputSignUpPassword")] [SerializeField] private TMP_InputField mInputSignUpPassword;
    [FormerlySerializedAs("inputSignUpNickname")] [SerializeField] private TMP_InputField mInputSignUpNickname;
    [FormerlySerializedAs("buttonSignUp")] [SerializeField] private Button mButtonSignUp;

    [FormerlySerializedAs("inputLoginEmail")]
    [Header("로그인 UI")]
    [SerializeField] private TMP_InputField mInputLoginEmail;
    [FormerlySerializedAs("inputLoginPassword")] [SerializeField] private TMP_InputField mInputLoginPassword;
    [FormerlySerializedAs("buttonLogin")] [SerializeField] private Button mButtonLogin;

    [FormerlySerializedAs("inputFriendEmail")]
    [Header("친구 기능 UI")]
    [SerializeField] private TMP_InputField mInputFriendEmail;
    [FormerlySerializedAs("buttonSendRequest")] [SerializeField] private Button mButtonSendRequest;
    [FormerlySerializedAs("buttonAcceptRequest")] [SerializeField] private Button mButtonAcceptRequest;
    [FormerlySerializedAs("buttonRemoveFriend")] [SerializeField] private Button mButtonRemoveFriend;
    [FormerlySerializedAs("buttonShowFriends")] [SerializeField] private Button mButtonShowFriends;
    [FormerlySerializedAs("buttonShowRequests")] [SerializeField] private Button mButtonShowRequests;

    private void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                mAuth = FirebaseAuth.DefaultInstance;
                mDB = FirebaseFirestore.DefaultInstance;

                mButtonSignUp.onClick.AddListener(OnSignUpClicked);
                mButtonLogin.onClick.AddListener(OnLoginClicked);
                mButtonSendRequest.onClick.AddListener(OnSendFriendRequest);
                mButtonAcceptRequest.onClick.AddListener(OnAcceptFriendRequest);
                mButtonRemoveFriend.onClick.AddListener(OnRemoveFriend);
                mButtonShowFriends.onClick.AddListener(OnShowFriends);
                mButtonShowRequests.onClick.AddListener(OnShowRequests);
            }
            else
            {
                Debug.LogError("Firebase 초기화 실패");
            }
        });
    }

    private void OnSignUpClicked()
    {
        string email = mInputSignUpEmail.text;
        string password = mInputSignUpPassword.text;
        string nickname = mInputSignUpNickname.text;

        mAuth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompletedSuccessfully)
            {
                FirebaseUser user = task.Result.User;
                SaveUserToFirestore(user.UserId, email, nickname, password);
            }
            else
            {
                Debug.LogError("회원가입 실패: " + task.Exception?.Message);
            }
        });
    }

    private void OnLoginClicked()
    {
        string email = mInputLoginEmail.text;
        string password = mInputLoginPassword.text;

        mAuth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompletedSuccessfully)
            {
                Debug.Log("로그인 성공: " + task.Result.User.Email);
            }
            else
            {
                Debug.LogError("로그인 실패: " + task.Exception?.Message);
            }
        });
    }

    private void SaveUserToFirestore(string uid, string email, string nickname, string password)
    {
        var data = new Dictionary<string, object>
        {
            { "email", email },
            { "nickname", nickname },
            { "password", HashPassword(password) }
        };

        mDB.Collection("users").Document(uid).SetAsync(data);
    }

    private string HashPassword(string password)
    {
        var sha256 = System.Security.Cryptography.SHA256.Create();
        var bytes = System.Text.Encoding.UTF8.GetBytes(password);
        var hash = sha256.ComputeHash(bytes);
        return BitConverter.ToString(hash).Replace("-", "").ToLower();
    }

    private async void OnSendFriendRequest()
    {
        try
        {
            string friendEmail = mInputFriendEmail.text;
            string myUid = mAuth.CurrentUser?.UserId;
            string friendUid = await FindUidByEmail(friendEmail);

            if (myUid == null || friendUid == null || myUid == friendUid) return;

            var myRef = mDB.Collection("users").Document(myUid).Collection("friends").Document(friendUid);
            var theirRef = mDB.Collection("users").Document(friendUid).Collection("requests").Document(myUid);

            var myData = new Dictionary<string, object> { { "status", "pending" }, { "timestamp", Timestamp.GetCurrentTimestamp() } };
            var requestData = new Dictionary<string, object> { { "status", "received" }, { "timestamp", Timestamp.GetCurrentTimestamp() } };

            await myRef.SetAsync(myData);
            await theirRef.SetAsync(requestData);

            Debug.Log("친구 요청 전송 완료");
        }
        catch (Exception e)
        {
            Debug.LogError("친구 요청 중 오류: " + e.Message);
        }
    }

    private async void OnAcceptFriendRequest()
    {
        try
        {
            string requesterEmail = mInputFriendEmail.text;
            string myUid = mAuth.CurrentUser?.UserId;
            string requesterUid = await FindUidByEmail(requesterEmail);

            if (myUid == null || requesterUid == null) return;

            var myRef = mDB.Collection("users").Document(myUid).Collection("friends").Document(requesterUid);
            var requestRef = mDB.Collection("users").Document(myUid).Collection("requests").Document(requesterUid);
            var reMyRef = mDB.Collection("users").Document(requesterUid).Collection("friends").Document(myUid);
            var reRequestRef = mDB.Collection("users").Document(requesterUid).Collection("requests").Document(myUid);

            var data = new Dictionary<string, object>
            {
                { "status", "accepted" },
                { "timestamp", Timestamp.GetCurrentTimestamp() }
            };

            await myRef.SetAsync(data);
            await requestRef.DeleteAsync();
            await reMyRef.SetAsync(data);
            await reRequestRef.DeleteAsync();

            Debug.Log("친구 요청 수락 완료");
        }
        catch (Exception e)
        {
            Debug.LogError("친구 수락 중 오류: " + e.Message);
        }
    }

    private async void OnRemoveFriend()
    {
        try
        {
            string friendEmail = mInputFriendEmail.text;
            string myUid = mAuth.CurrentUser?.UserId;
            string friendUid = await FindUidByEmail(friendEmail);

            if (myUid == null || friendUid == null) return;

            await mDB.Collection("users").Document(myUid).Collection("friends").Document(friendUid).DeleteAsync();
            await mDB.Collection("users").Document(friendUid).Collection("friends").Document(myUid).DeleteAsync();
            Debug.Log("친구 삭제 완료");
        }
        catch (Exception e)
        {
            Debug.LogError("친구 삭제 중 오류: " + e.Message);
        }
    }

    private async void OnShowFriends()
    {
        try
        {
            string myUid = mAuth.CurrentUser?.UserId;
            if (myUid == null) return;

            var snapshot = await mDB.Collection("users").Document(myUid).Collection("friends")
                .WhereEqualTo("status", "accepted").GetSnapshotAsync();

            foreach (var doc in snapshot.Documents)
            {
                string friendUid = doc.Id;
                var userSnap = await mDB.Collection("users").Document(friendUid).GetSnapshotAsync();
                if (userSnap.Exists && userSnap.TryGetValue("email", out string email))
                {
                    Debug.Log("친구: " + email);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("친구 목록 로딩 실패: " + e.Message);
        }
    }

    private async void OnShowRequests()
    {
        try
        {
            string myUid = mAuth.CurrentUser?.UserId;
            if (myUid == null) return;

            var snapshot = await mDB.Collection("users").Document(myUid).Collection("requests").GetSnapshotAsync();

            foreach (var doc in snapshot.Documents)
            {
                string requesterUid = doc.Id;
                var userSnap = await mDB.Collection("users").Document(requesterUid).GetSnapshotAsync();
                if (userSnap.Exists && userSnap.TryGetValue("email", out string email))
                {
                    Debug.Log("받은 친구 요청: " + email);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("요청 목록 로딩 실패: " + e.Message);
        }
    }

    private async System.Threading.Tasks.Task<string> FindUidByEmail(string email)
    {
        try
        {
            var snapshot = await mDB.Collection("users")
                .WhereEqualTo("email", email)
                .Limit(1)
                .GetSnapshotAsync();

            return snapshot.Count > 0 ? snapshot.Documents.First().Id : null;
        }
        catch (Exception e)
        {
            Debug.LogError("UID 찾기 실패: " + e.Message);
            return null;
        }
    }
}
