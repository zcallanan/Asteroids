using System;
using UniRx;
using UnityEngine;
using Zenject;

namespace PlayerScripts
{
    public class PlayerFacade : MonoBehaviour
    {
        private Player _model;
    
        [Inject]
        public void Construct(Player player)
        {
            _model = player;
        }

        public MeshCollider MeshCollider => _model.MeshCollider;
        public MeshRenderer MeshRenderer => _model.MeshRenderer;

        public Vector3 Facing => _model.Facing;

        public Vector3 Position => _model.Position;

        public int CurrentLives => _model.CurrentLives;

        // public bool IsDead => _model.IsDead;
    }
    
    
}
