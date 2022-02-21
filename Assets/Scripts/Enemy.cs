using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public enum KindEnemy { Bestial = 0, Obsessor = 1 };

    public KindEnemy kindEnemy;
    public int damage;
    public float timeSleep, timeAttack, timePerAttack, chaseSpeed, speedWalk, speedWalkInCallToArrive, speedWalkInCallToDestiny;
    public int maxEmfValue;
    public int minEmfValue;
    // Start is called before the first frame update
    void Awake()
    {
        this.kindEnemy = KindEnemy.Bestial;

        switch (this.kindEnemy)
        {
            case KindEnemy.Bestial:
                this.damage = 10;
                this.timeSleep = 180;
                this.timeAttack = 60;
                this.timePerAttack = 0.5f;
                this.chaseSpeed = 6;
                this.speedWalk = 3;
                this.maxEmfValue = 5;
                this.minEmfValue = 3;
                this.speedWalkInCallToArrive = this.chaseSpeed;
                this.speedWalkInCallToDestiny = this.speedWalk / 3;
                break;
            case KindEnemy.Obsessor:
                this.damage = 5;
                this.timeSleep = 180;
                this.timeAttack = 60;
                this.timePerAttack = 1.5f;
                this.chaseSpeed = 3;
                this.speedWalk = 1.5f;
                this.maxEmfValue = 3;
                this.minEmfValue = 1;
                this.speedWalkInCallToArrive = this.chaseSpeed;
                this.speedWalkInCallToDestiny = this.speedWalk / 3;
                break;
        }
    }
}
