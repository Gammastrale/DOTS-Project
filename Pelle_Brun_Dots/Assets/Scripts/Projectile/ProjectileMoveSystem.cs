using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using Unity.Physics;
using Unity.Collections;
using Unity.Mathematics;
using static UnityEngine.EventSystems.EventTrigger;


[UpdateBefore(typeof(TransformSystemGroup))]
public partial struct ProjectileMoveSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {


        PhysicsWorldSingleton physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
        float deltaTime = SystemAPI.Time.DeltaTime;

        foreach (var (transform, ProjectileStats) in SystemAPI.Query<RefRW<LocalTransform>, ProjectileComponents>())
        {
            transform.ValueRW.Position += transform.ValueRO.Up() * ProjectileStats.Speed * deltaTime;

            NativeList<ColliderCastHit> hits = new NativeList<ColliderCastHit>(Allocator.Temp);
            float3 point1 = new float3(transform.ValueRW.Position - transform.ValueRO.Right() * 0.15f);
            float3 point2 = new float3(transform.ValueRW.Position + transform.ValueRO.Right() * 0.15f);

            physicsWorld.CapsuleCastAll(point1, point2, ProjectileStats.Size / 2, float3.zero, 1f, ref hits, new CollisionFilter
            {
                BelongsTo = (uint)CollisionLayer.Default,
                CollidesWith = (uint)CollisionLayer.Enemy

            });

            if (hits.Length > 0)
            {
                for (int i = 0; i < hits.Length; i++)
                {
                    Entity hitEntity = hits[i].Entity;
                    if (state.EntityManager.HasComponent<EnemyComponent>(hitEntity))
                    {
                        
                        EnemyComponent enemyComponent = state.EntityManager.GetComponentData<EnemyComponent>(hitEntity);
                        enemyComponent.Health -= ProjectileStats.Damage;

                        state.EntityManager.SetComponentData(hitEntity, enemyComponent);
                        var ecb = new EntityCommandBuffer(Allocator.TempJob);
                        if (enemyComponent.Health <= 0)
                        {
                            
                            ecb.DestroyEntity(hitEntity);
                        }

                    }
                }
            }
        }

    }
}
