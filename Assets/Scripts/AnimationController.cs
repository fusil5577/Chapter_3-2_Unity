using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    protected Animation animator;
    protected PlayerController playerController;

    protected virtual void Awake()
    {
        animator = GetComponent<Animation>();
        playerController = GetComponent<PlayerController>();
    }
}
