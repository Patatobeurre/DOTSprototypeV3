using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Rendering;
using UnityEngine;
using Unity.Mathematics;


[Serializable]
[MaterialProperty("_ColorColor", MaterialPropertyFormat.Float4)]
public struct CustomColor : IComponentData
{
    public float4 Value;
}