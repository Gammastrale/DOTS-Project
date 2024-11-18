using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

[UpdateInGroup(typeof(LateSimulationSystemGroup), OrderLast = true)]
public partial struct IsDestroyManagmentSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<IsDestroying>();
    }

    public void OnUpdate(ref SystemState state)
    {
        var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.TempJob);
        foreach (var (tag, entity) in SystemAPI.Query<IsDestroying>().WithEntityAccess())
        {
            ecb.DestroyEntity(entity);
        }
        state.Dependency.Complete();
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}

public partial struct LifeTimeManagerSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<LifeTime>();
    }

    public void OnUpdate(ref SystemState state)
    {
        
        var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.TempJob);
        float deltaTime = SystemAPI.Time.DeltaTime;

        new LifeJob()
        {
            ecb = ecb,
            DeltaTime = deltaTime
        }.Schedule();
        state.Dependency.Complete();
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }


}

public partial struct HealthManagerSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<EnemyComponent>();
    }

    public void OnUpdate(ref SystemState state)
    {
        var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.TempJob);
        

        new HealthJob()
        {
            ecb = ecb,
            
        }.Schedule();
        state.Dependency.Complete();
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }

}



public partial struct LifeJob : IJobEntity
{

    public EntityCommandBuffer ecb;
    public float DeltaTime;
    public void Execute(Entity entity, ref LifeTime lifeTime)
    {
        lifeTime.Value -= DeltaTime;
        if (lifeTime.Value <= 0)
        {
            ecb.AddComponent<IsDestroying>(entity);
        }
    }
}

public partial struct HealthJob : IJobEntity
{
    public EntityCommandBuffer ecb;
    public void Execute(Entity entity, ref EnemyComponent Enemy)
    {

        if (Enemy.Health <= 0)
        {
            ecb.AddComponent<IsDestroying>(entity);
        }
    }
}