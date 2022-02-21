using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ManagerEnviromentSound : MonoBehaviour
{
    public AudioSource audioEnviromentSource;
    public AudioClip horrorEnviroment;
    public AudioClip horrorEnviromentOnAttack;
    private AudioClip currentSound;
    private AIEnemy.EnemyState lastEnemyState;
    private AIEnemy.EnemyState enemyState;

    private PhotonView pv;
    // Start is called before the first frame update
    void Start()
    {
        this.pv = this.GetComponent<PhotonView>();
        this.currentSound = this.horrorEnviroment;
        this.audioEnviromentSource.clip = this.currentSound;
        this.audioEnviromentSource.Play();
    }

    private void Update()
    {
        if (lastEnemyState != this.enemyState)
        {
            switch (this.enemyState)
            {
                case AIEnemy.EnemyState.Sleep:
                    this.currentSound = this.horrorEnviroment;
                    break;
                case AIEnemy.EnemyState.Attack:
                    this.currentSound = this.horrorEnviromentOnAttack;
                    break;
                case AIEnemy.EnemyState.Call:
                    this.currentSound = this.horrorEnviromentOnAttack;
                    break;
            }
            StartCoroutine(this.changeAudio());
            this.lastEnemyState = this.enemyState;
        }
    }

    private IEnumerator changeAudio()
    {
        float startVolume = this.audioEnviromentSource.volume;
        while (this.audioEnviromentSource.volume > 0)
        {
            this.audioEnviromentSource.volume -= startVolume * Time.deltaTime / 5;
            yield return null;

        }
        this.audioEnviromentSource.clip = this.currentSound;
        this.audioEnviromentSource.volume = startVolume;
        this.audioEnviromentSource.Play();
    }

    public void UpdateEnemyState(AIEnemy.EnemyState enemyState)
    {
        this.pv.RPC("UpdateEnemyStateRPC", RpcTarget.All, enemyState);
    }

    [PunRPC]
    private void UpdateEnemyStateRPC(AIEnemy.EnemyState enemyState)
    {
        this.enemyState = enemyState;
    }
}
