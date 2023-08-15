using UnityEngine;

public class GunController : MonoBehaviour
{
    public float GunHeight { get { return gunHold.position.y; } }

    [SerializeField] private Transform gunHold;
    [SerializeField] private Gun[] allGuns;

    private Gun _equippedGun;

    private void Start()
    {
        
    }

    public void EquipGun(Gun gunToEquip)
    {
        if(_equippedGun != null)
            Destroy(_equippedGun.gameObject);

        _equippedGun = Instantiate(gunToEquip, gunHold.position, gunHold.rotation, gunHold);
    }

    public void EquipGun(int gunIndex)
    {
        EquipGun(allGuns[gunIndex]);
    }

    public void Aim(Vector3 aimPoint)
    {
        if (_equippedGun != null)
            _equippedGun.Aim(aimPoint);
    }

    public void Reload()
    {
        if (_equippedGun != null)
            _equippedGun.Reload();
    }

    public void OnTriggerHold()
    {
        if(_equippedGun != null)
            _equippedGun.OnTriggerHold();
    }

    public void OnTriggerRelease()
    {
        if (_equippedGun != null)
            _equippedGun.OnTriggerRelease();
    }
}