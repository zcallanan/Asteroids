using StartMenuScripts.StartMenu;
using StartMenuScripts.Views;
using UnityEngine;
using Zenject;

namespace StartMenuScripts.Installers
{
    [CreateAssetMenu(fileName = "StartMenuSettings", menuName = "Installers/StartMenuSettings")]
    public class StartMenuSettingsInstaller : ScriptableObjectInstaller<StartMenuSettingsInstaller>
    {
        public StartMenuInstaller.Settings startMenuInstaller;
        public StartMenuHandler.Settings startMenuHandler;
        public StartMenuData.Settings startMenuData;
    
        public override void InstallBindings()
        {
            Container.BindInstance(startMenuInstaller).IfNotBound();
            Container.BindInstance(startMenuHandler).IfNotBound();
            Container.BindInstance(startMenuData).IfNotBound();
        }
    }
}