using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chatter : MonoBehaviour
{
    private Vector3 initialPosition;
    private ChatterType initialType;
    private Quaternion initialRotation;
    
    public enum ChatterType { Red, Black, Blue, Yellow, Gray }
    public ChatterType chatterType;
    public bool isDancing;
    public float dancingAngle = 180f;
    public Transform hat;

    private Renderer hatRenderer;
    public Material defaultMaterial;
    public Material hatMaterial;
    private Collider chatterCollider;
    
    public delegate void ChatterColorChangedHandler(Chatter chatter);
    public event ChatterColorChangedHandler OnChatterColorChanged;

    private void Awake()
    {
        hat = transform.Find("Hat");
        hatRenderer = hat.GetComponentInChildren<Renderer>();
        hatRenderer.material = new Material(hatRenderer.sharedMaterial); // Create a unique material instance for the hat
        hatMaterial = hatRenderer.material;
        chatterCollider = GetComponent<Collider>();
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        initialType = chatterType;
    }
    
    private void Start()
    {
        EnsureMaterialInstance();
        UpdateColor();
    }

    private void Update()
    {
        
    }

    void OnValidate()
    {
        if (hatRenderer == null)
        {
            hatRenderer = transform.Find("Hat").GetComponentInChildren<Renderer>();
        }
        
        if (chatterCollider == null)
        {
            chatterCollider = GetComponent<Collider>();
        }
        
        EnsureMaterialInstance();
        UpdateColor();
    }

    void UpdateColor()
    {
        if (hatRenderer != null)
        {
            switch (chatterType)
            {
                case ChatterType.Red:
                    hatRenderer.sharedMaterial.color = Color.red;
                    gameObject.layer = LayerMask.NameToLayer("Default");
                    break;
                case ChatterType.Black:
                    hatRenderer.sharedMaterial.color = Color.black;
                    gameObject.layer = LayerMask.NameToLayer("Default");
                    break;
                case ChatterType.Blue:
                    hatRenderer.sharedMaterial.color = Color.blue;
                    gameObject.layer = LayerMask.NameToLayer("Default");
                    break;
                case ChatterType.Yellow:
                    hatRenderer.sharedMaterial.color = Color.yellow;
                    gameObject.layer = LayerMask.NameToLayer("Default");
                    break;
                case ChatterType.Gray:
                    hatRenderer.sharedMaterial.color = Color.gray;
                    gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
                    break;
            }
        }
    }

    public void ChangeChatterType(ChatterType newType)
    {
        chatterType = newType;
        UpdateColor();
        OnChatterColorChanged?.Invoke(this);
    }

    void EnsureMaterialInstance()
    {
        if (hatRenderer != null)
        {
            // If sharedMaterial is null, assign the default material
            if (hatRenderer.sharedMaterial == null)
            {
                if (defaultMaterial != null)
                {
                    hatRenderer.sharedMaterial = defaultMaterial;
                }
                else
                {
                    // Create a new default material if none is assigned
                    hatRenderer.sharedMaterial = new Material(Shader.Find("Standard"));
                }
            }

            if (hatRenderer.sharedMaterial != null)
            {
                // Check if the material instance is already unique
                if (!hatRenderer.sharedMaterial.name.EndsWith("(Instance)"))
                {
                    hatRenderer.sharedMaterial = new Material(hatRenderer.sharedMaterial);
                    hatRenderer.sharedMaterial.name += " (Instance)"; // Rename to identify unique instances
                }
            }
        }
    }

    public void ResetChatter()
    {
        transform.position = initialPosition;
        transform.rotation = initialRotation;
        ChangeChatterType(initialType);
    }
}
