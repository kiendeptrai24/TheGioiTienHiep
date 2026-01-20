// Assets/_Game/WorldMap/Editor/MapBakerEditor.cs
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using WorldMap.Baking;
using WorldMap.Data;

namespace WorldMap.EditorTools
{
    public static class MapBakerEditor
    {
        [MenuItem("WorldMap/Bake Selected MapData")]
        public static void BakeSelected()
        {
            var map = Selection.activeObject as MapDataPreset;
            if (map == null)
            {
                Debug.LogWarning("Select a MapData asset first.");
                return;
            }

            var baker = new MapBaker();
            baker.BakeInto(map);
            Debug.Log("Baking...");
            EditorUtility.SetDirty(map);
            AssetDatabase.SaveAssets();
            Debug.Log("Bake done: " + map.name);
        }
    }
}
#endif
