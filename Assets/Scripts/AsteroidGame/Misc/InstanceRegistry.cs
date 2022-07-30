using System.Collections.Generic;
using AsteroidGame.PlayerScripts;
using AsteroidGame.Views;

namespace AsteroidGame.Misc
{
    public class InstanceRegistry
    {
        public readonly List<PlayerFacade> playerFacades = new List<PlayerFacade>();
        public readonly List<LivesView> playerLivesViews = new List<LivesView>();
        public readonly List<ScoreView> playerScoreViews = new List<ScoreView>();
    }
}
