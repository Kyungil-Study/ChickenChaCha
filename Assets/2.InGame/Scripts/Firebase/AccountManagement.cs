using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Firebase;
using Firebase.Firestore;
using Firebase.Auth;
using Firebase.Extensions;
using TMPro;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class AccountManagement : MonoBehaviour
{
    private FirebaseAuth mAuth;
    private bool mIsInitialized = false;
    private string mStatusMessage = "";
    private bool mIsLoggedIn = false;
    
    [FormerlySerializedAs("inputLoginEmail")] [SerializeField] private TMP_InputField mInputLoginEmail; // TMP_InputField로 변경
    [FormerlySerializedAs("inputLoginPassword")] [SerializeField] private TMP_InputField mInputLoginPassword; // TMP_InputField로 변경
    [FormerlySerializedAs("inputSignUpEmail")] [SerializeField] private TMP_InputField mInputSignUpEmail; // TMP_InputField로 변경
    [FormerlySerializedAs("inputSignUpPassword")] [SerializeField] private TMP_InputField mInputSignUpPassword; // TMP_InputField로 변경
    [FormerlySerializedAs("inputSignUpNickname")] [SerializeField] private TMP_InputField mInputSignUpNickname; // TMP_InputField로 변경
    
    [FormerlySerializedAs("buttonStart")] [SerializeField] private Button mButtonLogin;
    [FormerlySerializedAs("buttonSignUp")] [SerializeField] private Button mButtonSignUp;
    
    //[FormerlySerializedAs("loginButton")] [SerializeField] private LoginButton mLoginButton;
    [FormerlySerializedAs("NotificationText")] [SerializeField] private TMP_Text mNotificationText;
    
    private Dictionary<string, FirebaseAuth> mPlayerAuths = new Dictionary<string, FirebaseAuth>();
    private Dictionary<string, FirebaseFirestore> mPlayerFirestore = new Dictionary<string, FirebaseFirestore>();
    
    
    private void Start()
    {
        mButtonLogin.interactable = false;
        mButtonSignUp.interactable = false;
        InitFirebase();

    }

    private void InitFirebase()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                FirebaseApp app = FirebaseApp.DefaultInstance;
                mAuth = FirebaseAuth.DefaultInstance;
                mIsInitialized = true;
                mStatusMessage = "Firebase 초기화 성공";
                Debug.Log(mStatusMessage);

                // 초기화가 완료되면 버튼 활성화
                mButtonLogin.interactable = true;
                mButtonSignUp.interactable = true;
            }
            else
            {
                mStatusMessage = "Firebase 초기화 실패";
                Debug.Log(mStatusMessage);
            }
        });
    }

    public void OnLoginButtonClicked()
    {
        string email = mInputLoginEmail.text;
        string password = mInputLoginPassword.text;

        SignIn(email, password);

    }

    // private void SignIn(string email, string password)
    // {
    //     mStatusMessage = "로그인 시도 중..."; 
    //     Debug.Log(mStatusMessage);
    //     mAuth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
    //     {
    //         if (task.IsCanceled || task.IsFaulted)
    //         {
    //             mStatusMessage = "로그인 실패: " + task.Exception?.Message;
    //             Debug.Log(mStatusMessage);
    //             return;
    //         }
    //
    //         var result = task.Result;
    //         FirebaseUser user = result.User;
    //         MainSystem.Instance.SetUserData(user);
    //         mIsLoggedIn = true;
    //         mStatusMessage = $"로그인 성공: {user.Email}";
    //         Debug.Log(mStatusMessage);
    //     });
    // }

    public void OnSignUpButtonClicked()
    {
        string email = mInputSignUpEmail.text;
        string password = mInputSignUpPassword.text;
        string nickname = mInputSignUpNickname.text;
        CreateAccount(email, password, nickname);
    }

    private void CreateAccount(string email, string password, string nickname)
    {
        try
        {
            if (!mIsInitialized)
            {
                Debug.LogError("Firebase가 초기화되지 않았습니다.");
                return;
            }

            // 고유한 FirebaseApp 생성 (플레이어마다)
            FirebaseApp playerApp = FirebaseApp.Create(new AppOptions
            {
                ProjectId = FirebaseApp.DefaultInstance.Options.ProjectId,
                ApiKey = FirebaseApp.DefaultInstance.Options.ApiKey,
                AppId = FirebaseApp.DefaultInstance.Options.AppId
            }, email);

            // 각 플레이어의 FirebaseAuth와 Firestore 인스턴스 독립적 관리
            FirebaseAuth playerAuth = FirebaseAuth.GetAuth(playerApp);
            FirebaseFirestore firestore = FirebaseFirestore.GetInstance(playerApp);

            // 플레이어 별로 FirebaseAuth, Firestore 저장
            mPlayerAuths[email] = playerAuth;
            mPlayerFirestore[email] = firestore;

            playerAuth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted && !task.IsFaulted)
                {
                    FirebaseUser newUser = task.Result.User;
                    mStatusMessage = "회원가입 성공";

                    SaveUserToFirestore(newUser.UserId, email, HashPassword(password), nickname, email);
                }
                else
                {
                    mStatusMessage = "회원가입 실패: " + task.Exception?.Message;
                    mNotificationText.gameObject.SetActive(true);
                    mNotificationText.text = "already ID exist";
                    Debug.Log(mStatusMessage);
                }
            });
        }
        catch (Exception ex)
        {
            Debug.LogError("CreateAccount에서 예외 발생: " + ex.Message);
        }
    }
    
    private void SaveUserToFirestore(string userId, string email, string hashedPassword, string nickname,
        string playerEmail)
    {
        if (!mPlayerFirestore.ContainsKey(playerEmail))
        {
            Debug.LogError("Firestore 인스턴스를 찾을 수 없습니다.");
            return;
        }

        FirebaseFirestore db = mPlayerFirestore[playerEmail];
        DocumentReference userDocRef = db.Collection("users").Document(userId);

        Dictionary<string, object> userData = new Dictionary<string, object>
        {
            { "email", email },
            { "password", hashedPassword },
            { "nickname", nickname }
        };

        userDocRef.SetAsync(userData).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted && !task.IsFaulted)
            {
                Debug.Log("<color=green>Firestore에 사용자 정보 저장 성공</color>");
            }
            else
            {
                Debug.Log("<color=red>Firestore에 사용자 정보 저장 실패: " + task.Exception?.Message + "</color>");
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



    private void SignIn(string email, string password)
    {
        try
        {
            if (!mIsInitialized)
            {
                Debug.LogError("Firebase가 초기화되지 않았습니다.");
                return;
            }

            // 고유한 FirebaseApp 생성 (플레이어마다)
            FirebaseApp playerApp = FirebaseApp.Create(new AppOptions
            {
                ProjectId = FirebaseApp.DefaultInstance.Options.ProjectId,
                ApiKey = FirebaseApp.DefaultInstance.Options.ApiKey,
                AppId = FirebaseApp.DefaultInstance.Options.AppId
            }, email);

            // 각 플레이어의 FirebaseAuth와 Firestore 인스턴스 독립적 관리
            FirebaseAuth playerAuth = FirebaseAuth.GetAuth(playerApp);
            FirebaseFirestore firestore = FirebaseFirestore.GetInstance(playerApp);

            mPlayerAuths[email] = playerAuth;
            mPlayerFirestore[email] = firestore;

            playerAuth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
            {

                if (task.IsFaulted)
                {
                    if (task.Exception != null && task.Exception.InnerException is FirebaseException firebaseEx)
                    {
                        var errorCode = ((FirebaseException)firebaseEx).ErrorCode;

                        // 이메일이 없을 때
                        if (errorCode == (int)AuthError.UserNotFound)
                        {
                            mNotificationText.gameObject.SetActive(true);
                            mNotificationText.text = "Email not found";
                        }
                        // 비밀번호가 틀렸을 때
                        else if (errorCode == (int)AuthError.WrongPassword)
                        {
                            mNotificationText.gameObject.SetActive(true);
                            mNotificationText.text = "Password is incorrect";
                        }
                        else
                        {
                            mNotificationText.gameObject.SetActive(true);
                            mNotificationText.text = "ex";
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

                    mStatusMessage = "로그인 성공";
                    mIsLoggedIn = true;
                    Debug.Log(mStatusMessage);

                    LoadUserEmailAndPasswordFromFirestore(newUser.UserId, email);
                    //mLoginButton.OnStartButton();
                }

            });
        }
        catch (Exception ex)
        {
            Debug.LogError("SignIn에서 예외 발생: " + ex.Message);
        }
    }

    // Firestore에서 사용자 이메일과 비밀번호 불러오기 (사용자별 Firestore 사용)
    private void LoadUserEmailAndPasswordFromFirestore(string userId, string email)
    {
        if (!mPlayerFirestore.ContainsKey(email))
        {
            Debug.LogError("Firestore 인스턴스를 찾을 수 없습니다.");
            return;
        }

        FirebaseFirestore db = mPlayerFirestore[email];
        DocumentReference userDocRef = db.Collection("users").Document(userId);

        userDocRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Firestore에서 사용자 정보 로드 실패: " + task.Exception?.Message);
                return;
            }

            if (task.IsCompleted)
            {
                var snapshot = task.Result;
                if (snapshot.Exists)
                {
                    string userEmail = snapshot.GetValue<string>("email");
                    string userPassword = snapshot.ContainsField("password")
                        ? snapshot.GetValue<string>("password")
                        : "비밀번호 없음";
                    string userNickname = snapshot.GetValue<string>("nickname");

                    Debug.Log($"사용자 이메일: {userEmail}");
                    Debug.Log($"사용자 비밀번호 (해시): {userPassword}");
                    Debug.Log($"사용자 닉네임: {userNickname}");

                    // savePlayerBasicStat.Email = userEmail;
                    // savePlayerBasicStat.Password = userPassword;
                    // savePlayerBasicStat.Nickname = userNickname;
                }
                else
                {
                    Debug.LogWarning("Firestore에서 사용자 정보를 찾을 수 없습니다.");
                }
            }
        });
    }

    // Firestore에서 사용자 이메일과 비밀번호 불러오기
    private void LoadUserEmailAndPasswordFromFirestore(string userId)
    {
        FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
        DocumentReference userDocRef = db.Collection("users").Document(userId);

        userDocRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Firestore에서 사용자 정보 로드 실패: " + task.Exception?.Message);
                return;
            }

            if (task.IsCompleted)
            {
                var snapshot = task.Result;
                if (snapshot.Exists)
                {
                    string userEmail = snapshot.GetValue<string>("email");
                    string userPassword = snapshot.ContainsField("password")
                        ? snapshot.GetValue<string>("password")
                        : "비밀번호 없음";
                    string userNickname = snapshot.GetValue<string>("nickname");

                    Debug.Log($"사용자 이메일: {userEmail}");
                    Debug.Log($"사용자 비밀번호 (해시): {userPassword}");
                    Debug.Log($"사용자 닉네임: {userNickname}");
                }
                else
                {
                    Debug.LogWarning("Firestore에서 사용자 정보를 찾을 수 없습니다.");
                }
            }
        });
    }
}
