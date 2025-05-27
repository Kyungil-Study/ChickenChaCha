using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UILoginMenu : UISingleton<UILoginMenu>
{
    [Header("UI Login")]
    [SerializeField] private TMP_InputField mInputLoginEmail; // TMP_InputField로 변경
    [SerializeField] private TMP_InputField mInputLoginPassword; // TMP_InputField로 변경
    
    [Space(10)]
    [Header("UI Sign Up")]
    [SerializeField] private TMP_InputField mInputSignUpEmail; // TMP_InputField로 변경
    [SerializeField] private TMP_InputField mInputSignUpPassword; // TMP_InputField로 변경
    [SerializeField] private TMP_InputField mInputSignUpNickname; // TMP_InputField로 변경
    
    [Space(10)]
    [Header("UI Buttons Refs")]
    [SerializeField] private Button mButtonLogin;
    [SerializeField] private Button mButtonSignUp;
    
    [Space(10)]
    [Header("UI Buttons Events")]
    public UnityEvent<OnSignInEventArgs> OnLoginButtonClickedEvent;
    public UnityEvent<OnSignUpEventArgs> OnSignUpButtonClickedEvent;
    
    private void Awake()
    {
        mButtonLogin.onClick.AddListener(OnLoginButtonClicked);
        mButtonSignUp.onClick.AddListener(OnSignUpButtonClicked);
    }

    public void OnLoginButtonClicked()
    {
        OnSignInEventArgs args = new OnSignInEventArgs()
        {
            Email = mInputLoginEmail.text,
            Password = mInputLoginPassword.text
        };
        OnLoginButtonClickedEvent?.Invoke(args);
    }
    public void OnSignUpButtonClicked()
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
