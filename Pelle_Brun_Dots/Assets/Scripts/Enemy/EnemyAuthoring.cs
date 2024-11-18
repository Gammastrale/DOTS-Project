using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class EnemyAuthoring : MonoBehaviour
{
    public float EnemySpeed;
    public float EnemyHealth;
    float EnemyDirection = 1;
   
    public class EnemyAuthoringBaker : Baker<EnemyAuthoring>
    {
        public override void Bake(EnemyAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new EnemyComponent { Speed = authoring.EnemySpeed, Health = authoring.EnemyHealth, Direction = authoring.EnemyDirection });

        }
    }
}
