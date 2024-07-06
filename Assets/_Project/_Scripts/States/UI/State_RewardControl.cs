using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class State_RewardControl : MonoState
{
    [HorizontalGroup]public int Hours;
    [HorizontalGroup]public int Minutes; // Set the interval for rewards in hours
    public TextMeshProUGUI remainingTimeText; // Reference to the TextMeshProUGUI component
    public Button collectRewardButton; // Reference to the Button component

    private DS_PlayerPersistent _playerData;

    protected override void OnEnter()
    {
        base.OnEnter();
        _playerData = GlobalData.GetData<DS_PlayerPersistent>();
        collectRewardButton.onClick.AddListener(CheckForReward);
        StartCoroutine(UpdateRemainingTime());
    }

    protected override void OnExit()
    {
        base.OnExit();
        collectRewardButton.onClick.RemoveListener(CheckForReward);
        StopAllCoroutines();
    }

    void CheckForReward()
    {
        DateTime lastRewardTime = GetLastRewardTime();
        DateTime currentTime = WorldTimeAPI.Instance.GetCurrentDateTime();

        TimeSpan timeSinceLastReward = currentTime - lastRewardTime;
        if (timeSinceLastReward.TotalMinutes >= Hours * 60 + Minutes)
        {
            GrantReward();
            SetLastRewardTime(currentTime);
        }
        else
        {
            UpdateRemainingTimeText(Hours * 60 + Minutes - timeSinceLastReward.TotalMinutes);
        }
    }

    DateTime GetLastRewardTime()
    {
        string lastRewardTimeString = _playerData.LastTimeRewardCollected;
        if(_playerData.LastTimeRewardCollected.IsNullOrWhitespace()) return DateTime.MinValue;
        return DateTime.Parse(lastRewardTimeString);
    }

    void SetLastRewardTime(DateTime time)
    {
        _playerData.LastTimeRewardCollected = time.ToString();
    }

    void GrantReward()
    {
        // Implement your reward logic here
        Debug.Log("Reward granted!");
    }

    IEnumerator UpdateRemainingTime()
    {
        while (true)
        {
            DateTime lastRewardTime = GetLastRewardTime();
            DateTime currentTime = WorldTimeAPI.Instance.GetCurrentDateTime();
            TimeSpan timeSinceLastReward = currentTime - lastRewardTime;
            
            if (timeSinceLastReward.TotalMinutes < Hours * 60 + Minutes)
            {
                double minutesLeft = Hours * 60 + Minutes - timeSinceLastReward.TotalMinutes;
                UpdateRemainingTimeText(minutesLeft);
            }
            else
            {
                remainingTimeText.text = "Reward available!";
            }

            yield return null;
        }
    }

    void UpdateRemainingTimeText(double minutesLeft)
    {
        TimeSpan remainingTime = TimeSpan.FromMinutes(minutesLeft);
        remainingTimeText.text = string.Format("{0:D2}:{1:D2}:{2:D2}", remainingTime.Hours, remainingTime.Minutes, remainingTime.Seconds);
    }

    

    [Serializable]
    private class TimeApiResponse
    {
        public string datetime;
    }
}
