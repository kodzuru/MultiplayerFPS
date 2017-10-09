using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{


    [SerializeField]
    RectTransform thrusterFuelFill;//ссылка на UI

    [SerializeField]
    private GameObject pauseMenu; //ссылка на UI меню паузы

    private PlayerController controller;
    

    public void SetController(PlayerController _controller)
    {
        controller = _controller;
    }

    void SetFuelAmount(float _amount)
    {
        thrusterFuelFill.localScale = new Vector3(1f, _amount, 1f);
    }


    void Start()
    {
        PauseMenu.isOn = false;
    }

    void Update()
    {
        SetFuelAmount(controller.GetThrusterFuelAmount());

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("ESCAPE PUSHED");
            TogglePauseMenu();
        }
    }

    void TogglePauseMenu()
    {
        pauseMenu.SetActive(!pauseMenu.activeSelf); 
        PauseMenu.isOn = pauseMenu.activeSelf;
    }


}
