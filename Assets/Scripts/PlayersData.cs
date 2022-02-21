using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayersData : MonoBehaviour
{
    public Hashtable listPlayer = new Hashtable();
    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }
}
