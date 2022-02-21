using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;

public class GameConnection : MonoBehaviourPunCallbacks
{
    public Text chatLog;
    private void Awake()
    {
        this.chatLog.text += "\nConectando ao servidor...";
        PhotonNetwork.LocalPlayer.NickName = "Player " + Random.Range(1, 1000);
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();

        this.chatLog.text += "\n Conectado ao servidor.";

        if(PhotonNetwork.InLobby == false)
        {
            this.chatLog.text += "\nEntrando no lobby...";
            PhotonNetwork.JoinLobby();
        }
    }

    public override void OnJoinedLobby()
    {
        this.chatLog.text += "\nEntrei no Lobby!";
        this.chatLog.text += "\nEntrando na sala...";
        PhotonNetwork.JoinRoom("Room1");
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        base.OnJoinRoomFailed(returnCode, message);
        this.chatLog.text += "\n Erro ao entrar na sala: " + message + "| codigo: " + returnCode;

        if(returnCode == ErrorCode.GameDoesNotExist)
        {
            this.chatLog.text += "\nCriando sala...";
            RoomOptions roomOptions = new RoomOptions { MaxPlayers = 2};
            PhotonNetwork.CreateRoom("Room1", roomOptions, null);
        }
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        this.chatLog.text += "\n" +PhotonNetwork.LocalPlayer.NickName + " entrou na sala!";
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        this.chatLog.text += "\n" + newPlayer.NickName + " entrou na sala!";
    }
}
