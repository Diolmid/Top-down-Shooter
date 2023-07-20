using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] private int mapIndex;
    [SerializeField] private float tileSize;
    [SerializeField, Range(0, 1)] private float outlinePercent;
    [SerializeField] private Vector2 maxMapSize;
    [SerializeField] private Transform tilePrefab;
    [SerializeField] private Transform obstaclePrefab;
    [SerializeField] private Transform mapFloor;
    [SerializeField] private Transform navmeshFloor;
    [SerializeField] private Transform navmeshMaskPrefab;
    [SerializeField] private Map[] maps;

    private List<Coord> _allTileCoords;
    private Queue<Coord> _shuffledTileCoords;
    private Queue<Coord> _shuffledOpenTileCoords;
    private Transform[,] _tileMap;

    private Map _currentMap;

    private void Start()
    {
        FindObjectOfType<Spawner>().OnNewWave += OnNewWave;
    }

    private void OnNewWave(int waveNumber)
    {
        mapIndex = waveNumber - 1;
        GenerateMap();
    }

    public void GenerateMap()
    {
        _currentMap = maps[mapIndex];
        _tileMap = new Transform[_currentMap.mapSize.x, _currentMap.mapSize.y];
        System.Random prng = new System.Random(_currentMap.obstacleSeed);

        // Generating Coords
        _allTileCoords = new List<Coord>();
        for (int x = 0; x < _currentMap.mapSize.x; x++)
        {
            for (int y = 0; y < _currentMap.mapSize.y; y++)
            {
                _allTileCoords.Add(new Coord(x, y));
            }
        }
        _shuffledTileCoords = new Queue<Coord>(Utility.ShuffleArray(_allTileCoords.ToArray(), _currentMap.obstacleSeed));

        // Create map holder object
        string holderName = "Generated Map";
        if (transform.Find(holderName))
            DestroyImmediate(transform.Find(holderName).gameObject);

        var mapHolder = new GameObject(holderName).transform;
        mapHolder.parent = transform;

        // Spawning tiles
        for (int x = 0; x < _currentMap.mapSize.x; x++)
        {
            for(int y = 0; y < _currentMap.mapSize.y; y++)
            {
                var tilePosition = CoordToPosition(x, y);
                var newTile = Instantiate(tilePrefab, tilePosition, Quaternion.Euler(Vector3.right * 90), mapHolder);
                newTile.localScale = Vector3.one * (1 - outlinePercent) * tileSize;
                _tileMap[x, y] = newTile;
            }
        }

        //Spawning obstacles
        int obstacleCount = (int)(_currentMap.mapSize.x * _currentMap.mapSize.y * _currentMap.obstaclePercent);
        int currentObstacleCount = 0;
        bool[,] obstacleMap = new bool[(int)_currentMap.mapSize.x, (int)_currentMap.mapSize.y];
        List<Coord> allOpenCoords = new List<Coord>(_allTileCoords);

        for (int i = 0; i < obstacleCount; i++)
        {
            var randomCoord = GetRandomCoord();
            obstacleMap[randomCoord.x, randomCoord.y] = true;
            currentObstacleCount++;

            if(!randomCoord.Equals(_currentMap.mapCenter) && MapIsFullyAccessible(obstacleMap, currentObstacleCount))
            {
                float obstacleHeight = Mathf.Lerp(_currentMap.minObstacleHight, _currentMap.maxObstacleHight, (float)prng.NextDouble());
                var obstaclePosition = CoordToPosition(randomCoord.x, randomCoord.y);

                var newObstacle = Instantiate(obstaclePrefab, obstaclePosition + Vector3.up * obstacleHeight / 2, Quaternion.identity, mapHolder);
                newObstacle.localScale = new Vector3((1 - outlinePercent) * tileSize, obstacleHeight, (1 - outlinePercent) * tileSize);

                var obstacleRenderer = newObstacle.GetComponent<Renderer>();
                var obstacleMaterial = new Material(obstacleRenderer.sharedMaterial);
                float colourPercent = randomCoord.y / (float)_currentMap.mapSize.y;
                obstacleMaterial.color = Color.Lerp(_currentMap.foregroundColour, _currentMap.backgroundColour, colourPercent);
                obstacleRenderer.sharedMaterial = obstacleMaterial;

                allOpenCoords.Remove(randomCoord);
            }
            else
            {
                obstacleMap[randomCoord.x, randomCoord.y] = false;
                currentObstacleCount--;
            }
        }

        _shuffledOpenTileCoords = new Queue<Coord>(Utility.ShuffleArray(allOpenCoords.ToArray(), _currentMap.obstacleSeed));

        // Creating navmesh mask
        var maskLeft = Instantiate(navmeshMaskPrefab, Vector3.left * (_currentMap.mapSize.x + maxMapSize.x) / 4f * tileSize, Quaternion.identity, mapHolder);
        maskLeft.localScale = new Vector3((maxMapSize.x - _currentMap.mapSize.x) / 2f, 1, _currentMap.mapSize.y) * tileSize;
        var maskRight = Instantiate(navmeshMaskPrefab, Vector3.right * (_currentMap.mapSize.x + maxMapSize.x) / 4f * tileSize, Quaternion.identity, mapHolder);
        maskRight.localScale = new Vector3((maxMapSize.x - _currentMap.mapSize.x) / 2f, 1, _currentMap.mapSize.y) * tileSize;

        var maskTop = Instantiate(navmeshMaskPrefab, Vector3.forward * (_currentMap.mapSize.y + maxMapSize.y) / 4f * tileSize, Quaternion.identity, mapHolder);
        maskTop.localScale = new Vector3(maxMapSize.x, 1, (maxMapSize.y - _currentMap.mapSize.y) / 2f) * tileSize;
        var maskButtom = Instantiate(navmeshMaskPrefab, Vector3.back * (_currentMap.mapSize.y + maxMapSize.y) / 4f * tileSize, Quaternion.identity, mapHolder);
        maskButtom.localScale = new Vector3(maxMapSize.x, 1, (maxMapSize.y - _currentMap.mapSize.y) / 2f) * tileSize;
        
        navmeshFloor.localScale = new Vector3(maxMapSize.x, maxMapSize.y) * tileSize;
        mapFloor.localScale = new Vector3(_currentMap.mapSize.x * tileSize, _currentMap.mapSize.y * tileSize);
    }

    private bool MapIsFullyAccessible(bool[,] obstacleMap, int currentObstacleCount)
    {
        var mapFlags = new bool[obstacleMap.GetLength(0), obstacleMap.GetLength(1)];
        var queue = new Queue<Coord>();

        queue.Enqueue(_currentMap.mapCenter);
        mapFlags[_currentMap.mapCenter.x, _currentMap.mapCenter.y] = true;

        int accessibleTileCount = 1;
        while(queue.Count > 0)
        {
            var tile = queue.Dequeue();

            for(var x = -1; x <= 1; x ++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    int neighbourX = tile.x + x;
                    int neighbourY = tile.y + y;

                    if(x == 0 ^ y == 0)
                    {
                        if(neighbourX >= 0 && neighbourX < obstacleMap.GetLength(0) && neighbourY >= 0 && neighbourY < obstacleMap.GetLength(1))
                        {
                            if (!mapFlags[neighbourX, neighbourY] && !obstacleMap[neighbourX, neighbourY])
                            {
                                mapFlags[neighbourX, neighbourY] = true;
                                queue.Enqueue(new Coord(neighbourX, neighbourY));
                                accessibleTileCount++;
                            }
                        }
                    }
                }
            } 
        }

        int targetAccessibleTileCount = (int)(_currentMap.mapSize.x * _currentMap.mapSize.y - currentObstacleCount);
        return targetAccessibleTileCount == accessibleTileCount;
    }

    private Vector3 CoordToPosition(int x, int y)
    {
        return new Vector3(-_currentMap.mapSize.x / 2f + 0.5f + x, 0, -_currentMap.mapSize.y / 2f + 0.5f + y) * tileSize;
    }

    public Coord GetRandomCoord()
    {
        var randomCoord = _shuffledTileCoords.Dequeue();
        _shuffledTileCoords.Enqueue(randomCoord);
        return randomCoord;
    }

    public Transform GetRandomOpenTile()
    {
        var randomCoord = _shuffledOpenTileCoords.Dequeue();
        _shuffledOpenTileCoords.Enqueue(randomCoord);
        return _tileMap[randomCoord.x, randomCoord.y];
    }

    public Transform GetTileFromPosition(Vector3 position)
    {
        int x = Mathf.RoundToInt(position.x / tileSize + (_currentMap.mapSize.x - 1) / 2f);
        int y = Mathf.RoundToInt(position.z / tileSize + (_currentMap.mapSize.y - 1) / 2f);

        x = Mathf.Clamp(x, 0, _tileMap.GetLength(0) - 1);
        y = Mathf.Clamp(y, 0, _tileMap.GetLength(1) - 1);

        return _tileMap[x, y];
    }

    [System.Serializable]
    public struct Coord
    {
        public int x;
        public int y;

        public Coord(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }

    [System.Serializable]
    public class Map
    {
        public Coord mapSize;
        [Range(0, 1)] public float obstaclePercent;
        public int obstacleSeed;
        public float minObstacleHight;
        public float maxObstacleHight;
        public Color foregroundColour;
        public Color backgroundColour;

        public Coord mapCenter
        {
            get
            {
                return new Coord(mapSize.x / 2, mapSize.y / 2);
            }
        }
    }
}