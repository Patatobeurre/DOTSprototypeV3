using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class MapQuadrant : MonoBehaviour
{

    public int _mapWidth;  // x
    public int _mapLength;  // y
    public int _regionNumber;

    public NativeMultiHashMap<int, Entity> _regionQuadrant;
    // Start is called before the first frame update
    void Start()
    {
        _mapWidth = 40;
        _mapLength = 40;
        _regionNumber = (_mapWidth / 20) * (_mapLength / 20);
        _regionQuadrant = new NativeMultiHashMap<int, Entity>(_regionNumber, Allocator.TempJob);
    }

    static public void UpdateEntityRegion(Entity _entity, NativeMultiHashMap<int, Entity> _regionQuadrant, int _currentHashRegion, float3 _newPosition, int _mapWidth)
    {
        int _newHashRegion = Mathf.RoundToInt(_newPosition.x * _mapWidth +_newPosition.y);
        if (_currentHashRegion != _newHashRegion)
        {
            _regionQuadrant.Remove(_currentHashRegion, _entity);
            _currentHashRegion = _newHashRegion;
            _regionQuadrant.Add(_currentHashRegion, _entity);
        }
    }
}