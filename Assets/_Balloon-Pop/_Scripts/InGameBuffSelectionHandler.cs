using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class InGameBuffSelectionHandler : MonoCore
{
    [SerializeField] private InGameSelectableBuff[] _buffSelections;
    [SerializeField] private Canvas _buffSelectionCanvas;
    [SerializeField][FoldoutGroup("Buffs")] private ItemDefinition _buffAttackSpeed;
    [SerializeField][FoldoutGroup("Buffs")] private ItemDefinition _buffRicochet2;
    [SerializeField][FoldoutGroup("Buffs")] private ItemDefinition _buffRicochet4;
    [SerializeField] [FoldoutGroup("Buffs")] private ItemDefinition _buffDuoShot;
    [SerializeField] [FoldoutGroup("Buffs")] private ItemDefinition _buffHeal;
    [SerializeField] [FoldoutGroup("Buffs")] private ItemDefinition _buffIceBall;
    [SerializeField] [FoldoutGroup("Buffs")] private ItemDefinition _buffOrbitingBalls;
    [SerializeField] [FoldoutGroup("Buffs")] private ItemDefinition _buffShield;
    [SerializeField] [FoldoutGroup("Buffs")] private ItemDefinition _buffSideDarts;
    [SerializeField] [FoldoutGroup("Buffs")] private ItemDefinition _buffTripleShot;
    
    [SerializeField] private EventField<string> _onBuffSelected;
    private DS_PlayerPersistent _playerPersistent;
    private DS_PlayerRuntime _playerRuntime;
    [SerializeField]private int _buffSelectBuffer;

    private bool _selectingBuff;

    protected override void OnGameReady()
    {
        base.OnGameReady();
        _playerPersistent = GlobalData.GetData<DS_PlayerPersistent>();
        _playerRuntime = MainPlayer.Actor.GetData<DS_PlayerRuntime>();
        
        for (int i = 0; i < _buffSelections.Length; i++)
        {
            _buffSelections[i].onBuffSelected += OnBuffSelected;
        }

        _playerRuntime.RuntimePlayerLevel.onLevelUp += OnLevelUp;
    }

    private void OnDisable()
    {
        for (int i = 0; i < _buffSelections.Length; i++)
        {
            _buffSelections[i].onBuffSelected -= OnBuffSelected;
        }
        
        _playerRuntime.RuntimePlayerLevel.onLevelUp -= OnLevelUp;
    }


    private void OnLevelUp(int obj)
    {
        _buffSelectBuffer++;
        if (!_selectingBuff)
        {
            OpenBuffSelectionPanel();
        }
       
       
        /*_playerRuntime.AttackSpeedRuntime += _attackSpeedIncreasePerLevel;

        if (_playerRuntime.AttackSpeedRuntime > _playerPersistent.MaxAttackSpeed)
        {
            _playerRuntime.AttackSpeedRuntime = _playerPersistent.MaxAttackSpeed;
        }*/
    }

    private void OpenBuffSelectionPanel()
    {
        if (_buffSelectBuffer <= 0)
        {
            return;
        }
        List<ItemDefinition> buffs = SelectRandomBuffs(3);
        if(buffs == null) return;
        if(buffs.Count  == 0) return;
        
        Time.timeScale = 0;
        _buffSelectionCanvas.gameObject.SetActive(true);
        _selectingBuff = true;
        for (int i = 0; i < _buffSelections.Length; i++)
        {
            ItemDefinition buff = buffs.Count > i ? buffs[i] : null;
            if (buff == null)
            {
                _buffSelections[i].gameObject.SetActive(false);
                continue;
            }
            _buffSelections[i].gameObject.SetActive(true);
            _buffSelections[i].SetItem(buff);
        }
    }

    private void OnBuffSelected(string itemId)
    {
        _onBuffSelected.Raise(itemId);
        _selectingBuff = false;
        _buffSelectionCanvas.gameObject.SetActive(false);
        Time.timeScale = 1;
        _buffSelectBuffer--;
        OpenBuffSelectionPanel();
    }

    public List<ItemDefinition> SelectRandomBuffs(int X)
    {
        List<ItemDefinition> eligibleBuffs = new List<ItemDefinition>();

        // Add eligible buffs based on conditions
        if (AttackSpeedCondition())
        {
            eligibleBuffs.Add(_buffAttackSpeed);
        }
        // If no buffs are eligible, return null
        
        if(!BuffValidator.HasBuff(_buffRicochet2)) eligibleBuffs.Add(_buffRicochet2);
        if(!BuffValidator.HasBuff(_buffRicochet4)) eligibleBuffs.Add(_buffRicochet4);
        if(!BuffValidator.HasBuff(_buffDuoShot)) eligibleBuffs.Add(_buffDuoShot);
        if(!BuffValidator.HasBuff(_buffHeal)) eligibleBuffs.Add(_buffHeal);
        if(!BuffValidator.HasBuff(_buffIceBall)) eligibleBuffs.Add(_buffIceBall);
        if(!BuffValidator.HasBuff(_buffOrbitingBalls)) eligibleBuffs.Add(_buffOrbitingBalls);
        if(!BuffValidator.HasBuff(_buffShield)) eligibleBuffs.Add(_buffShield);
        if(!BuffValidator.HasBuff(_buffSideDarts)) eligibleBuffs.Add(_buffSideDarts);
        if(!BuffValidator.HasBuff(_buffTripleShot)) eligibleBuffs.Add(_buffTripleShot);
        
        if (eligibleBuffs.Count == 0)
        {
            return null;
        }
        // Shuffle the list to randomize the order
        for (int i = eligibleBuffs.Count - 1; i > 0; i--)
        {
            int randomIndex = UnityEngine.Random.Range(0, i + 1);
            ItemDefinition temp = eligibleBuffs[i];
            eligibleBuffs[i] = eligibleBuffs[randomIndex];
            eligibleBuffs[randomIndex] = temp;
        }

        // Take the first X items or as many as available
        int countToTake = Mathf.Min(X, eligibleBuffs.Count);
        List<ItemDefinition> selectedBuffs = eligibleBuffs.GetRange(0, countToTake);

        return selectedBuffs;
    }

    public bool AttackSpeedCondition()
    {
        return true;
    }

    public bool Ricochet2Condition()
    {
        return true;
    }

    public bool Ricochet4Condition()
    {
        return true;
    }
}