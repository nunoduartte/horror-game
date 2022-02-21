using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class ManagerConnection : MonoBehaviourPunCallbacks
{
    public PhotonView photonView;
    public GameObject createRoomMenu;
    public GameObject children;

    public TMP_InputField roomName;
    public TMP_InputField playerNameCreateRoom;
    public TMP_InputField playerNameJoinRoom;

    public GameObject scrollContent;
    public RoomScrollElementInfo roomScrollElementInfo;
    private List<RoomScrollElementInfo> listRooms = new List<RoomScrollElementInfo>();

    public GameObject playerScrollContent;
    public PlayerScrollElementInfo playerScrollElementInfo;
    private List<PlayerScrollElementInfo> listPlayerConfiguration = new List<PlayerScrollElementInfo>();
    private PlayersData playersData;

    private void Awake()
    {
        this.playersData = GameObject.Find("PlayersData").GetComponent<PlayersData>();
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.ConnectUsingSettings();
    }

    public void CreateRoom()
    {
        if(this.roomName.text.Length < 3 || this.playerNameCreateRoom.text.Length < 3 || PhotonNetwork.InRoom)
        {
            Debug.Log("falha ao criar sala");
            return;
        }

        PlayerConfiguration player = new PlayerConfiguration();
        player.playerName = this.playerNameCreateRoom.text;
        player.RandomCustomize();
        this.playersData.listPlayer.Add(player.playerName, player);

        PlayerScrollElementInfo element = Instantiate(playerScrollElementInfo);
        element.gameObject.name = "PlayerScrollElement" + player.playerName;
        element.transform.SetParent(this.playerScrollContent.transform);
        element.playerConfiguration = player;
        element.UpdateElements();
        this.listPlayerConfiguration.Add(element);

        PhotonNetwork.LocalPlayer.NickName = player.playerName;
        RoomOptions roomOptions = new RoomOptions { MaxPlayers = 2 };
        PhotonNetwork.CreateRoom(this.roomName.text, roomOptions, null);
        this.createRoomMenu.SetActive(false);
        this.children.SetActive(true);
    }
    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.Log("Entrei na sala");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);

        if (PhotonNetwork.IsMasterClient)
        {
            PlayerConfiguration player = new PlayerConfiguration();
            player.playerName = newPlayer.NickName;
            player.RandomCustomize(); 
            this.playersData.listPlayer.Add(player.playerName, player);

            PlayerScrollElementInfo element = Instantiate(playerScrollElementInfo);
            element.gameObject.name = "PlayerScrollElement" + player.playerName;
            element.transform.SetParent(this.playerScrollContent.transform);
            element.playerConfiguration = player;
            element.UpdateElements();
            this.listPlayerConfiguration.Add(element);

            this.CallUpdatePlayersList();
        }
    }
    private void CallUpdatePlayersList()
    {
        foreach(string key in this.playersData.listPlayer.Keys)
        {
            PlayerConfiguration player = (PlayerConfiguration) this.playersData.listPlayer[key];
            this.photonView.RPC("UpdatePlayerList", RpcTarget.Others, player.playerName, player.playerGender, player.playerRace);
        }
    }
    public void FindRoom()
    {
        if (PhotonNetwork.InLobby == false)
        {
            PhotonNetwork.JoinLobby();
        }
    }
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log("Atualizou lista");
        foreach (RoomInfo info in roomList)
        {
            if (info.RemovedFromList)
            {
                int i = this.listRooms.FindIndex(x => x.roomInfo.Name == info.Name);
                if(i != -1)
                {
                    Destroy(this.listRooms[i].gameObject);
                    this.listRooms.RemoveAt(i);
                }
            }
            else
            {
                RoomScrollElementInfo element = Instantiate(roomScrollElementInfo);
                if(element != null)
                {
                    Debug.Log(info);
                    element.roomInfo = info;
                    element.transform.SetParent(this.scrollContent.transform);
                    Button btn = element.GetComponentInChildren<Button>();
                    btn.onClick.AddListener(delegate { this.JoinRoom(element.roomInfo); });
                    this.listRooms.Add(element);
                }
            }
        }
    }
    public void JoinRoom(RoomInfo room)
    {
        if (this.playerNameJoinRoom.text.Length < 3)
        {
            return;
        }

        int i = this.listRooms.FindIndex(x => x.roomInfo.Name == room.Name);
        RoomScrollElementInfo element = this.listRooms[i];
        element.onClick();

        PhotonNetwork.LocalPlayer.NickName = this.playerNameJoinRoom.text;
        PhotonNetwork.JoinRoom(room.Name);
        this.ClearListRooms();
    }

    public void LeaveLobby()
    {
        //TODO: message like 'are you sure?'
        if (PhotonNetwork.InLobby == true)
        {
            PhotonNetwork.LeaveLobby();
        }
    }
    public override void OnLeftLobby()
    {
        this.ClearListRooms();
    }

    public void ClearListRooms()
    {
        foreach (RoomScrollElementInfo room in this.listRooms)
        {
            Destroy(room.gameObject);
        }
        this.listRooms.Clear();
    }

    public void LeaveRoom()
    {
        //TODO: message like 'are you sure?'
        if (PhotonNetwork.IsMasterClient || PhotonNetwork.InRoom == true)
        {
            PhotonNetwork.LeaveRoom();
        }
    }

    public override void OnLeftRoom()
    {
        foreach (string key in this.playersData.listPlayer.Keys)
        {
            Destroy(GameObject.Find("PlayerScrollElement" + key));
        }
        this.playersData.listPlayer.Clear();
        this.children.SetActive(false);
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        this.playersData.listPlayer.Remove(otherPlayer.NickName);
        Destroy(GameObject.Find("PlayerScrollElement" + otherPlayer.NickName));
    }

    public void PlayTheGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("Play!");
        }
    }
    [PunRPC]
    public void UpdatePlayerList(string playerName, string playerGender, string playerRace)
    {
        if (!this.playersData.listPlayer.ContainsKey(playerName))
        {
            PlayerConfiguration player = new PlayerConfiguration();
            player.playerName = playerName;
            player.playerGender = playerGender;
            player.playerRace = playerRace;
            this.playersData.listPlayer.Add(player.playerName, player);

            PlayerScrollElementInfo element = Instantiate(playerScrollElementInfo);
            element.gameObject.name = "PlayerScrollElement" + player.playerName;
            element.transform.SetParent(this.playerScrollContent.transform);
            element.playerConfiguration = player;
            element.UpdateElements();
            this.listPlayerConfiguration.Add(element);
        }
        else
        {
            PlayerConfiguration player = (PlayerConfiguration) this.playersData.listPlayer[playerName];
            player.playerGender = playerGender;
            player.playerRace = playerRace;

            PlayerScrollElementInfo element = GameObject.Find("PlayerScrollElement" + player.playerName).GetComponent<PlayerScrollElementInfo>();
            element.playerConfiguration = player;
            element.UpdateElements();
        }
    }
}
