using UnityEngine;

public class Gun : MonoBehaviour
{
    public enum FireMode { Auto, Burst, Single}
    [SerializeField] private FireMode fireMode;

    [SerializeField] private int bursCount;
    [SerializeField] private float timeBetweenShots = 1f;
    [SerializeField] private float muzzleVelocity = 35f;

    [SerializeField] private Projectile projectile;
    [SerializeField] private Transform[] projectileSpawn;
    [SerializeField] private Transform shell;
    [SerializeField] private Transform shellEjection;

    private float _nextShotTime;
    private int _shotsRemainingInBurst;

    private bool _triggerReleasedSinceLastShot;

    private MuzzleFlash _muzzleFlash;

    private void Awake()
    {
        _muzzleFlash = GetComponent<MuzzleFlash>();
    }

    private void Start()
    {
        _shotsRemainingInBurst = bursCount;
    }

    private void Shoot()
    {
        if(Time.time > _nextShotTime)
        {
            if(fireMode == FireMode.Burst)
            {
                if (_shotsRemainingInBurst == 0)
                    return;

                _shotsRemainingInBurst--;
            }
            else if(fireMode == FireMode.Single)
            {
                if (!_triggerReleasedSinceLastShot)
                    return;
            }

            for (int i = 0; i < projectileSpawn.Length; i++)
            {
                _nextShotTime = Time.time + timeBetweenShots;
                Projectile newProjectile = Instantiate(projectile, projectileSpawn[i].position, projectileSpawn[i].rotation);
                newProjectile.SetSpeed(muzzleVelocity);
            }

            Instantiate(shell, shellEjection.position, shellEjection.rotation);
            _muzzleFlash.Activate();
        }
    }

    public void OnTriggerHold()
    {
        Shoot();
        _triggerReleasedSinceLastShot = false;
    }

    public void OnTriggerRelease()
    {
        _triggerReleasedSinceLastShot = true;
        _shotsRemainingInBurst = bursCount;
    }
}