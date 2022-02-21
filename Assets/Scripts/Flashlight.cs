using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Flashlight : MonoBehaviour
{
    private bool canTurnOn, eventPressF;
    private float carga;
    public GameObject player;
    private PhotonView pv;
    // Start is called before the first frame update
    void Start()
    {
        this.pv = GetComponent<PhotonView>();
        this.carga = 300;
        this.gameObject.GetComponent<Light>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && this.pv.IsMine)
        {
            this.pv.RPC("EventPressF", RpcTarget.All, this.player.name);
        }
        if (this.eventPressF && this.canTurnOn)
        {
            if (this.gameObject.GetComponent<Light>().enabled == true)
            {
                this.TurnOff();
            }
            else
            {
                this.TurnOn();
            }
            this.eventPressF = false;
        }
        if (this.canTurnOn == false) this.TurnOff();
        if (this.gameObject.GetComponent<Light>().enabled == true && this.carga > 0) this.carga -= Time.deltaTime;
        if (this.carga <= 0) this.canTurnOn = false; else this.canTurnOn = true;
    }

    [PunRPC]
    public void EventPressF(string playerName)
    {
        if(player.GetComponent<PhotonView>().Controller.NickName == playerName)
        {
            this.eventPressF = true;
        }
    }
    public void TurnOn()
    {
        this.gameObject.GetComponent<Light>().enabled = true;
    }
    public void TurnOff()
    {
        this.gameObject.GetComponent<Light>().enabled = false;
    }
}
