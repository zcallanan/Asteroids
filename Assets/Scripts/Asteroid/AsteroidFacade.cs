using System;
using Misc;
using UnityEngine;
using Zenject;

namespace Asteroid
{
    public class AsteroidFacade : MonoBehaviour, IPoolable<int, IMemoryPool>, IDisposable
    {
        private GameLevelHandler _gameLevelHandler;
        private int _asteroidSize;
        

        [Inject]
        public void Construct(GameLevelHandler gameLevelHandler)
        {
            _gameLevelHandler = gameLevelHandler;
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (_asteroidSize == 2)
            {
                _gameLevelHandler.RegisterSmallDeathToDetermineNextLevel();
            }
        }

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
