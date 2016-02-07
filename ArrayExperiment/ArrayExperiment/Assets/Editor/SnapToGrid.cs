using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[System.Serializable]
public class CubeData
{
    private bool taken;
    private float x;
    private float y;
    private float z;
    private float ScaleX;
    private float ScaleY;
    private float ScaleZ;

    public float ReturnX { get { return x; } }
    public float ReturnY { get { return y; } }
    public float ReturnZ { get { return z; } }
    public Vector3 ReturnScale { get { return new Vector3(ScaleX, ScaleY, ScaleZ); } }
    public bool ReturnIsTaken { get { return taken; } }
    

    public CubeData(float _x, float _y, float _z, Vector3 _scale, bool _taken)
    {
        x = _x;
        y = _y;
        z = _z;
        taken = _taken;
        ScaleX = _scale.x;
        ScaleY = _scale.y;
        ScaleZ = _scale.z;
    }
}

public class SnapToGrid : EditorWindow {

    GameObject _selection;
    string _levelName;
    float snapValue = 0;
    float snapValueY = 0;
    CubeData[,,] StoreLevel;
    Vector2 _scrollPosition;


    //UNITY LIFECYCLE================================================================================================================
    [MenuItem("Custom Editor/Utils/Snap To Grid")]
    static void Init() {

        SnapToGrid window = (SnapToGrid)EditorWindow.GetWindow(typeof(SnapToGrid));
        window.Show();
    }

    void OnInspectorUpdate()
    {
        if (_selection != null && _selection.tag == "Cube")
        {
            _selection.transform.position = new Vector3(Mathf.Round(_selection.transform.position.x / snapValue) * snapValue,
                                                        Mathf.Round(_selection.transform.position.y / snapValueY) * snapValueY,
                                                        Mathf.Round(_selection.transform.position.z / snapValue) * snapValue);
        }
        else
        {
            if (Selection.activeGameObject != null)
                if (Selection.activeGameObject.tag == "Cube")
                    _selection = Selection.activeGameObject;
        }

        Repaint();
    }

    void OnSelectionChange() {

        _selection = Selection.activeGameObject;

    }

    void OnGUI() {

        GUILayout.Label("======LEVEL LOADER======");
        EditorGUILayout.Space();
        EditorGUILayout.BeginVertical();
        snapValue = EditorGUILayout.FloatField("Snap Value X/Z", snapValue);
        snapValueY = EditorGUILayout.FloatField("Snap Value Y", snapValueY);
        EditorGUILayout.Space();
        _levelName = EditorGUILayout.TextField("Save/Load Name", _levelName);
        EditorGUILayout.EndVertical();

        GUI.color = Color.green;
        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Save")) {
            SaveLevel(_levelName);
        }

        GUI.color = Color.yellow;
        if (GUILayout.Button("Load")) {
            LoadLevel(_levelName);
        }

        EditorGUILayout.EndHorizontal();

        GUILayout.Space(20);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.BeginScrollView(_scrollPosition, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
        GUILayout.Label("=======SAVED LEVELS=======");
        EditorGUILayout.Space();

        string s = ReturnSavedNames();
        if (s != "")
            GUILayout.Label(s);
        else
            GUILayout.Label("No Levels Found");

        EditorGUILayout.EndScrollView();

        EditorGUILayout.EndHorizontal();
    }

    //-----------------------------------------------------------------------------------------------------------------



    //Privates====================================================================================

    private void LoadLevel(string s)
    {
        if (File.Exists(Application.persistentDataPath + "/" + s + ".dat"))
        {
            GameObject[] cubes = GameObject.FindGameObjectsWithTag("Cube");
            for (int i = 0; i < cubes.Length; i++)
            {
                DestroyImmediate(cubes[i]);
            }

            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/" + s + ".dat", FileMode.OpenOrCreate);
            CubeData[,,] temp = (CubeData[,,])bf.Deserialize(file);
            file.Close();

            StoreLevel = temp;

            if (StoreLevel != null)
            {
                for (int x = 0; x < StoreLevel.GetLength(0); x++)
                {
                    for (int y = 0; y < StoreLevel.GetLength(1); y++)
                    {
                        for (int z = 0; z < StoreLevel.GetLength(2); z++)
                        {
                            if (StoreLevel[x, y, z] != null && StoreLevel[x, y, z].ReturnIsTaken)
                            {
                                GameObject box = GameObject.CreatePrimitive(PrimitiveType.Cube);
                                box.transform.position = new Vector3(StoreLevel[x, y, z].ReturnX, StoreLevel[x, y, z].ReturnY, StoreLevel[x, y, z].ReturnZ);
                                box.transform.localScale = StoreLevel[x, y, z].ReturnScale;
                                box.transform.tag = "Cube";
                            }
                        }
                    }
                }
            }
        }
        else
            Debug.Log(s + "    Does Not Exist");
    }

    private void SaveLevel(string s)
    {
        LevelManager _levManager = GameObject.Find("_LevelManager").GetComponent<LevelManager>();
        CubeData[,,] temp = new CubeData[_levManager.m_Width, 10, _levManager.m_Width];

        GameObject[] _cubes = GameObject.FindGameObjectsWithTag("Cube");

        for (int i = 0; i < _cubes.Length; i++)
        {
            Vector3 _pos = _cubes[i].transform.position;
            CubeData _data = new CubeData(_pos.x, _pos.y, _pos.z, _cubes[i].transform.localScale, true);
            temp[(int)Mathf.Abs(_pos.x), (int)Mathf.Abs(_pos.y), (int)Mathf.Abs(_pos.z)] = _data;
            DestroyImmediate((_cubes[i].gameObject));
        }

        CubeData[,,] array = temp;
        string fileName = Application.persistentDataPath + "/" + s + ".dat";
        BinaryFormatter bf = new BinaryFormatter();
        FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write);
        bf.Serialize(fs, array);
        fs.Close();
    }

    private string ReturnSavedNames()
    {
        DirectoryInfo dir = new DirectoryInfo(Application.persistentDataPath);
        FileInfo[] info = dir.GetFiles("*.*");
        string s = "";
        foreach (FileInfo f in info)
        {
            s += f.Name + "\n";
        }
        return s;
    }

    //--------------------------------------------------------------------------------------------
}
