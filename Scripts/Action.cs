using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Action : MonoBehaviour
{
    protected Entity entity;
    public bool hasUsedInAir;
    private void Awake(){
        entity = GetComponent<Entity>();
    }
    protected abstract void OnEnable();
    protected abstract void Update();
    protected abstract void OnDisable();
}
