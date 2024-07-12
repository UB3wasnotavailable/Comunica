using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanChatter : MonoBehaviour
{
    
    private Vector3 initialPosition;
    private ChatterType initialType;
    private Quaternion initialRotation;
    
    public enum ChatterType { Red, Black, Blue, Yellow, Gray }
    public ChatterType chatterType;
    public bool isDancing;
    public float dancingAngle = 180f;
    private Collider chatterCollider;
    
    private GameObject redHat;
    private GameObject blackHat;
    private GameObject blueHat;
    private GameObject grayHat;
    private GameObject yellowHat;


    private void Awake()
    {
        chatterCollider = GetComponent<Collider>();
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        initialType = chatterType;
    }
    void Start()
    {
        UpdateColor();
    }
    void UpdateColor()
    {
        switch (chatterType)
        {
            case ChatterType.Red:
                gameObject.layer = LayerMask.NameToLayer("Default");
                break;
            case ChatterType.Black:
                gameObject.layer = LayerMask.NameToLayer("Default");
                break;
            case ChatterType.Blue:
                gameObject.layer = LayerMask.NameToLayer("Default");
                break;
            case ChatterType.Yellow:
                gameObject.layer = LayerMask.NameToLayer("Default");
                break;
            case ChatterType.Gray:
                gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
                break;
        }
    }
    
    public void ChangeChatterType(ChatterType newType)
    {
        chatterType = newType;
        UpdateColor();
    }
    
    public void ResetChatter()
    {
        transform.position = initialPosition;
        transform.rotation = initialRotation;
        ChangeChatterType(initialType);
    }
}
