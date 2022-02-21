using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerAnimator : MonoBehaviour
{
    public Animator animator;
    private float horizontalMove, verticalMove;
    private PhotonView pv;
    public PlayerStats player;
    // Start is called before the first frame update
    void Start()
    {
        this.pv = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {
        if (pv.IsMine)
        {
            this.PlayerAnimation();
            if(this.player.sanity <= 0)
            {
                this.animator.SetBool("death", true);
                this.GetComponent<PlayerAnimator>().enabled = false;
            }
        }
    }
    private void PlayerAnimation()
    {
        this.horizontalMove = Input.GetAxisRaw("Horizontal");
        this.verticalMove = Input.GetAxisRaw("Vertical");
        this.animator.SetFloat("horizontalMove", this.horizontalMove);
        this.animator.SetFloat("verticalMove", this.verticalMove);
    }
}
