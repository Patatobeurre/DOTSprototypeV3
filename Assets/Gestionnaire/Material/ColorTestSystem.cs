using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[AlwaysUpdateSystem]
class ColorTestSystem : SystemBase
{
    protected override void OnUpdate()
    {
        /*Entities.ForEach((ref CustomColor color) =>
        {
            color.Value = new float4(
                0.2f ,
                0.2f ,
                0.2f ,
                1.0f);
        })
            .Schedule();*/


        Debug.Log("colorTestSystem");
    }
}
