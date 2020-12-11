/*using System.Collections;
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

[UpdateInGroup(typeof(InitializationSystemGroup))]
public class ColliderV2 : SystemBase
{
    private EntityQuery query;
    public EntityQuery _souffleQuery;
     

    protected override void OnUpdate()
    {

        for (int i = 1; i < GameSetting.HASH_REGION_NUM; i++)
        {
            query.SetSharedComponentFilter(new HashRegion { _hashRegion = i });
            _souffleQuery.SetSharedComponentFilter(new HashRegion { _hashRegion = i });

            ColliderSouffleDeFeu _colliderSouffleDeFeuJob = new ColliderSouffleDeFeu();

            _colliderSouffleDeFeuJob._damageCountHandle = GetComponentTypeHandle<DamageCount>(false);
            _colliderSouffleDeFeuJob._positionHandle = GetComponentTypeHandle<MoveStats>(true);
            _colliderSouffleDeFeuJob._souffleDeFeuTempArray = _souffleQuery.ToComponentDataArray<SouffleDeFeuTemp>(Allocator.TempJob);

            this.Dependency = _colliderSouffleDeFeuJob.Schedule(query, this.Dependency);

        }
    }

    public struct ColliderSouffleDeFeu : IJobChunk
    {
        public ComponentTypeHandle<DamageCount> _damageCountHandle;
        [ReadOnly]
        public ComponentTypeHandle<MoveStats> _positionHandle;
        public NativeArray<SouffleDeFeuTemp> _souffleDeFeuTempArray;

        public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
        {
            NativeArray<DamageCount> _damageCountArray = chunk.GetNativeArray(_damageCountHandle);
            NativeArray<MoveStats> _moveStats = chunk.GetNativeArray(_positionHandle);

            for (int i = 0; i > _damageCountArray.Length; i++)
            {
                for (int j = 0; j > _souffleDeFeuTempArray.Length; j++)
                {
                    float _dist = Vector3.Distance(_moveStats[i]._position, _souffleDeFeuTempArray[j]._position);
                    if (_dist < _souffleDeFeuTempArray[j]._radius)
                    {
                        DamageCount _damageCountTemp = _damageCountArray[j];
                        _damageCountTemp._damageFireCount += _souffleDeFeuTempArray[j]._damage;
                        _damageCountArray[j] = _damageCountTemp;
                    }
                }
            }
        }
    }
}*/