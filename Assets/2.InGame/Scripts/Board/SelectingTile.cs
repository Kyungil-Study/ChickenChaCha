using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectingTile : Tile, IPointerClickHandler
{
    private static readonly int SHOW_FACE = Animator.StringToHash("ShowFace");
    
    public Animator anim;
    public Action<Tile> onClick;
    
    public void OnPointerClick(PointerEventData eventData)
    {
        onClick?.Invoke(this);
    }

    public void ShowFace()
    {
        anim.SetTrigger(SHOW_FACE);
    }
}
