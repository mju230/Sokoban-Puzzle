using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public LevelLoader levelLoader;
    public GameManager gameManager;
    
    // Update is called once per frame
    void Update()
    {
        if (gameManager.CurrentState != GameManager.GameState.Playing) return;
        Vector2Int moveDir = Vector2Int.zero;

        if(Keyboard.current.wKey.wasPressedThisFrame || Keyboard.current.upArrowKey.wasPressedThisFrame) moveDir = Vector2Int.up;
        else if(Keyboard.current.sKey.wasPressedThisFrame || Keyboard.current.downArrowKey.wasPressedThisFrame) moveDir = Vector2Int.down;
        else if (Keyboard.current.aKey.wasPressedThisFrame || Keyboard.current.leftArrowKey.wasPressedThisFrame) moveDir = Vector2Int.left;
        else if (Keyboard.current.dKey.wasPressedThisFrame || Keyboard.current.rightArrowKey.wasPressedThisFrame) moveDir = Vector2Int.right;
        

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
        
        if (levelLoader.wallPositions.Contains(nextPlayerPos) || levelLoader.overflowWallPositions.ContainsKey(nextPlayerPos)){
            Debug.Log("벽 또는 오버플로벽에 막혀서 못갑니다");
            return;
        }
        else if(levelLoader.boxPositions.ContainsKey(nextPlayerPos))
        {

            Vector2Int currentBoxPos = nextPlayerPos;
            Vector2Int nextBoxPos = currentBoxPos + dir;
            

            if(levelLoader.overflowWallPositions.ContainsKey(nextBoxPos)){

                Vector2Int landingPos = nextBoxPos; 
                while (levelLoader.overflowWallPositions.ContainsKey(landingPos)) // 연속으로 BOO. 있을 때 대비
                {
                    landingPos += dir;
                }

                if(levelLoader.boxPositions.ContainsKey(landingPos)||
                    levelLoader.wallPositions.Contains(landingPos)||levelLoader.glitchWallPositions.ContainsKey(landingPos)){
                    Debug.Log("오버플로우 월 뒤에 박스 뒤에 벽 또는 다른 박스 또는 글리치 벽이 있어서 못갑니다");
                    return;
                }
                else
                {
                    levelLoader.MoveBox(currentBoxPos, landingPos);
                    Debug.Log($"지워진 상자 위치 {currentBoxPos} 새로운 상자 위치 {landingPos}");
                    gameManager.CheckBoxArrival(landingPos);
                }

            }
            else if (levelLoader.wallPositions.Contains(nextBoxPos) || levelLoader.boxPositions.ContainsKey(nextBoxPos)|| 
                        levelLoader.glitchWallPositions.ContainsKey(nextBoxPos))
            {
                Debug.Log("박스 뒤에 벽 또는 다른 박스 또는 글리치 벽이 있어서 못갑니다");
                return;
            }
            else
            {
                levelLoader.MoveBox(currentBoxPos, nextBoxPos);
                Debug.Log($"지워진 상자 위치 {currentBoxPos} 새로운 상자 위치 {nextBoxPos}");
                gameManager.CheckBoxArrival(nextBoxPos);
                
            }
        }
        
        transform.position = new Vector3 (nextPlayerPos.x, nextPlayerPos.y, 0);
        if (levelLoader.glitchWallPositions.ContainsKey(currentPosition))
        {
            levelLoader.GWallToWall(currentPosition);
        }

        gameManager.CheckStageClear();
    }

    // vector3 에서 vector2int로 교환하는 함수
    Vector2Int Vec3ToVec2Int(Vector3 pos)
    {
        return new Vector2Int(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y));
    }


}
