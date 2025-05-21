using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAdapter : MonoBehaviour, IToUI
{
    private static UIAdapter instance;

    public static UIAdapter Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<UIAdapter>();

                if (instance == null)
                {
                    Debug.LogError("UIAdapter couldn't be found, You should add it to the scene.");
                }
            }
            return instance;
        }
    }
}
