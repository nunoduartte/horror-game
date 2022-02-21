using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;
using System.IO;


public class PlayerStats : MonoBehaviour
{
    private const int OBJECTIVE_CAPTURE_PHOTO = 0;
    private const int OBJECTIVE_EMF = 1;
    private const int OBJECTIVE_GET_ANSWER = 2;
    private const int OBJECTIVE_USE_SALT = 3;
    private const int OBJECTIVE_USE_INCENSE = 4;

    public float sanity = 100;
    public UIPlayer uiPlayer;
    private UIPlayer.Equipment lastEquipment;
    public GameObject saltCircle;
    public ManagerEquipmentSound managerEquipmentSound;
    private PhotonView pv;
    public GameObject enemy;
    public AIEnemy AIEnemy;
    public float distance;
    public float distanceGoodSpirit;
    public GameObject goodSpirit;
    private float goodSpiritTemperature = 10;
    private float enemyTemperature = -12;
    public bool distanceDetected, lastDistanceDetected, lastMaxEmf, maxEmf;

    public string fileName;
    private string lastFileName;
    public byte[] bytes;

    public Objectives[] objectives = new Objectives[5];

    public GameObject camera;
    public SnapshotCamera snapCam;

    private bool collisionGround1Floor;
    private bool collisionGround2Floor;
    public GameObject body;
    public GameObject content;

    private bool inHouse = false;
    // Start is called before the first frame update
    void Start()
    {
        this.uiPlayer.gameObject.SetActive(true);
        this.pv = this.GetComponent<PhotonView>();
        this.enemy = GameObject.Find("Enemy");
        this.AIEnemy = this.enemy.GetComponent<AIEnemy>();
        this.lastFileName = this.fileName;
        this.ConfigureObjectives();
        this.uiPlayer.maxEmfValue = this.enemy.gameObject.GetComponent<Enemy>().maxEmfValue;
        this.uiPlayer.minEmfValue = this.enemy.gameObject.GetComponent<Enemy>().minEmfValue;
        this.uiPlayer.CreateObjectives(this.objectives);
        this.goodSpirit = GameObject.Find("GoodSpirit");
    }

    // Update is called once per frame
    void Update()
    {
        this.distance = Vector3.Distance(this.enemy.transform.position, transform.position);
        this.distanceGoodSpirit = Vector3.Distance(this.goodSpirit.transform.position, transform.position);
        if(this.distance <= 10)
        {
            if (this.distance <= 5 && (this.AIEnemy.enemyState == AIEnemy.EnemyState.Attack))
            {
                this.uiPlayer.currentEMF = this.uiPlayer.maxEmfValue;
                this.pv.RPC("UpdateDistanceDetected", RpcTarget.All, this.pv.Controller.NickName, true, true);
            }
            else
            {
                this.uiPlayer.currentEMF = this.uiPlayer.minEmfValue;
                this.pv.RPC("UpdateDistanceDetected", RpcTarget.All, this.pv.Controller.NickName, true, false);
            }
        }
        else
        {
            this.pv.RPC("UpdateDistanceDetected", RpcTarget.All, this.pv.Controller.NickName, false, false);
        }

        if(this.lastDistanceDetected != this.distanceDetected || this.lastMaxEmf != this.maxEmf)
        {
            this.managerEquipmentSound.StopSound(this.pv.Controller.NickName);
            this.managerEquipmentSound.UpdateSound(this.uiPlayer.currentEquipment, this.pv.Controller.NickName);
            this.lastDistanceDetected = this.distanceDetected;
            this.lastMaxEmf = this.maxEmf;
        }
        if (this.lastEquipment != this.uiPlayer.currentEquipment)
        {
            this.managerEquipmentSound.UpdateSound(this.uiPlayer.currentEquipment, this.pv.Controller.NickName);
            this.managerEquipmentSound.StopSound(this.pv.Controller.NickName);
            this.lastEquipment = this.uiPlayer.currentEquipment;
        }
        if(this.lastFileName != this.fileName)
        {
            this.pv.RPC("CapturePhoto", RpcTarget.All, this.pv.Controller.NickName, this.fileName, this.bytes);
            this.lastFileName = this.fileName;
        }
        switch (this.uiPlayer.currentEquipment)
        {
            case UIPlayer.Equipment.Camera:
                if (this.uiPlayer.equipmentValue.enabled)
                    this.uiPlayer.equipmentValue.enabled = false;
                this.CameraJob();
                break;

            case UIPlayer.Equipment.EMF:
                if (!this.uiPlayer.equipmentValue.enabled)
                    this.uiPlayer.equipmentValue.enabled = true;
                this.EMFJob();
                break;
            case UIPlayer.Equipment.Incense:
                if (this.uiPlayer.equipmentValue.enabled)
                    this.uiPlayer.equipmentValue.enabled = false;
                this.IncenseJob();
                break;
            case UIPlayer.Equipment.Radio:
                if (this.uiPlayer.equipmentValue.enabled)
                    this.uiPlayer.equipmentValue.enabled = false;
                this.RadioJob();
                break;
            case UIPlayer.Equipment.Salt:
                if (this.uiPlayer.equipmentValue.enabled)
                    this.uiPlayer.equipmentValue.enabled = false;
                this.SaltJob();
                break;
            case UIPlayer.Equipment.ThermalScaner:
                if (!this.uiPlayer.equipmentValue.enabled)
                    this.uiPlayer.equipmentValue.enabled = true;
                this.ThermalScanerJob();
                break;
        }
    }
    private void ObserverObjectiveCamera()
    {
        Ray ray;
        RaycastHit hit;
        ray = new Ray(this.camera.transform.position, this.camera.transform.forward);
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.gameObject.CompareTag("GoodSpirit"))
            {
                this.uiPlayer.CompleteObjective(OBJECTIVE_CAPTURE_PHOTO);
            }
        }
    }

    private void ObserverObjectiveEMF()
    {
        if (!this.uiPlayer.equipmentAvaiable) return;
        if(this.uiPlayer.maxEmfValue == 5 && this.maxEmf && !this.objectives[OBJECTIVE_EMF].isCheck)
        {
            Debug.Log("atingiu 5, objetivo concluido!");
            this.uiPlayer.CompleteObjective(OBJECTIVE_EMF);
        }
    }
    public void CompleteObjectiveIncense()
    {
        this.uiPlayer.CompleteObjective(OBJECTIVE_USE_INCENSE);
    }

    public void CompleteObjectiveRadio()
    {
        this.uiPlayer.CompleteObjective(OBJECTIVE_GET_ANSWER);
    }

    public void CompleteObjectiveSalt()
    {
        this.AIEnemy.StunEnemyAfterColliderSalt();
        this.uiPlayer.CompleteObjective(OBJECTIVE_USE_SALT);
    }

    private void ConfigureObjectives()
    {
        for(int i = 0; i < this.objectives.Length; i++)
        {
            this.objectives[i] = new Objectives();
        }
        this.objectives[0].description = "Capture uma foto de um espírito bom que está aprisionado.";
        this.objectives[1].description = "Detecte EMF nível 5.";
        this.objectives[2].description = "Obtenha uma resposta do espírito malígno.";
        this.objectives[3].description = "Defenda-se de um ataque.";
        this.objectives[4].description = "Liberte-se usando incenso.";
    }

    private void CameraJob()
    {
        if (this.uiPlayer.equipmentAvaiable)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (!this.objectives[OBJECTIVE_CAPTURE_PHOTO].isCheck)
                    this.ObserverObjectiveCamera();
                this.managerEquipmentSound.PlayOneShotSound(this.pv.Controller.NickName);
                this.snapCam.CallTakeSnapshot(this.pv.Controller.NickName);
            }
        }
    }

    private void EMFJob()
    {
        if (this.uiPlayer.equipmentAvaiable)
        {
            this.uiPlayer.equipmentValue.text = this.maxEmf + "";
            if(this.uiPlayer.currentEMF == this.uiPlayer.maxEmfValue)
                this.uiPlayer.equipmentValue.text = this.uiPlayer.currentEMF + "";
            else if((this.uiPlayer.currentEMF == this.uiPlayer.minEmfValue) && this.distance <= 10)
                this.uiPlayer.equipmentValue.text = UnityEngine.Random.Range(0,this.uiPlayer.currentEMF) + "";
            else
                this.uiPlayer.equipmentValue.text = "0";

            if (this.lastDistanceDetected != this.distanceDetected)
            {
                this.managerEquipmentSound.StopSound(this.pv.Controller.NickName);
                this.managerEquipmentSound.UpdateSound(this.uiPlayer.currentEquipment, this.pv.Controller.NickName);
                this.managerEquipmentSound.PlaySound(this.pv.Controller.NickName);
                this.lastDistanceDetected = this.distanceDetected;
            }

            if (!this.managerEquipmentSound.audioEquipmentSource.isPlaying)
            {
                this.managerEquipmentSound.UpdateSound(this.uiPlayer.currentEquipment, this.pv.Controller.NickName);
                this.managerEquipmentSound.PlaySound(this.pv.Controller.NickName);
            }
            this.ObserverObjectiveEMF();
        }
        else
        {
            this.uiPlayer.equipmentValue.text = "0";
            if (this.managerEquipmentSound.audioEquipmentSource.isPlaying)
            {
                this.managerEquipmentSound.UpdateSound(this.uiPlayer.currentEquipment, this.pv.Controller.NickName);
                this.managerEquipmentSound.StopSound(this.pv.Controller.NickName);
            }
        }
    }
    private void IncenseJob()
    {
        if (this.uiPlayer.equipmentAvaiable)
        {
            if (Input.GetMouseButtonDown(0))
            {
                GameObject g = PhotonNetwork.Instantiate("ParticleIncense", this.transform.position, this.transform.rotation);
                g.GetComponent<Incense>().player = this.gameObject;
                g.GetComponent<Incense>().setPlayer = true;
            }
        }
    }

    private void RadioJob()
    {
        if (this.uiPlayer.equipmentAvaiable)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Debug.Log("Call enemy");
                if (!this.AIEnemy.isSeeingPlayer)
                {
                    this.AIEnemy.UpdateCallPositionAndFloor(this.transform.position, this.collisionGround1Floor, this.collisionGround2Floor);
                    this.AIEnemy.UpdateStateEnemy(AIEnemy.EnemyState.Call);
                }
            }
        }
    }

    private void SaltJob()
    {
        if (this.uiPlayer.equipmentAvaiable)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 position = new Vector3(this.transform.position.x, this.transform.position.y - this.transform.localScale.y, this.transform.position.z);
                GameObject g = PhotonNetwork.Instantiate("SaltCircle", position, this.transform.rotation);
                g.GetComponent<Salt>().player = this.gameObject;
                g.GetComponent<Salt>().setPlayer = true;
            }
        }
    }

    private void ThermalScanerJob()
    {
        if (this.uiPlayer.equipmentAvaiable)
        {
            if(this.distanceGoodSpirit <= 5)
            {
                if(this.distanceGoodSpirit <= 2)
                    this.uiPlayer.equipmentValue.text = Math.Round(UnityEngine.Random.Range(this.goodSpiritTemperature - 0.2f, this.goodSpiritTemperature + 0.2f), 1) + "°C";
                else
                    this.uiPlayer.equipmentValue.text = Math.Round(UnityEngine.Random.Range(this.goodSpiritTemperature - 5, this.goodSpiritTemperature - 10), 1) + "°C";

            }
            else
            {
                this.uiPlayer.equipmentValue.text = Math.Round(UnityEngine.Random.Range(this.enemyTemperature - 1.5f, this.enemyTemperature + 1.5f), 1) + "°C";
            }

        }
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if(hit.gameObject.tag == "Ground1Floor")
        {
            this.collisionGround1Floor = true;
            this.collisionGround2Floor = false;
            Debug.Log("primeiro andar"); 
            if (!this.AIEnemy.fechouPorta && !this.inHouse)
            {
                this.inHouse = true;
                this.AIEnemy.UpdateCountInHouse(this.inHouse);
            }
        }
        else if(hit.gameObject.tag == "Ground2Floor")
        {
            this.collisionGround2Floor = true;
            this.collisionGround1Floor = false;
            Debug.Log("segundo andar");
            if (!this.AIEnemy.fechouPorta && !this.inHouse)
            {
                this.inHouse = true;
                this.AIEnemy.UpdateCountInHouse(this.inHouse);
            }
        }
        else
        {
            this.collisionGround1Floor = false;
            this.collisionGround2Floor = false;

            if (!this.AIEnemy.fechouPorta && this.inHouse)
            {
                this.inHouse = false;
                this.AIEnemy.UpdateCountInHouse(this.inHouse);
            }
        }
    }

    public void RecebeuAtaque(float damage)
    {
        //this.pv.RPC("RecebeuAtaqueRPC", RpcTarget.All, playerName, damage);
        this.sanity -= damage;
        if (this.sanity <= 0)
        {
            Debug.Log("Player morreu");
            this.body.transform.parent = null;
            this.pv.RPC("UpdateTag", RpcTarget.All, this.pv.Controller.NickName);
            this.camera.transform.parent = this.transform;
            if(this.content.activeInHierarchy)
                this.content.SetActive(false);

            this.AIEnemy.PlayerIsDead();
        }
    }

    [PunRPC]
    private void UpdateTag(string playerName)
    {
        if (this.GetComponent<PhotonView>().Controller.NickName == playerName)
        {
            this.gameObject.tag = "PlayerDead";
        }
    }

    [PunRPC]
    private void CapturePhoto(string playerName, string fileName, byte[] bytes)
    {
        if (this.GetComponent<PhotonView>().Controller.NickName == playerName)
        {
            string filePath = Application.persistentDataPath + "/Snapshots";
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }
            string path = Application.persistentDataPath + fileName;
            System.IO.File.WriteAllBytes(path, bytes);

        }
    }
    [PunRPC]
    public void UpdateDistanceDetected(string playerName, bool newDistanceDetected, bool maxEmf)
    {
        if (this.GetComponent<PhotonView>().Controller.NickName == playerName)
        {
            this.distanceDetected = newDistanceDetected;
            this.maxEmf = maxEmf;
        }
    }
}
