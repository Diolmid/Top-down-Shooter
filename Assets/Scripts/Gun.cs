using System.Collections;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public enum FireMode { Auto, Burst, Single}
    [SerializeField] private FireMode fireMode;

    [SerializeField] private int projectilesPerMag;
    [SerializeField] private int bursCount;
    [SerializeField] private float reloadTime = .3f;
    [SerializeField] private float timeBetweenShots = 1f;
    [SerializeField] private float muzzleVelocity = 35f;
    [SerializeField] private Projectile projectile;
    [SerializeField] private Transform[] projectileSpawn;

    [Header("Recoil")]
    [SerializeField] private float recoilMoveSettleTime = .1f;
    [SerializeField] private float recoilRotationSettleTime = .1f;
    [SerializeField] private Vector2 kickMinMax = new Vector2(.05f, .2f);
    [SerializeField] private Vector2 recoilAngleMinMax = new Vector2(3 , 5);

    [Header("Effects")]
    [SerializeField] private Transform shell;
    [SerializeField] private Transform shellEjection;

    private int _projectilesRemainingInMag;
    private int _shotsRemainingInBurst;
    private float _nextShotTime;

    private float _recoilAngle;
    private float _recoilRotationSmoothDampVelocity;
    private Vector3 _recoilSmoothDampVelocity;

    private bool _triggerReleasedSinceLastShot;
    private bool _isReloading;

    private MuzzleFlash _muzzleFlash;

    private void Awake()
    {
        _muzzleFlash = GetComponent<MuzzleFlash>();
    }

    private void Start()
    {
        _shotsRemainingInBurst = bursCount;
        _projectilesRemainingInMag = projectilesPerMag;
    }

    private void LateUpdate()
    {
        Recoil();

        if (!_isReloading && _projectilesRemainingInMag == 0)
            Reload();
    }

    private void Shoot()
    {
        if(Time.time > _nextShotTime && _projectilesRemainingInMag > 0 && !_isReloading)
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
                if (_projectilesRemainingInMag == 0)
                    break;

                _projectilesRemainingInMag--;
                _nextShotTime = Time.time + timeBetweenShots;
                Projectile newProjectile = Instantiate(projectile, projectileSpawn[i].position, projectileSpawn[i].rotation);
                newProjectile.SetSpeed(muzzleVelocity);
            }

            Instantiate(shell, shellEjection.position, shellEjection.rotation);
            _muzzleFlash.Activate();

            transform.localPosition -= Vector3.forward * Random.Range(kickMinMax.x, kickMinMax.y);
            _recoilAngle += Random.Range(recoilAngleMinMax.x, recoilAngleMinMax.y);
            _recoilAngle = Mathf.Clamp(_recoilAngle, 0, 30);
        }
    }

    public void Reload()
    {
        if(!_isReloading && _projectilesRemainingInMag != projectilesPerMag)
            StartCoroutine(AnimateReload());
    }

    private IEnumerator AnimateReload()
    {
        _isReloading = true;

        yield return new WaitForSeconds(.2f);

        float percent = 0;
        float reloadSpeed = 1 / reloadTime;
        float maxReloadAngle = 30;
        Vector3 initialRotation = transform.localEulerAngles;

        while(percent < 1)
        {
            percent += Time.deltaTime * reloadSpeed;
            float interpolation = (-Mathf.Pow(percent, 2) + percent) * 4;
            float reloadAngle = Mathf.Lerp(0, maxReloadAngle, interpolation);
            transform.localEulerAngles = initialRotation + Vector3.left * reloadAngle;

            yield return null;
        }

        _isReloading = false;
        _projectilesRemainingInMag = projectilesPerMag;
    }

    private void Recoil()
    {
        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, Vector3.zero, ref _recoilSmoothDampVelocity, recoilMoveSettleTime);
        _recoilAngle = Mathf.SmoothDamp(_recoilAngle, 0, ref _recoilRotationSmoothDampVelocity, recoilRotationSettleTime);
        transform.localEulerAngles = transform.localEulerAngles + Vector3.left * _recoilAngle;
    }

    public void Aim(Vector3 aimPoint)
    {
        if(!_isReloading)
            transform.LookAt(aimPoint);
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