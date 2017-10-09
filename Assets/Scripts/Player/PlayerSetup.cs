using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
//using PlayerController = UnityEngine.Networking.PlayerController;


[RequireComponent(typeof(Player))]
[RequireComponent(typeof(PlayerController))]
public class PlayerSetup : NetworkBehaviour
{

    [SerializeField]
    Behaviour[] componentsToDisable;//компоненты на сцене

    [SerializeField]
    private string remoteLayerMask = "RemotePlayer";//имя удалённого слоя игрока

    //Camera sceneCamera;//загрузочная камера

    [SerializeField]
    string dontDrawLayerName = "DontDraw";//название слоя который не рисуется

    [SerializeField]
    GameObject playerGraphics;

    [SerializeField]
    GameObject playerUIPrefab; //ссылка на префаб player UI

    [HideInInspector]
    public GameObject playerUIInstance;


	// Use this for initialization
	void Start () {
        //если мы не локальный игрок, т.е. объект на другой машине
	    if (!isLocalPlayer)
	    {
	        //выключить не нужные компоненты
	        DisableComponents();
            AssignRemoteLayer();

	    }
	    else//если локальный
	    {
	        //sceneCamera = Camera.main; //получаем ссылку на загрузочную камру
	        //if (sceneCamera != null)
	        //{
	        //    sceneCamera.gameObject.SetActive(false); //выключаем загрузочную камеру
	        //}

            //GameManager.instance.SetSceneCameraActive(false);


            //отключаем не нужную графику у локального игрока
            Util.SetLayerRecursively(playerGraphics, LayerMask.NameToLayer(dontDrawLayerName));
	        //SetLayerRecursively(playerGraphics, LayerMask.NameToLayer(dontDrawLayerName));


            //подключаем PlayerUI
	        playerUIInstance =  Instantiate(playerUIPrefab);
	        playerUIInstance.name = playerUIPrefab.name;

            //Configure PlayerUI
	        PlayerUI ui = playerUIInstance.GetComponent<PlayerUI>();
            if(ui == null)
                Debug.LogError("No PlayerUI component on PlayerUI prefab. " + ui.name);
            //передать контроллер в UI
            ui.SetController(GetComponent<PlayerController>());


            //RegisterPlayer();
            //регистрируем игрока
            GetComponent<Player>().SetupPlayer();
        }


	}

    //void SetLayerRecursively(GameObject obj, int newLayer)
    //{
    //    if (obj == null)
    //        return;
    //    
    //    obj.layer = newLayer;
    //
    //    foreach (Transform child in obj.transform)
    //    {
    //        SetLayerRecursively(child.gameObject, newLayer);
    //    }
    //}


    public override void OnStartClient()
        //вызывается когда загружается новый клиент
    {
        base.OnStartClient();

        string _netID = GetComponent<NetworkIdentity>().netId.ToString();//получаем ID игрока
        Player _player = GetComponent<Player>();//получаем класс самого игрока
        GameManager.RegisterPlayer(_netID, _player);//заносим игроков в словарь
    }

    //void RegisterPlayer()
    //{
    //    //получить идентификатор и утсановить новое имя игрока с ID
    //    string _ID = "Player " + GetComponent<NetworkIdentity>().netId;
    //    //переименовать игрока по ID
    //    transform.name = _ID;
    //}

    void AssignRemoteLayer()
        //определить удалённый слой
    {
        //меняет назвавние слоя у подключённого клиента
        gameObject.layer = LayerMask.NameToLayer(remoteLayerMask);
    }

    void DisableComponents()
    {
        foreach (Behaviour behaviour in componentsToDisable)
        {
            behaviour.enabled = false;
        }
    }


    void OnDisable()
    {
        //удаляем Player UI с экрана
        Destroy(playerUIInstance);

        //if(sceneCamera != null)
        //    sceneCamera.gameObject.SetActive(true);

        //выключаем главную камеру у локального игрока
        if(isLocalPlayer)
            GameManager.instance.SetSceneCameraActive(true);

        GameManager.UnRegisterPlayer(transform.name);//удаляем игрока


    }


}
