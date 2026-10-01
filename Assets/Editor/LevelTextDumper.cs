using UnityEngine;
using UnityEditor;

public class LevelTextDumper
{
    [MenuItem("StockGame/Dump Level Texts")]
    public static void Dump()
    {
        string[] files = { "1-1", "1-2", "1-3", "1-4", "1-5", "1-6" };
        foreach (var f in files)
        {
            var asset = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/MapText/" + f + ".txt");
            if (asset == null) { continue; }
            Debug.Log("=== " + f + " ===\n" + asset.text);
        }
    }
}
