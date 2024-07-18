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
    public bool isDisappearing;
    public float dancingAngle = 180f;
    public Transform hat;
    public Transform balloon;

    private Renderer hatRenderer;
    private Renderer balloonRenderer;
    public Material defaultMaterial;
    private Collider chatterCollider;
    
    public delegate void ChatterColorChangedHandler(Chatter chatter);
    public event ChatterColorChangedHandler OnChatterColorChanged;

    private void Awake()
    {
        hat = transform.Find("Hat");
        hatRenderer = hat.GetComponentInChildren<Renderer>();
        hatRenderer.material = new Material(hatRenderer.sharedMaterial); 
        balloon = transform.Find("Balloon");
        balloonRenderer = balloon.GetComponent<Renderer>();
        balloonRenderer.material = new Material(balloonRenderer.sharedMaterial);
        chatterCollider = GetComponent<Collider>();
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        initialType = chatterType;
    }
    
    private void Start()
    {
        EnsureMaterialInstance();
        UpdateColor();
        UpdateBalloonColor();
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
        
        if (balloonRenderer == null)
        {
            balloonRenderer = transform.Find("Balloon").GetComponent<Renderer>();
        }
        
        if (chatterCollider == null)
        {
            chatterCollider = GetComponent<Collider>();
        }
        
        EnsureMaterialInstance();
        UpdateColor();
        UpdateBalloonColor();
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

     void UpdateBalloonColor()
    {
        if (!isDancing && !isDisappearing)
        {
            balloonRenderer.enabled = false;
        }
        else if (isDancing)
        {
            balloonRenderer.enabled = true;
            balloonRenderer.sharedMaterial.color = Color.green;
        }
        else if (isDisappearing)
        {
            balloonRenderer.enabled = true;
            balloonRenderer.sharedMaterial.color = Color.red;
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
        
        if (balloonRenderer != null)
        {
            // If sharedMaterial is null, assign the default material
            if (balloonRenderer.sharedMaterial == null)
            {
                if (defaultMaterial != null)
                {
                    balloonRenderer.sharedMaterial = defaultMaterial;
                }
                else
                {
                    // Create a new default material if none is assigned
                    balloonRenderer.sharedMaterial = new Material(Shader.Find("Standard"));
                }
            }

            if (balloonRenderer.sharedMaterial != null)
            {
                // Check if the material instance is already unique
                if (!balloonRenderer.sharedMaterial.name.EndsWith("(Instance)"))
                {
                    balloonRenderer.sharedMaterial = new Material(hatRenderer.sharedMaterial);
                    balloonRenderer.sharedMaterial.name += " (Instance)"; // Rename to identify unique instances
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
    
    public void StartMoving(Vector3 direction, float duration)
    {
        StartCoroutine(MoveForDuration(direction, duration));
    }
    
    public IEnumerator MoveForDuration(Vector3 direction, float duration)
    {
        float elapsedTime = 0f;
        Vector3 startPosition = transform.position;

        while (elapsedTime < duration)
        {
            transform.position = startPosition + direction * (10 * elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the chatter ends exactly at the final position
        transform.position = startPosition + direction;
    }
}
