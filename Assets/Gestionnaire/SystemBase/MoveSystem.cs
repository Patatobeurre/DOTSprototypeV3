using System.Collections;
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


[UpdateInGroup(typeof(TransformSystemGroup))]
public class MoveSystem : SystemBase
{
    EntityQuery _query;

    EndSimulationEntityCommandBufferSystem m_EndSimulationEcbSystem;

    protected override void OnCreate()
    {
        base.OnCreate();

        m_EndSimulationEcbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        _query = GetEntityQuery(typeof(HashRegion), ComponentType.ReadOnly<MoveStats>(), ComponentType.ReadWrite<Translation>()); ;
    }

    protected override void OnUpdate()
    {
        
        _query.SetChangedVersionFilter(typeof(MoveStats));

        MoveJob _moveJob = new MoveJob();
        _moveJob._deltaTime = Time.DeltaTime;
        _moveJob._moveStatsHandle = GetComponentTypeHandle<MoveStats>(false);
        _moveJob._translationHandle = GetComponentTypeHandle<Translation>(false);
        _moveJob._entityTypeHandle = GetEntityTypeHandle();
        _moveJob._ecb = m_EndSimulationEcbSystem.CreateCommandBuffer().AsParallelWriter();
        _moveJob.ScheduleParallel(_query, Dependency);

        m_EndSimulationEcbSystem.AddJobHandleForProducer(this.Dependency);
    }

    public struct MoveJob : IJobChunk
    {
        public float _deltaTime;
        public ComponentTypeHandle<MoveStats> _moveStatsHandle;
        public ComponentTypeHandle<Translation> _translationHandle;
        public EntityCommandBuffer.ParallelWriter _ecb;
        [ReadOnly] public EntityTypeHandle _entityTypeHandle;

        public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
        {
            NativeArray<MoveStats> _moveStatsArray = chunk.GetNativeArray(_moveStatsHandle);
            NativeArray<Translation> _translationArray = chunk.GetNativeArray(_translationHandle);
            NativeArray<Entity> _entityArray = chunk.GetNativeArray(_entityTypeHandle);


            for (int i = 0; i < chunk.ChunkEntityCount; i++)
            {
                if (Vector3.Distance(_translationArray[i].Value, _moveStatsArray[i]._newPosition) > 0.01)
                {
                    float3 _moveDir = math.normalize(_moveStatsArray[i]._newPosition - _translationArray[i].Value);

                    _translationArray[i] = new Translation { Value = _moveDir * _moveStatsArray[i]._moveSpeed * _deltaTime };

                    int _newHashRegion = Mathf.RoundToInt(_translationArray[i].Value.x * GameSetting.MAP_WIDTH + _translationArray[i].Value.y);
                    if (_moveStatsArray[i]._region != _newHashRegion)
                    {
                        _ecb.SetSharedComponent<HashRegion>(chunkIndex, _entityArray[i], new HashRegion { _hashRegion = _newHashRegion });
                    }
                }
                else
                {
                    _translationArray[i] = new Translation { Value = _moveStatsArray[i]._newPosition }; ;

                    int _newHashRegion = Mathf.RoundToInt(_translationArray[i].Value.x * GameSetting.MAP_WIDTH + _translationArray[i].Value.y);
                    if (_moveStatsArray[i]._region != _newHashRegion)
                    {
                        _ecb.SetSharedComponent<HashRegion>(chunkIndex, _entityArray[i], new HashRegion { _hashRegion = _newHashRegion });
                    }
                }
            }
        }
    }
}
