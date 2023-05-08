using UnityEngine;

public class GunController : MonoBehaviour
{
    [SerializeField] private Transform gunHold;
    [SerializeField] private Gun startingGun;

    private Gun _equippedGun;

    private void Start()
    {
        if(startingGun != null)
            EquipGun(startingGun);
    }

    public void EquipGun(Gun gunToEquip)
    {
        if(_equippedGun != null)
            Destroy(_equippedGun.gameObject);

        _equippedGun = Instantiate(gunToEquip, gunHold.position, gunHold.rotation, gunHold);
    }

    public void Shoot()
    {
        if(_equippedGun != null)
            _equippedGun.Shoot();
    }
}