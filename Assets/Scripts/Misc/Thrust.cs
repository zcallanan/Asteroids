using System;
using System.Collections;
using System.Collections.Generic;
using Misc;
using UnityEngine;
using Zenject;

public class Thrust : MonoBehaviour, IPoolable<IMemoryPool>, IDisposable
{
    public Transform Parent
    {
        get => transform.parent; 
        set => transform.parent = value;
    }

    public Vector3 Position
    {
        set => transform.position = value;
    }

    public Vector3 Facing
    {
        set => transform.forward = value;
    }
    
    private IMemoryPool _pool;

    public void OnDespawned()
    {
        _pool = null;
    }

    public void OnSpawned(IMemoryPool pool)
    {
        _pool = pool;
    }

    public void Dispose()
    {
        _pool?.Despawn(this);
    }
    
    public class Factory : PlaceholderFactory<Thrust>
    {
    }
}
