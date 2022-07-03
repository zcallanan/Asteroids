using Misc;
using UniRx;
using UniRx.Triggers;
using Zenject;

namespace UfoScripts
{
    public class UfoCollisionHandler : IInitializable
    {
        private readonly Ufo _ufo;
        private readonly ScoreHandler _scoreHandler;

        public UfoCollisionHandler(
            Ufo ufo,
            ScoreHandler scoreHandler)
        {
            _ufo = ufo;
            _scoreHandler = scoreHandler;
        }

        public void Initialize()
        {
            OnCollisionUpdateScore();
        }

        private void OnCollisionUpdateScore()
        {
            _ufo.OnTriggerEnterAsObservable()
                .Subscribe(_ => _scoreHandler.UpdateScore(_ufo.Size))
                .AddTo(_ufo.gameObject);
        }
    }
}
