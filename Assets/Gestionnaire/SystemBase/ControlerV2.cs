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

public class ControlerV2 : SystemBase
{
    protected override void OnUpdate()
    {
        float dT = Time.DeltaTime;
        bool _forwardKeyGet = Input.GetKey(KeyCode.Z);
        bool _backwardKeyGet = Input.GetKey(KeyCode.S);
        bool _leftKeyGet = Input.GetKey(KeyCode.Q);
        bool _rightKeyGet = Input.GetKey(KeyCode.D);

        bool _keySkillOneGet = Input.GetKey(KeyCode.H);
        bool _keySkillTwoGet = Input.GetKey(KeyCode.J);
        bool _keySkillThreeGet = Input.GetKey(KeyCode.K);
        bool _keySkillFourGet = Input.GetKey(KeyCode.L);
        bool _keySkillFiveGet = Input.GetKey(KeyCode.M);

        bool _leftTurnKeyGet = Input.GetKey(KeyCode.A);
        bool _rightTurnKeyGet = Input.GetKey(KeyCode.E);



        // move
        if (_forwardKeyGet)
        {
            Entities
                .WithName("isControled")
                .ForEach((ref MoveStats _moveStats) =>
                {
                    if (_moveStats._canMove >= 0)
                    {
                        _moveStats = new MoveStats()
                        {
                            _newPosition = _moveStats._newPosition + new Vector3(0,0,1)
                        };
                    }
                }).Schedule();
        }
        if (_backwardKeyGet)
        {
            Entities
                .WithName("isControled")
                .ForEach((ref MoveStats _moveStats) =>
                {
                    if (_moveStats._canMove >= 0)
                    {
                        _moveStats = new MoveStats()
                        {
                            _newPosition = _moveStats._newPosition + new Vector3(0, 0, -1)
                        };
                    }
                }).Schedule();
        }
        if (_leftKeyGet)
        {
            Entities
                .WithName("isControled")
                .ForEach((ref MoveStats _moveStats) =>
                {
                    if (_moveStats._canMove >= 0)
                    {
                        _moveStats = new MoveStats()
                        {
                            _newPosition = _moveStats._newPosition + new Vector3(-1, 0, 0)
                        };
                    }
                }).Schedule();
        }
        if (_rightKeyGet)
        {
            Entities
                .WithName("isControled")
                .ForEach((ref MoveStats _moveStats) =>
                {
                    if (_moveStats._canMove >= 0)
                    {
                        _moveStats = new MoveStats()
                        {
                            _newPosition = _moveStats._newPosition + new Vector3(1, 0, 0)
                        };
                    }
                }).Schedule();
        }

        // Ability
        if (_keySkillOneGet)
        {
            Entities
                .WithName("isControled")
                .ForEach((ref AbilityPerform _abilityPerform, in AbilityStats _abilityStats) =>
                {
                    if (_abilityStats._canUseAbility >= 0)
                    {
                        if (_abilityPerform._couldownSkillOne <= 0)
                        {
                            _abilityPerform = new AbilityPerform()
                            {
                                _SkillOne = _abilityStats._SkillOne,
                                _couldownSkillOne = _abilityStats._couldownSkillOne
                            };
                        }
                    }
                }).Schedule();
        }
        if (_keySkillTwoGet)
        {
            Entities
                .WithName("isControled")
                .ForEach((ref AbilityPerform _abilityPerform, in AbilityStats _abilityStats, in TransformAccessArray _transformAccessArray) =>
                {
                    if (_abilityStats._canUseAbility >= 0)
                    {
                        if (_abilityPerform._couldownSkillTwo <= 0)
                        {
                            _abilityPerform = new AbilityPerform()
                            {
                                _SkillTwo = _abilityStats._SkillTwo,
                                _couldownSkillTwo = _abilityStats._couldownSkillTwo
                            };
                        }
                    }
                }).Schedule();
        }
        if (_keySkillThreeGet)
        {
            Entities
                .WithName("isControled")
                .ForEach((ref AbilityPerform _abilityPerform, in AbilityStats _abilityStats) =>
                {
                    if (_abilityStats._canUseAbility >= 0)
                    {
                        if (_abilityPerform._couldownSkillThree <= 0)
                        {
                            _abilityPerform = new AbilityPerform()
                            {
                                _SkillThree = _abilityStats._SkillThree,
                                _couldownSkillThree = _abilityStats._couldownSkillThree
                            };
                        }
                    }
                }).Schedule();
        }
        if (_keySkillFourGet)
        {
            Entities
                .WithName("isControled")
                .ForEach((ref AbilityPerform _abilityPerform, in AbilityStats _abilityStats) =>
                {
                    if (_abilityStats._canUseAbility >= 0)
                    {
                        if (_abilityPerform._couldownSkillFour <= 0)
                        {
                            _abilityPerform = new AbilityPerform()
                            {
                                _SkillFour = _abilityStats._SkillFour,
                                _couldownSkillFour = _abilityStats._couldownSkillFour
                            };
                        }
                    }
                }).Schedule();
        }
        if (_keySkillFiveGet)
        {
            Entities
                .WithName("isControled")
                .ForEach((ref AbilityPerform _abilityPerform, in AbilityStats _abilityStats) =>
                {
                    if (_abilityStats._canUseAbility >= 0)
                    {
                        if (_abilityPerform._couldownSkillFour <= 0)
                        {
                            _abilityPerform = new AbilityPerform()
                            {
                                _SkillFive = _abilityStats._SkillFive,
                                _couldownSkillFive = _abilityStats._couldownSkillFive
                            };
                        }
                    }
                }).Schedule();
        }


    }
}
*/