using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

[UpdateBefore(typeof(TransformSystemGroup))]
public partial struct EnemyMoveSystem : ISystem
{

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float deltaTime = SystemAPI.Time.DeltaTime;

        foreach (var (transform, moveSpeed) in SystemAPI.Query<RefRW<LocalTransform>, EnemyComponent>())
        {
            transform.ValueRW.Position += transform.ValueRO.Right() * moveSpeed.Speed * deltaTime * moveSpeed.Direction;

        }

    }
}
