using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public LevelLoader levelLoader;
    public GameObject mainPanel;
    public GameObject selectChapterPanel;
    public GameObject chapterOnePanel;
    public GameObject chapterTwoPanel;
    public GameObject chapterThreePanel;
    public GameObject nextStagePanel;

    public enum GameState
    {
        Menu,
        Playing,
        Cleared
    }

    public GameState CurrentState{ get; private set;} = GameState.Menu;

    void Awake()
    {
        mainPanel.SetActive(true);
        selectChapterPanel.SetActive(false);
        nextStagePanel.SetActive(false);
        chapterOnePanel.SetActive(false);
        chapterTwoPanel.SetActive(false);
        chapterThreePanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (CurrentState != GameState.Playing) return;
        
        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            RestartStage();
        }
        else if (Keyboard.current.tKey.wasPressedThisFrame)
        {
            RestartGame();
        }
        else if (Keyboard.current.qKey.wasPressedThisFrame)
        {
            BeforeStage();
        }
        else if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            NextStage();
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

    

    private void RestartStage()
    {
        levelLoader.LoadLevel();
    }

    public void CheckBoxArrival(Vector2Int boxPos)
    {
        if (!CheckIsGoal(boxPos)) return;
        
        Debug.Log("박스 골인지점 도착, 박스 삭제");
        levelLoader.RemoveBox(boxPos);
        
    }

    public void StageClear()
    {

        CurrentState = GameState.Cleared;
        nextStagePanel.SetActive(true);
        Debug.Log("스테이지 클리어 다음스테이지 or 로비로");
    }

    public void CheckStageClear()
    {
        if (CurrentState != GameState.Playing) return;
        if (levelLoader.boxPositions.Count == 0)
        {
            StageClear();
        }
    }

    #region 버튼 모음
    public void OnSelectChapterButtonClick()
    {
        selectChapterPanel.SetActive(true);
    }

    public void OnChapterOneButtonClick()
    {
        chapterOnePanel.SetActive(true);
    }

    public void OnStageButtonClick(int stageIndex)
    {
        levelLoader.curStage = stageIndex;
        selectChapterPanel.SetActive(false);
        chapterOnePanel.SetActive(false);
        chapterTwoPanel.SetActive(false);
        chapterThreePanel.SetActive(false);
        mainPanel.SetActive(false);
        levelLoader.LoadLevel();
        CurrentState = GameState.Playing;
    }

    public void OnChapterTwoButtonClick()
    {
        chapterTwoPanel.SetActive(true);
    }

    public void OnChapterThreeButtonClick()
    {
        chapterThreePanel.SetActive(true);
    }

    public void OnNextStageButtonClick()
    {
        levelLoader.curStage += 1;
        nextStagePanel.SetActive(false);
        levelLoader.LoadLevel();
        CurrentState = GameState.Playing;
    }

    public void OnMainLobbyButtonClick()
    {
        CurrentState = GameState.Menu;
        levelLoader.CleanCurrentStage();
        nextStagePanel.SetActive(false);
        mainPanel.SetActive(true);
    }

    #endregion

    #region 게임 테스트용 스테이지 컨트롤
    private void NextStage()
    {
        levelLoader.curStage += 1;
        levelLoader.LoadLevel();
    }

    private void RestartGame()
    {
        levelLoader.curStage = 0;
        levelLoader.LoadLevel();
    }

    private void BeforeStage()
    {
        if(levelLoader.curStage == 0) return;
        levelLoader.curStage -= 1;
        levelLoader.LoadLevel();
    }

    #endregion
}
