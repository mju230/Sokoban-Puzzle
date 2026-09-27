using UnityEngine;

public class GameManager : MonoBehaviour
{
    public LevelLoader leveloader;
    
    public TextAsset[] stages;
    public int curStage = 0;

    void Awake()
    {
        curStage = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    public bool CheckIsGoal(Vector2Int BoxPos)
    {
        
        if (leveloader.targetPositions.Contains(BoxPos))
        {
            return true;
        }
        return false;
            
    }

    public void StageClear()
    {
        //TODO 스테이지 클리어 시 다음 스테이지 or 로비 or 다시 플레이 버튼 UI 띄우도록
        Debug.Log("스테이지 클리어 다음스테이지 구성");
        foreach(Transform child in leveloader.levelParent)
        {
            Destroy(child.gameObject);
        }
        curStage += 1;
        leveloader.LoadLevel();
    }

}
