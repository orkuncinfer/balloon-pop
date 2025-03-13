using System;
using System.Collections.Generic;
using UnityEngine;

public class DataTest : MonoBehaviour
{
    public float Health;
    public bool Dead;
    public Vector2 vectorr;

    public List<int> listt;

    private void Start()
    {
        if (ES3.KeyExists("deneme"+GetInstanceID()))
        {
           // object loadedData = ES3.Load("deneme" + GetInstanceID());
            //JsonUtility.FromJsonOverwrite((string)loadedData, this);
            ES3.LoadInto("deneme" + GetInstanceID(), this);
        }
    }

    private void OnDestroy()
    {
        //string json = JsonUtility.ToJson(this,true);
        ES3.Save("deneme"+GetInstanceID(),this);
    }
}