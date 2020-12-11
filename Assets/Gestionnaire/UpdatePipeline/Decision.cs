using Unity.Entities;
using UnityEngine;
using Unity.Collections;
using Unity.Transforms;
using UnityEngine.Jobs;
using Unity.Mathematics;
using Unity.Jobs;
using Unity.Burst;
using System.Linq;
using Unity.Physics.Systems;
using Unity.Physics;
using System.Collections.Generic;
using CodeMonkey.Utils;

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(InitDecision))]
[UpdateBefore(typeof(BeginSimulationEntityCommandBufferSystem))]
public class Decision : SystemBase
{
    protected override void OnUpdate()
    {
        
    }
}
