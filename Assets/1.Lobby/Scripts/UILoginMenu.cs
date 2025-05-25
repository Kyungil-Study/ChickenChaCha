using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UILoginMenu : MonoBehaviour
{
    [FormerlySerializedAs("inputLoginEmail")] [SerializeField] private TMP_InputField mInputLoginEmail; // TMP_InputField로 변경
    [FormerlySerializedAs("inputLoginPassword")] [SerializeField] private TMP_InputField mInputLoginPassword; // TMP_InputField로 변경
    [FormerlySerializedAs("inputSignUpEmail")] [SerializeField] private TMP_InputField mInputSignUpEmail; // TMP_InputField로 변경
    [FormerlySerializedAs("inputSignUpPassword")] [SerializeField] private TMP_InputField mInputSignUpPassword; // TMP_InputField로 변경
    [FormerlySerializedAs("inputSignUpNickname")] [SerializeField] private TMP_InputField mInputSignUpNickname; // TMP_InputField로 변경
    
    [FormerlySerializedAs("buttonStart")] [SerializeField] private Button mButtonLogin;
    [FormerlySerializedAs("buttonSignUp")] [SerializeField] private Button mButtonSignUp;
    
    //[FormerlySerializedAs("loginButton")] [SerializeField] private LoginButton mLoginButton;
    [FormerlySerializedAs("NotificationText")] [SerializeField] private TMP_Text mNotificationText;
    
    
    public UnityEvent<OnSignInEventArgs> OnLoginButtonClickedEvent;
    public UnityEvent<OnSignUpEventArgs> OnSignUpButtonClickedEvent;
    
    private void Awake()
    {
        mButtonLogin.onClick.AddListener(OnLoginButtonClicked);
        mButtonSignUp.onClick.AddListener(OnSignUpButtonClicked);
    }

    private void OnLoginButtonClicked()
    {
        OnSignInEventArgs args = new OnSignInEventArgs()
        {
            Email = mInputLoginEmail.text,
            Password = mInputLoginPassword.text
        };
        OnLoginButtonClickedEvent?.Invoke(args);
    }
    private void OnSignUpButtonClicked()
    {
        OnSignUpEventArgs args = new OnSignUpEventArgs()
        {
            Email = mInputSignUpEmail.text,
            Password = mInputSignUpPassword.text,
            Nickname = mInputSignUpNickname.text
        };
        OnSignUpButtonClickedEvent?.Invoke(args);
    }

}
