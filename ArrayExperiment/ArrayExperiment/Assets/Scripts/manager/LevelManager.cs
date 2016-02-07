using UnityEngine;
using System.Collections;

public class LevelManager : MonoBehaviour {

    public int m_Width;
    public int m_Height;
    public GameObject[,] _GameObjectArray;
    public int[,] _IntArray;

    public GameObject m_Cube;

	// Use this for initialization
	void Start () {

        _GameObjectArray = new GameObject[m_Width, m_Height];
        _IntArray = new int[m_Width, m_Height];
        PopulateWorld(m_Width, m_Height);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    private void PopulateWorld(int _width, int _height)
    {
        for (int x = 0; x < _width; x++)
        {
            for (int z = 0; z < _height; z++)
            {
                GameObject cube;
                cube = Instantiate(m_Cube, new Vector3(x, 0, z), transform.rotation) as GameObject;
                _GameObjectArray[x, z] = cube;
                _IntArray[x, z] = 1;
            }
        }
    }

    public void ImplementArray<T>(T[] param)
    {
    }
}
