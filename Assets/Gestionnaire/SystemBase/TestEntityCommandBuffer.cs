﻿using System.Collections;
using System.Collections.Generic;
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
using UnityEngine.UIElements;

// Systems can schedule work to run on worker threads.
// However, creating and removing Entities can only be done on the main thread to prevent race conditions.
// The system uses an EntityCommandBuffer to defer tasks that can't be done inside the Job.

// ReSharper disable once InconsistentNaming
[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(GameSetting))]
public class TestEntityCommandBuffer : SystemBase
{
    // BeginInitializationEntityCommandBufferSystem is used to create a command buffer which will then be played back
    // when that barrier system executes.
    // Though the instantiation command is recorded in the SpawnJob, it's not actually processed (or "played back")
    // until the corresponding EntityCommandBufferSystem is updated. To ensure that the transform system has a chance
    // to run on the newly-spawned entities before they're rendered for the first time, the SpawnerSystem_FromEntity
    // will use the BeginSimulationEntityCommandBufferSystem to play back its commands. This introduces a one-frame lag
    // between recording the commands and instantiating the entities, but in practice this is usually not noticeable.
    BeginInitializationEntityCommandBufferSystem m_EntityCommandBufferSystem;
    EntityArchetype _souffleDeFeu;

    EntityQuery _query;

    protected override void OnCreate()
    {
        // Cache the BeginInitializationEntityCommandBufferSystem in a field, so we don't have to create it every frame
        m_EntityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
        _souffleDeFeu = EntityManager.CreateArchetype(
        typeof(HashRegion),
        typeof(SouffleDeFeuTemp)
        );

        _query = GetEntityQuery(typeof(SouffleDeFeuTemp));
    }

    protected override void OnUpdate()
    {
        //Instead of performing structural changes directly, a Job can add a command to an EntityCommandBuffer to perform such changes on the main thread after the Job has finished.
        //Command buffers allow you to perform any, potentially costly, calculations on a worker thread, while queuing up the actual insertions and deletions for later.
        var commandBuffer = m_EntityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter();

        // Schedule the Entities.ForEach lambda job that will add Instantiate commands to the EntityCommandBuffer.
        // Since this job only runs on the first frame, we want to ensure Burst compiles it before running to get the best performance (3rd parameter of WithBurst)
        // The actual job will be cached once it is compiled (it will only get Burst compiled once).
        if (Input.GetKey(KeyCode.O) == true)
        {
            Entities
            .WithName("SpawnerSystem_FromEntity")
            //.WithBurst(FloatMode.Default, FloatPrecision.Standard, true)
            .ForEach((Entity _entity, int entityInQueryIndex, in SouffleDeFeuTemp spawnerFromEntity) =>
            {

                /*
                for (var x = 0; x < spawnerFromEntity.CountX; x++)
                {
                    for (var y = 0; y < spawnerFromEntity.CountY; y++)
                    {
                        var instance = commandBuffer.Instantiate(entityInQueryIndex, spawnerFromEntity.Prefab);

                        // Place the instantiated in a grid with some noise
                        var position = math.transform(location.Value,
                            new float3(x * 1.3F, noise.cnoise(new float2(x, y) * 0.21F) * 2, y * 1.3F));
                        commandBuffer.SetComponent(entityInQueryIndex, instance, new Translation { Value = position });
                    }
                }*/
                //var instance = commandBuffer.Instantiate(entityInQueryIndex, entity);
                /*var position = math.transform(location.Value,
                new float3(5 * 1.3F, noise.cnoise(new float2(5, 3) * 0.21F) * 2, 6 * 1.3F));
                commandBuffer.SetComponent(entityInQueryIndex, entity, new Translation { Value = position });*/

                //commandBuffer.Instantiate(entityInQueryIndex, _entity);

                //commandBuffer.DestroyEntity(entityInQueryIndex, entity);
            }).ScheduleParallel();

            /*
            JobJob _jobJob = new JobJob();
            _jobJob._ecb = commandBuffer;
            _jobJob._entityHandle = GetEntityTypeHandle();
            _jobJob._entityArchetype = _souffleDeFeu;
            Dependency = _jobJob.ScheduleParallel(_query, Dependency);*/
        }



        // SpawnJob runs in parallel with no sync point until the barrier system executes.
        // When the barrier system executes we want to complete the SpawnJob and then play back the commands (Creating the entities and placing them).
        // We need to tell the barrier system which job it needs to complete before it can play back the commands.
        m_EntityCommandBufferSystem.AddJobHandleForProducer(Dependency);
    }

    public struct JobJob : IJobChunk
    {
        public EntityCommandBuffer.ParallelWriter _ecb;
       [ReadOnly] public EntityTypeHandle _entityHandle;
        [ReadOnly] public EntityArchetype _entityArchetype;
        public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
        {
            NativeArray<Entity> _entityArray = chunk.GetNativeArray(_entityHandle);
            EntityArchetype _archetype = _entityArchetype;
            _ecb.CreateEntity(chunkIndex, _archetype);
        }
    }
}
