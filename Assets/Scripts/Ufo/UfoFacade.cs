using System;
using UnityEngine;
using Zenject;

namespace Ufo
{
    public class UfoFacade : MonoBehaviour, IPoolable<IMemoryPool>, IDisposable
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
            throw new NotImplementedException();
        }

        public void OnSpawned(IMemoryPool pool)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
        
        public class Factory : PlaceholderFactory<UfoFacade>
        {
        }
    }
}
