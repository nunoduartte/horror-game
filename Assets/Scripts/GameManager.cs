using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameManager : MonoBehaviour
{
    public PlayersData playersData;
    private PhotonView pv;
    public Transform[] spawnPoints;
    public GameObject goodSpirit;
    public GameObject myPlayer;
    public List<Door> doors;
    public AIEnemy enemy;
    void Start()
    {
        this.pv = GetComponent<PhotonView>();
        int spawnPicker = Random.Range(0, this.spawnPoints.Length);
        this.playersData = GameObject.Find("PlayersData").GetComponent<PlayersData>();
        PlayerConfiguration player = (PlayerConfiguration)this.playersData.listPlayer[PhotonNetwork.LocalPlayer.NickName];
        string path = "Player" + player.playerRace + player.playerGender;
        this.myPlayer = PhotonNetwork.Instantiate(path, this.spawnPoints[spawnPicker].position, this.spawnPoints[spawnPicker].rotation);
        this.myPlayer.name = player.playerName;
        this.myPlayer.GetComponent<PlayerMovement>().enabled = true;
        this.myPlayer.GetComponent<PlayerStats>().enabled = true;
        foreach (Door d in this.doors)
        {
            d.Jogador = this.myPlayer;
            d.keysList = this.myPlayer.GetComponent<Keys>();
        }
        this.myPlayer.GetComponent<Keys>().playersKeys.Add(0);
        GameObject.Find("Camera").SetActive(false);
        this.myPlayer.GetComponent<PlayerMovement>().camera.tag = "MainCamera";
        this.myPlayer.GetComponent<PlayerMovement>().camera.GetComponent<MouseLook>().enabled = true;
        this.myPlayer.GetComponent<PlayerMovement>().camera.GetComponent<AudioListener>().enabled = true;
        this.myPlayer.GetComponent<PlayerMovement>().camera.GetComponent<Camera>().clearFlags = CameraClearFlags.Skybox;
        int layer1 = LayerMask.NameToLayer("Default");
        int layer2 = LayerMask.NameToLayer("TransparentFX");
        int layer3 = LayerMask.NameToLayer("Ignore Raycast");
        int layer4 = LayerMask.NameToLayer("Water");
        int layer5 = LayerMask.NameToLayer("UI");
        int layer6 = LayerMask.NameToLayer("Ground");
        this.myPlayer.GetComponent<PlayerMovement>().camera.GetComponent<Camera>().cullingMask = (1 << layer1) | (1 << layer2) | (1 << layer3) | (1 << layer4) | (1 << layer5) | (1 << layer6);
        this.pv.RPC("UpdatePlayerListRPC", RpcTarget.All);

        //Instantiate(this.goodSpirit, this.AIPoints[Random.Range(0, this.AIPoints.Length)]);
        //Instantiate(this.goodSpirit, this.AIPoints[1]);
    }

    [PunRPC]
    public void UpdatePlayerListRPC()
    {
        Debug.Log("atualizou lista de players");
        this.enemy.UpdatePlayerList();
    }
}
