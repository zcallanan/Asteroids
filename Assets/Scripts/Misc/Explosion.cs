using System;
using UnityEngine;
using Zenject;

namespace Misc
{
    public class Explosion : MonoBehaviour, IPoolable<IMemoryPool>, IDisposable
    {
        private IMemoryPool _pool;

        // [Inject]
        // public void Construct()
        // {
        //     
        // }

        public void OnDespawned()
        {
            _pool = null;
        }

        public void OnSpawned(IMemoryPool pool)
        {
            _pool = pool;
        }
        
        public class Factory : PlaceholderFactory<Explosion>
        {
        }

        public void Dispose()
        {
            _pool?.Despawn(this);
        }
    }
}
