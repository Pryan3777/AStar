using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Square : MonoBehaviour
{
    [SerializeField] TMPro.TextMeshProUGUI _displayTL;
    [SerializeField] TMPro.TextMeshProUGUI _displayTR;
    [SerializeField] TMPro.TextMeshProUGUI _displayBL;
    [SerializeField] TMPro.TextMeshProUGUI _displayM;

    [SerializeField] SpriteRenderer[] _backwardArrows;
    [SerializeField] SpriteRenderer[] _forwardArrows;
    [SerializeField] SpriteRenderer _border;
    [SerializeField] SpriteRenderer _background;

    [SerializeField] Color _baseBGColor;
    [SerializeField] Color _unvisitedBGColor;
    [SerializeField] Color _visitedBGColor;

    [SerializeField] Color _baseBorderColor;
    [SerializeField] Color _startBorderColor;
    [SerializeField] Color _endBorderColor;
    [SerializeField] Color _wallBorderColor;

    GridManager _gridManager;
    int[] _space;
    bool[] _directionBackward;
    bool[] _directionForward;

    int _startDistance;
    int _endDistance = 0;
    int _penalty = 0;
    int _value;
    int _visitedStatus = 0;

    int _intMax = 10000;

    bool _isStart = false;
    bool _isEnd = false;
    bool _backtracked = false;

    private void Awake()
    {
        _directionBackward = new bool[4];
        _directionForward = new bool[4];
        _space = new int[2];
        _startDistance = _intMax;
        _value = _intMax;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(_gridManager != null)
        {
            _displayTL.SetText(_startDistance < _intMax ? _startDistance.ToString() : "-");
            _displayM.SetText(_value < _intMax ? _value.ToString() : "-");
            _displayTR.SetText(_endDistance.ToString());
            _displayBL.SetText((_penalty * _gridManager.GetPenaltyValue()).ToString());

            if(_gridManager.HasBacktracked())
            {
                for (int i = 0; i < 4; i++)
                {
                    _backwardArrows[i].enabled = false;
                    _forwardArrows[i].enabled = _directionForward[i];
                }
                if(_backtracked)
                {
                    _background.color = _visitedBGColor;
                }
                else
                {
                    _background.color = _baseBGColor;
                }
            }
            else
            {
                for (int i = 0; i < 4; i++)
                {
                    _forwardArrows[i].enabled = false;
                    _backwardArrows[i].enabled = _directionBackward[i];
                }
                switch (_visitedStatus)
                {
                    case 0:
                        _background.color = _baseBGColor;
                        break;
                    case 1:
                        _background.color = _unvisitedBGColor;
                        break;
                    case 2:
                        _background.color = _visitedBGColor;
                        break;
                }
            }

            if(_isStart)
            {
                _border.color = _startBorderColor;
            }
            else if(_isEnd)
            {
                _border.color = _endBorderColor;
            }
            else if(_penalty == 1)
            {
                _border.color = _wallBorderColor;
            }
            else 
            {
                _border.color = _baseBorderColor;
            }
        }
    }

    public void SetStart()
    {
        _startDistance = 0;
        _value = _endDistance;
        _border.color = _startBorderColor;
        _isStart = true;
    }

    public void SetEnd()
    {
        _border.color = _endBorderColor;
        _isEnd = true;
    }

    public void SetGridManager(GridManager gridManager)
    {
        _gridManager = gridManager;
    }

    public void SetSpace(int x, int y)
    {
        _space[0] = x;
        _space[1] = y;
    }

    public void SetStartDistance(int v)
    {
        _startDistance = v;
    }

    public void SetValue(int v)
    {
        _value = v;
    }

    public void SetEndDistance(int x, int y)
    {
        int distanceX = (x - _space[0] > 0 ? x - _space[0] : _space[0] - x);
        int distanceY = (y - _space[1] > 0 ? y - _space[1] : _space[1] - y);
        _endDistance = distanceX + distanceY;
    }

    public bool Calculate(int inputValue, int direction)
    {
        bool result = false;
        if(inputValue < _startDistance)
        {
            result = true;
            _startDistance = inputValue;
            _value = _startDistance + _endDistance + (_gridManager.GetPenaltyValue() * _penalty);

            for (int i = 0; i < 4; ++i)
            {
                _directionBackward[i] = false;
            }
        }
        if(inputValue <= _startDistance)
        {
            _directionBackward[direction] = true;
        }
        return result;
    }



    public int GetValue()
    {
        return _value;
    }

    public int GetX()
    {
        return _space[0];
    }

    public int GetY()
    {
        return _space[1];
    }

    public int GetStartDistance()
    {
        return _startDistance;
    }

    public void ResetDirections()
    {
        for(int i = 0; i < 4; i++)
        {
            _directionBackward[i] = false;
            _directionForward[i] = false;
        }
    }

    public int GetEndDistance()
    {
        return _endDistance;
    }

    public void TogglePenalty()
    {
        _penalty = 1 - _penalty;
    }

    public int GetPenalty()
    {
        return _penalty;
    }

    public void SetVisited(int v)
    {
        _visitedStatus = v;
    }

    public void SetBacktracked(bool b)
    {
        _backtracked = b;
    }

    public void Backtrack(int direction)
    {
        if(direction < 4)
        {
            _directionForward[direction] = true;
        }
        if( _backtracked )
        {
            return;
        }
        _backtracked = true;
        if (_directionBackward[0])_gridManager.Backtrack(GetX(), GetY() - 1, 3);
        if (_directionBackward[1]) _gridManager.Backtrack(GetX() - 1, GetY(), 2);
        if (_directionBackward[2]) _gridManager.Backtrack(GetX() + 1, GetY(), 1);
        if (_directionBackward[3]) _gridManager.Backtrack(GetX(), GetY() + 1, 0);
    }
}

