using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.Serialization;

// 플레이어 입력 처리 받는 스크립트

public class InputHandler : NetworkBehaviour
{
    private Camera camera;
    [SerializeField] private LayerMask clickLayer;

    public Tile selectedTile;
    
    private bool IsMyTurn = false;
    private bool bClicked;
    
    public override void Spawned()
    {
        camera = Camera.main;
        bClicked = false;
        
        // networkPlayer = GetComponent<NetworkPlayer>();
    }
    
    void Update()
    {
        // if (IsMyTurn == false) return;   // 내 턴일때만 입력 가능하게 예외처리
        
        if (Input.GetKey(KeyCode.Mouse0))
        {
            bClicked = true;
        }

        if (Input.GetKey(KeyCode.Alpha1))
        {
            Debug.Log(selectedTile);
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
                var tile = hit.collider.gameObject.GetComponent<Tile>();
                Debug.Log($"[클릭 성공] {hit.collider.name}");
                
                SelectTileInfo(tile);
            }
            else
            {
                Debug.Log("[클릭 실패]");
            }
        }
    }
    
    public void SelectTileInfo(Tile tile)
    {
        selectedTile = tile;
        Debug.Log("선택된 타일 : " + selectedTile?.name);
    }
    
    private void OnGUI()
    {
        GUI.Label(new Rect(20, 20, 250, 30), $"Click Handled: {bClicked}");
    }
}