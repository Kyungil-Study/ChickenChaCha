using System.Collections;
using System.Collections.Generic;
using Firebase.Auth;
using Firebase.Extensions;
using UnityEngine;

public class TestChat : MonoBehaviour
{
    [SerializeField] private ChatUI chatUI;

    private void Start()
    {
        // Firebase 익명 로그인
        FirebaseAuth.DefaultInstance.SignInAnonymouslyAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompletedSuccessfully)
            {
                FirebaseUser user = task.Result.User;
                string userId = user.UserId;
                string nickname = "익명_" + userId.Substring(0, 6);

                Debug.Log($"[TestChat] 익명 로그인 성공: {nickname}");
                chatUI.InitializeUser(userId, nickname);
            }
            else
            {
                Debug.LogError("[TestChat] 익명 로그인 실패: " + task.Exception?.Message);
            }
        });
    }
}
