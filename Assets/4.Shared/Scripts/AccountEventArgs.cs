using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnLogInEventArgs : EventArgs
{
    public string UserID;
}

public class OnAcceptFriendEventArgs : EventArgs
{
    public List<string> AcceptedEmails;
}

public class OnInviteEventArgs : EventArgs
{
    public string InviteRoomName;
}


public class OnSignInEventArgs : EventArgs
{
    public string Email;
    public string Password;
}
    
public class OnSignUpEventArgs : EventArgs
{
    public string Email;
    public string Password;
    public string Nickname;
}