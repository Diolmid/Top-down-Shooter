using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField] private float timeBetweenShots = 1f;
    [SerializeField] private float muzzleVelocity = 35f;

    [SerializeField] private Projectile projectile;
    [SerializeField] private Transform muzzle;
    [SerializeField] private Transform shell;
    [SerializeField] private Transform shellEjection;

    private float _nextShotTime;

    private MuzzleFlash _muzzleFlash;

    private void Awake()
    {
        //_muzzleFlash = GetComponent<MuzzleFlash>();
    }

    public void Shoot()
    {
        if(Time.time > _nextShotTime)
        {
            _nextShotTime = Time.time + timeBetweenShots;

            Projectile newProjectile = Instantiate(projectile, muzzle.position, muzzle.rotation);
            newProjectile.SetSpeed(muzzleVelocity);

            Instantiate(shell, shellEjection.position, shellEjection.rotation);
            //_muzzleFlash.Activate();
        }
    }
}