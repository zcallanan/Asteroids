using System;
using UnityEngine;
using Zenject;

namespace Misc
{
    public class Explosion : MonoBehaviour, IPoolable<IMemoryPool>, IDisposable
    {
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
        
        public class Factory : PlaceholderFactory<Explosion>
        {
        }
    }
}
