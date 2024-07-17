using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelNode : MonoBehaviour
{
    [SerializeField] private GameObject _locked;
    [SerializeField] private GameObject _unlocked;

    [SerializeField] private GameObject _star1Empty;
    [SerializeField] private GameObject _star2Empty;
    [SerializeField] private GameObject _star3Empty;
    
    [SerializeField] private GameObject _star1Filled;
    [SerializeField] private GameObject _star2Filled;
    [SerializeField] private GameObject _star3Filled;

    [SerializeField] private int _levelNumber;
    [SerializeField] private TextMeshProUGUI[] _levelNumberTexts;
     
    public void SetLocked(bool locked)
    {
        _locked.SetActive(locked);
        _unlocked.SetActive(!locked);
    }
    
    public void SetStars(int stars)
    {
        _star1Empty.SetActive(true);
        _star2Empty.SetActive(true);
        _star3Empty.SetActive(true);
        
        _star1Filled.SetActive(stars >= 1);
        _star2Filled.SetActive(stars >= 2);
        _star3Filled.SetActive(stars >= 3);
    }
    
    public void SetLevelNumber(int levelNumber)
    {
        _levelNumber = levelNumber;
        foreach (var text in _levelNumberTexts)
        {
            text.text = _levelNumber.ToString();
        }
    }
}
