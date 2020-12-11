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

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(HeroesController))]

public class SouffleDeFeuCollider : SystemBase
{
    private EntityQuery _souffleQuery;
    EntityQuery _query;
    EndSimulationEntityCommandBufferSystem m_EndSimulationEcbSystem;
    BeginInitializationEntityCommandBufferSystem m_EntityCommandBufferSystem;

    EntityCommandBuffer.ParallelWriter _ecb;

    protected override void OnCreate()
    {
        base.OnCreate();

        m_EndSimulationEcbSystem = EntityManager.World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        m_EntityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();

        _souffleQuery = EntityManager.CreateEntityQuery(typeof(HashRegion), ComponentType.ReadOnly<SouffleDeFeuTemp>());

        _query = GetEntityQuery(typeof(HashRegion), ComponentType.ReadOnly<MoveStats>(), ComponentType.ReadWrite<DamageCount>());
    }

    protected override void OnUpdate()
    {
        //_ecb = m_EndSimulationEcbSystem.CreateCommandBuffer().AsParallelWriter();
        _ecb = m_EntityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter();


        for (int i = 0; i < GameSetting.HASH_REGION_NUM; i++)
        {
            _query.SetSharedComponentFilter(new HashRegion { _hashRegion = i });
            _souffleQuery.SetSharedComponentFilter(new HashRegion { _hashRegion = i });


            if (_souffleQuery.CalculateEntityCount() > 0)
            {
                Debug.Log(_query.CalculateEntityCount() + "[souffleColliderqueryCount");
                NativeArray<SouffleDeFeuTemp> _souffleDeFeu = new NativeArray<SouffleDeFeuTemp>(_souffleQuery.CalculateEntityCount(), Allocator.TempJob);


                SouffleColliderJob _souffleColliderJob = new SouffleColliderJob();

                _souffleColliderJob._souffleDeFeuTempArray = _souffleDeFeu;
                _souffleColliderJob._souffleHandle = GetComponentTypeHandle<SouffleDeFeuTemp>(true);
                _souffleColliderJob._entityHandle = GetEntityTypeHandle();
                _souffleColliderJob._ecb = _ecb;
                this.Dependency = _souffleColliderJob.ScheduleParallel(_souffleQuery, this.Dependency);
                this.Dependency.Complete();

                Debug.Log(_souffleDeFeu.Length + "jeanmichelle");
                SouffleDamageJob _souffleDamageJob = new SouffleDamageJob();

                _souffleDamageJob._moveStatsHandle = GetComponentTypeHandle<MoveStats>(true);
                _souffleDamageJob._damageCountHandle = GetComponentTypeHandle<DamageCount>(false);
                _souffleDamageJob._souffleTempArray = _souffleDeFeu;
                this.Dependency = _souffleDamageJob.ScheduleParallel(_query, this.Dependency);


                this.Dependency.Complete();
                //Debug.Log(_souffleDeFeu.Count() + "jeanmichelle");
                _souffleDeFeu.Dispose();
            }


        }

        m_EntityCommandBufferSystem.AddJobHandleForProducer(this.Dependency);
        
    }

    public struct SouffleColliderJob : IJobChunk
    {
        [ReadOnly] public ComponentTypeHandle<SouffleDeFeuTemp> _souffleHandle;
        public NativeArray<SouffleDeFeuTemp> _souffleDeFeuTempArray;
        [ReadOnly] public EntityTypeHandle _entityHandle;
        public EntityCommandBuffer.ParallelWriter _ecb;

        public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
        {
            NativeArray<SouffleDeFeuTemp> _souffleArray = chunk.GetNativeArray(_souffleHandle);
            NativeArray<Entity> _entityArray = chunk.GetNativeArray(_entityHandle);
            NativeArray<SouffleDeFeuTemp> _souffleTempArray = _souffleDeFeuTempArray;

            for (int i = 0; i < chunk.Count; i++)
            {
                _souffleTempArray[i] = _souffleArray[i];
                _ecb.SetComponent<SouffleDeFeuTemp>(chunkIndex, _entityArray[i], new SouffleDeFeuTemp{ _damage = 1, _position = new Vector3(0, 0, 0), _radius = 15, _team = 18 });
                _ecb.DestroyEntity(chunkIndex, _entityArray[i]);
            }
        }
    }

    public struct SouffleDamageJob : IJobChunk
    {
        [ReadOnly] public ComponentTypeHandle<MoveStats> _moveStatsHandle;
        public ComponentTypeHandle<DamageCount> _damageCountHandle;
        public NativeArray<SouffleDeFeuTemp> _souffleTempArray;
        //[ReadOnly] public EntityTypeHandle _entityHandle;
        public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
        {
            NativeArray<MoveStats> _moveStatsArray = chunk.GetNativeArray(_moveStatsHandle);
            NativeArray<DamageCount> _damageCountArray = chunk.GetNativeArray(_damageCountHandle);
            NativeArray<SouffleDeFeuTemp> _souffleTempArrayo = _souffleTempArray; 

            for (int i = 0; i < chunk.Count; i++)
            {
                for (int j = 0; j < _souffleTempArrayo.Length; j++)
                {
                    float _dist = Vector3.Distance(_moveStatsArray[i]._position, _souffleTempArrayo[j]._position);
                    if (_dist < _souffleTempArrayo[j]._radius)
                    {
                        DamageCount _damageCountTemp = _damageCountArray[j];
                        _damageCountTemp._damageFireCount += _souffleTempArrayo[j]._damage;
                        _damageCountArray[j] = _damageCountTemp;
                    }
                }
            }

        }
    }
}