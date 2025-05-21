using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerController : NetworkBehaviour
{
    public float moveSpeed = 5f; // 이동 속도
    public float gravity = -9.81f; // 중력 값

    private CharacterController characterController;
    private Vector3 velocity; // 중력 적용을 위한 속도 벡터
    
    public MeshRenderer MeshRenderer;
    
    [Networked, OnChangedRender(nameof(ColorChanged))]
    public Color NetworkedColor { get; set; }
    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        
    }
    void Update()
    {
        if (HasStateAuthority && Input.GetKeyDown(KeyCode.E))
        {
            // Changing the material color here directly does not work since this code is only executed on the client pressing the button and not on every client.
            NetworkedColor = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f);
        }
        
        if (HasStateAuthority && Input.GetKeyDown(KeyCode.R))
        {
            GameManager.Instance.RPC_AddScore();
        }
        
    }
    void ColorChanged()
    {
        GameManager.Instance.ColorChanged(MeshRenderer, NetworkedColor);
    }

    
    public override void FixedUpdateNetwork()
    {
        // 방향키 입력 받기
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        // 이동 벡터 계산
        Vector3 move = transform.right * moveX + transform.forward * moveZ;

        // CharacterController를 사용하여 이동
        characterController.Move(move * (moveSpeed * Runner.DeltaTime));
    }
}
