using System.Collections.Generic;
using UnityEngine;


public class LevelLoader : MonoBehaviour
{
    public GameObject targetMarkerPrefab;
    public HashSet<Vector2Int> wallPositions = new HashSet<Vector2Int>();
    public HashSet<Vector2Int> glitchWallPositions = new HashSet<Vector2Int>();
    public HashSet<Vector2Int> targetPositions = new HashSet<Vector2Int>();
    public Dictionary<Vector2Int, GameObject> boxPositions = new Dictionary<Vector2Int, GameObject>();

    public Transform levelParent;

    public GameManager gameManager;
    public TextAsset levelFile; // 텍스트 파일
    public GameObject wallPrefab;
    public GameObject glitchWallPrefab;
    public GameObject boxPrefab;
    public GameObject floorPrefab;
    public Transform player; 

    void Start()
    {
        LoadLevel();
    }

    public void LoadLevel()
    {
        //레벨 로드 하기 전에 리스트나 딕셔너리 다 비우기(다음 스테이지)
        wallPositions.Clear();
        glitchWallPositions.Clear();
        targetPositions.Clear();
        boxPositions.Clear();
        levelFile = gameManager.stages[gameManager.curStage];
        string[] rows = levelFile.text.Split('\n');

        for (int row = 0; row < rows.Length; row++)
        {
            for (int col = 0; col < rows[row].Length; col++)
            {
                char symbol = rows[row][col];
                int x = col;
                int y = rows.Length - 1 - row; 

                switch (symbol)
                {
                    case '#': // 못지나가는 벽
                        Instantiate(wallPrefab, new Vector3(x, y, 0), Quaternion.identity, levelParent);
                        wallPositions.Add(new Vector2Int(x, y));
                        break;

                    case 'G': // 글리치 벽
                        Instantiate(glitchWallPrefab, new Vector3(x,y,0), Quaternion.identity, levelParent);
                        glitchWallPositions.Add(new Vector2Int(x, y));
                        break;

                    case 'B': // 박스
                        Instantiate(floorPrefab, new Vector3(x, y, 0), Quaternion.identity, levelParent);
                        GameObject curBox = Instantiate(boxPrefab, new Vector3(x, y, 0), Quaternion.identity, levelParent);
                        boxPositions.Add(new Vector2Int(x, y), curBox);
                        break;

                    case 'P': // 플레이어 위치
                        Instantiate(floorPrefab, new Vector3(x, y, 0), Quaternion.identity, levelParent);
                        player.position = new Vector3(x, y, 0);
                        break;
                    case '.': // 그냥 바닥 타일
                        Instantiate(floorPrefab, new Vector3(x, y, 0), Quaternion.identity, levelParent);
                        break;

                    case 'X': // 골인지점
                        Instantiate(floorPrefab, new Vector3(x, y, 0), Quaternion.identity, levelParent);
                        Instantiate(targetMarkerPrefab, new Vector3(x, y, 0), Quaternion.identity, levelParent);
                        targetPositions.Add(new Vector2Int(x, y));
                        break;
                }
            }
        }
    }
}