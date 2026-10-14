using System.Collections.Generic;
using UnityEngine;

public class LevelLoader : MonoBehaviour
{
    public GameObject targetMarkerPrefab;
    public HashSet<Vector2Int> wallPositions = new HashSet<Vector2Int>();
    public HashSet<Vector2Int> targetPositions = new HashSet<Vector2Int>();
    public HashSet<Vector2Int> overflowWallPositions = new HashSet<Vector2Int>();
    public HashSet<Vector2Int> allTilePositions = new HashSet<Vector2Int>();

    // 텔레포트용
    public Dictionary<int, Vector2Int> pendingEntrance = new Dictionary<int, Vector2Int>();
    public Dictionary<int, Vector2Int> pendingExit = new Dictionary<int, Vector2Int>();
    public Dictionary<Vector2Int,Vector2Int> teleportPositions = new Dictionary<Vector2Int, Vector2Int>();
    public HashSet<Vector2Int> portalExitPositions = new HashSet<Vector2Int>();
    
    public Dictionary<Vector2Int, GameObject> glitchWallPositions = new Dictionary<Vector2Int, GameObject>();
    public Dictionary<Vector2Int, GameObject> boxPositions = new Dictionary<Vector2Int, GameObject>();
    

    public Transform levelParent;

    public TextAsset[] stages;
    public GameObject wallPrefab;
    public GameObject glitchWallPrefab;
    public GameObject overflowWallPrefab;
    public GameObject crackedGlitchWallPrefab;
    public GameObject teleportEntPrefab;
    public GameObject teleportExitPrefab;
    public GameObject boxPrefab;
    public GameObject floorPrefab;

    public Transform player; 

    public int curStage = 0;


    public bool LoadLevel()
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

                bool isMapTile = symbol == '#' || symbol == 'G' || symbol == 'O' ||
                                symbol == 'B' || symbol == 'P' || symbol == '.' ||
                                symbol == 'X' ||
                                (symbol >= '1' && symbol <= '9') ||
                                (symbol >= 'a' && symbol <= 'i');

                if(isMapTile) allTilePositions.Add(new Vector2Int(x, y));

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
                        Instantiate(overflowWallPrefab, new Vector3(x, y, 0), Quaternion.identity, levelParent);
                        overflowWallPositions.Add(new Vector2Int(x,y));
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

                    // 텔레포터
                    default:
                        if(symbol >= '1' && symbol <= '9') CreateEntPortal(x, y, symbol);
                        else if(symbol >= 'a' && symbol <= 'i') CreateExitPortal(x, y, symbol);
                        break;

                }
            }
        }

        if(!CheckPortalPairs()) {
            Debug.LogError("포탈 입구 출구 짝이 맞지않습니다.");
            return false;
            }

        return true;
    }

    private void CreateEntPortal(int x, int y, char symbol)
    {
        if(pendingEntrance.ContainsKey(symbol - '1'))
        {
            Debug.LogError($"입구 {symbol} 중복됨");
            return;
        }
        Vector2Int pos = new Vector2Int(x, y);
        Instantiate(floorPrefab, new Vector3(x, y, 0), Quaternion.identity, levelParent);
        GameObject portal = Instantiate(teleportEntPrefab, new Vector3(x, y, 0), Quaternion.identity, levelParent);
        portal.GetComponent<PortalVisual>().Initialize(symbol - '1');
        pendingEntrance[symbol - '1'] = pos;
    }
    private void CreateExitPortal(int x, int y, char symbol)
    {
        if (pendingExit.ContainsKey(symbol - 'a'))
        {
            Debug.LogError($"출구 {symbol} 중복됨");
            return;
        }
        Vector2Int pos = new Vector2Int(x, y);
        Instantiate(floorPrefab, new Vector3(x, y, 0), Quaternion.identity, levelParent);
        GameObject portal = Instantiate(teleportExitPrefab, new Vector3(x, y, 0), Quaternion.identity, levelParent);
        portal.GetComponent<PortalVisual>().Initialize(symbol - 'a');
        pendingExit[symbol - 'a'] = pos;
        portalExitPositions.Add(pos);
    }

    private bool CheckPortalPairs()
    {
        bool isValid = true;
        foreach (var ent in pendingEntrance)
        {
            int num = ent.Key;
            Vector2Int entPos = ent.Value;
            if (pendingExit.ContainsKey(num))
            {
                teleportPositions[entPos] = pendingExit[num];
            }
            else
            {
                Debug.LogError($"{(char)('1'+num)}입구에 맞는 출구가 없습니다.");
                isValid = false;
            }
        }

        foreach(var exit in pendingExit)
        {
            if (!pendingEntrance.ContainsKey(exit.Key))
            {
                Debug.LogError($"출구 {(char)('a' + exit.Key)} 에 맞는 입구가 없습니다.");
                isValid = false;
            }
        }

        return isValid;
    }

    public bool CanBoxLand(Vector2Int pos)
    {
        if (!allTilePositions.Contains(pos)) return false;

        if (wallPositions.Contains(pos)) return false;
        if (glitchWallPositions.ContainsKey(pos)) return false;
        if (overflowWallPositions.Contains(pos)) return false;
        if (boxPositions.ContainsKey(pos)) return false;

        if (pendingEntrance.ContainsValue(pos)) return false;
        if (portalExitPositions.Contains(pos)) return false;

        return true;

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
        allTilePositions.Clear();
        wallPositions.Clear();
        glitchWallPositions.Clear();
        targetPositions.Clear();
        boxPositions.Clear();
        overflowWallPositions.Clear();
        pendingEntrance.Clear();
        pendingExit.Clear();
        teleportPositions.Clear();
        portalExitPositions.Clear();
        foreach (Transform child in levelParent)
        {
            Destroy(child.gameObject);
        }
    }

}
