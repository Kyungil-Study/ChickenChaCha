using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class SelectingTile : Tile, IPointerClickHandler
{
    public Action<Tile> onClick;
    public void OnPointerClick(PointerEventData eventData)
    {
        onClick?.Invoke(this);
    }
}
