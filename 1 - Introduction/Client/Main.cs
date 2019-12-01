using CitizenFX.Core;
using CitizenFX.Core.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Client.Util;

namespace Client
{
    public class Main:BaseScript
    {
        public Main()
        {
            API.RegisterCommand("givehorse", new Action(TestHorse), false);

            API.RegisterCommand("startscenario", new Action(TestStartScenario), false);
            API.RegisterCommand("stopscenario", new Action(TestStopScenario), false);

            API.RegisterCommand("giveweapon", new Action(TestWeapon), false);
            Tick += OnTick;
        }

        [Tick]
        private async Task OnTick()
        {
            if(API.IsControlJustPressed(API.PlayerPedId(), 0x24978A28))
            {
                TestHorse();
            }
        }

        private void TestStartScenario()
        {
            Function.Call(Hash._TASK_START_SCENARIO_IN_PLACE, API.PlayerPedId(), API.GetHashKey("WORLD_HUMAN_SMOKE"), -1, true, false, false, false);
        }

        private void TestStopScenario()
        {
            Function.Call(Hash.CLEAR_PED_TASKS, API.PlayerPedId());
        }

        private void TestWeapon()
        {
            int playerPedId = API.PlayerPedId();
            GiveWeaponToPed(playerPedId, WeaponHash.RepeaterCarbine);
            GiveWeaponToPed(playerPedId, WeaponHash.KitBinoculars);
            GiveWeaponToPed(playerPedId, WeaponHash.Lasso);
        }

        private void GiveWeaponToPed(int ped, WeaponHash weapon)
        {
            Function.Call((Hash)0x5E3BDDBCB83F3D84, ped, (uint)weapon, 100, true, true, 1, false, 0.5F, 1.0F, false, 0);
        }

        private async void TestHorse()
        {
            int playerPedId = API.PlayerPedId();
            int hash = GenHash("coach2");
            if (!await LoadModel(hash))
            {
                return;
            }
            Vector3 pos = Function.Call<Vector3>(Hash.GET_ENTITY_COORDS, playerPedId);
            Vector3 forwardPos = Function.Call<Vector3>(Hash.GET_ENTITY_FORWARD_VECTOR, playerPedId);
            pos += (forwardPos * 5);
            float hdg = Function.Call<float>(Hash.GET_ENTITY_HEADING, playerPedId);
            int veh = Function.Call<int>(Hash.CREATE_VEHICLE, hash, pos.X, pos.Y, pos.Z, hdg, 0, 0, 0, false, false, 0, 0);
            Function.Call(Hash.SET_ENTITY_AS_MISSION_ENTITY, veh, true, true);
        }
    }
}
