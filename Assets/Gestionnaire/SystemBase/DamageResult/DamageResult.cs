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
using UnityEngine.UIElements;

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(SouffleDeFeuCollider))]
public class DamageResult : SystemBase
{
    EntityQuery _query;
    EndSimulationEntityCommandBufferSystem m_EndSimulationEcbSystem;

    protected override void OnCreate()
    {
        base.OnCreate();

        m_EndSimulationEcbSystem = EntityManager.World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();


        _query = GetEntityQuery(typeof(HerosArchetype), ComponentType.ReadWrite<DamageCount>());


    }

    protected override void OnUpdate()
    {
        EntityCommandBuffer.ParallelWriter _ecb = m_EndSimulationEcbSystem.CreateCommandBuffer().AsParallelWriter();

        _query.SetChangedVersionFilter(typeof(DamageCount));
        Debug.Log(_query.IsEmpty + "pouet");
        Debug.Log(_query.CalculateEntityCount() + "damageresult");
        if (!_query.IsEmpty)
        {
            DamageResultJob _damageResultJob = new DamageResultJob();
            _damageResultJob._damageCountHandle = this.GetComponentTypeHandle<DamageCount>(false);
            _damageResultJob._healthPointHandle = this.GetComponentTypeHandle<HealthPoint>(false);
            _damageResultJob._EntityHandle = this.GetEntityTypeHandle();
            _damageResultJob._ecb = _ecb;

            this.Dependency = _damageResultJob.ScheduleParallel(_query, this.Dependency);
            
        }


        m_EndSimulationEcbSystem.AddJobHandleForProducer(this.Dependency);

    }

    
    public struct DamageResultJob : IJobChunk
    {
        
        public ComponentTypeHandle<DamageCount> _damageCountHandle;
        public ComponentTypeHandle<HealthPoint> _healthPointHandle;
        [ReadOnly] public EntityTypeHandle _EntityHandle;
        public EntityCommandBuffer.ParallelWriter _ecb;

       
        void IJobChunk.Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
        {
            NativeArray<DamageCount> _damageCount = chunk.GetNativeArray(_damageCountHandle);
            NativeArray<HealthPoint> _healthPoint = chunk.GetNativeArray(_healthPointHandle);
            NativeArray<Entity> _entity = chunk.GetNativeArray(_EntityHandle);

            for (int i = 0; i < _damageCount.Length; i++)
            {
                _healthPoint[i] = new HealthPoint()
                {
                    _healthPoint = _healthPoint[i]._healthPoint + _damageCount[i]._heal - _damageCount[i]._damageExplosionCount - _damageCount[i]._damageFireCount
                };

                _damageCount[i] = new DamageCount();
                
                if(_healthPoint[i]._healthPoint <= 0)
                {
                    _ecb.DestroyEntity(chunkIndex, _entity[i]);
                }
            }
        }
    }
}
