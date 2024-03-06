using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Flavor{
    Base,
    Spicy,
    Salty,
    Sweet,
    Bitter,
    Minty,
    Sour,
    Savory
}

public enum State{
    Idle,
    Running,
    Rising,
    Falling,
    Whacking,
    Whirling,
    Swimming
}

public abstract class Entity : MonoBehaviour
{
    // Common Variables
    public Rigidbody entityRigidbody;
    public GameData gameData;
    private Vector3 groundPlane;
    public float yVelocity;
    public Vector2 movementInput;
    public Vector2 movementDirection;
    public float speedModifier;
    public State currentState;
    public Action currentAction;

    // Shadow Section
    public Transform shadow;
    protected void UpdateShadow(){
        RaycastHit hit;
        Ray ray = new Ray(transform.position, -Vector3.up);
        if (Physics.Raycast(ray, out hit, 100f, gameData.terrainLayers)){
            shadow.position = hit.point;
        }
    }
    
    // Ground Check Section
    protected abstract void OnGrounded();
    private bool _isGrounded;
    public bool isGrounded {
        get { return _isGrounded; }
        private set { if (value && !_isGrounded) { OnGrounded(); } _isGrounded = value; }
    }
    protected void IterateGroundCheck(){
        RaycastHit hit;
        if (Physics.SphereCast(transform.position, 0.4f, -Vector3.up, out hit, 0.7f, gameData.terrainLayers)){
            if (Mathf.Abs(hit.normal.x) > 0.7f || Mathf.Abs(hit.normal.y) < 0.7f || Mathf.Abs(hit.normal.z) > 0.7f){
                movementDirection = new Vector2(hit.normal.x, hit.normal.z);
                isGrounded = false;
                return;
            }
            if (movementInput == Vector2.zero) movementDirection = Vector2.zero;
            groundPlane = hit.normal;
            isGrounded = true;
            if (yVelocity < 0f) yVelocity = 0f;
            return;
        }
        isGrounded = false;
        groundPlane = Vector3.zero;
        yVelocity += gameData.gravity * speedModifier * Time.deltaTime;
    }

    // Movement Section
    public float movementSpeed;
    public float currentSpeed;
    public float acceleration;
    protected void IterateMovement(){
        var targetSpeed = movementSpeed * movementDirection.magnitude * speedModifier;
        currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, acceleration * speedModifier * Time.deltaTime);
        var movement = new Vector3(movementDirection.x, 0f, movementDirection.y);
        if (groundPlane != Vector3.zero) movement = Vector3.ProjectOnPlane(movement, groundPlane).normalized;
        entityRigidbody.velocity = new Vector3(movement.x * currentSpeed, movement.y * currentSpeed + yVelocity, movement.z * currentSpeed);
    }

    // Rotation Section
    private float smoothTime;
    private float smoothVelocity;
    protected void IterateRotation(){
        if (movementDirection.sqrMagnitude == 0) return;
        var targetAngle = Mathf.Atan2(movementDirection.x, movementDirection.y) * Mathf.Rad2Deg;
        var angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref smoothVelocity, smoothTime);
        transform.rotation = Quaternion.Euler(0f, angle, 0f);
    }

    public Animation animation;
    public string currentAnimation;
    
}
