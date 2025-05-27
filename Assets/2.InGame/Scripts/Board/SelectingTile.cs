using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectingTile : Tile, IPointerClickHandler
{
    private static readonly int SHOW_FACE = Animator.StringToHash("ShowFace");
    public static readonly int HIDE_FACE = Animator.StringToHash("HideFace");
    public Animator anim;

    public Action<Tile> onClick;

    public void OnPointerClick(PointerEventData eventData)
    {
        onClick?.Invoke(this);
    }

    public void ShowFace()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("FaceDown") == false)
        {
            return;
        }

        anim.SetTrigger(SHOW_FACE);
    }

    public void HideFace()
    {
        anim.SetTrigger(HIDE_FACE);
    }

    public override void Spawned()
    {
        base.Spawned();
        BoardManager.Instance.selectingTiles[Info.index] = this;
    }
}