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

        private static bool eventSpawned = false;
        private static bool eventOnScene = false;
        private static bool eventDespawned = false;

        private static int gTimer;

        private static List<int> horses = new List<int> { };
        private static List<int> peds = new List<int> { };
        private static List<int> blips = new List<int> { };

        public Main()
        {
            API.RegisterCommand("givehorse", new Action(TestHorse), false);

            API.RegisterCommand("startscenario", new Action(TestStartScenario), false);
            API.RegisterCommand("stopscenario", new Action(TestStopScenario), false);

            API.RegisterCommand("giveweapon", new Action(TestWeapon), false);

            API.RegisterCommand("law", new Action(LawEvent), false);
            API.RegisterCommand("lawbye", new Action(Deregister), false);
            Tick += OnTick;
        }

        private async static void LawEvent()
        {
            eventSpawned = true;
            int playerPedId = API.PlayerPedId();
            Vector3 pos = GetPedCoords(playerPedId);

            Vector3 spawnPos = Vector3.Zero;
            float spawnHdg = 0F;
            int unk1 = 0;
            API.GetNthClosestVehicleNodeWithHeading(pos.X, pos.Y, pos.Z, 25, ref spawnPos, ref spawnHdg, ref unk1, 0, 0, 0);

            for(int i = 0; i < 5; i++)
            {
                Lawman spawnedLawman = await CreateLawman(spawnPos, spawnHdg);
                horses.Add(spawnedLawman.Horse);
                peds.Add(spawnedLawman.Ped);
                blips.Add(spawnedLawman.Blip);

                spawnPos += GetEntityForwardVector(spawnedLawman.Ped, 3);
                API.TaskGoToEntity(spawnedLawman.Ped, playerPedId, -1, 15F, 10F, 0F, 0);
            }
        }

        [Tick]
        private async Task OnTick()
        {
            if(API.IsControlJustPressed(API.PlayerPedId(), 0x24978A28))
            {
                TestHorse();
            }

            if(API.GetGameTimer() - gTimer >= 500)
            {
                gTimer = API.GetGameTimer();
                Loop();
            }
        }

        private async static void Loop()
        {
            if (eventSpawned)
            {
                int playerPedId = API.PlayerPedId();

                if(!eventDespawned && API.GetEntityHealth(playerPedId) == 0)
                {
                    eventDespawned = true;
                    await BaseScript.Delay(2000);
                    Deregister();
                }

                Vector3 playerPos = GetPedCoords(playerPedId);
                if (!eventOnScene)
                {
                    foreach (int ped in peds)
                    {
                        Vector3 pedPos = GetPedCoords(ped);
                        float distance = API.GetDistanceBetweenCoords(pedPos.X, pedPos.Y, pedPos.Z, playerPos.X, playerPos.Y, playerPos.Z, true);
                        if (distance < 20F)
                        {
                            eventOnScene = true;
                            foreach (int spawnedPed in peds)
                            {
                                API.TaskCombatPed(spawnedPed, playerPedId, 0, 16);
                            }
                        }
                    }
                }
            }
        }

        private static void Deregister()
        {
            eventOnScene = false;
            eventSpawned = false;

            foreach (int horse in horses)
            {
                int hRef = horse;
                API.DeleteEntity(ref hRef);
            }
            foreach (int ped in peds)
            {
                int pRef = ped;
                API.DeleteEntity(ref pRef);
            }
            foreach (int blip in blips)
            {
                int bRef = blip;
                API.RemoveBlip(ref bRef);
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
