using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public MatchSettings matchSettings;

    [SerializeField]
    private GameObject sceneCamera; //камера на сцене


    #region SINGLETON_GameManager
    //ссылка на класс GameManager 
    public static GameManager instance;

        void Awake()
        {
            if (instance != null)
            {
                Debug.LogError("More then one GameManager in scene");
            }
            else
            {
                instance = this;
            }
        }
    #endregion


    //активация\дизактивация камеры
    public void SetSceneCameraActive(bool isActive)
    {
        if (sceneCamera == null)
            return;

        sceneCamera.SetActive(isActive);
    }


    #region PLAYER_TRACKING



    private const string PLAYER_ID_PREFIX = "Player ";//начало имён всех игроков
    private static Dictionary<string, Player> players = new Dictionary<string, Player>(); //коллекция(словарь) ID-PLAYER



    public static void RegisterPlayer(string _netID, Player _player)
        //регистрация игрока
    {
        string _playerID = PLAYER_ID_PREFIX + _netID;//составное ID игрока
        players.Add(_playerID, _player);//заполняем словарь игроками
        _player.transform.name = _playerID;//переименовываем игрока
    }

    public static void UnRegisterPlayer(string _playerID)
        //удаляем регистрацию игрока
    {
        players.Remove(_playerID);//удаляем из словаря
    }

    public static Player GetPlayer(string _playerID)
        //получить игрока по ID
    {
        return players[_playerID];//находим игрока в массиве словаря по ID и возвращаем его класс
    }

    //void OnGUI()
    //{
    //    
    //    GUILayout.BeginArea(new Rect(200, 200, 200, 500));
    //    GUILayout.BeginVertical();
    //
    //    foreach (string _playerID in players.Keys)
    //    {
    //        GUILayout.Label(_playerID + "  -  " + players[_playerID].transform.name);
    //    }
    //
    //    GUILayout.EndVertical();
    //    GUILayout.EndArea();
    //
    //
    //}
#endregion

}
