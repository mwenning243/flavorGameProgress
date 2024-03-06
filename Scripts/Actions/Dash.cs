using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dash : Action
{
    public float dashSpeed;
    public float dashTimer;
    private float timer;
    protected override void OnEnable(){
        if (entity.currentState != State.Swimming && !entity.isGrounded && hasUsedInAir){ this.enabled = false; return; }
        if (!entity.isGrounded && !hasUsedInAir) hasUsedInAir = true;
        if (entity.currentState == State.Swimming) hasUsedInAir = false;
        entity.yVelocity = 0f;
        entity.entityRigidbody.velocity = entity.transform.forward * dashSpeed * entity.speedModifier;
        timer = Time.time;
        entity.animation.Play("Lunge");
        entity.currentAnimation = "Lunge";
        if (entity.currentAction != null && entity.currentAction != this) entity.currentAction.enabled = false;
        entity.currentAction = this;
    }
    protected override void Update(){
        if (Time.time - timer >= dashTimer) { this.enabled = false; return; }
    }
    protected override void OnDisable(){
        if (entity.currentAction == this) entity.currentAction = null;
    }
}
