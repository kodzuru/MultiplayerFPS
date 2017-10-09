using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(WeaponManager))]
public class PlayerShoot : NetworkBehaviour
{

    private const string PLAYER_TAG = "Player";




    [SerializeField]
    private Camera cam;//ссылка на камеру

    //[SerializeField]
    //private PlayerWeapon currentWeapon;//ссылка на класс оружия

    //[SerializeField]
    //private GameObject weaponGFX;//ссылка на оружие

    //[SerializeField]
    //private string weaponLayerName = "Weapon";//называние слоя оружия


    [SerializeField]
    private LayerMask mask; //слой с которым будет взаимодействовать выстрел

    private PlayerWeapon currentWeapon;//ссылка на класс оружия
    private WeaponManager weaponManager;



	// Use this for initialization
	void Start () {

        //если нет ссылки на камеру
	    if (cam == null)
	    {
            Debug.LogError("PlayerShoot: No camera referenced!");
	        enabled = false;//выключаем скрипт
	    }

        //помещаем оружие на слой weapon
	    //weaponGFX.layer = LayerMask.NameToLayer(weaponLayerName);

	    weaponManager = GetComponent<WeaponManager>();


	}
	
	// Update is called once per frame
	void Update ()
	{

        currentWeapon = weaponManager.GetCurrentWeapon();

        //if pause is ON
        if(PauseMenu.isOn)
            return;


	    if (currentWeapon.fireRate <= 0)
	    {
	        //если нажата кнопка стрелять
	        if (Input.GetButtonDown("Fire1"))
	        {
	            Shoot(); //стреляем
	        }
	    }
	    else
	    {
            //если нажата кнопка стрелять
            if (Input.GetButtonDown("Fire1"))
            {
                InvokeRepeating("Shoot", 0f, 1f/currentWeapon.fireRate);
            }else if (Input.GetButtonUp("Fire1"))
            {
                CancelInvoke("Shoot");
            }
        }


    }

    [Client]//всегда вызывается у клиента и никогда на сервере
    void Shoot()
    {

        Debug.Log("T");

        //WeaponGraphics w = weaponManager.GetCurrentGraphics();
        //Instantiate(w, w.transform.position, w.transform.rotation);

        //если не локальный игрок стреляет
        if (!isLocalPlayer)
        {
            return;
        }
        //Стреляем, вызывается на сервере
        CmdOnShoot();


        RaycastHit _hit;//создаём переменную выстрела
        //если мы попали в нужный объект, то...
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out _hit, currentWeapon.range, mask))
        {
            //если тэг нашей цели player
            if (_hit.collider.tag == PLAYER_TAG)
            {
                Debug.Log("We hit " + _hit.collider.name);
                CmdPlayerShot(_hit.collider.name, currentWeapon.damage);//передаём на сервер инфу во что мы попали
            }

            //спавним эффект попадания по объекту, вызывается на сервере
            CmdOnHit(_hit.point, _hit.normal);

        }
    }

    [Command] //вызывается только на сервере
    void CmdPlayerShot(string _playerID, int _damage)
    {
        Debug.Log(_playerID + " has been shot.");

        Player _player = GameManager.GetPlayer(_playerID);//получить класс игрока
        _player.RpcTakeDamage(_damage);//получаем дамагу

    }

    //вызывается на сервере когда игрок стреляет
    [Command]
    void CmdOnShoot()
    {
        RpcDoShootEffect();
    }
    //вызывается на сервере когда игрок попадает
    [Command]
    void CmdOnHit(Vector3 _pos, Vector3 _normal)
        //_pos - позиция попадания, _normal - нормали поверхности куда попали
    {
        RpcDoHitEffect(_pos, _normal);
    }

    [ClientRpc] //вызывается для всех клиентов когда необходим эффект выстрела
    void RpcDoShootEffect()
    {
        //проигрываем систему частиц
        weaponManager.GetCurrentGraphics().muzzleFlash.Play();
    }
    [ClientRpc] //вызывается для всех клиентов когда необходим эффект попадания в цель
    void RpcDoHitEffect(Vector3 _pos, Vector3 _normal)
    {
        //проигрываем систему частиц
        GameObject _hitEffect = (GameObject) Instantiate(weaponManager.GetCurrentGraphics().hitEffectPrefab, _pos, Quaternion.LookRotation(_normal));
        Destroy(_hitEffect, 1f);
    }


}
