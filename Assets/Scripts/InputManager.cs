using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField] GameObject _gridManagerObject;
    GridManager _gridManager;


    // Start is called before the first frame update
    void Start()
    {
        _gridManager = _gridManagerObject.GetComponent<GridManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.S))
        {
            _gridManager.Step();
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            _gridManager.Calculate();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            _gridManager.Clear();
        }
        if(Input.GetKeyDown(KeyCode.DownArrow))
        {
            _gridManager.SelectorDown();
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            _gridManager.SelectorUp();
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            _gridManager.SelectorLeft();
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            _gridManager.SelectorRight();
        }
        if( Input.GetKeyDown(KeyCode.Space))
        {
            _gridManager.SelectorToggleWall();
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            _gridManager.IncreasePenalty();
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            _gridManager.DecreasePenalty();
        }
    }
}
