using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class RoomUI : MonoBehaviour
{
    [SerializeField] Button startGameButton;
    [SerializeField] Button InviteButton;
    private void Awake()
    {
        startGameButton.interactable = false;
        SessionManager.Instance.callbacks.OnFulledRoom += () =>
        {
            startGameButton.interactable = true;
        };
        if (SessionManager.Instance.IsMasterClient == false)
        {
            gameObject.SetActive(false);
        }
        
        startGameButton.onClick.AddListener(OnStartGame);
    }

    private void OnStartGame()
    {
        if (SessionManager.Instance.CanStartGame == true)
        {
            gameObject.SetActive(false);
            GameManager.Instance.GameStart();
        }
    }
    
}
