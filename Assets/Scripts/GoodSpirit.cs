using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoodSpirit : MonoBehaviour
{
    private bool lastStateVisible = false;
    public bool isVisible = false;
    private float maxTimeVisible = 30;
    private float timeToBeVisible = 0;
    private AIEnemy AIEnemy;
    public Transform[] AIPoints;
    // Start is called before the first frame update
    void Start()
    {
        this.AIEnemy = GameObject.Find("Enemy").GetComponent<AIEnemy>();
        this.transform.position = this.AIPoints[Random.Range(0, this.AIPoints.Length)].position;
        this.isVisible = true;
        this.lastStateVisible = this.isVisible;
    }

    // Update is called once per frame
    void Update()
    {
        if (this.AIEnemy.enemyState == AIEnemy.EnemyState.Attack)
        {
            this.isVisible = true;
            this.timeToBeVisible += Time.deltaTime;
            if (this.timeToBeVisible >= this.maxTimeVisible)
            {
                this.isVisible = false;
                this.timeToBeVisible = 0;
            }
        }
        else
        {
            this.isVisible = false;
        }

        if (this.isVisible != this.lastStateVisible)
        {
            updateChildren(this.isVisible);
            this.lastStateVisible = this.isVisible;
        }
    }

    private void updateChildren(bool isVisible)
    {
        for(int i = 0; i < this.transform.childCount; i++)
        {
            this.transform.GetChild(i).gameObject.SetActive(isVisible);
        }
    }
}
