using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using Unity.Physics;
using Unity.Transforms;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct ColliderGlobu : IComponentData
{
    public float _radius;
    public LayerMask _layerMask;
    public float3 _startPosition;
    public quaternion _orientation;
    public float _length;
}