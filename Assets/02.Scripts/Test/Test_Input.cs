using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class Test_Input : NetworkBehaviour
{
    void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 300, 20), $"HasInputAuthority: {Object.HasInputAuthority}");
    }
    
    public override void FixedUpdateNetwork()
    {
        if (GetInput(out MyInput input))
        {
            if (input.MouseClick)
            {
                Debug.Log("마우스 클릭 입력 감지됨!");
            }
        }
    }
}