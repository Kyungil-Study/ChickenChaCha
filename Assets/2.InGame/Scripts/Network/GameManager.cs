using System;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using UnityEngine;

public class GameManager : DontDestroyOnNetwork<GameManager>
{
    public NetworkPlayer[] players;
    
    private void Update()
    {
        if (Runner.IsSharedModeMasterClient && Input.GetKeyDown(KeyCode.Space))
        {
            GameStart();
        }
    }
    
    private void GameStart()
    {
        Debug.Log("게임 시작");
        BoardManager.Instance.InitBoard(players);
    }
}
