using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectingTile : Tile, IPointerClickHandler
{
    public static SelectingTile Instance;
    public Action<Tile> onClick;
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("타일 클릭됨");
        onClick?.Invoke(this);
    }
}
