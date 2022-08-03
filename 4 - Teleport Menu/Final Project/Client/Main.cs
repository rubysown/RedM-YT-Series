using CitizenFX.Core;
using CitizenFX.Core.Native;
using System;
using System.Threading.Tasks;

namespace Client
{
    public class Main : BaseScript
    {
        public Main()
        {
            API.RegisterCommand("tmenu", new Action(TeleportMenu), false);
        }

        private void TeleportMenu()
        {
            Client.TeleportMenu.OpenMenu();
        }

        [EventHandler("onClientResourceStart")]
        private void ResourceStart()
        {
            Client.TeleportMenu.CreateMenu();
        }

        [Tick]
        private async Task OnTick()
        {
            if (API.IsControlJustReleased(0, 0x3C0A40F2)) TeleportMenu();
        }
    }
}
