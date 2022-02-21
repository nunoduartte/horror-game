using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ManagerEquipmentSound : MonoBehaviour
{
    public AudioSource audioEquipmentSource;
    public AudioClip emfSoundStart;
    public AudioClip emfSoundDetected;
    public AudioClip cameraPhotoCapture;
    public AudioClip currentSound;
    private PhotonView pv;
    public GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        this.pv = this.GetComponent<PhotonView>();
        this.currentSound = this.cameraPhotoCapture;
        this.audioEquipmentSource.clip = this.currentSound;
    }

    public void UpdateSound(UIPlayer.Equipment equipment, string playerName)
    {
        this.pv.RPC("UpdateSoundRPC", RpcTarget.All, equipment, playerName);
    }
    public void PlaySound(string playerName)
    {
        this.pv.RPC("PlaySoundRPC", RpcTarget.All, playerName);
    }

    public void PlayOneShotSound(string playerName)
    {
        this.pv.RPC("PlayOneShotSoundRPC", RpcTarget.All, playerName);
    }

    public void StopSound(string playerName)
    {
        this.pv.RPC("StopSoundRPC", RpcTarget.All, playerName);
    }

    [PunRPC]
    private void UpdateSoundRPC(UIPlayer.Equipment equipment, string playerName)
    {
        if (player.GetComponent<PhotonView>().Controller.NickName == playerName)
        {
            switch (equipment)
            {
                case UIPlayer.Equipment.Camera:
                    this.currentSound = this.cameraPhotoCapture;
                    this.audioEquipmentSource.clip = this.currentSound;
                    this.audioEquipmentSource.volume = 0.5f;
                    break;
                case UIPlayer.Equipment.EMF:
                    if (player.GetComponent<PlayerStats>().distanceDetected)
                    {
                        this.currentSound = this.emfSoundDetected;
                        if (player.GetComponent<PlayerStats>().maxEmf)
                            this.audioEquipmentSource.volume = 1;
                        else
                            this.audioEquipmentSource.volume = 0.5f;
                    }
                    else
                    {
                        this.currentSound = this.emfSoundStart;
                        this.audioEquipmentSource.volume = 0.5f;
                    }
                    this.audioEquipmentSource.clip = this.currentSound;
                    break;
            }
        }
    }

    [PunRPC]
    private void PlaySoundRPC(string playerName)
    {
        if (player.GetComponent<PhotonView>().Controller.NickName == playerName)
        {
            this.Play();
        }
    }

    [PunRPC]
    private void PlayOneShotSoundRPC(string playerName)
    {
        if (player.GetComponent<PhotonView>().Controller.NickName == playerName)
        {
            this.PlayOneShot();
        }
    }

    [PunRPC]
    private void StopSoundRPC(string playerName)
    {
        if (player.GetComponent<PhotonView>().Controller.NickName == playerName)
        {
            this.Stop();
        }
    }
    private void Play()
    {
        if (!audioEquipmentSource.isPlaying)
        {
            this.audioEquipmentSource.Play();
        }
    }
    private void PlayOneShot()
    {
        this.audioEquipmentSource.PlayOneShot(this.currentSound);
    }
    private void Stop()
    {
        if (audioEquipmentSource.isPlaying)
        {
            this.audioEquipmentSource.Stop();
        }
    }
}
