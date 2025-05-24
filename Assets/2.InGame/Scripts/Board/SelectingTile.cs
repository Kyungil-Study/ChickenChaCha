using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectingTile : Tile, IPointerClickHandler
{
    public Animator anim;
    
    private static readonly int SHOW_FACE = Animator.StringToHash("ShowFace");
    private static readonly int HIDE_FACE = Animator.StringToHash("HideFace");
    

    public Action<Tile> onClick;
    public void OnPointerClick(PointerEventData eventData)
    {
        onClick?.Invoke(this);
    }
    public void ShowFace()
    {
        anim.SetTrigger(SHOW_FACE);
    }
    
    public void HideFace()
    {
        anim.SetTrigger(HIDE_FACE);
    }
    
    public override void Spawned()
    {
        base.Spawned();
        if (Runner.IsSharedModeMasterClient == false)
        {
            BoardManager.Instance.selectingTiles[Info.index] = this;
        }
    }

}
