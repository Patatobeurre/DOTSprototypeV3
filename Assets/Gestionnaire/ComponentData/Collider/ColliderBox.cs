using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using Unity.Physics;
using Unity.Transforms;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct ColliderBox : IComponentData
{
    public float _width;
    public float _length;
    public LayerMask _layerMask;
    public float3 _position;
    public quaternion _orientation;
}
