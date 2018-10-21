using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour {
    private Animator animator;

    // Use this for initialization
    void Start () {
        animator = GetComponent<Animator>();
        animator.SetBool("Walk", false);
    }
	
	public void SetFalse()
    {
        animator.SetBool("Walk", false);
    }

    public void SetTrue()
    {
        animator.SetBool("Walk", true);
    }
}
