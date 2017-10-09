using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(PlayerSetup))]
public class Player : NetworkBehaviour
{

    [SyncVar]
    private bool _isDead = false; //переменная мёртвый ли игрок
    public bool isDead
    {
        get { return _isDead; }
        protected set { _isDead = value; }
    }

    [SerializeField]
    private int maxHealth = 100;//максимальное значение ХП

    [SyncVar, SerializeField]//пушит на оба клинта переменную
    private int currentHealth;

    [SerializeField]
    private Behaviour[] disableOnDeath;//объекты которые отключаем после смерти
    private bool[] wasEnabled;//объекты которые необходимо включить

    [SerializeField]
    private GameObject[] disableGameObjectsOnDeath;//объекты которые отключаем после смерти


    [SerializeField]
    private GameObject deathEffect;//ссылка на частицы смерти
    [SerializeField]
    private GameObject spawnEffect;//ссылка на частицы респавна


    private bool firstSetup = true;// проверяет первый ли запуск игрока



    public void SetupPlayer()
    {

        if (isLocalPlayer)
        {
            //Switch cameras
            GameManager.instance.SetSceneCameraActive(false);
            GetComponent<PlayerSetup>().playerUIInstance.SetActive(true);

        }

        CmdBroadCastNewPlayerSetup();
    }

    [Command]
    private void CmdBroadCastNewPlayerSetup()
    {
        RpcSetupPlayerOnAllClient();
    }
    [ClientRpc]
    private void RpcSetupPlayerOnAllClient()
    {
        if (firstSetup)
        {
            wasEnabled = new bool[disableOnDeath.Length];
            for (int i = 0; i < wasEnabled.Length; i++)
            {
                wasEnabled[i] = disableOnDeath[i].enabled;
            }
            firstSetup = false;
        }

        SetDefaults();
    }

    void Update()
    {
        if(!isLocalPlayer)
            return;
        if (Input.GetKeyDown(KeyCode.K))
        {
            RpcTakeDamage(40);
        }

    }


    public void SetDefaults()
    {
        isDead = false;

        currentHealth = maxHealth;

        //Enable components
        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = wasEnabled[i];
        }

        //Enable GameObjects
        foreach (GameObject o in disableGameObjectsOnDeath)
        {
            o.SetActive(true);
        }

        Collider _col = GetComponent<Collider>();
        if (_col != null)
            _col.enabled = true;


        //Create Spawn effect
        GameObject _spawnEffect = (GameObject)Instantiate(spawnEffect, transform.position, spawnEffect.transform.rotation);
        Destroy(_spawnEffect, 3f);

    }

    [ClientRpc]//This is an attribute that can be put on methods of NetworkBehaviour classes to allow them to be invoked on clients from a server.
    public void RpcTakeDamage(int _damage)
    {
        //если мертвы не обрабатываем функцию
        if(isDead)
            return;



        currentHealth -= _damage;//применяем полученый димаг
        Debug.Log(transform.name + " now has " + currentHealth + " health.");

        //если ХП меньше нуля умираем
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;

        //disable player component
        foreach (Behaviour b in disableOnDeath)
        {
            b.enabled = false;
        }

        //Disable GameObjects
        foreach (GameObject o in disableGameObjectsOnDeath)
        {
            o.SetActive(false);
        }


        //если коллайдеры включены, отключаем их
        Collider _col = GetComponent<Collider>();
        if (_col != null)
            _col.enabled = false;

        //Spawn deathEffect
        GameObject _deathEffect = (GameObject)Instantiate(deathEffect, transform.position, deathEffect.transform.rotation);
        Destroy(_deathEffect, 3f);

        Debug.Log(transform.name + " is DEAD!");

        //Switch cameras
        if (isLocalPlayer)
        {
            GameManager.instance.SetSceneCameraActive(true);
            GetComponent<PlayerSetup>().playerUIInstance.SetActive(false);
        }


        //Call respawn method
        StartCoroutine(Respawn());

    }


    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(GameManager.instance.matchSettings.respawnTime);//задержка

        //получаем позицию спавна игрока
        Transform _spawnPoint = NetworkManager.singleton.GetStartPosition();
        transform.position = _spawnPoint.position;
        transform.rotation = _spawnPoint.rotation;

        yield return new WaitForSeconds(0.1f);

        SetupPlayer();

        Debug.Log("Respawn player " + transform.name);

    }


}
