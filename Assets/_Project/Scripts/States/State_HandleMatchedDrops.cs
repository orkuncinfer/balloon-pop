using System.Collections;
using UnityEngine;

public class State_HandleMatchedDrops : MonoState
{
    private DS_TileBoard _boardData;

    [SerializeField] private float _explodeDelay; // for better visualization
    private float _initTime;
    [SerializeField] private GenericKey _hasShieldTag;
    protected override void OnEnter()
    {
        base.OnEnter();
        _initTime = Time.time;
        _boardData = Owner.GetData<DS_TileBoard>();
        
        for (int i = 0; i < _boardData.MatchedDrops.Count; i++)
        {
            _boardData.MatchedDrops[i].GetData<DS_TileDrop>().Animator.SetTrigger("Explode");
        }
    }
    protected override void OnUpdate()
    {
        base.OnUpdate();
        if (Time.time >= _initTime + _explodeDelay)
        {
            for (int i = 0; i < _boardData.MatchedDrops.Count; i++)
            {
                DS_TileDrop dropData= _boardData.MatchedDrops[i].GetData<DS_TileDrop>();
                dropData.IsMatched = true;

                if (dropData.CurrentCell != null)
                {
                    if (dropData.CurrentCell.ContainsTag(_hasShieldTag.ID))
                    {
                        dropData.CurrentCell.RemoveTag(_hasShieldTag.ID);
                    }
                }
            }
            _boardData.MatchedDrops.Clear();
            CheckoutExit();
        }
    }
}