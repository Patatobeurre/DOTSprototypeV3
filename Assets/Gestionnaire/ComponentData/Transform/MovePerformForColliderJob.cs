using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine.UIElements;

public struct MovePerformForColliderJob : IComponentData
{
    public float _radius;
    public int _layerMask;
    public float3 _newPosition;
}