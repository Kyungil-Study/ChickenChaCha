using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class SelectingTile : Tile, IPointerClickHandler
{
<<<<<<< HEAD
=======
    private static readonly int SHOW_FACE = Animator.StringToHash("ShowFace");
    private static readonly int HIDE_FACE = Animator.StringToHash("HideFace");
    
    public Animator anim;
>>>>>>> Develop
    public Action<Tile> onClick;
    public void OnPointerClick(PointerEventData eventData)
    {
        onClick?.Invoke(this);
    }
<<<<<<< HEAD
=======

    public void ShowFace()
    {
        anim.SetTrigger(SHOW_FACE);
    }
    
    public void HideFace()
    {
        anim.SetTrigger(HIDE_FACE);
    }
>>>>>>> Develop
}
