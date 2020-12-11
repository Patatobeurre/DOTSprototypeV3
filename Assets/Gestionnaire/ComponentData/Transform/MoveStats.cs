using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine.UIElements;

[GenerateAuthoringComponent]
public struct MoveStats : IComponentData
{
    public int _canMove;
    public int _region;
    public float3 _position;
    public float3 _newPosition;
    public float _moveSpeed;
}