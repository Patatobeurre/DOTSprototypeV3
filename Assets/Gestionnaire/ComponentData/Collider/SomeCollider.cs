using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using Unity.Physics;
using Unity.Transforms;

[GenerateAuthoringComponent]
public struct SomeCollider : IComponentData
{
    public int _meshColliderType; //1 for sphere
    public Vector3 _position;
    public LayerMask _layerMask;
}
