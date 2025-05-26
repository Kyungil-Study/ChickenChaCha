using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainSystem : MonoBehaviour
{
    private static MainSystem mInstance;
    public static MainSystem Instance
    {
        get
        {
            if (mInstance== null)
            {
                mInstance = FindObjectOfType<MainSystem>();
                DontDestroyOnLoad(mInstance.gameObject);
            }
            return mInstance;
        }
    }

    private void Awake()
    {
        if (mInstance== null)
        {
            mInstance = FindObjectOfType<MainSystem>();
            DontDestroyOnLoad(mInstance.gameObject);
        }
        else if(Instance!=this)
        {
            Destroy(gameObject);
        }
    }
}
