using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Creature : Entity
{
    protected abstract void UpdateHealthIcon();
    protected abstract void UpdateManaIcon();
    protected abstract void UpdateAcid();
    protected abstract void UpdateStun();

    private float _health;
    private float _mana;
    public float health { get { return _health; } set { _health = Mathf.Clamp(value, 0f, 1f); UpdateHealthIcon(); } }
    public float mana { get { return _mana; } set { _mana = Mathf.Clamp(value, 0f, 1f); UpdateManaIcon(); } }

    private int _spicy;
    private int _salty;
    private int _sweet;
    private int _bitter;
    private int _minty;
    private int _sour;
    private int _savory;
    
    public int spicy { get { return _spicy; } set { _spicy = Mathf.Clamp(value, 0, 99); } }
    public int salty { get { return _salty; } set { _salty = Mathf.Clamp(value, 0, 99); } }
    public int sweet { get { return _sweet; } set { _sweet = Mathf.Clamp(value, 0, 99); } }
    public int bitter { get { return _bitter; } set { _bitter = Mathf.Clamp(value, 0, 99); } }
    public int minty { get { return _minty; } set { _minty = Mathf.Clamp(value, 0, 99); } }
    public int sour { get { return _sour; } set { _sour = Mathf.Clamp(value, 0, 99); } }
    public int savory { get { return _savory; } set { _savory = Mathf.Clamp(value, 0, 99); } }

    public float tolerance { get { return 1 - Mathf.Pow(0.92f, sour) * Mathf.Pow(0.96f, (minty + bitter) / 2f); } }
    public float resilience { get { return 1 - Mathf.Pow(0.92f, salty) * Mathf.Pow(0.96f, (bitter + sweet) / 2f); } }
    public float recovery { get { return 1 - Mathf.Pow(0.92f, spicy) * Mathf.Pow(0.96f, (sweet + minty) / 2f); } }

    private float _stun;
    private float _acid;
    public float stun { get { return _stun; } set { if (!stunActive || _stun > value) { _stun = Mathf.Clamp(value, 0f, 1f); UpdateStun(); } } }
    public float acid { get { return _acid; } set { if (!acidActive || _acid > value) { _acid = Mathf.Clamp(value, 0f, 1f); UpdateAcid(); } } }
    protected bool acidActive;
    protected bool stunActive;
    private float statusTimer;
    protected void IterateStatus(){
        if (stun <= 0 && acid <= 0) return;
        if (Time.time - statusTimer > 1f){
            statusTimer = Time.time;
            if (acid > 0) { health -= 0.01f; if (acidActive) acid -= 0.04f; }
            if (stun > 0) { mana -= 0.05f; if (stunActive) stun -= 0.1f; }
        }
    }

    [SerializeField] private Flavor _currentFlavor;
    [SerializeField] private Flavor _leftFlavor;
    [SerializeField] private Flavor _rightFlavor;
    public Flavor currentFlavor { get { return _currentFlavor; } set { _currentFlavor = value; UpdateToCurrentFlavor(); OnCurrentFlavorChange(); } } // Update Player Color, currentWhack/Whirl
    public Flavor leftFlavor { get { return _leftFlavor; } set { _leftFlavor = value; OnLeftFlavorChange(); } }
    public Flavor rightFlavor { get { return _rightFlavor; } set { _rightFlavor = value; OnRightFlavorChange(); } } 

    public Jump jump;
    public Dash dash;

    public Whack whack;
    public Whirl whirl;

    public SkinnedMeshRenderer modelMeshRenderer;

    private void UpdateToCurrentFlavor(){
        Material[] materials = modelMeshRenderer.materials;
        materials[0] = gameData.materials[(int)currentFlavor * 2 + 1];
        materials[1] = gameData.materials[(int)currentFlavor * 2];
        modelMeshRenderer.materials = materials;

        Destroy(whack);
        Destroy(whirl);
        switch(currentFlavor){
            case Flavor.Base:
                whack = gameObject.AddComponent<Whack>();
                whirl = gameObject.AddComponent<Whirl>();
                return;
        }
    }
    protected abstract void OnLeftFlavorChange();
    protected abstract void OnRightFlavorChange();
    protected abstract void OnCurrentFlavorChange();

    protected void IterateAnimation(){
        if (currentState == State.Swimming && currentAnimation != "Rising") { animation.Play("Rising"); currentAnimation = "Rising"; return; }
        if (!isGrounded && yVelocity > 0 && currentAnimation != "Rising") { animation.Play("Rising"); currentAnimation = "Rising"; return; } 
        if (!isGrounded && yVelocity <= 0 && currentAnimation != "Falling") { animation.Play("Falling"); currentAnimation = "Falling"; return; } 
        if (isGrounded && entityRigidbody.velocity.magnitude > 0 && currentAnimation != "Running") { animation.Play("Running"); currentAnimation = "Running"; return; } 
        if (isGrounded && entityRigidbody.velocity.magnitude == 0 && currentAnimation != "Idle") { animation.Play("Idle"); currentAnimation = "Idle"; return; } 
    }
}
