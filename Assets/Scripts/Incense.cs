using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Incense : MonoBehaviour
{
    public GameObject player;
    public bool setPlayer = false;
    private Transform enemy;
    private AIEnemy AIEnemy;
    private float distance;
    private float timeLife;

    void Start()
    {
        this.enemy = GameObject.Find("Enemy").transform;
        this.AIEnemy = enemy.GetComponent<AIEnemy>();
    }

    void Update()
    {
        this.distance = Vector3.Distance(this.transform.position, this.enemy.transform.position);
        if (this.distance <= 2)
        {
            if (this.setPlayer)
            {
                this.player.GetComponent<PlayerStats>().CompleteObjectiveIncense(); 
                if (this.AIEnemy.isStuckTogether)
                {
                    Debug.Log("Incenso stuna inimigo!");
                    this.AIEnemy.FreePlayerStuckTogether();
                    PhotonNetwork.Destroy(this.GetComponent<PhotonView>());
                }
            }
        }
        this.timeLife += Time.deltaTime;
        if(this.timeLife >= 2)
            PhotonNetwork.Destroy(this.GetComponent<PhotonView>());
    }
}
