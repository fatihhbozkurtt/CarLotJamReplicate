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

            grid.GenerateGrid();

            EditorUtility.SetDirty(grid);
        }
    }
}

