using Fusion;
using UnityEngine;

[RequireComponent(typeof(NetworkObject))]
public class DontDestroyOnNetwork<T> : NetworkBehaviour where T : DontDestroyOnNetwork<T>
{
    // NetworkObject 스크립트에서 Is Master Client Object 옵션을 반드시 체크하세요.
    public static T Instance { get; private set; }
    protected virtual void Awake()
    {
        if (Instance == null)
        {
            Instance = this as T;
        }
    }
}
