using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;


[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateBefore(typeof(TransformSystemGroup))]
public partial struct FireProjectileSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.TempJob);
        foreach (var (projectilePrefab, transform, lifeTime) in SystemAPI.Query<ProjectilePrefab, LocalTransform,ProjectileLifeTime>().WithAll<FireProjectileTag>())
        {
            var newProjectile = ecb.Instantiate(projectilePrefab.Value);
            var projectileTransform = LocalTransform.FromPositionRotation(transform.Position+new Unity.Mathematics.float3(0, 0.5f, 0), transform.Rotation);
            ecb.SetComponent(newProjectile, projectileTransform);
            ecb.AddComponent(newProjectile, new LifeTime { Value = lifeTime.Value });

        }
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}
