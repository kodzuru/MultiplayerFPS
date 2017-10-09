using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;

public class JoinGame : MonoBehaviour
{
    private NetworkManager networkManager;

    List<GameObject> roomList = new List<GameObject>(); //список хостов которых удалост найти

    [SerializeField]
    private Text statusText;//текст loading

    [SerializeField]
    private GameObject roomListItemPrefab; //ссылка на префаб кнопки

    [SerializeField]
    private Transform roomListParent;//ссылка на позицию родителя кнопки


	// Use this for initialization
	void Start ()
    {
		networkManager = NetworkManager.singleton;
	    if (networkManager.matchMaker == null)
	    {
	        networkManager.StartMatchMaker();
	    }

        //обновить список хостов
	    RefreshRoomList();


    }

    public void RefreshRoomList()
    {
        ClearRoomList();
        networkManager.matchMaker.ListMatches(0, 20, "", true, 0, 0, OnMatchList);
        statusText.text = "Loading...";
    }

    public void OnMatchList(bool success, string extendedInfo, List<MatchInfoSnapshot> matchList)
    {
        // ...
        statusText.text = "";

        //если матчей не нашло
        if (!success || matchList == null)
        {
            statusText.text = "Couldn't get matches";
            return;
        }
        //поиск хостов
        foreach (MatchInfoSnapshot matchInfoSnapshot in matchList)
        {
            //спавним кнопку
            GameObject _roomListItemGO = Instantiate(roomListItemPrefab);
            //устанавливаем родителя кнопке
            _roomListItemGO.transform.SetParent(roomListParent);

            //получаем компонент кнопки
            RoomListItem _roomListItem = _roomListItemGO.GetComponent<RoomListItem>();
            if (_roomListItem != null)
            {
                //передаём инфу о кнопке в класс RoomListItem с информацией о хосте и делегата подключения к хосту
                _roomListItem.Setup(matchInfoSnapshot, snapshot =>
                {
                    Debug.Log("joining match: " + snapshot.name);
                    //подключаемся к матчу
                    networkManager.matchMaker.JoinMatch(snapshot.networkId, "", "", "", 0, 0, networkManager.OnMatchJoined);
                    //очищаем список хостов
                    ClearRoomList();
                    //меняем текст статуса хостов
                    statusText.text = "JOINING...";
                });
            }


            /*
               Have a component sit on the gameobject
               that will take care of setting up the name/amount of users
               as well as setting up a callback function that will join the game.

                method will call when will chose room
            */
            //добавляем объект кнопки в список хостов
            roomList.Add(_roomListItemGO);
         }

        //если найдено 0 хостов
        if (roomList.Count == 0)
        {
            statusText.text = "No Room found.";
        }


    }

    void ClearRoomList()
    {
        //уничтожаем объекты с хостами(кнопк UI)
        foreach (GameObject o in roomList)
        {
            Destroy(o);
        }
        roomList.Clear();
    }

}
