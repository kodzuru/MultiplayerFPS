using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class HostGame : MonoBehaviour
{


    private uint roomSize = 6;//количество возможных игроков в руме

    private string roomName;// имя румы

    private NetworkManager networkManager; //ссылка на объект network Manager

    // Use this for initialization
    void Start()
    {
        networkManager = NetworkManager.singleton; //получаем инстанс
        if (networkManager.matchMaker == null) //если получили инстанс
        {
            networkManager.StartMatchMaker();//запускаем match maker
        }
    }



    public void SetRoomName(string _name)
    {
        roomName = _name;
    }

    public void CreateRoom()
    {
        if (roomName != "" && roomName != null)
        {
            Debug.Log("Creating Room: " + roomName + " with room for " + roomSize + " players.");

            //Create room
            networkManager.matchMaker.CreateMatch(roomName, roomSize, true, "", "", "", 0, 0, networkManager.OnMatchCreate);
        }
    }


	
}
