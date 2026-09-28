using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public LevelLoader levelLoader;
    public GameManager gameManager;
    // Update is called once per frame
    void Update()
    {
        Vector2Int moveDir = Vector2Int.zero;

        if(Keyboard.current.wKey.wasPressedThisFrame) moveDir = Vector2Int.up;
        else if(Keyboard.current.sKey.wasPressedThisFrame) moveDir = Vector2Int.down;
        else if (Keyboard.current.aKey.wasPressedThisFrame) moveDir = Vector2Int.left;
        else if (Keyboard.current.dKey.wasPressedThisFrame) moveDir = Vector2Int.right;

        if(moveDir != Vector2Int.zero)
        {
            MovePlayer(moveDir);
        }
    }

    void MovePlayer(Vector2Int dir)
    {
        Vector2Int currentPosition = Vec3ToVec2Int(transform.position);
        
        Vector2Int nextPlayerPos = currentPosition + dir;
        Debug.Log($"다음 플레이어 위치 {nextPlayerPos}");
        if (levelLoader.wallPositions.Contains(nextPlayerPos)){
            Debug.Log("벽에 막혀서 못갑니다");
            return;
        }
        else if(levelLoader.boxPositions.ContainsKey(nextPlayerPos))
        {

            Vector2Int currentBoxPos = nextPlayerPos;
            Vector2Int nextBoxPos = currentBoxPos + dir;
            if (levelLoader.wallPositions.Contains(nextBoxPos) || levelLoader.boxPositions.ContainsKey(nextBoxPos))
            {
                Debug.Log("박스 뒤에 벽 또는 다른 박스가 있어서 못갑니다");
                return;
            }
            else
            {
                levelLoader.MoveBox(currentBoxPos, nextBoxPos);
                Debug.Log($"지워진 상자 위치 {currentBoxPos} 새로운 상자 위치 {nextBoxPos}");
                if (gameManager.CheckIsGoal(nextBoxPos))
                {
                    Debug.Log("박스 골인지점 도착, 박스 삭제");
                    levelLoader.RemoveBox(nextBoxPos);
                    if(levelLoader.boxPositions.Count == 0) {
                        gameManager.StageClear();
                        return;
                    }
                }
                
            }
        }

        transform.position = new Vector3 (nextPlayerPos.x, nextPlayerPos.y, 0);
    }

    // vector3 에서 vector2int로 교환하는 함수
    Vector2Int Vec3ToVec2Int(Vector3 pos)
    {
        return new Vector2Int(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y));
    }


}
