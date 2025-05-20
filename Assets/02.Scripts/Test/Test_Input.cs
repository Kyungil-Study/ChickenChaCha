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
    
    [SerializeField] private float moveDistance = 1f;  // 한 칸 이동 거리
    [SerializeField] private Vector3 moveDirection = Vector3.left; // 이동 방향
    [SerializeField] private NetworkPrefabRef cubePrefab; // Cube 프리팹 참조
    private NetworkObject spawnedCube;
    private Transform cubeTransform;
    
    public override void Spawned()
    {
        if (HasInputAuthority)
        {
            spawnedCube = Runner.Spawn(cubePrefab, transform.position + Vector3.forward, 
                Quaternion.identity, Object.InputAuthority);
            cubeTransform = spawnedCube.transform;
        }
    }
    
    public override void FixedUpdateNetwork()
    {
        if (!HasInputAuthority) return;

        if (GetInput(out MyInput input) && input.MouseClick)
        {
            Ray ray = Camera.main.ScreenPointToRay(input.ClickPosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                var action = hit.collider.GetComponent<Test_TileAction>();
                if (action != null)
                {
                    if (action.actionType == TileActionType.Move)
                    {
                        cubeTransform.position += moveDirection * moveDistance;
                        Debug.Log("🟢 Move 클릭됨 → 한 칸 전진!");
                    }
                    else
                    {
                        Debug.Log("🔴 Stop 클릭됨 → 정지!");
                    }
                }
            }
        }
    }

}