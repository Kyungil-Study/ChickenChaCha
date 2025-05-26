using System.Collections;
using System.Collections.Generic;
using Firebase.Auth;
using UnityEngine;

public class LobbyPageSwapManager : MonoBehaviour
{
    [SerializeField] private GameObject loginPage;

    [SerializeField] private GameObject lobbyPage;
    
    
    private void Awake()
    {
        if (loginPage == null || lobbyPage == null)
        {
            Debug.LogError("LobbyPageSwapManager: Missing page references!");
            return;
        }
        AccountManagement.Instance.OnLogInEvent += OnLogIn;
    }

    private void OnLogIn(OnLogInEventArgs args)
    {
        // Start with the login page active
        loginPage.SetActive(false);
        lobbyPage.SetActive(true);
        
    }
}
