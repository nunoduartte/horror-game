using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerGruntEnemySound : MonoBehaviour
{
    public AudioClip[] grunts;
    public AIEnemy AIEnemy;
    private AudioSource audioManagerGruntEnemy;

    void Start()
    {
        this.audioManagerGruntEnemy = this.GetComponent<AudioSource>();
    }

    void Update()
    {
        if (this.AIEnemy.enemyState == AIEnemy.EnemyState.Attack)
        {
            if (this.AIEnemy.isSeeingPlayer)
            {
                if (this.audioManagerGruntEnemy.volume < 1)
                    StartCoroutine(this.IncreaseAudio());
            }
            else
            {
                if (this.audioManagerGruntEnemy.volume > 0.51f)
                    StartCoroutine(this.TurnDownAudio());

            }

            if (!this.audioManagerGruntEnemy.isPlaying)
            {
                this.audioManagerGruntEnemy.clip = this.grunts[Random.Range(0, this.grunts.Length)];
                this.audioManagerGruntEnemy.Play();
            }
        }
        else
        {
            if (this.audioManagerGruntEnemy.isPlaying)
                this.audioManagerGruntEnemy.Stop();
        }

    }
    private IEnumerator IncreaseAudio()
    {
        Debug.Log("aumentou audio");
        float startVolume = this.audioManagerGruntEnemy.volume;
        while (this.audioManagerGruntEnemy.volume < 1.01f)
        {
            this.audioManagerGruntEnemy.volume += startVolume * Time.deltaTime;
            yield return null;

        }
    }
    private IEnumerator TurnDownAudio()
    {
        Debug.Log("baixou audio");
        float startVolume = this.audioManagerGruntEnemy.volume;
        while (this.audioManagerGruntEnemy.volume > 0.5f)
        {
            this.audioManagerGruntEnemy.volume -= startVolume * Time.deltaTime;
            yield return null;

        }
    }
}
