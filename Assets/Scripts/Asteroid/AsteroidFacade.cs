using System;
using Misc;
using UnityEngine;
using Zenject;

namespace Asteroid
{
    public class AsteroidFacade : MonoBehaviour, IPoolable<int, IMemoryPool>, IDisposable
    {
        private GameLevelHandler _gameLevelHandler;
        private ScoreHandler _scoreHandler;
        private int _asteroidSize;
        

        [Inject]
        public void Construct(
            GameLevelHandler gameLevelHandler,
            ScoreHandler scoreHandler)
        {
            _gameLevelHandler = gameLevelHandler;
            _scoreHandler = scoreHandler;
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (_asteroidSize == 0)
            {
                _scoreHandler.UpdateScore(ScoreTypes.SmallAsteroid);
            }
            else if (_asteroidSize == 1)
            {
                _scoreHandler.UpdateScore(ScoreTypes.MediumAsteroid);
            }
            else if (_asteroidSize == 2)
            {
                _scoreHandler.UpdateScore(ScoreTypes.LargeAsteroid);
                
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
