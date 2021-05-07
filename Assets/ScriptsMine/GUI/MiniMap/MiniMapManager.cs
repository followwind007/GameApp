using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapLoader: MonoBehaviour {

    private static MiniMapLoader _instance;

    private Transform _playerTrans;

    //private Vector2 _mapSize = new Vector2(2000, 2000);
    //private Vector2 _anchorPosition = new Vector2(-1000, -1000);
    //地图分块
    //private Vector2 _mapBlockDivision = new Vector2(10, 10);
    private Vector2 _mapBlockSize = new Vector2(200, 200);

    private Vector2 _mapPreLocation = new Vector2(0, 0);
    private Vector2 _mapCurLocation = new Vector2(0, 0);

    private bool _initDone = false;

    public delegate void OnMapMove(Vector2 move);

    public MiniMapLoader Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new MiniMapLoader();
            }
            return _instance;
        }
    }

    public void SetMapConfig(int mapId, Transform playerTrans)
    {
        _playerTrans = playerTrans;
        _initDone = true;
    }

    private void Update()
    {
        if (!_initDone)
        {
            return;
        }
        UpdatePlayerPosition();
    }

    private void UpdatePlayerPosition()
    {
        Vector2 pos = new Vector2(_playerTrans.position.x, _playerTrans.position.z);
        int x = Mathf.FloorToInt(pos.x / _mapBlockSize.x);
        int y = Mathf.FloorToInt(pos.y / _mapBlockSize.y);
        Vector2 location = new Vector2(x, y);
        if (location != _mapCurLocation)
        {
            _mapPreLocation = _mapCurLocation;
            _mapCurLocation = location;
            OnMapLocationChange();
        }
        
    }

    private void OnMapLocationChange()
    {
        Debug.Log(_mapPreLocation + ":" + _mapCurLocation);
    }

    private void LoadBlock()
    {

    }

    private void UnloadBlock()
    {

    }

	
}
