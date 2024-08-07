using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameManager GM;
    public Animator anim;
    
    public float speed;
    public float augmentedSpeed;
    public float distanceThreshold = 7f;
    public float currentSpeed;
    public bool isSpeedAdjusted = false;
    private Vector3 moveDirection;
    private Vector3 lastMoveDirection;
    public bool isChatting = false;
    private Chatter currentChatter = null; 
    public InteractionManager interactionManager;

    public bool isRotating = false;
    private Quaternion initialRotation;
    private Quaternion targetRotation;
    private Vector3 rotationPoint;
    private float rotationSpeed = 180f;
    private Queue<Chatter.ChatterType> lastThreeChatters = new Queue<Chatter.ChatterType>(3);
    private List<Chatter> interactedGrayChatters = new List<Chatter>();
    private bool isIgnoring = false;

    private void Start()
    {
        GM = GameObject.FindWithTag("GameController").GetComponent<GameManager>();
        anim = GetComponent<Animator>();
        interactionManager = GameObject.Find("Canvas").GetComponentInChildren<InteractionManager>();
    }

    void Update()
    {
        if (isRotating)
        {
            RotatePlayer();
        }
        else
        {
            if (currentSpeed == 0)
            {
                CheckForBlueChatters();
                Debug.Log("cercando blue chatters");
            }

            if (isChatting == true)
            {
                Chatter closestChatter = GetClosestChatter();
                currentChatter = closestChatter;

                if (Time.timeScale != 0)
                {
                    if (Input.GetKeyDown(KeyCode.K))
                    {
                        currentChatter.ChangeChatterType(Chatter.ChatterType.Red);
                    }
                    else if (Input.GetKeyDown(KeyCode.J))
                    {
                        currentChatter.ChangeChatterType(Chatter.ChatterType.Black);
                    }
                }
                
                HandleChatterInteraction(currentChatter);
            }
            else
            {
                if (!isSpeedAdjusted)
                {
                    AdjustSpeedBasedOnDistance();
                    isSpeedAdjusted = true;
                }
                Move();
            }
            
            CheckInteractedGrayChatters();

            if (isChatting)
            {
                switch (lastMoveDirection)
                {
                    case var forward when lastMoveDirection == Vector3.forward:
                        anim.SetTrigger("StopWalkingUp");
                        break;
                    case var back when lastMoveDirection == Vector3.back:
                        anim.SetTrigger("StopWalkingDown");
                        break;
                    case var left when lastMoveDirection == Vector3.left:
                        anim.SetTrigger("StopWalkingLeft");
                        break;
                    case var right when lastMoveDirection == Vector3.right:
                        anim.SetTrigger("StopWalkingRight");
                        break;
                }
            }
            else
            {
                switch (moveDirection)
                {
                    case var forward when moveDirection == Vector3.forward:
                        anim.SetTrigger("StartWalkingUp");
                        break;
                    case var back when moveDirection == Vector3.back:
                        anim.SetTrigger("StartWalkingDown");
                        break;
                    case var left when moveDirection == Vector3.left:
                        anim.SetTrigger("StartWalkingLeft");
                        break;
                    case var right when moveDirection == Vector3.right:
                        anim.SetTrigger("StartWalkingRight");
                        break;
                }
            }
        }
    }

    void Move()
    {
        transform.position += moveDirection * currentSpeed * Time.deltaTime;
        
        //Quaternion targetRotation = Quaternion.identity;
        //switch (moveDirection)
        //{
        //    case Vector3 forward when moveDirection == Vector3.forward:
        //        targetRotation = Quaternion.Euler(0, 0, 0);
        //        break;
        //    case Vector3 back when moveDirection == Vector3.back:
        //        targetRotation = Quaternion.Euler(0, 180, 0);
        //        break;
        //    case Vector3 left when moveDirection == Vector3.left:
        //        targetRotation = Quaternion.Euler(0, 270, 0);
        //        break;
        //    case Vector3 right when moveDirection == Vector3.right:
        //        targetRotation = Quaternion.Euler(0, 90, 0); 
        //        break;
        //}
        //transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
    
    void AdjustSpeedBasedOnDistance()
    {
        Ray ray = new Ray(transform.position, moveDirection);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            float distance = hit.distance;
            if (distance > distanceThreshold)
            {
                currentSpeed = augmentedSpeed;
            }
            else
            {
                currentSpeed = speed;
            }
        }
        else
        {
            currentSpeed = speed;
        }
    }
    

    void OnTriggerEnter(Collider other)
    {
        lastMoveDirection = moveDirection;

        if (other.CompareTag("Wall"))
        {
            GM.ui.ShowLoseMenu();
        }
        
        if (other.CompareTag("Chatter"))
        {
            isChatting = true;
            Chatter chatter = other.GetComponent<Chatter>();
            if (chatter != null)
            {
                // chatter.OnChatterColorChanged += HandleChatterColorChanged;
                HandleChatterInteraction(chatter);
                
                if (currentChatter.isDancing)
                {
                    StartRotationWithChatter(currentChatter);
                }
                
                // if (currentChatter.chatterType == Chatter.ChatterType.Gray && !isIgnoring)
                // {
                //     isIgnoring = true;
                //     GM.ui.ShowGrayChatterMenu();
                // }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Chatter"))
        {
            if (currentChatter != null)
            {
                // currentChatter.OnChatterColorChanged -= HandleChatterColorChanged;

                if (currentChatter.chatterType == Chatter.ChatterType.Gray)
                {
                    Debug.Log("me ne sto andando da un chatter grigio");
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
        isSpeedAdjusted = false;
        TrackChatterInteraction(chatter.chatterType);
        
        switch (chatter.chatterType)
        {
            case Chatter.ChatterType.Red:
                currentChatter.isDancing = false;
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
                isChatting = false;
                GM.ui.ShowWinMenu();
                // interactionManager.ResetBoxes();
                break;
            case Chatter.ChatterType.Gray:
                if (!isIgnoring)
                {
                    GM.ui.ShowGrayChatterMenu();
                    isIgnoring = true;
                }
                break;
        }
    }
    
    public void TeleportTo(Vector3 position, Quaternion rotation)
    {
        transform.position = position;
        transform.rotation = rotation;
        currentSpeed = 0;
        isChatting = false;
        currentChatter = null;
    }

    void PushAwayFromChatter(Transform chatterTransform)
    {
        Vector3 directionToChatter = chatterTransform.position - transform.position;
        Vector3 cardinalDirection = GetCardinalDirection(-directionToChatter);
        moveDirection = cardinalDirection; 
        if (currentChatter.isDisappearing)
        {
            // per ora sale 5 sec ma poi torna giù. Da rivedere (spegnere il rigidbody o farlo salire per più tempo)
            var currentChatterScript = currentChatter.GetComponent<Chatter>();
            currentChatterScript.StartMoving(Vector3.up, 5);
            // currentChatter.gameObject.SetActive(false);
        }
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
        currentSpeed = speed;
        // if (currentChatter.isDisappearing)
        // {
        //     // per ora sale 5 sec ma poi torna giù. Da rivedere (spegnere il rigidbody o farlo salire per più tempo)
        //     var currentChatterScript = currentChatter.GetComponent<Chatter>();
        //     currentChatterScript.StartMoving(Vector3.up, 5);
        //     // currentChatter.gameObject.SetActive(false);
        // }
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
                Debug.Log("Found closest blue chatter: " + closestBlueChatter.gameObject.name);
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
        else
        {
            RotatePlayer();
        }
    }
    
    public void GrayChatterOptionIgnore()
    {
        currentChatter.GetComponent<Collider>().isTrigger = false;
        GM.ui.HideGrayChatterMenu();
        isChatting = false;
        currentSpeed = speed;
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
            if (Vector3.Distance(transform.position, chatter.transform.position) > 0.7f) 
            {
                chatter.ChangeChatterType(Chatter.ChatterType.Gray);
                interactedGrayChatters.RemoveAt(i);
            }
        }
    }
    
    void TrackChatterInteraction(Chatter.ChatterType chatterType)
    {
        if (lastThreeChatters.Count >= 3)
        {
            lastThreeChatters.Dequeue();
        }
        lastThreeChatters.Enqueue(chatterType);
        CheckForRedChatterLoop();
    }
    
    void CheckForRedChatterLoop()
    {
        if (lastThreeChatters.Count == 3)
        {
            bool allRed = true;
            foreach (Chatter.ChatterType chatterType in lastThreeChatters)
            {
                if (chatterType != Chatter.ChatterType.Red)
                {
                    allRed = false;
                    break;
                }
            }
            if (allRed)
            {
                GM.ui.ShowLoseMenu(); 
            }
        }
    }
}
