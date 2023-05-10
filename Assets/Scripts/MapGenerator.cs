using UnityEngine;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] private int obstacleSeed = 10;
    [Range(0f, 1f)]
    [SerializeField] private float outlinePercent;
    [SerializeField] private Transform tilePrefab;
    [SerializeField] private Vector2 mapSize;
    [SerializeField] private Transform obstaclePrefab;

    private List<Coord> _allTileCoords;
    private Queue<Coord> _shuffledTileCoords;

    private void Start()
    {
        GenerateMap();
    }

    public void GenerateMap()
    {
        _allTileCoords = new List<Coord>();
        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                _allTileCoords.Add(new Coord(x, y));
            }
        }
        _shuffledTileCoords = new Queue<Coord>(Utility.ShuffleArray(_allTileCoords.ToArray(), obstacleSeed));

        string holderName = "Generated Map";
        if (transform.Find(holderName))
            DestroyImmediate(transform.Find(holderName).gameObject);

        var mapHolder = new GameObject(holderName).transform;
        mapHolder.parent = transform;

        for (int x = 0; x < mapSize.x; x++)
        {
            for(int y = 0; y < mapSize.y; y++)
            {
                var tilePosition = CoordToPosition(x, y);
                var newTile = Instantiate(tilePrefab, tilePosition, Quaternion.Euler(Vector3.right * 90), mapHolder);
                newTile.localScale = Vector3.one * (1 - outlinePercent);
            }
        }

        int obstacleCount = 10;
        for (int i = 0; i < obstacleCount; i++)
        {
            var randomCoord = GetRandomCoord();
            var obstaclePosition = CoordToPosition(randomCoord.x, randomCoord.y);
            var newObstacle = Instantiate(obstaclePrefab, obstaclePosition + Vector3.up * 0.5f, Quaternion.identity, mapHolder);
        }
    }

    private Vector3 CoordToPosition(int x, int y)
    {
        return new Vector3(-mapSize.x / 2 + 0.5f + x, 0, -mapSize.y / 2 + 0.5f + y);
    }

    public Coord GetRandomCoord()
    {
        var randomCoord = _shuffledTileCoords.Dequeue();
        _shuffledTileCoords.Enqueue(randomCoord);
        return randomCoord;
    } 

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
}