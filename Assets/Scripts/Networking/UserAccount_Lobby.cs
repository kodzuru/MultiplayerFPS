using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UserAccount_Lobby : MonoBehaviour
{

    [SerializeField]
    private Text usernameText;

    void Start()
    {
        if(UserAccountManager.IsLoggedIn)
            usernameText.text = UserAccountManager.PlayerUsername;
    }

    public void LogoutButtonPressed()
    {
        if(UserAccountManager.IsLoggedIn)
            UserAccountManager.instance.LogOut();
    }
}
