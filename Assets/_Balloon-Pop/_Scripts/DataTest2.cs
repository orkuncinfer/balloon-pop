using System.Collections.Generic;
using UnityEngine;

public class DataTest2 : MonoBehaviour
{
    public float Health;
    public bool Alive;
    public Vector2 vectorr;

    public List<int> listt;

    private void Start()
    {
        if (ES3.KeyExists("deneme"))
        {
            ES3.LoadInto("deneme",this);
        }
    }

    private void OnDestroy()
    {
        string json = JsonUtility.ToJson(this);
        ES3.Save("deneme",json);
    }
}