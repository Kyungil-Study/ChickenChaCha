using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using Firebase.Firestore;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UserManager : MonoBehaviour
{
    private FirebaseAuth auth;
    private FirebaseFirestore db;

    private bool isInitialized = false;

    [Header("회원가입 UI")]
    [SerializeField] private TMP_InputField inputSignUpEmail;
    [SerializeField] private TMP_InputField inputSignUpPassword;
    [SerializeField] private TMP_InputField inputSignUpNickname;
    [SerializeField] private Button buttonSignUp;

    [Header("로그인 UI")]
    [SerializeField] private TMP_InputField inputLoginEmail;
    [SerializeField] private TMP_InputField inputLoginPassword;
    [SerializeField] private Button buttonLogin;

    [Header("친구 기능 UI")]
    [SerializeField] private TMP_InputField inputFriendEmail;
    [SerializeField] private Button buttonAddFriend;
    [SerializeField] private Button buttonRemoveFriend;
    [SerializeField] private Button buttonShowFriends;

    private void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                auth = FirebaseAuth.DefaultInstance;
                db = FirebaseFirestore.DefaultInstance;
                isInitialized = true;
                Debug.Log("<color=green>Firebase 초기화 완료</color>");

                buttonLogin.onClick.AddListener(OnLoginClicked);
                buttonSignUp.onClick.AddListener(OnSignUpClicked);
                buttonAddFriend.onClick.AddListener(OnAddFriendClicked);
                buttonRemoveFriend.onClick.AddListener(OnRemoveFriendClicked);
                buttonShowFriends.onClick.AddListener(OnShowFriendsClicked);
            }
            else
            {
                Debug.LogError("Firebase 초기화 실패");
            }
        });
    }

    #region 로그인 / 회원가입

    private void OnSignUpClicked()
    {
        string email = inputSignUpEmail.text;
        string password = inputSignUpPassword.text;
        string nickname = inputSignUpNickname.text;

        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompletedSuccessfully)
            {
                FirebaseUser user = task.Result.User;
                Debug.Log("회원가입 성공: " + user.Email);

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
        string email = inputLoginEmail.text;
        string password = inputLoginPassword.text;

        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompletedSuccessfully)
            {
                FirebaseUser user = task.Result.User;
                Debug.Log("<color=green>로그인 성공: " + user.Email + "</color>");
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
            { "password", HashPassword(password) } // 여기 중요!
        };

        db.Collection("users").Document(uid).SetAsync(data).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompletedSuccessfully)
            {
                Debug.Log("Firestore에 사용자 정보 저장 완료");
            }
            else
            {
                Debug.LogError("Firestore 저장 실패: " + task.Exception?.Message);
            }
        });
    }
    
    private string HashPassword(string password)
    {
        var sha256 = System.Security.Cryptography.SHA256.Create();
        var bytes = System.Text.Encoding.UTF8.GetBytes(password);
        var hash = sha256.ComputeHash(bytes);
        return BitConverter.ToString(hash).Replace("-", "").ToLower();
    }

    #endregion

    #region 친구 기능

    private void OnAddFriendClicked()
    {
        string friendEmail = inputFriendEmail.text;
        AddFriendByEmail(friendEmail, success =>
        {
            Debug.Log(success ? "친구 추가 성공" : "친구 추가 실패");
        });
    }

    private void OnRemoveFriendClicked()
    {
        string friendEmail = inputFriendEmail.text;
        RemoveFriendByEmail(friendEmail, success =>
        {
            Debug.Log(success ? "친구 삭제 성공" : "친구 삭제 실패");
        });
    }

    private void OnShowFriendsClicked()
    {
        GetFriendList(friendEmails =>
        {
            Debug.Log("📋 친구 목록:");
            foreach (var email in friendEmails)
                Debug.Log("- " + email);
        });
    }

    private async void AddFriendByEmail(string friendEmail, Action<bool> callback)
    {
        if (auth.CurrentUser == null)
        {
            Debug.LogError("로그인되지 않았습니다.");
            callback(false);
            return;
        }

        string myUid = auth.CurrentUser.UserId;

        string friendUid = await FindUidByEmail(friendEmail);
        if (friendUid == null || friendUid == myUid)
        {
            callback(false);
            return;
        }

        var data = new Dictionary<string, object>
        {
            { "status", "accepted" },
            { "timestamp", Timestamp.GetCurrentTimestamp() }
        };

        var myRef = db.Collection("users").Document(myUid).Collection("friends").Document(friendUid);
        //var friendRef = db.Collection("users").Document(friendUid).Collection("friends").Document(myUid);

        await myRef.SetAsync(data);
        //await friendRef.SetAsync(data);

        callback(true);
    }

    private async void RemoveFriendByEmail(string friendEmail, Action<bool> callback)
    {
        if (auth.CurrentUser == null)
        {
            Debug.LogError("로그인되지 않았습니다.");
            callback(false);
            return;
        }

        string myUid = auth.CurrentUser.UserId;
        string friendUid = await FindUidByEmail(friendEmail);
        if (friendUid == null)
        {
            callback(false);
            return;
        }

        await db.Collection("users").Document(myUid).Collection("friends").Document(friendUid).DeleteAsync();
        //await db.Collection("users").Document(friendUid).Collection("friends").Document(myUid).DeleteAsync();

        callback(true);
    }

    private async void GetFriendList(Action<List<string>> callback)
    {
        var result = new List<string>();
        if (auth.CurrentUser == null)
        {
            callback(result);
            return;
        }

        string myUid = auth.CurrentUser.UserId;

        var snapshot = await db.Collection("users").Document(myUid).Collection("friends")
                               .WhereEqualTo("status", "accepted").GetSnapshotAsync();

        foreach (var doc in snapshot.Documents)
        {
            string friendUid = doc.Id;
            var friendSnap = await db.Collection("users").Document(friendUid).GetSnapshotAsync();
            if (friendSnap.Exists && friendSnap.TryGetValue("email", out string email))
            {
                result.Add(email);
            }
        }

        callback(result);
    }

    private async System.Threading.Tasks.Task<string> FindUidByEmail(string email)
    {
        var snapshot = await db.Collection("users")
                               .WhereEqualTo("email", email)
                               .Limit(1)
                               .GetSnapshotAsync();

        return snapshot.Count > 0 ? snapshot.Documents.First().Id : null;
    }

    #endregion
}