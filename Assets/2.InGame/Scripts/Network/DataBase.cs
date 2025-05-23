using System;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class DataBase : NetworkBehaviour
{
    public List<PlayerInfo> players;
    public int playerCount;
    private void Awake()
    {
        players = new List<PlayerInfo>();
    }

    private void Update()
    {
        playerCount = players.Count;
    }

    public void AddPlayer(PlayerInfo playerInfo)
    {
        players.Add(playerInfo);
    }
}
