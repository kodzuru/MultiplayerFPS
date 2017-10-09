using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DatabaseControl;
using UnityEngine.SceneManagement;

public class UserAccountManager : MonoBehaviour {

    #region SINGLETON_USER_ACCOUNT_MANAGER
    //----------------SINGLETON----------------------//
    public static UserAccountManager instance;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this);
    }
    //----------------SINGLETON----------------------//
    #endregion

    //These store the username and password of the player when they have logged in
    public static string PlayerUsername { get; protected set; }
    public static string PlayerPassword = "";
    public static string LoggedIn_Data { get; protected set; }


    //залогинен ли игрок
    public static bool IsLoggedIn { get; protected set; }

    [SerializeField]
    string loggedInSceneName = "Lobby";
    [SerializeField]
    string loggedOutSceneName = "LoginMenu";


    public void LogOut()
    {
        PlayerUsername = "";
        PlayerPassword = "";

        IsLoggedIn = false;

        SceneManager.LoadScene(loggedOutSceneName);

        Debug.Log("LogOut");
    }

    public void LogIn(string username, string password)
    {
        PlayerUsername = username;
        PlayerPassword = password;

        IsLoggedIn = true;

        SceneManager.LoadScene(loggedInSceneName);

        Debug.Log("Logeg in as a: " + PlayerUsername);
    }

    public void SendData(string data)
    {
        //Called when the player hits 'Set Data' to change the data string on their account. Switches UI to 'Loading...' and starts coroutine to set the players data string on the server
        if (IsLoggedIn)
            StartCoroutine(SetData(PlayerUsername, PlayerPassword, data));
        else
            Debug.LogError("Мы не залогинены для отправления данных в БД");
    }

    public void TakeData()
    {
        //Called when the player hits 'Get Data' to retrieve the data string on their account. Switches UI to 'Loading...' and starts coroutine to get the players data string from the server
        if (IsLoggedIn)
            StartCoroutine(GetData(PlayerUsername, PlayerPassword));
        else
            Debug.LogError("Мы не залогинены для получения данных из БД");

    }

    IEnumerator GetData(string playerUsername, string playerPassword)
    {
        IEnumerator e = DCF.GetUserData(playerUsername, playerPassword); // << Send request to get the player's data string. Provides the username and password
        while (e.MoveNext())
        {
            yield return e.Current;
        }
        string response = e.Current as string; // << The returned string from the request

        if (response == "Error")
        {
            //There was another error. Automatically logs player out. This error message should never appear, but is here just in case.
            instance.LogOut();
            Debug.LogError("Error: Unknown Error. Please try again later.");
        }
        else
        {
            //The player's data was retrieved. Goes back to loggedIn UI and displays the retrieved data in the InputField
            LoggedIn_Data = response;
        }
    }

    IEnumerator SetData(string playerUsername, string playerPassword, string data)
    {
        IEnumerator e = DCF.SetUserData(playerUsername, playerPassword, data); // << Send request to set the player's data string. Provides the username, password and new data string
        while (e.MoveNext())
        {
            yield return e.Current;
        }
        string response = e.Current as string; // << The returned string from the request

        if (response == "Success")
        {
            //The data string was set correctly. Goes back to LoggedIn UI
            Debug.LogAssertion("The data string was set correctly into DB");
        }
        else
        {
            //There was another error. Automatically logs player out. This error message should never appear, but is here just in case.
           instance.LogOut();

            Debug.LogError("Error: Unknown Error. Please try again later.");
        }
    }

   
}
