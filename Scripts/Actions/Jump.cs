using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jump : Action
{
    public float jumpSpeed;
    protected override void OnEnable(){
        if (entity.currentState != State.Swimming && !entity.isGrounded && hasUsedInAir) { this.enabled = false; return; }
        entity.yVelocity = jumpSpeed * entity.speedModifier;
        if (entity.currentAction != null && entity.currentAction != this) entity.currentAction.enabled = false;
        entity.currentAction = this;
        if (entity.currentState == State.Swimming) { hasUsedInAir = false; this.enabled = false; return; }
        if (!entity.isGrounded && !hasUsedInAir) { hasUsedInAir = true; this.enabled = false; return; }
    }
    protected override void Update(){
        if (entity.isGrounded || entity.currentState == State.Swimming) { hasUsedInAir = false; this.enabled = false; return; }
    }
    protected override void OnDisable(){
        if (entity.currentAction == this) entity.currentAction = null;
    }
    
}
