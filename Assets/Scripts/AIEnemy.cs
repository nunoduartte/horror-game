using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
public class AIEnemy : MonoBehaviour
{
    public enum EnemyState { Sleep, Attack, Call}
    public EnemyState enemyState = EnemyState.Sleep;
    private EnemyState lastEnemyState = EnemyState.Sleep;
    private Enemy enemy;
    private PhotonView pv;
    public Transform currentPlayer;
    public Transform backCurrentPlayer;
    public Transform[] players;
    float distanciaMaxima = 2.5f;
    private NavMeshAgent naveMesh;
    private float DistanciaDoPlayer, DistanciaDoAIPoint;

    private float[] doorDistance = new float[7];
    public Transform[] doors = new Transform[7];


    public Transform[] centerAIPoints1Floor;
    public Transform[] centerAIPoints2Floor;
    private Vector3 callPosition;
    private Vector3 callDestiny;
    private float destinyDistance;
    private float timeInDestiny;
    public bool hasDestiny, hasArrived = false;
    public bool firstFloor;
    public bool secondFloor;

    public float DistanciaDePercepcao = 30, DistanciaDeSeguir = 20, DistanciaDeAtacar = 2, VelocidadeDePerseguicao, TempoPorAtaque, damage;
    public bool isSeeingPlayer;
    public bool isStuckTogether, stun;
    private float timeToUnStun = 2;

    public Transform[] DestinosAleatorios;
    private int AIPointAtual;
    private bool PerseguindoAlgo, contadorPerseguindoAlgo, atacandoAlgo, abriuPorta, naoQuisAbrirPorta, AIPointChange, canAttack;
    private float cronometroDaPerseguicao, cronometroAtaque, cronometroAbriuPorta, cronometroAIPointChange;
    public float cronometroOnSleep, cronometroOnAttack, timerCanAttack;
    public float timeSleep;
    public float timeAttack;

    public AudioSource audioEnemySource;
    public AudioClip footstep;
    public AudioClip monsterFeeding;
    public Animator animator;
    public GameObject model;
    public ManagerEnviromentSound enviromentSound;
    private bool changeScene = false;

    public int countInHouse = 0;
    public bool fechouPorta = false;
    public GameObject doorToClose;

    void Start()
    {
        this.pv = this.GetComponent<PhotonView>();
        AIPointAtual = Random.Range(0, DestinosAleatorios.Length);
        naveMesh = transform.GetComponent<NavMeshAgent>();
        this.enemy = this.GetComponent<Enemy>();
        this.damage = this.enemy.damage;
        this.timeAttack = this.enemy.timeAttack;
        this.timeSleep = this.enemy.timeSleep;
        this.TempoPorAtaque = this.enemy.timePerAttack;
        this.VelocidadeDePerseguicao = this.enemy.chaseSpeed;
        this.animator.SetBool("isWalkCrawl", false);
        this.model.SetActive(false);
        this.enviromentSound = GameObject.FindGameObjectWithTag("EnviromentSound").GetComponent<ManagerEnviromentSound>();
    }
    void Update()
    {

        if (!this.fechouPorta && (this.countInHouse == this.players.Length))
        {
            this.doorToClose.GetComponent<Door>().UpdateDoorKeyToLock();
            this.fechouPorta = true;
        }

        if (this.players.Length == 0 && PhotonNetwork.IsMasterClient && !this.changeScene)
        {
            Debug.Log("Acabou o jogo!");
            PhotonNetwork.LoadLevel(3);
            this.changeScene = true;
            Cursor.lockState = CursorLockMode.None;
            return;
        }

        if (this.changeScene)
            return;

        switch (this.enemyState)
        {
            case EnemyState.Sleep:
                this.OnSleep();
                break;

            case EnemyState.Attack:
                if(!this.stun)
                    this.OnAttack();
                break;

            case EnemyState.Call:

                if (!this.firstFloor && !this.secondFloor)
                    this.UpdateStateEnemy(EnemyState.Attack);

                if (this.PerseguindoAlgo)
                    this.PerseguindoAlgo = false;

                this.OnCall();
                break;
        }
        if(this.lastEnemyState != this.enemyState)
        {
            this.enviromentSound.UpdateEnemyState(this.enemyState);
            this.lastEnemyState = this.enemyState;
        }
    }

    void OnSleep()
    {
        if(this.isStuckTogether)
            this.isStuckTogether = false;

        if(this.isSeeingPlayer)
            this.isSeeingPlayer = false;

        if(this.model.activeInHierarchy)
            this.model.SetActive(false);

        if (this.GetComponent<NavMeshAgent>().enabled == true)
            this.GetComponent<NavMeshAgent>().enabled = false;

        if (this.audioEnemySource.isPlaying)
            this.audioEnemySource.Stop();

        this.animator.SetBool("isWalkCrawl", false);
        this.cronometroOnSleep += Time.deltaTime;
        if (this.cronometroOnSleep >= timeSleep)
        {
            this.timeSleep -= this.timeSleep * 0.1f;
            this.cronometroOnSleep = 0;
            this.enemyState = EnemyState.Attack;
            this.canAttack = false;
        }
    }

    void OnAttack()
    {
        if (!this.model.activeInHierarchy)
            this.model.SetActive(true);


        if (!this.canAttack)
        {
            this.timerCanAttack += Time.deltaTime;
            if (this.timerCanAttack >= 5)
            {
                this.canAttack = true;
                this.timerCanAttack = 0;
            }
            return;
        }

            if (this.GetComponent<NavMeshAgent>().enabled == false)
            this.GetComponent<NavMeshAgent>().enabled = true;

        this.audioEnemySource.pitch = 1;

        this.cronometroOnAttack += Time.deltaTime;
        if (this.cronometroOnAttack >= timeAttack)
        {
            this.timeAttack += this.timeAttack * 0.1f;
            this.cronometroOnAttack = 0;
            this.enemyState = EnemyState.Sleep;
        }

        if (this.isStuckTogether)
        {
            this.transform.position = new Vector3(this.backCurrentPlayer.position.x, this.transform.position.y, this.backCurrentPlayer.position.z);
            this.transform.localEulerAngles = new Vector3(this.transform.localEulerAngles.x, this.currentPlayer.localEulerAngles.y, this.transform.localEulerAngles.z);

            this.animator.SetBool("isWalkCrawl", false);
            this.animator.SetBool("isStuckTogether", true);

            if (this.audioEnemySource.clip != this.monsterFeeding)
            {
                this.audioEnemySource.Stop();
                this.audioEnemySource.clip = this.monsterFeeding;
            }

            if(!this.audioEnemySource.isPlaying)
                this.audioEnemySource.Play();

            DistanciaDoPlayer = Vector3.Distance(this.currentPlayer.transform.position, transform.position);
            this.checkAttack();
            this.AttackRoutine();
            return;
        }

        if (this.audioEnemySource.clip != this.footstep)
        {
            this.audioEnemySource.Stop();
            this.audioEnemySource.clip = this.footstep;
        }

        if (!this.audioEnemySource.isPlaying)
            this.audioEnemySource.Play();

        this.animator.SetBool("isWalkCrawl", true);
        this.animator.SetBool("isStuckTogether", false);

        this.checkPlayersDistance();
        DistanciaDoPlayer = Vector3.Distance(this.currentPlayer.transform.position, transform.position);
        DistanciaDoAIPoint = Vector3.Distance(DestinosAleatorios[AIPointAtual].transform.position, transform.position);

        //============================== RAYCAST ===================================//
        RaycastHit hit;
        Vector3 deOnde = transform.position;
        Vector3 paraOnde = this.currentPlayer.transform.position;
        Vector3 direction = paraOnde - deOnde;
        if (Physics.Raycast(transform.position, direction, out hit, 1000) && DistanciaDoPlayer < DistanciaDePercepcao)
        {
            if (hit.collider.gameObject.CompareTag("Player"))
            {
                this.isSeeingPlayer = true;
            }
            else if (hit.collider.gameObject.CompareTag("GoodSpirit"))
            {
                this.isSeeingPlayer = false;
            }
            else
            {
                this.isSeeingPlayer = false;
            }
        }
        //================ CHECHAGENS E DECISOES DO INIMIGO ================//
        if (DistanciaDoPlayer > DistanciaDePercepcao)
        {
            Passear(DestinosAleatorios[AIPointAtual].position, this.enemy.speedWalk);
        }
        if (DistanciaDoPlayer <= DistanciaDePercepcao && DistanciaDoPlayer > DistanciaDeSeguir)
        {
            if (this.isSeeingPlayer == true)
            {
                Olhar();
            }
            else
            {
                Passear(DestinosAleatorios[AIPointAtual].position, this.enemy.speedWalk);
            }
        }
        if (DistanciaDoPlayer <= DistanciaDeSeguir && DistanciaDoPlayer > DistanciaDeAtacar)
        {
            if (this.isSeeingPlayer == true)
            {
                Perseguir(this.enemy.chaseSpeed);
                PerseguindoAlgo = true;
            }
            else
            {
                Passear(DestinosAleatorios[AIPointAtual].position, this.enemy.speedWalk);
            }
        }

        this.checkAttack();

        //COMANDOS DE PASSEAR
        if (DistanciaDoAIPoint <= 2)
        {
            AIPointAtual = Random.Range(0, DestinosAleatorios.Length);
            Passear(DestinosAleatorios[AIPointAtual].position, this.enemy.speedWalk);
        }
        //CONTADORES DE PERSEGUICAO
        if (contadorPerseguindoAlgo == true)
        {
            cronometroDaPerseguicao += Time.deltaTime;
        }
        if (cronometroDaPerseguicao >= 5 && this.isSeeingPlayer == false)
        {
            contadorPerseguindoAlgo = false;
            cronometroDaPerseguicao = 0;
            PerseguindoAlgo = false;
        }

        this.AttackRoutine();

        if (abriuPorta)
        {
            this.cronometroAbriuPorta += Time.deltaTime;
            if (this.cronometroAbriuPorta >= 5)
            {
                this.cronometroAbriuPorta = 0;
                this.abriuPorta = false;
                this.naoQuisAbrirPorta = false;
            }
        }
        else
        {
            if (!naoQuisAbrirPorta)
                this.checkOpenDoor();
            else
            {
                this.AIPointAtual = Random.Range(0, DestinosAleatorios.Length);
                this.AIPointChange = true;
            }
        }

        if (this.AIPointChange)
        {
            this.cronometroAIPointChange += Time.deltaTime;
            if(this.cronometroAIPointChange >= 3)
            {
                this.naoQuisAbrirPorta = false;
                this.cronometroAIPointChange = 0;
                this.AIPointChange = false;
            }
        }
    }
    
    private void checkAttack()
    {
        if (DistanciaDoPlayer <= DistanciaDeAtacar)
        {
            Atacar();

            if (!this.isStuckTogether)
                this.isStuckTogether = this.shouldStuckTogether();
        }
    }

    private void AttackRoutine()
    {
        // CONTADOR DE ATAQUE
        if (atacandoAlgo == true)
        {
            cronometroAtaque += Time.deltaTime;
        }
        if (cronometroAtaque >= TempoPorAtaque && DistanciaDoPlayer <= DistanciaDeAtacar)
        {
            atacandoAlgo = true;
            cronometroAtaque = 0;
            if(this.currentPlayer != null && this.currentPlayer.GetComponent<PlayerStats>().enabled)
                this.currentPlayer.GetComponent<PlayerStats>().RecebeuAtaque(this.damage);
            Debug.Log("recebeuAtaque");
        }
        else if (cronometroAtaque >= TempoPorAtaque && DistanciaDoPlayer > DistanciaDeAtacar)
        {
            atacandoAlgo = false;
            cronometroAtaque = 0;
            Debug.Log("errou");
        }
    }

    void OnCall()
    {
        if (this.model.activeInHierarchy)
            this.model.SetActive(false);

        if (this.GetComponent<NavMeshAgent>().enabled == false)
            this.GetComponent<NavMeshAgent>().enabled = true;

        for (int i = 0; i < this.doors.Length; i++)
        {
            this.doorDistance[i] = Vector3.Distance(this.doors[i].transform.position, this.transform.position);
            if (this.doorDistance[i] <= this.distanciaMaxima)
            {
                if (this.doors[i].GetComponent<Door>().estaFechada)
                    this.doors[i].GetComponent<Door>().EventoUpdatePorta();
            }
        }

        if (!this.hasArrived)
        {
            if (!hasDestiny)
            {
                if (this.firstFloor)
                    this.callDestiny = this.checkLessDistance(this.callPosition, this.convertTransformToVector3Array(this.centerAIPoints2Floor));
                else
                    this.callDestiny = this.checkLessDistance(this.callPosition, this.convertTransformToVector3Array(this.centerAIPoints1Floor));

                this.hasDestiny = true;
            }
            else
            {
                this.animator.SetBool("isWalkCrawl", true);
                if (this.audioEnemySource.isPlaying)
                    this.audioEnemySource.Stop();
                this.destinyDistance = Vector3.Distance(this.callDestiny, this.transform.position);
                if (this.destinyDistance <= 1)
                {
                    this.hasDestiny = false;
                    this.hasArrived = true;
                }
                else
                    this.Passear(this.callDestiny, this.enemy.speedWalkInCallToArrive);
            }
        }
        else
        {
            if (!this.hasDestiny)
            {
                if (this.firstFloor)
                    this.callDestiny = this.checkLessDistance(this.callPosition, this.convertTransformToVector3Array(this.centerAIPoints1Floor));
                else
                    this.callDestiny = this.checkLessDistance(this.callPosition, this.convertTransformToVector3Array(this.centerAIPoints2Floor));

                this.hasDestiny = true;
            }
            else
            {
                if (!this.audioEnemySource.isPlaying)
                {
                    this.audioEnemySource.clip = this.footstep;
                    this.audioEnemySource.Play();
                }

                    this.animator.SetBool("isWalkCrawl", true);
                this.audioEnemySource.pitch = 0.6f;

                this.destinyDistance = Vector3.Distance(this.callDestiny, this.transform.position);
                if (this.destinyDistance <= 1)
                {

                    this.model.SetActive(true);
                    if (this.audioEnemySource.isPlaying)
                        this.audioEnemySource.Stop();

                    this.animator.SetBool("isWalkCrawl", false);
                    this.timeInDestiny += Time.deltaTime;
                    if (this.timeInDestiny >= 3)
                    {
                        this.enemyState = EnemyState.Attack;
                        this.hasDestiny = false;
                        this.hasArrived = false;
                        this.canAttack = true;
                        int indexLessDistance = this.checkIndexToLessDistance(this.transform.position, this.convertTransformToVector3Array(this.players));
                        if (Vector3.Distance(this.players[indexLessDistance].position, this.transform.position) < 15)
                        {
                            if (PhotonNetwork.LocalPlayer.NickName == this.players[indexLessDistance].gameObject.name)
                                GameObject.Find(this.players[indexLessDistance].gameObject.name).GetComponent<PlayerStats>().CompleteObjectiveRadio();

                        }
                        this.timeInDestiny = 0;
                    }
                }
                else
                    this.Passear(this.callDestiny, this.enemy.speedWalkInCallToDestiny);
            }
        }
    }
    private Vector3[] convertTransformToVector3Array(Transform[] transforms)
    {
        Vector3[] vectors = new Vector3[transforms.Length];
        for(int i = 0; i < vectors.Length; i++)
        {
            vectors[i] = transforms[i].position;
        }
        return vectors;
    }
    private int checkIndexToLessDistance(Vector3 current, Vector3[] distances)
    {
        float lessDistance = 100000;
        int index = 0;
        for (int i = 0; i < distances.Length; i++)
        {
            float distance = Vector3.Distance(distances[i], current);
            if (distance < lessDistance)
            {
                lessDistance = distance;
                index = i;
            }
        }
        return index;
    }

    private Vector3 checkLessDistance(Vector3 current, Vector3[] distances)
    {
        float lessDistance = 100000;
        int index = 0;
        for (int i = 0; i < distances.Length; i++)
        {
            float distance = Vector3.Distance(distances[i], current);
            if (distance < lessDistance)
            {
                lessDistance = distance;
                index = i;
            }
        }
        return distances[index];
    }

    public void UpdatePlayerList()
    {
        GameObject[] playersObj = GameObject.FindGameObjectsWithTag("Player");
        if(playersObj.Length != this.players.Length)
        {
            this.players = new Transform[playersObj.Length];
            for(int i = 0; i < this.players.Length; i++)
            {
                this.players[i] = playersObj[i].transform;
            }
        }
    }

    void checkPlayersDistance()
    {
        float distance = 10000000;
        foreach(Transform t in this.players)
        {
            float playerDistance = Vector3.Distance(t.transform.position, this.transform.position);
            if(playerDistance < distance)
            {
                distance = playerDistance;
                this.currentPlayer = t;
                this.backCurrentPlayer = this.currentPlayer.Find("BackCheck");
            }
        }
    }
    void checkOpenDoor()
    {
        for(int i = 0; i < this.doors.Length; i++)
        {
            this.doorDistance[i] = Vector3.Distance(this.doors[i].transform.position, this.transform.position);
            if(this.doorDistance[i] <= this.distanciaMaxima)
            {
                if(!this.shouldOpenDoor() && this.doors[i].GetComponent<Door>().estaFechada)
                {
                    this.naoQuisAbrirPorta = true;
                    return;
                }
                if((i == this.AIPointAtual) && this.doors[i].GetComponent<Door>().estaFechada)
                {
                    this.doors[i].GetComponent<Door>().EventoUpdatePorta();
                    this.abriuPorta = true;
                }
            }
        }
    }

    private bool shouldStuckTogether()
    {
        int n = Random.Range(100, 1000);
        if (n % 3 == 0)
        {
            return true;
        }
        return false;
    }

    private bool shouldOpenDoor()
    {
        int n = Random.Range(0, 10000);
        if(n % 2 == 0)
        {
            return true;
        }
        return false;
    }
    void Passear(Vector3 destination, float velocity)
    {
        if (PerseguindoAlgo == false)
        {
            naveMesh.acceleration = 5;
            naveMesh.speed = velocity;
            naveMesh.destination = destination;
        }
        else if (PerseguindoAlgo == true)
        {
            contadorPerseguindoAlgo = true;
        }
    }
    void Olhar()
    {
        naveMesh.speed = 0;
        transform.LookAt(this.currentPlayer);
    }
    void Perseguir(float velocity)
    {
        naveMesh.acceleration = 8;
        naveMesh.speed = velocity;
        naveMesh.destination = this.currentPlayer.position;
    }
    void Atacar()
    {
        atacandoAlgo = true;
    }

    public void FreePlayerStuckTogether()
    {
        this.pv.RPC("FreePlayerStuckTogetherRPC", RpcTarget.All);
    }

    public void StunEnemyAfterColliderSalt()
    {
        this.pv.RPC("StunEnemyAfterColliderSaltRPC", RpcTarget.All);
    }
    private IEnumerator UnStun()
    {
        float startTimeToUnStun = this.timeToUnStun;
        while (this.timeToUnStun > 0)
        {
            this.timeToUnStun -= startTimeToUnStun * Time.deltaTime / 5;
            yield return null;

        }
        this.timeToUnStun = startTimeToUnStun;
        this.stun = false;
    }

    public void PlayerIsDead()
    {
        this.pv.RPC("PlayerIsDeadRPC", RpcTarget.All);
    }

    [PunRPC]
    private void PlayerIsDeadRPC()
    {
        if (this.currentPlayer != null && this.currentPlayer.gameObject.tag == "PlayerDead")
        {
            this.isStuckTogether = false;
            this.isSeeingPlayer = false;
            this.UpdatePlayerList();
            this.currentPlayer = null;
        }
    }

    public void UpdateCountInHouse(bool add)
    {
        this.pv.RPC("UpdateCountInHouseRPC", RpcTarget.All, add);
    }
    IEnumerator wait()
    {
        yield return new WaitForSeconds(0.5f);
    }

    [PunRPC]
    private void UpdateCountInHouseRPC(bool add)
    {
        StartCoroutine(this.wait());
        if (add)
            this.countInHouse++;
        else
            this.countInHouse--;

        Debug.Log("Fecha a porta!");
    }
    public void UpdateStateEnemy(EnemyState enemyState)
    {
        this.pv.RPC("UpdateStateEnemyRPC", RpcTarget.All, enemyState);
    }

    public void UpdateCallPositionAndFloor(Vector3 callPosition, bool firstFloor, bool secondFloor)
    {
        this.pv.RPC("UpdateCallPositionAndFloorRPC", RpcTarget.All, callPosition, firstFloor, secondFloor);
    }
    
    [PunRPC]  
    private void StunEnemyAfterColliderSaltRPC()
    {
        Debug.Log("stuna inimigo!");
        this.stun = true;
        StartCoroutine(this.UnStun());
    }

    [PunRPC]    
    private void FreePlayerStuckTogetherRPC()
    {
        Debug.Log("libera player");
        this.isStuckTogether = false;
        this.stun = true;
        StartCoroutine(this.UnStun());
    }

    [PunRPC]
    private void UpdateStateEnemyRPC(EnemyState enemyState)
    {
        this.enemyState = enemyState;
    }

    [PunRPC]
    private void UpdateCallPositionAndFloorRPC(Vector3 callPosition, bool firstFloor, bool secondFloor)
    {
        this.callPosition = callPosition;
        this.firstFloor = firstFloor;
        this.secondFloor = secondFloor;
    }
}