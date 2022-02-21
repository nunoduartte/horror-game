using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerMovement : MonoBehaviour
{
    public GameObject camera;
    public CharacterController controller;
    public float speed = 12;
    public float gravity = -9.81f;
    private PhotonView pv;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    private bool isGrounded;
    public AudioSource footstep;
    public GameObject player;

    private Vector3 velocity;
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
            this.Movimentation();
        }
    }

    private void Movimentation()
    {
        this.isGrounded = Physics.CheckSphere(this.groundCheck.position, this.groundDistance, this.groundMask);

        if (this.isGrounded && this.velocity.y < 0)
        {
            this.velocity.y = -2f;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        if (x != 0 || z != 0)
        {
            if(!this.footstep.isPlaying)
                this.pv.RPC("PlayRPC", RpcTarget.All, this.pv.Controller.NickName);
        }
        else
            this.pv.RPC("StopRPC", RpcTarget.All, this.pv.Controller.NickName);

        Vector3 move = this.transform.right * x + this.transform.forward * z;

        this.controller.Move(move * speed * Time.deltaTime);

        this.velocity.y += this.gravity * Time.deltaTime;

        this.controller.Move(this.velocity * Time.deltaTime);
    }

    [PunRPC]
    private void PlayRPC(string playerName)
    {
        if (this.player.GetComponent<PhotonView>().Controller.NickName == playerName)
        {
            this.footstep.Play();
        }
    }

    [PunRPC]
    private void StopRPC(string playerName)
    {
        if (this.player.GetComponent<PhotonView>().Controller.NickName == playerName)
        {
            this.footstep.Stop();
        }
    }
}
