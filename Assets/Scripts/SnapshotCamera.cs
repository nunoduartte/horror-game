using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Photon.Pun;

public class SnapshotCamera : MonoBehaviour
{
    Camera snapCam;
    int resWidth = 256;
    int resHeight = 256;
    private string playerName;
    private PhotonView pv;
    public GameObject player;

    void Awake()
    {
        snapCam = GetComponent<Camera>();
        if(snapCam.targetTexture == null)
        {
            snapCam.targetTexture = new RenderTexture(resWidth, resHeight, 24);
        }
        else
        {
            resWidth = snapCam.targetTexture.width;
            resHeight = snapCam.targetTexture.height;
        }
        snapCam.gameObject.SetActive(false);
    }

    void Start()
    {
        this.pv = this.GetComponent<PhotonView>();
    }

    public void CallTakeSnapshot(string playerName)
    {
        this.playerName = playerName;
        snapCam.gameObject.SetActive(true);
    }
    private void LateUpdate()
    {
        if (snapCam.gameObject.activeInHierarchy)
        {
            Texture2D snapshot = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);
            snapCam.Render();
            RenderTexture.active = snapCam.targetTexture;
            snapshot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
            this.player.GetComponent<PlayerStats>().bytes = snapshot.EncodeToPNG();
            this.player.GetComponent<PlayerStats>().fileName = SnapshotName(this.playerName);
            snapCam.gameObject.SetActive(false);
        }
    }
    string SnapshotName(string playerName)
    {
        return string.Format("/Snapshots/snap_{0}_{1}x{2}_{3}_{4}.png", this.player.GetComponent<PhotonView>().Controller.NickName, resWidth, resHeight, playerName, System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
    }
}
