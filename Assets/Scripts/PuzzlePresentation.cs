using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class PuzzlePresentation : MonoBehaviour
{
    public GameManager gameManager;
    public Camera gameCamera;
    public GameObject hud;
    public TMP_Text stageText;
    public TMP_Text boxesText;
    public TMP_Text soundText;

    private AudioSource source;
    private AudioClip stepSound, pushSound, goalSound, clearSound, clickSound;
    private bool muted;
    private Vector3 previousPosition;
    private int previousBoxes, previousBoxHash, mapInstance;
    private float previousAspect;
    private GameManager.GameState previousState;
    private Button[] buttons;

    private void Start()
    {
        source = gameObject.AddComponent<AudioSource>();
        source.playOnAwake = false;
        source.spatialBlend = 0f;
        source.volume = 0.22f;
        stepSound = MakeTone("Step", 150f, 0.045f);
        pushSound = MakeTone("Push", 95f, 0.09f);
        goalSound = MakeTone("Goal", 660f, 0.16f);
        clearSound = MakeTone("Clear", 440f, 0.42f, true);
        clickSound = MakeTone("Click", 380f, 0.055f);
        buttons = FindObjectsByType<Button>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (Button button in buttons) button.onClick.AddListener(PlayClick);
        previousState = gameManager.CurrentState;
        previousPosition = gameManager.levelLoader.player.position;
    }

    private void LateUpdate()
    {
        LevelLoader loader = gameManager.levelLoader;
        var state = gameManager.CurrentState;
        hud.SetActive(state != GameManager.GameState.Menu);

        int currentMap = loader.levelParent.childCount > 0
            ? loader.levelParent.GetChild(0).GetInstanceID() : 0;
        bool changedMap = currentMap != mapInstance;
        if ((changedMap || !Mathf.Approximately(previousAspect, gameCamera.aspect)) && loader.allTilePositions.Count > 0)
        {
            FrameMap(loader);
            mapInstance = currentMap;
            previousAspect = gameCamera.aspect;
        }

        if (loader.curStage >= 0 && loader.curStage < loader.stages.Length)
        {
            string name = loader.stages[loader.curStage].name;
            int stageNumber;
            if (name.StartsWith("1-") && int.TryParse(name.Substring(2), out stageNumber))
                stageText.text = $"CHAPTER {(stageNumber - 1) / 5 + 1:00}  /  STAGE {(stageNumber - 1) % 5 + 1:00}";
            else stageText.text = "WORKSHOP  /  " + name.ToUpperInvariant();
        }
        boxesText.text = $"BOXES LEFT   {loader.boxPositions.Count:00}";

        int boxHash = 0;
        foreach (var position in loader.boxPositions.Keys) boxHash ^= position.GetHashCode();
        if (state == GameManager.GameState.Cleared && previousState != state) Play(clearSound);
        else if (!changedMap && previousState == GameManager.GameState.Playing && state == previousState)
        {
            if (loader.boxPositions.Count < previousBoxes) Play(goalSound);
            else if (boxHash != previousBoxHash) Play(pushSound);
            else if (loader.player.position != previousPosition) Play(stepSound);
        }

        previousBoxes = loader.boxPositions.Count;
        previousBoxHash = boxHash;
        previousPosition = loader.player.position;
        previousState = state;
    }

    private void FrameMap(LevelLoader loader)
    {
        Vector2 min = new Vector2(float.MaxValue, float.MaxValue);
        Vector2 max = new Vector2(float.MinValue, float.MinValue);
        foreach (var position in loader.allTilePositions)
        {
            min = Vector2.Min(min, position);
            max = Vector2.Max(max, position);
        }
        Vector2 center = (min + max) * 0.5f;
        gameCamera.transform.position = new Vector3(center.x, center.y, gameCamera.transform.position.z);

        gameCamera.orthographicSize = Mathf.Max((max.y - min.y + 1f) / (2f * 0.70f),
            (max.x - min.x + 1f) / (2f * gameCamera.aspect * 0.82f));
    }

    public void ToggleSound()
    {
        muted = !muted;
        soundText.text = muted ? "SOUND OFF" : "SOUND ON";
    }

    public void QuitApplication() { Application.Quit(); }
    private void PlayClick() { Play(clickSound); }
    private void Play(AudioClip clip) { if (!muted && source != null) source.PlayOneShot(clip); }

    private AudioClip MakeTone(string name, float frequency, float duration, bool chord = false)
    {
        const int rate = 22050;
        float[] data = new float[Mathf.CeilToInt(rate * duration)];
        for (int i = 0; i < data.Length; i++)
        {
            float t = (float)i / rate;
            float attack = Mathf.Min(1f, t / 0.006f);
            float envelope = attack * Mathf.Pow(1f - t / duration, 2f);
            float wave = Mathf.Sin(2f * Mathf.PI * frequency * t);
            if (chord) wave = (wave + Mathf.Sin(2f * Mathf.PI * frequency * 1.25f * t)
                + Mathf.Sin(2f * Mathf.PI * frequency * 1.5f * t)) / 3f;
            data[i] = wave * envelope * 0.45f;
        }
        AudioClip clip = AudioClip.Create(name, data.Length, 1, rate, false);
        clip.SetData(data, 0);
        return clip;
    }

    private void OnDestroy()
    {
        if (buttons != null)
            foreach (Button button in buttons) if (button != null) button.onClick.RemoveListener(PlayClick);
        foreach (AudioClip clip in new[] { stepSound, pushSound, goalSound, clearSound, clickSound })
            if (clip != null) Destroy(clip);
    }
}
