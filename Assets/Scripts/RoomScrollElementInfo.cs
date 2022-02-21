using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class RoomScrollElementInfo : MonoBehaviour
{
    private GameObject findRoomMenu;
    private GameObject waitingPlayersRoomMenu;

    public RoomInfo roomInfo;
    public TMP_Text roomNameText;
    public Button buttonJoin;

    private void Start()
    {
        this.findRoomMenu = GameObject.Find("FindRoomMenu");
        this.waitingPlayersRoomMenu = GameObject.Find("WaitingPlayersRoom");
        this.roomNameText.text = this.roomInfo.Name;
    }
    public void onClick()
    {
        this.findRoomMenu.SetActive(false);
        this.waitingPlayersRoomMenu.SetActiveRecursively(true);
    }
}
