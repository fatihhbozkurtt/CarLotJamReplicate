using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Grid)), CanEditMultipleObjects]
public class GridManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Generate Grid"))
        {
            Grid grid = target as Grid;

            if (grid != null)
            {
                grid.GenerateGrid();
                EditorUtility.SetDirty(grid);
            }
            else
            {
                Debug.LogError("Target is not a Grid component.");
            }
        }
    }
}


