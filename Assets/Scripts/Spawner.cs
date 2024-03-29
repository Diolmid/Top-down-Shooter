using System.Collections;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public event System.Action<int> OnNewWave;

    public Wave[] waves;
    [SerializeField] private Enemy enemy;

    private int _enemiesRemainingToSpawn;
    private int _enemiesRemainingAlive;
    private float _nextSpawnTime;

    private bool _isDisabled;

    private int _currentWaveNumber;
    private Wave _currentWave;

    private float _campThresholdDistance = 1.5f;
    private float _timeBetweenCampingChecks = 2;
    private float _nextCapmCheckTime;
    private bool _isCamping;
    private Vector3 _campPositionOld;
    private LivingEntity _playerEntity;
    private Transform _playerTransform;

    private MapGenerator _mapGenerator;

    private void Awake()
    {
        _mapGenerator = FindObjectOfType<MapGenerator>();
        _playerEntity = FindObjectOfType<Player>();
        _playerTransform = _playerEntity.transform;
    }

    private void Start()
    {
        _nextCapmCheckTime = _timeBetweenCampingChecks + Time.time;
        _campPositionOld = _playerTransform.position;
        _playerEntity.OnDeath += OnPlayerDeath;

        NextWave();
    }


    private void Update()
    {
        if (_isDisabled)
            return;

        if(Time.time > _nextCapmCheckTime)
        {
            _nextCapmCheckTime = Time.time + _timeBetweenCampingChecks;

            _isCamping = (Vector3.Distance(_playerTransform.position, _campPositionOld) < _campThresholdDistance);
            _campPositionOld = _playerTransform.position;
        }

        if((_enemiesRemainingToSpawn > 0 || _currentWave.infinite) && Time.time > _nextSpawnTime)
        {
            _enemiesRemainingToSpawn--;
            _nextSpawnTime = Time.time + _currentWave.timeBetweenSpawns;

            StartCoroutine(SpawnEnemy());
        }
    }

    private IEnumerator SpawnEnemy()
    {
        float spawnTimer = 0;
        float spawnDelay = 1;
        float tileFlashSpeed = 4;

        var spawnTile =  _mapGenerator.GetRandomOpenTile();
        if (_isCamping)
            spawnTile = _mapGenerator.GetTileFromPosition(_playerTransform.position);

        var tileMat = spawnTile.GetComponent<Renderer>().material;
        var initialClour = Color.white;
        var flashColour = Color.red;

        while(spawnTimer < spawnDelay)
        {
            tileMat.color = Color.Lerp(initialClour, flashColour, Mathf.PingPong(spawnTimer * tileFlashSpeed, 1));

            spawnTimer += Time.deltaTime;
            yield return null;
        }

        var spawnedEnemy = Instantiate(enemy, spawnTile.position + Vector3.up, Quaternion.identity);
        spawnedEnemy.OnDeath += OnEnemyDeath;
        spawnedEnemy.SetCharacteristics(_currentWave.moveSpeed, _currentWave.hitsToKillPlayer, _currentWave.enemyHealth);
    }

    private void OnEnemyDeath()
    {
        _enemiesRemainingAlive--;

        if(_enemiesRemainingAlive == 0 )
            NextWave();
    }
    
    private void OnPlayerDeath()
    {
        _isDisabled = true;
    }

    private void ResetPlayerPosition()
    {
        _playerTransform.position = _mapGenerator.GetTileFromPosition(Vector3.zero).position + Vector3.up * 3;
    }

    private void NextWave()
    {
        if(_currentWaveNumber > 0)
            AudioManager.instance.PlaySound2D("Level Complete");

        _currentWaveNumber++;
        if (_currentWaveNumber - 1 < waves.Length)
        {
            _currentWave = waves[_currentWaveNumber - 1];

            _enemiesRemainingToSpawn = _currentWave.enemyCount;
            _enemiesRemainingAlive = _enemiesRemainingToSpawn;

            if (OnNewWave != null)
                OnNewWave(_currentWaveNumber);

            ResetPlayerPosition();
        }
    }

        [System.Serializable]
    public class Wave
    {

        public int enemyCount;
        public float timeBetweenSpawns;

        public float moveSpeed;
        public int hitsToKillPlayer;
        public float enemyHealth;

        public bool infinite;
    }
}