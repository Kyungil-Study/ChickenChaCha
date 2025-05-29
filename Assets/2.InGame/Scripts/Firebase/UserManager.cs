using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using Firebase.Firestore;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public enum EUserState
{
    LoggedOut,
    LggingIn,
    LoggedIn,
}

public class UserManager : MonoBehaviour
{
    private static UserManager mInstance;
    public static UserManager Instance
    {
        get
        {
            if (mInstance == null)
            {
                mInstance = FindObjectOfType<UserManager>();
            }
            return mInstance;
        }
        
    }
    
    
    public class GameUser
    {
        public FirebaseUser User;
        public string NickName = "Anonymous";
        public string Email => User.IsAnonymous ? "No Email" : User.Email;
        
        public bool IsAnonymous => User.IsAnonymous;
    }
    
    private FirebaseApp mApp;
    private FirebaseAuth mAuth;
    private FirebaseFirestore mDB;
    private GameUser mUser = new GameUser();
    
    private int mUserCnt = 0; // 중복 로그인 확인용 변수

    public FirebaseApp App => mApp;
    public FirebaseAuth Auth => mAuth;
    public FirebaseFirestore DB => mDB;

    
    public GameUser User => mUser;
    
    private bool mIsInitialized = false;
    public EUserState UserState { get; private set; } = EUserState.LoggedOut;
    public event Action<OnLogInEventArgs> OnLogInEvent;

    private async void Start()
    {
        Debug.Log("UserManager Start");

        DependencyStatus dependencyStatus = await FirebaseApp.CheckAndFixDependenciesAsync();
        if (dependencyStatus == DependencyStatus.Available)
        {
            AppOptions options = new AppOptions()
            {
                ApiKey = FirebaseApp.DefaultInstance.Options.ApiKey,
                AppId = FirebaseApp.DefaultInstance.Options.AppId,
                DatabaseUrl = FirebaseApp.DefaultInstance.Options.DatabaseUrl,
                MessageSenderId = FirebaseApp.DefaultInstance.Options.MessageSenderId,
                ProjectId = FirebaseApp.DefaultInstance.Options.ProjectId,
                StorageBucket = FirebaseApp.DefaultInstance.Options.StorageBucket
            };
            var app = FirebaseApp.Create(options, Guid.NewGuid().ToString());
                
            mAuth = FirebaseAuth.GetAuth(app);
            mDB = FirebaseFirestore.GetInstance(app);
                
            /*
            mButtonSendRequest.onClick.AddListener(OnSendFriendRequest);
            mButtonRemoveFriend.onClick.AddListener(OnRemoveFriend);
            mButtonShowFriends.onClick.AddListener(OnShowFriends);
            */

            mIsInitialized = true;
            Debug.Log("Firebase 초기화 성공");
        }
        else
        {
            Debug.LogError("Firebase 초기화 실패");
        }
    }
    
    public void OnGhostLoginButtonClicked()
    {
        AnoymousLogin();
    }

    private void AnoymousLogin()
    {
        try
        {
            if (mIsInitialized == false) 
            {
                Debug.LogError("Firebase가 초기화되지 않았습니다.");
                return;
            }

            if (UserState != EUserState.LoggedOut)
            {
                Debug.Log("이미 로그인 중입니다.");
                return;
            }
            UserState = EUserState.LggingIn;

            mAuth.SignInAnonymouslyAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError("로그인 중 알 수 없는 예외 발생: " + task.Exception?.Message);
                    return;
                }

                if (task.IsCompleted)
                {
                    FirebaseUser newUser = task.Result.User;
                    Debug.Log("로그인 성공: " + newUser.Email);
                    FriendManager.MyUid = newUser?.UserId;
                    Debug.Log("CurrentUser: " + newUser?.Email);

                    OnLogIn(newUser);
                }

            });
        }
        catch (Exception ex)
        {
            Debug.LogError("SignIn에서 예외 발생: " + ex.Message);
            UserState = EUserState.LoggedOut;
        }
    }

    public void OnLoginButtonClicked(OnSignInEventArgs args)
    {
        SignIn(args.Email, args.Password);
    }
    public void OnSignUpButtonClicked(OnSignUpEventArgs args)
    {
        CreateAccount(args.Email, args.Password, args.Nickname);
    }
    
    public event Action OnLoadedUserInfomation;
    private void OnLogIn(FirebaseUser newUser)
    {
        mUser.User = newUser;
        LoadUserInformation();
        OnLogInEventArgs args = new OnLogInEventArgs()
        {
            UserID = mUser.User.UserId
        };
        OnLogInEvent?.Invoke(args);
        UserState = EUserState.LoggedIn;
        Debug.Log($"로그인 완료: {mUser.NickName}");
       
    }
    
    public async void OnlyOne(FirebaseUser user) // 중복 로그인 체크 함수
    {
        if (string.IsNullOrEmpty(user?.UserId))
        {
            Debug.LogError("❌ 로그인 유저 정보 없음. 중복 로그인 체크 실패.");
            return;
        }
        
        var userRef = mDB.Collection("users").Document(user.UserId);

        try
        {
            var doc = await userRef.GetSnapshotAsync();
            if (doc.Exists && doc.ContainsField("isOnline") && doc.GetValue<bool>("isOnline"))
            {
                Debug.LogWarning("🚫 이미 접속 중인 계정입니다!");
                mUserCnt++; // 중복일 시 변화
                Application.Quit();
            }
            else
            {
                await userRef.UpdateAsync(new Dictionary<string, object> {
                    { "isOnline", true },
                    { "lastLoginAt", Timestamp.GetCurrentTimestamp() }
                });
            }
        }
        catch (Exception e)
        {
            Debug.LogError("❌ 중복 로그인 확인 실패: " + e.Message);
        }
    }

    
    private void OnApplicationQuit() // 유니티 생명 주기(앱이 꺼질 때(종료될 때))
    {
        if (mUserCnt == 0)
        {
            var userRef = mDB.Collection("users").Document(mUser.User.UserId);
            userRef.UpdateAsync(new Dictionary<string, object> {
                { "isOnline", false }
            });
        }
    }
    
    private void SignIn(string email, string password)
    {
        try
        {
            if (mIsInitialized == false) 
            {
                Debug.LogError("Firebase가 초기화되지 않았습니다.");
                return;
            }

            mAuth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    if (task.Exception != null && task.Exception.InnerException is FirebaseException firebaseEx)
                    {
                        var errorCode = ((FirebaseException)firebaseEx).ErrorCode;

                        if (errorCode == (int)AuthError.UserNotFound)
                        {
                            Debug.LogError("Email not found");
                        }
                        else if (errorCode == (int)AuthError.WrongPassword)
                        {
                            Debug.LogError("Password is incorrect");
                        }
                        else
                        {
                            Debug.LogError("로그인 중 예외 발생: " + firebaseEx.Message);
                        }
                    }
                    else
                    {
                        Debug.LogError("로그인 중 알 수 없는 예외 발생: " + task.Exception?.Message);
                    }

                    return;
                }

                if (task.IsCompleted)
                {
                    FirebaseUser newUser = task.Result.User;
                    Debug.Log("로그인 성공: " + newUser.Email);
                    FriendManager.MyUid = newUser?.UserId;
                    Debug.Log("CurrentUser: " + newUser?.Email);
                    OnlyOne(newUser);
                    OnLogIn(newUser);
                }

            });
        }
        catch (Exception ex)
        {
            Debug.LogError("SignIn에서 예외 발생: " + ex.Message);
        }
    }
    
    private void CreateAccount(string email, string password, string nickname)
    {
        try
        {
            if (mIsInitialized == false)
            {
                Debug.LogError("Firebase가 초기화되지 않았습니다.");
                return;
            }

            mAuth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted && !task.IsFaulted)
                {
                    FirebaseUser newUser = task.Result.User;
                    Debug.Log("회원가입 성공");

                    SaveUserToFirestore(newUser.UserId, email, nickname, password);
                }
                else
                {
                    Debug.Log("회원가입 실패: " + task.Exception?.Message);
                }
            });
        }
        catch (Exception ex)
        {
            Debug.LogError("CreateAccount에서 예외 발생: " + ex.Message);
        }
    }

    private void SaveUserToFirestore(string uid, string email, string nickname, string password)
    {
        var data = new Dictionary<string, object>
        {
            { "email", email },
            { "nickname", nickname },
            { "password", HashPassword(password) },
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

    public async void OnSendFriendRequest(string email)
    {
        try
        {
            string friendEmail = email;
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

    public async void OnAcceptFriendRequest(OnAcceptFriendEventArgs args, Action OnComplete)
    {
        try
        {
            var emails = args.AcceptedEmails;

            foreach (var email in emails)
            {
                string requesterEmail = email;
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
            }
            
            

            Debug.Log("친구 요청 수락 완료");
        }
        catch (Exception e)
        {
            Debug.LogError("친구 수락 중 오류: " + e.Message);
        }
        
        OnComplete.Invoke();
    }

    public async void OnRemoveFriend(List<string> removeEmails, Action OnComplete)
    {
        try
        {
            string myUid = mAuth.CurrentUser?.UserId;

            foreach (var friendEmail in removeEmails)
            {
                string friendUid = await FindUidByEmail(friendEmail);
                if (myUid == null || friendUid == null) return;
                await mDB.Collection("users").Document(myUid).Collection("friends").Document(friendUid).DeleteAsync();
                await mDB.Collection("users").Document(friendUid).Collection("friends").Document(myUid).DeleteAsync();
                
                Debug.Log($"친구 삭제 {friendEmail}");

            }
            Debug.Log("친구 삭제 완료");
        }
        catch (Exception e)
        {
            Debug.LogError("친구 삭제 중 오류: " + e.Message);
        }
        
        OnComplete?.Invoke();
    }
    
    public List<string> FriendList { get; private set; } = new List<string>();
    public async void OnShowFriends(Action OnCompleteTask)
    {
        try
        {
            string myUid = mAuth.CurrentUser?.UserId;
            if (myUid == null) return;

            var snapshot = await mDB.Collection("users").Document(myUid).Collection("friends")
                .WhereEqualTo("status", "accepted").GetSnapshotAsync();

            FriendList.Clear();
            foreach (var doc in snapshot.Documents)
            {
                string friendUid = doc.Id;
                var userSnap = await mDB.Collection("users").Document(friendUid).GetSnapshotAsync();
                if (userSnap.Exists && userSnap.TryGetValue("email", out string email))
                {
                    Debug.Log("친구: " + email);
                    
                    FriendList.Add(email);
                }
            }
            
        }
        catch (Exception e)
        {
            Debug.LogError("친구 목록 로딩 실패: " + e.Message);
        }
        
        OnCompleteTask?.Invoke();
    }

    
    private List<string> mRequestFriendList = new List<string>();
    public List<string> RequestFriendList => mRequestFriendList;
    public async void OnShowRequests(Action OnCompleteTask)
    {
        Debug.Log($"OnShowRequests 호출됨");
        mRequestFriendList.Clear();
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
                    mRequestFriendList.Add(email);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("요청 목록 로딩 실패: " + e.Message);
        }
        OnCompleteTask?.Invoke();
    }

    
    private async void LoadUserInformation()
    {
        try
        {
             await mDB.Collection("users").Document(mAuth.CurrentUser.UserId).GetSnapshotAsync().ContinueWithOnMainThread(
                taks =>
                {
                    var userSnap = taks.Result;
                    if (userSnap.Exists && userSnap.TryGetValue("nickname", out string nickname))
                    {
                        mUser.NickName = nickname;
                    }
                    OnLoadedUserInfomation?.Invoke();
                    Debug.Log($"사용자 이름 로드 완료: {mUser.NickName}");
                });
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        
        
    }

    private async Task<string> FindUidByEmail(string email)
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
