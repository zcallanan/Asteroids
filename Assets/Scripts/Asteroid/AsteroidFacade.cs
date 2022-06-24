using System;
using UnityEngine;
using Zenject;

namespace Asteroid
{
    public class AsteroidFacade : MonoBehaviour, IPoolable<int, IMemoryPool>, IDisposable
    {
        public void OnDespawned()
        {
            throw new NotImplementedException();
        }

        public void OnSpawned(int size, IMemoryPool pool)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    
        public class Factory : PlaceholderFactory<int, AsteroidFacade>
        {
        }
    }
}
