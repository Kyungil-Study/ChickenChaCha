using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum EChickenAnimation
{
    Waiting,
    Active,
    Trepid,
    Robbed,
    Left,
}
public class AnimationControll : MonoBehaviour
{
    [SerializeField] private Animator anim;

    public void PlayAnimation(EChickenAnimation state)
    {
        switch (state)
        {
            case EChickenAnimation.Waiting:
                anim.Play("Idle_A");
                anim.Play("Eyes_Blink");
                break;
            case EChickenAnimation.Active:
                anim.Play("Attack");
                anim.Play("Eyes_Happy");
                break;
            case EChickenAnimation.Trepid:
                anim.Play("Fear");
                anim.Play("Eyes_Trauma");
                break;
            case EChickenAnimation.Robbed:
                anim.Play("Spin");
                anim.Play("Eyes_Spin");
                break;
            case EChickenAnimation.Left:
                anim.Play("Sit");
                anim.Play("Eyes_Sleep");
                break;
        }
    }
}
