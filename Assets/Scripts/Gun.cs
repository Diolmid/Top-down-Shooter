using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField] private float timeBetweenShots = 1f;
    [SerializeField] private float muzzleVelocity = 35f;

    [SerializeField] private Projectile projectile;
    [SerializeField] private Transform muzzle;

    private float _nextShotTime;

    public void Shoot()
    {
        if(Time.time > _nextShotTime)
        {
            _nextShotTime = Time.time + timeBetweenShots;

            Projectile newProjectile = Instantiate(projectile, muzzle.position, muzzle.rotation);
            newProjectile.SetSpeed(muzzleVelocity);
        }

    }
}