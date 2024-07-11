using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameManager GM;
    
    public float speed;
    private Vector3 moveDirection;
    public bool isChatting = false;
    private Chatter currentChatter = null;

    private bool isRotating = false;
    private Quaternion initialRotation;
    private Quaternion targetRotation;
    private Vector3 rotationPoint;
    private float rotationSpeed = 180f;

    private List<Chatter> interactedGrayChatters = new List<Chatter>();
    private bool isIgnoring = false;

    private void Start()
    {
        GM = GameObject.FindWithTag("GameController").GetComponent<GameManager>();;
    }

    void Update()
    {
        if (isRotating)
        {
            RotatePlayer();
        }
        else
        {
            if (speed == 0)
            {
                CheckForBlueChatters();
            }

            if (isChatting == true)
            {
                Chatter closestChatter = GetClosestChatter();
                currentChatter = closestChatter;

                if (Input.GetKeyDown(KeyCode.K))
                {
                    currentChatter.ChangeChatterType(Chatter.ChatterType.Red);
                }
                else if (Input.GetKeyDown(KeyCode.J))
                {
                    currentChatter.ChangeChatterType(Chatter.ChatterType.Black);
                }

                HandleChatterInteraction(currentChatter);
            }
            else
            {
                Move();
            }
            
            CheckInteractedGrayChatters();
        }
    }

    void Move()
    {
        transform.position += moveDirection * speed * Time.deltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
        isChatting = true;
        
        if (other.CompareTag("Chatter"))
        {
            Chatter chatter = other.GetComponent<Chatter>();
            if (chatter != null)
            {
                chatter.OnChatterColorChanged += HandleChatterColorChanged;
                HandleChatterInteraction(chatter);
                
                if (currentChatter.isDancing)
                {
                    StartRotationWithChatter(currentChatter);
                }
                
                if (currentChatter.chatterType == Chatter.ChatterType.Gray && !isIgnoring)
                {
                    GM.ui.ShowGrayChatterMenu();
                    isIgnoring = true;
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Chatter"))
        {
            if (currentChatter != null)
            {
                currentChatter.OnChatterColorChanged -= HandleChatterColorChanged;

                if (currentChatter.chatterType == Chatter.ChatterType.Gray)
                {
                    StartCoroutine(ResetColliderToTrigger(currentChatter.GetComponent<Collider>(), 0.5f));
                    currentChatter.ChangeChatterType(Chatter.ChatterType.Gray);
                    isIgnoring = false;
                    //currentChatter.GetComponent<Collider>().isTrigger = true;
                }
            }

            currentChatter = null;
            // isChatting = false;
        }
    }
    
    private IEnumerator ResetColliderToTrigger(Collider collider, float delay)
    {
        yield return new WaitForSeconds(delay);
        collider.isTrigger = true;
    }

    // private void OnCollisionExit(Collision collision)
    // {
    //     if (collision.gameObject.CompareTag("Chatter"))
    //     {
    //         Chatter chatter = collision.gameObject.GetComponent<Chatter>();
    //         if (chatter.chatterType == Chatter.ChatterType.Gray)
    //         {
    //             isIgnoring = false;
    //             currentChatter.GetComponent<Collider>().isTrigger = true;
    //         }
    //     }
    // }

    void HandleChatterColorChanged(Chatter chatter)
    {
        if (chatter == currentChatter)
        {
            HandleChatterInteraction(chatter);
        }
    }

    void HandleChatterInteraction(Chatter chatter)
    {
        currentChatter = chatter;
        
        switch (chatter.chatterType)
        {
            case Chatter.ChatterType.Red:
                PushAwayFromChatter(chatter.transform);
                break;
            case Chatter.ChatterType.Black:
                Stop();
                CheckForBlueChatters();
                break;
            case Chatter.ChatterType.Blue:
                Stop();
                break;
            case Chatter.ChatterType.Yellow:
                Stop();
                GM.ui.ShowWinMenu();
                break;
            case Chatter.ChatterType.Gray:
                GM.ui.ShowGrayChatterMenu();
                break;
        }
    }
    
    public void TeleportTo(Vector3 position, Quaternion rotation)
    {
        transform.position = position;
        transform.rotation = rotation;
        speed = 0;
        isChatting = false;
        currentChatter = null;
    }

    void PushAwayFromChatter(Transform chatterTransform)
    {
        Vector3 directionToChatter = chatterTransform.position - transform.position;
        Vector3 cardinalDirection = GetCardinalDirection(-directionToChatter);
        moveDirection = cardinalDirection; 
        isChatting = false;
    }

    void Stop()
    {
        if (currentChatter != null)
        {
            Vector3 directionToChatter = transform.position - currentChatter.transform.position;
            Vector3 cardinalDirection = GetCardinalDirection(directionToChatter);
            float separationDistance = 0.7f;
            transform.position = currentChatter.transform.position + cardinalDirection * separationDistance;
        }
        moveDirection = Vector3.zero; 
    }
    
    void AttractToChatter(Transform chatterTransform)
    {
        Vector3 directionToChatter = chatterTransform.position - transform.position;
        moveDirection = GetCardinalDirection(directionToChatter);
        speed = 5;
        isChatting = false;
    }
    
    void StartRotationWithChatter(Chatter chatter)
    {
        if (isRotating)
        {
            return;
        }
        
        initialRotation = chatter.transform.rotation;
        targetRotation = Quaternion.Euler(0, chatter.dancingAngle, 0) * initialRotation;
        rotationPoint = chatter.transform.position;
        isRotating = true;
        
        // Vector3 rotationPoint = chatter.transform.position;
        // transform.RotateAround(rotationPoint, Vector3.up, 180f);
    }
    
    Chatter GetClosestChatter()
    {
        GameObject[] chatters = GameObject.FindGameObjectsWithTag("Chatter");
        GameObject closestChatter = null;
        float closestDistance = Mathf.Infinity;
        Vector3 playerPosition = transform.position;

        foreach (GameObject chatter in chatters)
        {
            float distanceToChatter = Vector3.Distance(chatter.transform.position, playerPosition);
            if (distanceToChatter < closestDistance)
            {
                closestDistance = distanceToChatter;
                closestChatter = chatter;
            }
        }

        if (closestChatter != null)
        {
            return closestChatter.GetComponent<Chatter>();
        }

        return null;
    }
    
    Chatter GetClosestChatter(List<Chatter> chatters)
    {
        Chatter closestChatter = null;
        float closestDistance = Mathf.Infinity;
        Vector3 playerPosition = transform.position;

        foreach (Chatter chatter in chatters)
        {
            float distanceToChatter = Vector3.Distance(chatter.transform.position, playerPosition);
            if (distanceToChatter < closestDistance)
            {
                closestDistance = distanceToChatter;
                closestChatter = chatter;
            }
        }

        return closestChatter;
    }
    
    public static Vector3 GetCardinalDirection(Vector3 direction)
    {
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.z))
        {
            return direction.x > 0 ? Vector3.right : Vector3.left;
        }
        else
        {
            return direction.z > 0 ? Vector3.forward : Vector3.back;
        }
    }
    
    public void CheckForBlueChatters()
    {
        List<Chatter> blueChatters = new List<Chatter>();
        Vector3[] directions = { Vector3.forward, Vector3.back, Vector3.left, Vector3.right };
        int layerMask = ~LayerMask.GetMask("Ignore Raycast");

        foreach (Vector3 direction in directions)
        {
            Ray ray = new Ray(transform.position, direction);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            {
                Chatter chatter = hit.transform.GetComponent<Chatter>();
                if (chatter != null && chatter.chatterType == Chatter.ChatterType.Blue)
                {
                    blueChatters.Add(chatter);
                }
            }
        }

        if (blueChatters.Count > 0)
        {
            Chatter closestBlueChatter = GetClosestChatter(blueChatters);
            if (closestBlueChatter != null)
            {
                AttractToChatter(closestBlueChatter.transform);
            }
        }
    }
    
    void RotatePlayer()
    {
        if (currentChatter == null) return;

        // Rotate the chatter
        currentChatter.transform.rotation = Quaternion.RotateTowards(currentChatter.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        // Calculate the relative position to the chatter and apply the same rotation to the player
        Vector3 relativePosition = transform.position - rotationPoint;
        relativePosition = Quaternion.RotateTowards(Quaternion.identity, targetRotation * Quaternion.Inverse(initialRotation), rotationSpeed * Time.deltaTime) * relativePosition;
        transform.position = rotationPoint + relativePosition;

        // Check if rotation is complete
        if (Quaternion.Angle(currentChatter.transform.rotation, targetRotation) < 0.1f)
        {
            currentChatter.transform.rotation = targetRotation; // Ensure the final rotation is exact
            isRotating = false;
            Debug.Log("Rotation complete.");
        }
    }
    
    public void GrayChatterOptionIgnore()
    {
        currentChatter.GetComponent<Collider>().isTrigger = false;
        GM.ui.HideGrayChatterMenu();
        isChatting = false;
        speed = 5;
    }
    
    public void GrayChatterOptionInteract()
    {
        currentChatter.ChangeChatterType(Chatter.ChatterType.Black);
        GM.ui.HideGrayChatterMenu();
        interactedGrayChatters.Add(currentChatter);
    }
    
    private void CheckInteractedGrayChatters()
    {
        for (int i = interactedGrayChatters.Count - 1; i >= 0; i--)
        {
            Chatter chatter = interactedGrayChatters[i];
            if (Vector3.Distance(transform.position, chatter.transform.position) > 1.5f) 
            {
                chatter.ChangeChatterType(Chatter.ChatterType.Gray);
                interactedGrayChatters.RemoveAt(i);
            }
        }
    }
}
