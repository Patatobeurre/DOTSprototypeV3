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
[UpdateAfter(typeof(GameSetting))]
public class HeroesController : SystemBase
{
    EntityQuery _heroesQuery;
    EndSimulationEntityCommandBufferSystem m_EndSimulationEcbSystem;
    BeginInitializationEntityCommandBufferSystem m_EntityCommandBufferSystem;
    EntityArchetype _souffleDeFeu;
    //EntityCommandBuffer.ParallelWriter _ecb;

    EntityQuery _query;
    EntityQuery _queryInput;

    //EntityArchetype _souffleDeFeu;
    bool _inputZ;

    bool Input_Mouse_0_GetUp;
    bool Input_Mouse_0_GetDown;
    bool Input_Mouse_0_Up;

    protected override void OnCreate()
    {
        //base.OnCreate();

        m_EndSimulationEcbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();

        m_EntityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();

        _heroesQuery = GetEntityQuery(typeof(HerosArchetype), ComponentType.ReadOnly<MoveStats>(), ComponentType.ReadOnly<SouffleDeFeuStats>(), ComponentType.ReadWrite<AbilityStats>(), ComponentType.ReadOnly<AbilityPerform>() );
        _souffleDeFeu = EntityManager.CreateArchetype(
        typeof(HashRegion),
        typeof(SouffleDeFeuTemp)
        );

        _query = GetEntityQuery(typeof(SouffleDeFeuTemp));
        _queryInput = GetEntityQuery(typeof(InputComponent));

    }
    protected override void OnUpdate()
    {
        //_ecb = m_EndSimulationEcbSystem.CreateCommandBuffer().AsParallelWriter();
        var _ecb = m_EntityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter();

        //this.GetEntityQuery(typeof(HerosArchetype), ComponentType.ReadOnly<MoveStats>(), ComponentType.ReadOnly<SouffleDeFeuTemp>(), ComponentType.ReadWrite<AbilityStats>());

        if (_queryInput.CalculateEntityCount() > 0)
        {
            var pouettt = _queryInput.GetSingletonEntity();
            if (Input_Mouse_0_Up && (Input_Mouse_0_Up != EntityManager.HasComponent<Input_Mouse_0_Up>(pouettt)))
            {
                Input_Mouse_0_GetDown = true;
                Input_Mouse_0_Up = false;
            }
            if (!Input_Mouse_0_Up && (!Input_Mouse_0_Up != EntityManager.HasComponent<Input_Mouse_0_Up>(pouettt)))
            {
                Input_Mouse_0_GetUp = true;
                Input_Mouse_0_Up = true;
            }
        }

        if (Input.GetKey(KeyCode.Z) == true)
        {
            Debug.Log("InputZZ!!");
            MoveJob _moveJob = new MoveJob();
            _moveJob._moveStatsHandle = GetComponentTypeHandle<MoveStats>(false);
            _moveJob._dt = Time.DeltaTime;
            _moveJob._direction = new float3(1, 0, 0);
            this.Dependency = _moveJob.ScheduleParallel(_heroesQuery, this.Dependency);
        }
        if (Input.GetKey(KeyCode.Q) == true)
        {
            Debug.Log("InputZZ!!");
            MoveJob _moveJob = new MoveJob();
            _moveJob._moveStatsHandle = GetComponentTypeHandle<MoveStats>(false);
            _moveJob._dt = Time.DeltaTime;
            _moveJob._direction = new float3(1, 0, 0);
            this.Dependency = _moveJob.ScheduleParallel(_heroesQuery, this.Dependency);
        }
        if (Input.GetKey(KeyCode.S) == true)
        {
            Debug.Log("InputZZ!!");
            MoveJob _moveJob = new MoveJob();
            _moveJob._moveStatsHandle = GetComponentTypeHandle<MoveStats>(false);
            _moveJob._dt = Time.DeltaTime;
            _moveJob._direction = new float3(1, 0, 0);
            this.Dependency = _moveJob.ScheduleParallel(_heroesQuery, this.Dependency);
        }
        if (Input.GetKey(KeyCode.D) == true)
        {
            Debug.Log("InputZZ!!");
            MoveJob _moveJob = new MoveJob();
            _moveJob._moveStatsHandle = GetComponentTypeHandle<MoveStats>(false);
            _moveJob._dt = Time.DeltaTime;
            _moveJob._direction = new float3(1, 0, 0);
            this.Dependency = _moveJob.ScheduleParallel(_heroesQuery, this.Dependency);
        }


        if (Input.GetKey(KeyCode.A) == true)
        {
            Debug.Log("pouetAAAAAAAAAAA");
            Debug.Log(_query.CalculateEntityCount() + "_souffleQuery");
            AbilityOneJob _abilityOneJob = new AbilityOneJob();
            _abilityOneJob._souffleHandle = GetComponentTypeHandle<SouffleDeFeuStats>(true);
            _abilityOneJob._abilityPerform = GetComponentTypeHandle<AbilityPerform>(true);
            _abilityOneJob._souffleDeFeu = _souffleDeFeu;
            _abilityOneJob._ecb = _ecb;
            _abilityOneJob._entityHandle = GetEntityTypeHandle();
            this.Dependency = _abilityOneJob.ScheduleParallel(_heroesQuery, this.Dependency);


            Dependency.Complete();
        }
        m_EntityCommandBufferSystem.AddJobHandleForProducer(Dependency);



        m_EntityCommandBufferSystem.AddJobHandleForProducer(this.Dependency);

        this.Dependency.Complete();
        
    }

    private struct MoveJob : IJobChunk
    {
        public ComponentTypeHandle<MoveStats> _moveStatsHandle;
        public ComponentTypeHandle<Translation> _translationHandle;
        public float3 _direction;
        public float _dt;
        void IJobChunk.Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
        {
            NativeArray<MoveStats> _moveStatsArray = chunk.GetNativeArray(_moveStatsHandle);
            NativeArray<Translation> _translationArray = chunk.GetNativeArray(_translationHandle);
            for (int i = 0; i < chunk.ChunkEntityCount; i++)
            {
                if (_moveStatsArray[i]._canMove == 1)
                {
                    _moveStatsArray[i] = new MoveStats
                    {
                        _canMove = _moveStatsArray[i]._canMove,
                        _newPosition = _translationArray[i].Value + (_direction * _dt *  _moveStatsArray[i]._moveSpeed),
                        _moveSpeed = _moveStatsArray[i]._moveSpeed
                    };
                }
            }
        }
    }

    private struct AbilityOneJob : IJobChunk
    {
        [ReadOnly] public ComponentTypeHandle<SouffleDeFeuStats> _souffleHandle;
        [ReadOnly] public ComponentTypeHandle<AbilityPerform> _abilityPerform;
        [ReadOnly] public EntityTypeHandle _entityHandle;
        public EntityArchetype _souffleDeFeu;
        public EntityCommandBuffer.ParallelWriter _ecb;

        void IJobChunk.Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
        {
            NativeArray<SouffleDeFeuStats> _souffleStatsArray = chunk.GetNativeArray(_souffleHandle);
            NativeArray<AbilityPerform> _abilityPerformArray = chunk.GetNativeArray(_abilityPerform);
            NativeArray<Entity> _entityArray = chunk.GetNativeArray(_entityHandle);
            
            for (int i = 0; i < chunk.ChunkEntityCount; i++)
            {
                //_ecb.CreateEntity(chunkIndex, _souffleDeFeu);
                if (_abilityPerformArray[i]._couldownSkillOne <= 0)
                {
                    SouffleDeFeuTemp _souffleDeFeuTemp = new SouffleDeFeuTemp
                    {
                        _damage = _souffleStatsArray[i]._damage,
                        _position = _souffleStatsArray[i]._position,
                        _radius = _souffleStatsArray[i]._radius,
                        _team = _souffleStatsArray[i]._team
                    };
                    Entity _entityTemp = _ecb.CreateEntity(chunkIndex, _souffleDeFeu);
                    int _newHashRegion = Mathf.RoundToInt(_souffleStatsArray[i]._position.x * GameSetting.MAP_WIDTH + _souffleStatsArray[i]._position.y);
                    HashRegion _hRegion = new HashRegion() { _hashRegion = 1 };
                    _ecb.SetComponent<SouffleDeFeuTemp>(chunkIndex, _entityTemp, _souffleDeFeuTemp);
                    _ecb.SetSharedComponent<HashRegion>(chunkIndex, _entityTemp, _hRegion);
                }
            }
        }
    }

    public struct JobJob : IJobChunk
    {
        public EntityCommandBuffer.ParallelWriter _ecb;
        [ReadOnly] public EntityTypeHandle _entityHandle;
        public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
        {
            NativeArray<Entity> _entityArray = chunk.GetNativeArray(_entityHandle);
            _ecb.Instantiate(chunkIndex, _entityArray[0]);
        }
    }


    /*   //[BurstCompile]
       private struct AbilitySouffleDeFeu : IJobChunk
       {
           public ComponentTypeHandle<SouffleDeFeuTemp> _souffleDeFeuHandle;
           public NativeArray<SouffleDeFeuTemp> _souffleDeFeuTempArray;
           //public SharedComponentTypeHandle<HashRegion> _hashRegionHandle;
           [ReadOnly] public EntityTypeHandle _entityHandle;
           public EntityCommandBuffer.ParallelWriter _ecb;

           void IJobChunk.Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
           {
               NativeArray<SouffleDeFeuTemp> _souffleArray = chunk.GetNativeArray(_souffleDeFeuHandle);
               NativeArray<Entity> _entityArray = chunk.GetNativeArray(_entityHandle);
               for (int i = 0; i < _souffleArray.Length; i++)
               {
                   int _newHashRegion = Mathf.RoundToInt(_souffleArray[i]._position.x * GameSetting.MAP_WIDTH + _souffleArray[i]._position.y);
                   HashRegion _hRegion = new HashRegion() { _hashRegion = _newHashRegion };
                   _ecb.SetSharedComponent<HashRegion>(chunkIndex, _entityArray[i], _hRegion);
                   _souffleDeFeuTempArray[i] = new SouffleDeFeuTemp();
                   _souffleArray[i] = _souffleDeFeuTempArray[i];
               }
           }
       }


       public struct SpawnSouffleDeFeu : IJobChunk
       {
           public int _entityCount;
           public int _pouet;
           public EntityArchetype _souffleDeFeu;
           public EntityCommandBuffer.ParallelWriter _ecb;
           void IJobChunk.Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
           {

               for (int i = 0; i < _entityCount; i++)
               {
                   _ecb.CreateEntity(_pouet, _souffleDeFeu);

               }
           }
       }*/
}