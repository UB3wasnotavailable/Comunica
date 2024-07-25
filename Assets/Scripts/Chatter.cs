using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Chatter : MonoBehaviour
{
    private Vector3 initialPosition;
    private ChatterType initialType;
    private Quaternion initialRotation;

    // public int dialogueIndex;
    public InteractionManager interactionManager;

    public enum ChatterType { Red, Black, Blue, Yellow, Gray }
    public ChatterType chatterType;
    public bool isDancing;
    public float dancingAngle = 180f;

    private Renderer rend;
    public Material defaultMaterial;
    private Collider chatterCollider;
    
    public delegate void ChatterColorChangedHandler(Chatter chatter);
    public event ChatterColorChangedHandler OnChatterColorChanged;

    
    private void Awake()
    {
        rend = GetComponent<Renderer>();
        rend.material = new Material(rend.sharedMaterial);
        chatterCollider = GetComponent<Collider>();
        interactionManager = GameObject.Find("Canvas").GetComponentInChildren<InteractionManager>();
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        initialType = chatterType;
    }
    void Start()
    {
        EnsureMaterialInstance();
        UpdateColor();
    }

    private void Update()
    {
        
    }

    void OnValidate()
    {
        if (rend == null)
        {
            rend = GetComponent<Renderer>();
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
        switch (chatterType)
        {
            case ChatterType.Red:
                rend.sharedMaterial.color = Color.red;
                gameObject.layer = LayerMask.NameToLayer("Default");
                break;
            case ChatterType.Black:
                rend.sharedMaterial.color = Color.black;
                gameObject.layer = LayerMask.NameToLayer("Default");
                break;
            case ChatterType.Blue:
                rend.sharedMaterial.color = Color.blue;
                gameObject.layer = LayerMask.NameToLayer("Default");
                break;
            case ChatterType.Yellow:
                rend.sharedMaterial.color = Color.yellow;
                gameObject.layer = LayerMask.NameToLayer("Default");
                break;
            case ChatterType.Gray:
                rend.sharedMaterial.color = Color.magenta;
                gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
                break;
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
        if (rend != null)
        {
            // If sharedMaterial is null, assign the default material
            if (rend.sharedMaterial == null)
            {
                if (defaultMaterial != null)
                {
                    rend.sharedMaterial = defaultMaterial;
                }
                else
                {
                    // Create a new default material if none is assigned
                    rend.sharedMaterial = new Material(Shader.Find("Standard"));
                }
            }

            if (rend.sharedMaterial != null)
            {
                // Check if the material instance is already unique
                if (!rend.sharedMaterial.name.EndsWith("(Instance)"))
                {
                    rend.sharedMaterial = new Material(rend.sharedMaterial);
                    rend.sharedMaterial.name += " (Instance)"; // Rename to identify unique instances
                }
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Verifica se l'oggetto che interagisce è il player
        if (other.CompareTag("Player"))
        {
            // Attiva il dialogo corretto nel manager
            interactionManager.ActivateDialogueBox(interactionManager.dialogueIndex);
            if (interactionManager.dialogueIndex < interactionManager.dialogueBoxes.Length)
            {
                interactionManager.dialogueIndex += 1;
            }
            else
            {
                interactionManager.dialogueIndex = 0;
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
