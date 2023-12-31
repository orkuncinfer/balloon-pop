using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_CheckForMatch : MonoState
{
    [SerializeField] private EventSignal _checkForMatchEvent;

    private DS_TileBoard _boardData;
    protected override void OnEnter()
    {
        base.OnEnter();
        _boardData = Owner.GetData<DS_TileBoard>();
        _boardData.HasMatch = false;

        GetMatches();
        _checkForMatchEvent.Register(OnCheckForMatch);
    }

    protected override void OnExit()
    {
        base.OnExit();
        _checkForMatchEvent.Unregister(OnCheckForMatch);
    }

    private void OnCheckForMatch()
    {
        if (_boardData.LastSwipedTileDrop) // swipe match
        {
            _boardData.LastSwipedTileDrop.GetData<DS_TileDrop>().OnIsSettledChanged += OnSwipedDropSettledChanged;
        }
        else // automatic match
        {
            GetMatches();
        }
    }
    
    public bool GetMatches()
    {
        int totalMatch = 0;
        // horizontal Matches
        for (int y = 0; y < _boardData.Height; y++)
        {
            for (int x = 0; x < _boardData.Width - 2; x++)
            {
                GenericKey currentKey = _boardData.GetDropTypeKeyWithCoordinates(x, y);
                GenericKey rightKey1 = _boardData.GetDropTypeKeyWithCoordinates(x + 1, y);
                GenericKey rightKey2 = _boardData.GetDropTypeKeyWithCoordinates(x + 2, y);

                if (currentKey == _boardData.RocketDropKey)
                    currentKey = rightKey1;
                if (rightKey1 == _boardData.RocketDropKey)
                    rightKey1 = currentKey;
                if (rightKey2 == _boardData.RocketDropKey)
                    rightKey2 = currentKey;
                
                if(currentKey == null || rightKey1 == null|| rightKey2 == null ) continue;
                
                if (currentKey == rightKey1 && currentKey == rightKey2)
                {
                    int matchLength = 3;
                    while (x + matchLength < _boardData.Width && 
                           _boardData.GetDropTypeKeyWithCoordinates(x,y) == _boardData.GetDropTypeKeyWithCoordinates(x + matchLength,y) || 
                           _boardData.GetDropTypeKeyWithCoordinates(x + matchLength,y) == _boardData.RocketDropKey)
                    {
                        matchLength++;
                    }
                    for (int i = 0; i < matchLength; i++)
                    {
                       _boardData.MatchedDrops.Add(_boardData.GetDropActorWithCoordinates(x + i,y));
                       totalMatch++;
                    }
                    x += matchLength - 1;
                }
            }
        }

        // vertical Matches
        for (int x = 0; x < _boardData.Width; x++)
        {
            for (int y = 0; y < _boardData.Height - 2; y++)
            {
                GenericKey currentKey = _boardData.GetDropTypeKeyWithCoordinates(x, y);
                GenericKey upKey1 = _boardData.GetDropTypeKeyWithCoordinates(x, y + 1);
                GenericKey upKey2 = _boardData.GetDropTypeKeyWithCoordinates(x, y + 2);
                if(currentKey == null || upKey1 == null|| upKey2 == null ) continue;
                
                if (currentKey == _boardData.RocketDropKey)
                    currentKey = upKey1;
                if (upKey1 == _boardData.RocketDropKey)
                    upKey1 = currentKey;
                if (upKey2 == _boardData.RocketDropKey)
                    upKey2 = currentKey;
                
                if (currentKey == upKey1 && currentKey == upKey2)
                {
                    int matchLength = 3;
                    while (y + matchLength < _boardData.Height && 
                           _boardData.GetDropTypeKeyWithCoordinates(x,y) == _boardData.GetDropTypeKeyWithCoordinates(x,y + matchLength))
                    {
                        matchLength++;
                    }
                    for (int i = 0; i < matchLength; i++)
                    {
                        _boardData.MatchedDrops.Add(_boardData.GetDropActorWithCoordinates(x,y + i));
                        totalMatch++;
                    }
                    y += matchLength - 1;
                }
            }
        }

        if (totalMatch > 0)
        {
            _boardData.HasMatch = true;
            return true;
        }
        else
        {
            _boardData.HasMatch = false;
            return false;
        }
    }
    
    private void OnSwipedDropSettledChanged(Actor arg1, bool arg2, bool arg3)
    {
        if (arg3 == true)
        {
            _boardData.LastSwipedTileDrop.GetData<DS_TileDrop>().OnIsSettledChanged -= OnSwipedDropSettledChanged;
            if (GetMatches())
            {
                
            }
            else
            {
                _boardData.LastSwipeCommand.Undo();
                _boardData.LastSwipeCommand.ReturnToPool();  
            }
            _boardData.LastSwipedTileDrop = null;
        }
    }

  

    
}