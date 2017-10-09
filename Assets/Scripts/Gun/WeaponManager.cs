using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class WeaponManager : NetworkBehaviour
{
    [SerializeField]
    private string weaponLayerName = "Weapon";//называние слоя оружия

    [SerializeField]
    private Transform weaponHolder;//ссылка на позицию куда крепится оружие


    [SerializeField]
    private PlayerWeapon primaryWeapon;//префаб оружия

    private PlayerWeapon currentWeapon;//текущее оружие
    private WeaponGraphics currentGraphics;//текущая графика оружия


    void Start()
    {
        EquipWeapon(primaryWeapon);
    }

    void EquipWeapon(PlayerWeapon _weapon)
    {
        currentWeapon = _weapon;//одеваем оружие
        //спавно оружия на сцене
        GameObject _weaponInstance =  (GameObject)Instantiate(_weapon.graphics, weaponHolder.position, weaponHolder.rotation);
        //крепим оружие как потомка
        _weaponInstance.transform.SetParent(weaponHolder);

        //получить из объекта компонент графики выстрелов оружия
        currentGraphics = _weaponInstance.GetComponent<WeaponGraphics>();
        if(currentGraphics == null)
            Debug.LogError("No WeaponGraphics component on the weapon object. " + _weaponInstance.name);


        //если игрок локальный
        if (isLocalPlayer)
        {
            //_weaponInstance.layer = LayerMask.NameToLayer(weaponLayerName);//помещаем оружие на слой weapon
            //помещаем оружие на слой weapon - рекурсивным методом
            Util.SetLayerRecursively(_weaponInstance, LayerMask.NameToLayer(weaponLayerName));
        }
    }


    public PlayerWeapon GetCurrentWeapon()
    {
        return currentWeapon;
    }

    public WeaponGraphics GetCurrentGraphics()
    {
        return currentGraphics;
    }


}
