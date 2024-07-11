using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatterTrigger : MonoBehaviour
{
    private Chatter parentChatter;

    void Start()
    {
        parentChatter = GetComponentInParent<Chatter>();
    }
    
}
