using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class Player : Creature
{

    private void Awake(){
        cam = Camera.main.transform;
        camParent = Camera.main.transform.parent;
    }

    public Image healthBar;
    public Image healthIcon;
    public Image manaBar;
    public Image manaIcon;
    protected override void UpdateHealthIcon(){
        healthBar.fillAmount = health;
    }
    protected override void UpdateManaIcon(){
        manaBar.fillAmount = mana;
    }
    protected override void UpdateAcid(){
        if (acid >= 1) { acidActive = true; healthIcon.sprite = gameData.healthIcons[1]; healthIcon.fillMethod = Image.FillMethod.Radial360; return; }
        if (acid <= 0) { acidActive = false; healthIcon.sprite = gameData.healthIcons[0]; healthIcon.fillMethod = Image.FillMethod.Vertical; healthIcon.fillAmount = 1f; return; }
        healthIcon.fillAmount = acid;
    }
    protected override void UpdateStun(){
        if (stun >= 1) { stunActive = true; manaIcon.sprite = gameData.manaIcons[1]; manaIcon.fillMethod = Image.FillMethod.Radial360; return; }
        if (stun <= 0) { stunActive = false; manaIcon.sprite = gameData.manaIcons[0]; manaIcon.fillMethod = Image.FillMethod.Vertical; manaIcon.fillAmount = 1f; return; }
        manaIcon.fillAmount = stun;
    }
    protected override void OnGrounded(){
        foreach (Action action in GetComponents<Action>()){
            action.hasUsedInAir = false;
        }
    }
    public Image leftFlavorIcon;
    public Image rightFlavorIcon;
    protected override void OnLeftFlavorChange(){
        leftFlavorIcon.sprite = gameData.flavorIcons[(int)leftFlavor * 2];
        if (currentFlavor == leftFlavor) currentFlavor = Flavor.Base;
    }
    protected override void OnRightFlavorChange(){
        rightFlavorIcon.sprite = gameData.flavorIcons[(int)rightFlavor * 2];
        if (currentFlavor == rightFlavor) currentFlavor = Flavor.Base;
    }
    // might need to make cleaner
    // leftFlavorIndex = (int)leftFlavor * 2 + (currentFlavor == leftFlavor ? 1 : 0)
    // then set sprite just once
    protected override void OnCurrentFlavorChange(){
        if (currentFlavor == leftFlavor){
            leftFlavorIcon.sprite = gameData.flavorIcons[(int)leftFlavor * 2 + 1];
            rightFlavorIcon.sprite = gameData.flavorIcons[(int)rightFlavor * 2];
            leftFlavorIcon.transform.localScale = 1.2f * Vector3.one;
            rightFlavorIcon.transform.localScale = 0.8f * Vector3.one;
            return;
        }
        if (currentFlavor == rightFlavor){
            leftFlavorIcon.sprite = gameData.flavorIcons[(int)leftFlavor * 2];
            rightFlavorIcon.sprite = gameData.flavorIcons[(int)rightFlavor * 2 + 1];
            leftFlavorIcon.transform.localScale = 0.8f * Vector3.one;
            rightFlavorIcon.transform.localScale = 1.2f * Vector3.one;
            return;
        }
        leftFlavorIcon.sprite = gameData.flavorIcons[(int)leftFlavor * 2];
        rightFlavorIcon.sprite = gameData.flavorIcons[(int)rightFlavor * 2];
        leftFlavorIcon.transform.localScale = Vector3.one;
        rightFlavorIcon.transform.localScale = Vector3.one;
    }

    private Vector2 lookDirection;
    private Transform camParent;
    private Transform cam;
    public float lookSpeed;
    public float lookDistance;
    public void OnLook(InputAction.CallbackContext context){
        lookDirection = context.ReadValue<Vector2>();
    }
    private void IterateCamera(){
        camParent.position = transform.position;
        if (lookDirection.sqrMagnitude != 0)
            camParent.localEulerAngles += new Vector3(-lookDirection.y * lookSpeed * Time.deltaTime, lookDirection.x * lookSpeed * Time.deltaTime, 0f);
        RaycastHit hit;
        Ray ray = new Ray(camParent.position, -camParent.forward);
        if (Physics.Raycast(ray, out hit, lookDistance, gameData.terrainLayers)){
            camParent.localScale = new Vector3(1f, 1f, Vector3.Distance(camParent.position, hit.point) / (lookDistance * 1.1f));
        }
        else camParent.localScale = Vector3.one;
        cam.LookAt(transform.position);
    }

    public void OnRightFlavorToggle(InputAction.CallbackContext context){
        if (!context.started) return;
        if (currentFlavor == rightFlavor) currentFlavor = Flavor.Base;
        else currentFlavor = rightFlavor;
    }
    public void OnLeftFlavorToggle(InputAction.CallbackContext context){
        if (!context.started) return;
        if (currentFlavor == leftFlavor) currentFlavor = Flavor.Base;
        else currentFlavor = leftFlavor;
    }
    public void OnMove(InputAction.CallbackContext context){
        if (context.canceled) { movementInput = Vector2.zero; return; }
        movementInput = context.ReadValue<Vector2>();
        float camHeading = camParent.eulerAngles.y;
        Quaternion controlRotation = Quaternion.Euler(0f, camHeading, 0f);
        Vector3 direction = controlRotation * new Vector3(movementInput.x, 0f, movementInput.y);
        movementDirection = new Vector2(direction.x, direction.z);
    }
    public void OnJump(InputAction.CallbackContext context){
        if (!context.started) return;
        jump.enabled = true;
    }
    public void OnDash(InputAction.CallbackContext context){
        if (!context.started) return;
        dash.enabled = true;
    }

    private void Update(){
        if (!dash.enabled) {
            IterateGroundCheck();
            IterateMovement();
            IterateRotation();
            IterateAnimation();
        }
        IterateStatus();
        IterateCamera();
    }
}
