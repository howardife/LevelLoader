using UnityEngine;
using UnityEditor;
using System.Collections;

public class SetLevel :  EditorWindow {

    [MenuItem("Custom Editor/Level Editor/SetLevel")]
    static void Init() {

        SetLevel window = (SetLevel)EditorWindow.GetWindow(typeof(SetLevel));
        window.Show();
    }
}
