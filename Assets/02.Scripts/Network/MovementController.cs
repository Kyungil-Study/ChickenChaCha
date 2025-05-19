using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    GameObject particle;

    void Update()
    {
        foreach(Touch touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began)
            {                
                // Construct a ray from the current touch coordinates
                Ray ray = Camera.main.ScreenPointToRay(touch.position);
                if (Physics.Raycast(ray, out RaycastHit hitResult))
                {
                    // Create a particle if hit
                    GameObject part = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    part.transform.position = hitResult.point;
                }
            }
        }
    }
}
