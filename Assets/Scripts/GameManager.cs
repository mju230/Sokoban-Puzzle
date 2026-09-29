using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public LevelLoader levelLoader;
    

    void Awake()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            RestartStage();
        } 
    }



    public bool CheckIsGoal(Vector2Int boxPos)
    {
        
        if (levelLoader.targetPositions.Contains(boxPos))
        {
            return true;
        }
        return false;
            
    }

    public void StageClear()
    {
        //TODO 스테이지 클리어 시 다음 스테이지 or 로비 or 다시 플레이 버튼 UI 띄우도록
        Debug.Log("스테이지 클리어 다음스테이지 구성");
        levelLoader.curStage += 1;
        levelLoader.LoadLevel();
    }

    private void RestartStage()
    {
        levelLoader.LoadLevel();
    }

}
