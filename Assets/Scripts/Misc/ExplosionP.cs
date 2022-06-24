using System;
using UnityEngine;
using Zenject;

namespace Misc
{
    public class ExplosionP : MonoBehaviour, IPoolable<IMemoryPool>, IDisposable
    {
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        public void OnDespawned()
        {
            throw new System.NotImplementedException();
        }

        public void OnSpawned(IMemoryPool pool)
        {
            throw new System.NotImplementedException();
        }
        
        public class Factory : PlaceholderFactory<ExplosionP>
        {
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
