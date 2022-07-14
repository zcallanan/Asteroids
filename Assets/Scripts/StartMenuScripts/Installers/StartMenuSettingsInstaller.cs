using StartMenuScripts.StartMenu;
using UnityEngine;
using Zenject;

namespace StartMenuScripts.Installers
{
    [CreateAssetMenu(fileName = "StartMenuSettings", menuName = "Installers/StartMenuSettings")]
    public class StartMenuSettingsInstaller : ScriptableObjectInstaller<StartMenuSettingsInstaller>
    {
        public StartMenuHandler.Settings startMenuHandler;
    
        public override void InstallBindings()
        {
            Container.BindInstance(startMenuHandler).IfNotBound();
        }
    }
}