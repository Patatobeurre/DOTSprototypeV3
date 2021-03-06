﻿using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using Unity.Physics;
using Unity.Transforms;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct ColliderSphere : IComponentData
{
    public float _radius;
    public LayerMask _layerMask;
    public float3 _position;
}
