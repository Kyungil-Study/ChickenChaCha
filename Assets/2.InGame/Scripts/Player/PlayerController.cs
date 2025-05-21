using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerController : NetworkBehaviour
{
    private Camera camera;
    [SerializeField] private LayerMask tileLayer;

    private int number = 0;
    
    private void Awake()
    {
        camera = Camera.main;
    }
    void Update()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, tileLayer))
            {
                Debug.Log(hit.collider.gameObject.name);
                number++;
            }
            else
            {
                Debug.Log("Raycast hit nothing");
            }
        }
    }
    
    public override void FixedUpdateNetwork()
    {
        if (Object.HasInputAuthority == false) return;

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit, tileLayer))
            {
                Debug.Log("입력 성공");
            }
            else
            {
                Debug.Log("입력 실패");
            }
        }
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(20, 20, 200, 40), $"Last Clicked: {number.ToString()}");
    }
}
