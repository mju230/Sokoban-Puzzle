using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public LevelLoader levelLoader;
    public GameObject mainPanel;
    public GameObject selectChapterPanel;
    public GameObject chapterOnePanel;
    public GameObject chapterTwoPanel;
    public GameObject chapterThreePanel;
    public GameObject nextStagePanel;
    public GameObject toMainLobbyPanel;
    public GameObject inGameBackButton;
    public Button[] stageButtons;
    private const string UnlockedStageKey = "GlitchWorkshop.HighestUnlockedStage";
    public int HighestUnlockedStage { get; private set; }
    public enum GameState
    {
        Menu,
        Playing,
        Cleared
    }

    public GameState CurrentState{ get; private set;} = GameState.Menu;

    void Awake()
    {
        HighestUnlockedStage = Mathf.Clamp(PlayerPrefs.GetInt(UnlockedStageKey, 0),
            0, Mathf.Max(0, levelLoader.stages.Length - 1));
        RefreshStageButtons();
        inGameBackButton.SetActive(false);
        mainPanel.SetActive(true);
        selectChapterPanel.SetActive(false);
        nextStagePanel.SetActive(false);
        toMainLobbyPanel.SetActive(false);
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
        if (CurrentState != GameState.Playing) return;
        int nextIndex = Mathf.Min(levelLoader.curStage + 1, levelLoader.stages.Length - 1);
        if (nextIndex > HighestUnlockedStage)
        {
            HighestUnlockedStage = nextIndex;
            PlayerPrefs.SetInt(UnlockedStageKey, HighestUnlockedStage);
            PlayerPrefs.Save();
            RefreshStageButtons();
        }
        inGameBackButton.SetActive(false);
        CurrentState = GameState.Cleared;
        bool isLastStage = levelLoader.curStage >= levelLoader.stages.Length - 1;
        nextStagePanel.SetActive(!isLastStage);
        toMainLobbyPanel.SetActive(isLastStage);
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
        if (stageIndex < 0 || stageIndex >= levelLoader.stages.Length ||
            stageIndex > HighestUnlockedStage) return;
        levelLoader.curStage = stageIndex;

        if (!levelLoader.LoadLevel())
        {
            return;
        }
        
        selectChapterPanel.SetActive(false);
        chapterOnePanel.SetActive(false);
        chapterTwoPanel.SetActive(false);
        chapterThreePanel.SetActive(false);
        mainPanel.SetActive(false);
        nextStagePanel.SetActive(false);
        toMainLobbyPanel.SetActive(false);

        CurrentState = GameState.Playing;
        inGameBackButton.SetActive(true);
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
        if (CurrentState != GameState.Cleared) return;
        OnStageButtonClick(levelLoader.curStage + 1);
    }

    public void OnMainLobbyButtonClick()
    {
        CurrentState = GameState.Menu;
        inGameBackButton.SetActive(false);
        levelLoader.CleanCurrentStage();
        nextStagePanel.SetActive(false);
        toMainLobbyPanel.SetActive(false);
        mainPanel.SetActive(true);
    }

    #endregion

    private void RefreshStageButtons()
    {
        for (int i = 0; i < stageButtons.Length; i++)
        {
            bool unlocked = i <= HighestUnlockedStage && i < levelLoader.stages.Length;
            stageButtons[i].interactable = unlocked;
            var label = stageButtons[i].GetComponentInChildren<TMPro.TMP_Text>(true);
            if (label != null)
                label.text = $"{i / 5 + 1} - {i % 5 + 1}" + (unlocked ? "" : "\n<size=14>LOCKED</size>");
        }
    }

    private void RestartGame()
    {
        levelLoader.curStage = 0;
        levelLoader.LoadLevel();
    }

}
