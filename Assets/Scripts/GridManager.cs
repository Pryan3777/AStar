using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{

    [SerializeField] private int _width;
    [SerializeField] private int _height;
    [SerializeField] private GameObject _squarePrefab;
    [SerializeField] private int[] _start;
    [SerializeField] private int[] _end;


    [SerializeField] GameObject _selector;
    int[] _selectorPosition;


    private GameObject[,] _grid;
    private int _penaltyValue = 1;
    private List<GameObject> _unvisitedQueue;
    private List<GameObject> _visitedQueue;

    int _intMax = 10000;
    int _stepValue = 1;
    int _endValue;

    bool _backtracked = false;

    // Start is called before the first frame update
    void Start()
    {
        _selectorPosition = new int[2] { 0, 0};
        _endValue = _intMax;
        _unvisitedQueue = new List<GameObject>();
        _visitedQueue = new List<GameObject>();
        _grid = new GameObject[_width, _height];
        for(int i = 0; i < _width; i++)
        {
            for(int j = 0; j < _height; j++)
            {
                _grid[i,j] = Instantiate(_squarePrefab, transform.position + new Vector3((float)i - ((float)(_width - 1) / 2.0f), (float)j - ((float)(_height - 1) / 2.0f), 0.0f), Quaternion.identity, transform);
                Square SquareScript = _grid[i, j].GetComponent<Square>();
                SquareScript.SetGridManager(this);
                SquareScript.SetSpace(i, j);
                SquareScript.SetEndDistance(_end[0], _end[1]);

                if(i == _start[0] && j == _start[1])
                {
                    SquareScript.SetStart();
                    _unvisitedQueue.Add(_grid[i,j]);
                    SquareScript.SetVisited(1);
                }
                if (i == _end[0] && j == _end[1])
                {
                    SquareScript.SetEnd();
                }
            }
        }
        _selector.transform.position = _grid[0, 0].transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int GetPenaltyValue()
    {
        return _penaltyValue;
    }

    public bool Step()
    {
        if (_unvisitedQueue[0].GetComponent<Square>().GetValue() > _endValue)
        {
            if(_backtracked)
            {
                return false;
            }
            else
            {
                Backtrack(_end[0], _end[1], 4);
                _backtracked = true;
            }
        }


        Debug.Log("Step");
        GameObject current = PopUnvisited();
        Square currentSquare = current.GetComponent<Square>();
        currentSquare.SetVisited(2);

        
        int x = currentSquare.GetX();
        int y = currentSquare.GetY();
        int value = currentSquare.GetStartDistance() + (_penaltyValue * currentSquare.GetPenalty());

        StepTo(x, y + 1, value, 0);
        StepTo(x, y - 1, value, 3);
        StepTo(x + 1, y, value, 1);
        StepTo(x - 1, y, value, 2);


        return true;
    }

    void StepTo(int x, int y, int value, int direction)
    {
        if (x < 0 || x >= _width || y < 0 || y >= _height)
        {
            return;
        }

        GameObject steppedToSquare = _grid[x, y];

        if (_visitedQueue.Contains(steppedToSquare))
        {
            return;
        }

        Square square = steppedToSquare.GetComponent<Square>();
        
        if(square.Calculate(value + _stepValue, direction))
        {
            _unvisitedQueue.Remove(steppedToSquare);
            PushUnvisited(steppedToSquare);
            square.SetVisited(1);
        }

        if(x == _end[0] && y == _end[1])
        {
            _endValue = square.GetValue();
        }
    }

    public void Calculate()
    {
        while(Step())
        {
            
        }
    }

    public void Clear()
    {
        _backtracked = false;
        _visitedQueue.Clear();
        _unvisitedQueue.Clear();
        _endValue = _intMax;

        for (int i = 0; i < _width; i++)
        {
            for (int j = 0; j < _height; j++)
            {
                Square squareScript = _grid[i, j].GetComponent<Square>();
                squareScript.SetStartDistance(_intMax);
                squareScript.SetValue(_intMax);
                squareScript.ResetDirections();
                squareScript.SetVisited(0);
                squareScript.SetBacktracked(false);

                if (i == _start[0] && j == _start[1])
                {
                    squareScript.SetStartDistance(0);
                    squareScript.SetValue(squareScript.GetEndDistance());
                    squareScript.SetVisited(1);
                    _unvisitedQueue.Add(_grid[i, j]);
                }
            }
        }
    }

    public void PushUnvisited(GameObject newSquare)
    {
        bool inserted = false;
        for(int i = 0; i < _unvisitedQueue.Count; i++)
        {
            if(newSquare.GetComponent<Square>().GetValue() <= _unvisitedQueue[i].GetComponent<Square>().GetValue())
            {
                inserted = true;
                _unvisitedQueue.Insert(i, newSquare);
                break;
            }
        }
        if(!inserted)
        {
            _unvisitedQueue.Add(newSquare);
        }
    }

    public GameObject PopUnvisited()
    {
        GameObject topSquare = _unvisitedQueue[0];
        _unvisitedQueue.RemoveAt(0);
        return topSquare;
    }

    public GameObject PopVisited()
    {
        GameObject topSquare = _visitedQueue[0];
        _visitedQueue.RemoveAt(0);
        return topSquare;
    }

    public void SelectorLeft()
    {
        _selectorPosition[0]--;
        if (_selectorPosition[0] < 0) _selectorPosition[0] = 0;
        _selector.transform.position = _grid[_selectorPosition[0], _selectorPosition[1]].transform.position;
    }

    public void SelectorRight()
    {
        _selectorPosition[0]++;
        if (_selectorPosition[0] >= _width) _selectorPosition[0] = _width - 1;
        _selector.transform.position = _grid[_selectorPosition[0], _selectorPosition[1]].transform.position;
    }

    public void SelectorUp()
    {
        _selectorPosition[1]++;
        if (_selectorPosition[1] >= _height) _selectorPosition[1] = _height - 1;
        _selector.transform.position = _grid[_selectorPosition[0], _selectorPosition[1]].transform.position;
    }

    public void SelectorDown()
    {
        _selectorPosition[1]--;
        if (_selectorPosition[1] < 0) _selectorPosition[1] = 0;
        _selector.transform.position = _grid[_selectorPosition[0], _selectorPosition[1]].transform.position;
    }

    public void SelectorToggleWall()
    {
        _grid[_selectorPosition[0], _selectorPosition[1]].GetComponent<Square>().TogglePenalty();
        Clear();
    }

    public void IncreasePenalty()
    {
        _penaltyValue++;
        Clear();
    }

    public void DecreasePenalty()
    {
        _penaltyValue--;
        if(_penaltyValue < 0)
        {
            _penaltyValue = 0;
        }
        Clear();
    }

    public bool HasBacktracked()
    {
        return _backtracked;
    }

    public void Backtrack(int x, int y, int direction)
    {
        _grid[x, y].GetComponent<Square>().Backtrack(direction);
    }
}
