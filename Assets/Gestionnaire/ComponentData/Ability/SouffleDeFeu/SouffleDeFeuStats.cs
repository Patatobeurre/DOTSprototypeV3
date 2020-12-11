using UnityEngine;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine.Jobs;
using Unity.Transforms;

[GenerateAuthoringComponent]
public struct SouffleDeFeuStats : IComponentData
{
    public float3 _position;
    public int _team;
    public float _damage;
    public float _radius;
}
