using System;
using UnityEngine;
using Zenject;

namespace AsteroidGame.Misc
{
    public class Thrust : MonoBehaviour, IPoolable<IMemoryPool>, IDisposable
    {
        public Transform Parent
        {
            get => transform.parent; 
            set => transform.parent = value;
        }

        public void SetPosition(Vector3 position)
        {
            transform.position = position;
        }

        public void SetFacing(Vector3 facing)
        {
            transform.forward = facing;
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
}
