using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class UIPlayer : MonoBehaviour
{
    public enum Equipment { Camera = 0, EMF = 1, Incense = 2, Radio = 3, Salt = 4, ThermalScaner = 5 };

    public int maxEmfValue;
    public int minEmfValue;
    public int currentEMF;
    public Equipment currentEquipment = Equipment.EMF;
    public Sprite[] equipment = new Sprite[4];
    private Sprite currentSprite;
    public Image currentImagem;
    public TMP_Text equipmentValue;
    public bool equipmentAvaiable;
    private PhotonView pv;

    public GameObject paper;
    public GameObject imageEquipment;
    public GameObject imageCameraHUD;
    public GameObject objectivesScrollContent;

    public ObjectiveScrollElement objectiveScrollElement;
    public ObjectiveScrollElement[] objectivesScrollElements;

    public PlayerStats player;

    // Start is called before the first frame update
    void Start()
    {
        this.pv = this.GetComponent<PhotonView>();
        this.currentSprite = this.equipment[(int)this.currentEquipment];
        this.currentImagem.sprite = this.currentSprite;
        this.setAlphaEquipment();
        if (this.pv.IsMine)
        {
            this.paper.SetActive(true);
            this.imageEquipment.SetActive(true);
            this.imageCameraHUD.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(this.currentEquipment == Equipment.Camera && this.equipmentAvaiable) this.imageCameraHUD.SetActive(true); else this.imageCameraHUD.SetActive(false);

        if (Input.GetMouseButtonDown(1) && this.pv.IsMine)
        {
            this.turnEquipment();
        }
        if (Input.GetKeyDown(KeyCode.Q) && this.pv.IsMine)
        {
            this.changeEquipment();
            this.currentSprite = this.equipment[(int)this.currentEquipment];
            this.currentImagem.sprite = this.currentSprite;
            this.equipmentAvaiable = false;
            this.setAlphaEquipment();
        }
        if (Input.GetKeyDown(KeyCode.J) && this.pv.IsMine)
        {
            if (this.paper.activeInHierarchy)
                this.paper.SetActive(false);
            else
                this.paper.SetActive(true);
        }
    }

    void turnEquipment()
    {
        if (this.equipmentAvaiable == false)
            this.equipmentAvaiable = true;
        else
            this.equipmentAvaiable = false;
        this.setAlphaEquipment();
    }

    void setAlphaEquipment()
    {
        if(this.equipmentAvaiable == true) 
            this.currentImagem.color = new Color(this.currentImagem.color.r, this.currentImagem.color.g, this.currentImagem.color.b, 1f);
        else
            this.currentImagem.color = new Color(this.currentImagem.color.r, this.currentImagem.color.g, this.currentImagem.color.b, 0.3f);
    }
    void changeEquipment()
    {
        if (this.currentEquipment == Equipment.Camera)
        {
            this.currentEquipment = Equipment.EMF;
        }
        else if (this.currentEquipment == Equipment.EMF)
        {
            this.currentEquipment = Equipment.Incense;
        }
        else if (this.currentEquipment == Equipment.Incense)
        {
            this.currentEquipment = Equipment.Radio;
        }
        else if (this.currentEquipment == Equipment.Radio)
        {
            this.currentEquipment = Equipment.Salt;
        }
        else if (this.currentEquipment == Equipment.Salt)
        {
            this.currentEquipment = Equipment.ThermalScaner;
        }
        else if (this.currentEquipment == Equipment.ThermalScaner)
        {
            this.currentEquipment = Equipment.Camera;
        }
    }

    public void CompleteObjective(int number)
    {
        this.pv.RPC("CompleteObjectiveRPC", RpcTarget.All, number);
        this.CheckObjectives();
    }
    private void CheckObjectives()
    {
        for(int i = 0; i < this.player.objectives.Length; i++)
        {
            if (!this.player.objectives[i].isCheck)
                return;
        }
        Debug.Log("Você venceu!");

        if (PhotonNetwork.IsMasterClient)
            PhotonNetwork.LoadLevel(2);
    }
    [PunRPC]
    private void CompleteObjectiveRPC(int number)
    {
        GameObject[] canvas = GameObject.FindGameObjectsWithTag("Player");
        foreach(GameObject c in canvas)
        {
            UIPlayer u = c.GetComponentInChildren<UIPlayer>();
            if (u.objectivesScrollElements.Length > 0)
                u.objectivesScrollElements[number].setCurrentImage(true);
        }
        this.player.objectives[number].isCheck = true;
    }
    public void CreateObjectives(Objectives[] objectives)
    {
        Debug.Log("create objectives");
        this.objectivesScrollElements = new ObjectiveScrollElement[objectives.Length];
        for (int i = 0; i < objectives.Length; i++)
        {
            this.objectivesScrollElements[i] = Instantiate(this.objectiveScrollElement);
            if (this.objectivesScrollElements[i] != null)
            {
                this.objectivesScrollElements[i].transform.SetParent(this.objectivesScrollContent.transform);
                this.objectivesScrollElements[i].description.text = objectives[i].description;
                this.objectivesScrollElements[i].setCurrentImage(objectives[i].isCheck);
            }
        }
    }
}
