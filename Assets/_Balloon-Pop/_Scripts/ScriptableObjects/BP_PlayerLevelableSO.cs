    using System;
    using System.Collections;
using System.Collections.Generic;
    using Sirenix.OdinInspector;
    using UnityEngine;
    using UnityEngine.Serialization;

    [SOCreatable("Level")]
public class BP_PlayerLevelableSO : ScriptableObject
{
    [Header("Level Settings")]
    [ES3NonSerializable]public int MaxLevel = 20;
    [ES3NonSerializable]public AnimationCurve experienceCurve;
    
    [Header("Current State")]
    public int Level = 1;
     public float CurrentExperience = 0;

    public event Action<float> onExpAdded;
    public event Action<int> onLevelUp;
    public event Action<float> onExpChanged;

    private void OnEnable()
    {
        if (experienceCurve == null)
        {
            experienceCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 100), new Keyframe(MaxLevel, 10000));
        }
    }

    [Button]
    public float GetExperienceForLevel(int targetLevel)
    {
        return experienceCurve.Evaluate((float)targetLevel / MaxLevel);
    }
    [Button]
    public void AddExperience(float amount)
    {
        CurrentExperience += amount;
        onExpAdded?.Invoke(amount);
        onExpChanged?.Invoke(CurrentExperience);
        CheckLevelUp();
    }
    [Button]
    public void ResetLevel()
    {
        Level = 0;
        CurrentExperience = 0;
        onExpChanged?.Invoke(CurrentExperience);
    }

    private void CheckLevelUp()
    {
        while (CurrentExperience >= GetExperienceForLevel(Level) && Level < MaxLevel)
        {
            CurrentExperience -= GetExperienceForLevel(Level);
            Level++;

            onLevelUp?.Invoke(Level);
            onExpChanged?.Invoke(CurrentExperience);
        }

        if (Level >= MaxLevel)
        {
            CurrentExperience = 0; // Reset experience after reaching max level
            onExpChanged?.Invoke(CurrentExperience);
        }
    }

    public float GetCurrentLevelProgress()
    {
        if (Level >= MaxLevel)
        {
            return 1;
        }

        float currentLevelExp = GetExperienceForLevel(Level);
        float nextLevelExp = GetExperienceForLevel(Level + 1);

        return (CurrentExperience - currentLevelExp) / (nextLevelExp - currentLevelExp);
    }
}
