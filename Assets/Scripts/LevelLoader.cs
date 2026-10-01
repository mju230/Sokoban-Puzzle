using System.Collections.Generic;
using UnityEngine;


public class LevelLoader : MonoBehaviour
{
    public GameObject targetMarkerPrefab;
    public HashSet<Vector2Int> wallPositions = new HashSet<Vector2Int>();
    public HashSet<Vector2Int> targetPositions = new HashSet<Vector2Int>();
    public Dictionary<Vector2Int, GameObject> glitchWallPositions = new Dictionary<Vector2Int, GameObject>();
    public Dictionary<Vector2Int, GameObject> boxPositions = new Dictionary<Vector2Int, GameObject>();
    public Dictionary<Vector2Int, GameObject> overflowWallPositions = new Dictionary<Vector2Int, GameObject>();

    public Transform levelParent;

    public TextAsset[] stages;
    public GameObject wallPrefab;
    public GameObject glitchWallPrefab;
    public GameObject overflowWallPrefab;
    public GameObject crackedGlitchWallPrefab;
    public GameObject boxPrefab;
    public GameObject floorPrefab;

    public Transform player; 

    public int curStage = 0;

    void Start()
    {
        
    }

    public void LoadLevel()
    {
        //레벨 로드 하기 전에 필요한 부분들 다 지우기(다음 스테이지)
        CleanCurrentStage();
        string[] rows = stages[curStage].text.Split('\n');

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
                        GameObject glitchWall = Instantiate(glitchWallPrefab, new Vector3(x,y,0), Quaternion.identity, levelParent);
                        glitchWallPositions.Add(new Vector2Int(x, y), glitchWall);
                        break;

                    case 'O': // 오버플로우 벽
                        GameObject overflowWall = Instantiate(overflowWallPrefab, new Vector3(x, y, 0), Quaternion.identity, levelParent);
                        overflowWallPositions.Add(new Vector2Int(x,y), overflowWall);
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

    public void MoveBox(Vector2Int from, Vector2Int to)
    {
        GameObject theBox = boxPositions[from];
        boxPositions.Remove(from);
        boxPositions.Add(to, theBox);
        theBox.transform.position = new Vector3(to.x, to.y, 0);
    }

    public void RemoveBox(Vector2Int boxPos)
    {
        Destroy(boxPositions[boxPos]);
        boxPositions.Remove(boxPos);
    }


    // 한번 지나간 글리치 벽을 일반 벽으로 변환
    public void GWallToWall(Vector2Int curPos)
    {
        Destroy(glitchWallPositions[curPos]);
        glitchWallPositions.Remove(curPos);
        Instantiate(crackedGlitchWallPrefab, new Vector3(curPos.x,curPos.y, 0), Quaternion.identity,levelParent);
        wallPositions.Add(curPos);
    }

    public void CleanCurrentStage()
    {
        wallPositions.Clear();
        glitchWallPositions.Clear();
        targetPositions.Clear();
        boxPositions.Clear();
        overflowWallPositions.Clear();
        foreach (Transform child in levelParent)
        {
            Destroy(child.gameObject);
        }
    }

}