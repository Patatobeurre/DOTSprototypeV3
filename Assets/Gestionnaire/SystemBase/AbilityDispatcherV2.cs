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

public class AbilityDispatcherV2 : SystemBase
{
    EndSimulationEntityCommandBufferSystem m_EndSimulationEcbSystem;
    private EntityQuery _query;
    private EntityQuery _abilityQuery;
    private EntityQuery _souffle_Query;

    protected override void OnCreate()
    {
        base.OnCreate();

        // Find the ECB system once and store it for later usage
        m_EndSimulationEcbSystem = World
            .GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();

        _query = GetEntityQuery(ComponentType.ReadOnly<AbilityPerform>(),
        typeof(HashRegion));
        _query.SetChangedVersionFilter(typeof(AbilityPerform));  // --> normalement cela fonctionne, on aura que les unités qui sont contrôlés et qui ont fait quelques chose

        _abilityQuery = GetEntityQuery(typeof(SouffleDeFeuTemp));
    }


    protected override void OnUpdate()
    { 
        _abilityQuery.SetSharedComponentFilter(new HashRegion { _hashRegion = 0 });  // On prend les ability qui sont inactives (les entitybilitys)
        //_abilityQuery.ToComponentDataArray<SouffleDeFeuTemp>(Allocator.Temp);  // On les mets dans un array

        var ecb = m_EndSimulationEcbSystem.CreateCommandBuffer().AsParallelWriter();

        Entities.WithChangeFilter<AbilityPerform>()
            .ForEach((Entity entity, int entityInQueryIndex, ref SouffleDeFeuTemp _souffleDeFeuTemp, ref HashRegion _hashRegion, in SouffleDeFeuStats _souffleDeFeuStats, in MoveStats _moveStats, in AbilityPerform _abilityPerform) =>
            {
                if (_abilityPerform._SkillOne == 1 || _abilityPerform._SkillTwo == 1) // on active l'Entitability
                {
                    _souffleDeFeuTemp._damage = _souffleDeFeuStats._damage;
                    _souffleDeFeuTemp._team = _souffleDeFeuStats._team;
                    _souffleDeFeuTemp._radius = _souffleDeFeuStats._radius;
                    _souffleDeFeuTemp._position = _moveStats._position;
                }

                int _newHashRegion = Mathf.RoundToInt(_moveStats._newPosition.x * GameSetting.MAP_WIDTH + _moveStats._newPosition.y);

                HashRegion _hRegion = new HashRegion() { _hashRegion = _newHashRegion };
                ecb.SetSharedComponent<HashRegion>(entityInQueryIndex, entity, _hRegion);

            }).ScheduleParallel();

        // Make sure that the ECB system knows about our job
        m_EndSimulationEcbSystem.AddJobHandleForProducer(this.Dependency);
    }

    struct AbilityDispatcher : IJobChunk
    {

        void IJobChunk.Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
        {
            
        }
    }
}
*/