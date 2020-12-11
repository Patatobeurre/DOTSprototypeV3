using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine.UIElements;
using Unity.Transforms;

[GenerateAuthoringComponent]
public struct SouffleDeFeuColliderForJob : IComponentData
{
    public float3 _position;
    public LayerMask _layerMask;
    public float  _radius;
    public float _damageExplosion;
}