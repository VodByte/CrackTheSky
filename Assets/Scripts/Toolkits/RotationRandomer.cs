using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
public class RotationRandomer : EditorWindow
{
    Vector3 _randRange = new Vector3();

    [MenuItem("G.H.S/RotationRandomize")]
    public static void OpenWindow()
    {
        GetWindow(typeof(RotationRandomer));
    }

    private void OnGUI()
    {
        _randRange.x = EditorGUILayout.Slider("X Random Range", _randRange.x, 0.0f, 360.0f);
        _randRange.y = EditorGUILayout.Slider("Y Random Range", _randRange.y, 0.0f, 360.0f);
        _randRange.z = EditorGUILayout.Slider("Z Random Range", _randRange.z, 0.0f, 360.0f);

        if (GUILayout.Button("Randomize"))
        {
            foreach (var obj in Selection.gameObjects)
            {
                var newRot = Quaternion.Euler(
                    Vector3.Scale(Random.insideUnitSphere, _randRange));
                obj.transform.rotation = newRot;
            }
        }
    }
}
#endif