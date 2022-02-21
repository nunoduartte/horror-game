using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Salt : MonoBehaviour
{
    public GameObject player;
    public bool setPlayer = false;
    public Transform enemy;
    private AIEnemy AIEnemy;
    private float distance;
    private float timeLife;

    void Start()
    {
        this.enemy = GameObject.Find("Enemy").transform;
        this.AIEnemy = this.enemy.GetComponent<AIEnemy>();
    }

    void Update()
    {
        this.distance = Vector3.Distance(this.transform.position, this.enemy.transform.position);
        if(this.distance <= 3 && !this.AIEnemy.isStuckTogether)
        {
            if (this.setPlayer)
                this.player.GetComponent<PlayerStats>().CompleteObjectiveSalt();
            PhotonNetwork.Destroy(this.GetComponent<PhotonView>());
        }

        this.timeLife += Time.deltaTime;
        if (this.timeLife >= 10)
            PhotonNetwork.Destroy(this.GetComponent<PhotonView>());
    }
}
