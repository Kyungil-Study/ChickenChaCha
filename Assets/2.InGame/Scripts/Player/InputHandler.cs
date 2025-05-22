using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.Serialization;

public class InputHandler : NetworkBehaviour
{
    private Camera camera;
    [SerializeField] private LayerMask clickLayer;

    private bool bClicked;
    private float clickTime;
    private Ray ray;
    private int number = 0;

    public override void Spawned()
    {
        camera = Camera.main;
        bClicked = false;
    }
    
    void Update()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            bClicked = true;
        }
    }
    public override void FixedUpdateNetwork()
    {
        if (bClicked)
        {
            bClicked = false; // 플래그 초기화

            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit, Mathf.Infinity, clickLayer))
            {
                Debug.Log($"[클릭 성공] {hit.collider.name}");

                // 매니저 클래스나 로직 전달
                // var selectable = hit.collider.GetComponent<ISelectable>();
                // if (selectable != null)
                // {
                //     
                // }
            }
            else
            {
                Debug.Log("[클릭 실패]");
            }
        }
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(20, 20, 250, 30), $"Click Handled: {bClicked}");
    }
}
