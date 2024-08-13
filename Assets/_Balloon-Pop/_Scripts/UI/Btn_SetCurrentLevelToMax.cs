using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Btn_SetCurrentLevelToMax : MonoBehaviour
{
    public void SetCurrentLevel()
    {
        GlobalData.GetData<DS_GameModePersistent>().CurrentLevelIndex = GlobalData.GetData<DS_GameModePersistent>().MaxReachedLevelIndex;
    }
}
