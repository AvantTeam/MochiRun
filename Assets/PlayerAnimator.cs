using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    private const string landed = "onGround", running = "isRunning", runSpeed = "runSpeed", deadTrigger = "dead";
    private string[] resetTriggers = {deadTrigger};
    GameObject player;
    PlayerControl pcon;
    Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        pcon = player.GetComponent<PlayerControl>();
        animator = GetComponent<Animator>();
        pcon.animator = this;
        reset();
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetBool(landed, pcon.landed);
        animator.SetBool(running, pcon.state == PlayerControl.STATE.RUN);
        animator.SetFloat(runSpeed, Mathf.Abs(player.GetComponent<Rigidbody2D>().velocity.x));
    }

    public void Kill(bool deathEffect) {
        if(!deathEffect) animator.SetTrigger(deadTrigger);
    }

    public void reset() {
        foreach(string t in resetTriggers) {
            animator.ResetTrigger(t);
        }
    }
}
