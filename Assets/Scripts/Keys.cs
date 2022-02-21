using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Keys : MonoBehaviour
{
    [HideInInspector] public List<int> playersKeys = new List<int>();
    void Start()
    {
        if (transform.gameObject.tag != "Player")
        {
            transform.gameObject.tag = "Player";
        }
    }
}