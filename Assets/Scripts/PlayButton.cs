using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PlayButton : MonoBehaviour
{
    private Button playButton;

    // Start is called before the first frame update
    void Start()
    {
        this.playButton = this.GetComponent<Button>();
        this.playButton.onClick.AddListener(delegate { this.OnClick(); });
    }

    // Update is called once per frame
    void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            this.playButton.interactable = true;
        }
        else
        {
            this.playButton.interactable = false;
        }
    }

    public void OnClick()
    {
        this.playButton.interactable = false;
        PhotonNetwork.LoadLevel(1);
    }
}
