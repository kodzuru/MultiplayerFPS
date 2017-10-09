using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerWeapon
{

    public string name = "Glock";//названия оружия

    public int damage = 10;//дамаг оружия

    public float range = 100f; //расстояния стрельбы оружия

    public float fireRate = 0f;//скорость стрельбы

    public GameObject graphics;//ссылка на графику оружия


}
