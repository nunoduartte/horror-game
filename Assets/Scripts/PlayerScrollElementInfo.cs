using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;


public class PlayerScrollElementInfo : MonoBehaviour
{
    private ManagerConnection managerConnection;
    public PlayerConfiguration playerConfiguration;

    public TMP_Text playerNameText;
    public Button avatarButton;
    public TMP_Text avatarText;

    private List<string> combinations = new List<string>();

    private void Start()
    {
        combinations.Add("B,M");
        combinations.Add("B,F");
        combinations.Add("W,M");
        combinations.Add("W,F");

        this.managerConnection = GameObject.Find("Canvas").GetComponent<ManagerConnection>();
    }
    public void UpdateElements()
    {
        this.playerNameText.text = this.playerConfiguration.playerName;

        ColorBlock colors = this.avatarButton.colors;

        switch (this.playerConfiguration.playerRace)
        {
            case "B":
                colors.normalColor = Color.black;
                colors.highlightedColor = Color.black;
                colors.selectedColor = Color.black;
                this.avatarText.color = Color.white;
                break;
            case "W":
                colors.normalColor = Color.white;
                colors.highlightedColor = Color.white;
                colors.selectedColor = Color.white;
                this.avatarText.color = Color.black;
                break;
        }

        this.avatarButton.colors = colors;
        this.avatarText.text = this.playerConfiguration.playerGender;
    }

    public void OnClickAvatar()
    {
        if (PhotonNetwork.LocalPlayer.NickName != this.playerConfiguration.playerName)
            return;

        string temp = this.playerConfiguration.playerRace + "," + this.playerConfiguration.playerGender;
        int i = this.combinations.IndexOf(temp);
        if (i == this.combinations.Count - 1)
        {
            i = 0;
        }
        else
        {
            i++;
        }
        this.playerConfiguration.playerRace = this.combinations[i].Split(',')[0];
        this.playerConfiguration.playerGender = this.combinations[i].Split(',')[1];

        this.UpdateElements();
        this.managerConnection.photonView.RPC("UpdatePlayerList", RpcTarget.Others, this.playerConfiguration.playerName, this.playerConfiguration.playerGender, this.playerConfiguration.playerRace);

    }
}
