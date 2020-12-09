using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace Client
{
    public class Main : BaseScript
    {
        private int startBlipId;
        private int destinationBlipId;
        private int vehicleId;
        private int vehicleBlipId;
        private List<Vector3> jobDestinations = new List<Vector3> { };

        private bool canStartMission = false;
        private bool waitingToStart = false;
        private bool inProgress = false;
        private bool isEnding = false;
        private bool waitingForCarCleanup = false;

        private List<string> cartModels = new List<string> { "CART05", "CART07" };
        private Vector3 startLocation = new Vector3(-311F, 806F, 118F);
        private Vector3 vehStartLocation = new Vector3(-316F, 824F, 118F);
        private List<Vector3> baseDestinations = new List<Vector3> {
            new Vector3(-278.58F, 644.33F, 113.84F),
            new Vector3(-251.21F, 770.13F, 117.48F),
            new Vector3(-280.01F, 868.88F, 120.87F)
        };

        private readonly Random random = new Random();

        public Main()
        {
            API.RegisterCommand("worldpos", new Action(Util.WorldPositionFetcher), false);

            Util.worldMarkers.Add(startLocation);
            startBlipId = Util.AddBlipForCoords(2033377404, startLocation);
            API.SetBlipNameFromTextFile(startBlipId, "BLIP_SALOON");
            canStartMission = true;
        }

        [Tick]
        private async Task OnTick()
        {
            int playerPedId = API.PlayerPedId();
            Vector3 playerPos = API.GetEntityCoords(playerPedId, true, true);

            // Check for starting the mission
            if (canStartMission)
            {
                if(API.GetDistanceBetweenCoords(playerPos.X, playerPos.Y, playerPos.Z, startLocation.X, startLocation.Y, startLocation.Z, true) <= 2F)
                {
                    Util.DisplayHelpText("Press T to start a delivery mission");

                    // Accept input
                    if(API.IsControlJustPressed(API.PlayerPedId(), 0x9720FCEE))
                    {
                        StartDelivery();
                    }
                }
            }

            // Wait for Player to get in vehicle
            if (waitingToStart)
            {
                if(API.GetPlayersLastVehicle() != 0 && API.GetPlayersLastVehicle() == vehicleId)
                {
                    waitingToStart = false;
                    inProgress = true;
                    API.RemoveBlip(ref vehicleBlipId);
                    vehicleBlipId = Util.AddBlipForEntity(vehicleId, -1749618580, false);
                    destinationBlipId = Util.AddBlipForCoords(-308585968, jobDestinations[0], true);
                    Util.DisplayText("Proceed to the first delivery destination.");
                }
            }

            // Check progress
            if (inProgress)
            {
                VehicleChecks();

                // Check if at destination
                Vector3 vehiclePos = API.GetEntityCoords(vehicleId, true, true);
                if(API.GetDistanceBetweenCoords(vehiclePos.X, vehiclePos.Y, vehiclePos.Z, jobDestinations[0].X, jobDestinations[0].Y, jobDestinations[0].Z, true) <= 5F){
                    if(API.GetEntityVelocity(playerPedId, 0) == Vector3.Zero)
                    {
                        Util.DisplayHelpText("Press T to deliver");
                        if(API.IsControlJustPressed(API.PlayerPedId(), 0x9720FCEE))
                        {
                            API.RemoveBlip(ref destinationBlipId);
                            jobDestinations.RemoveAt(0);
                            if(jobDestinations.Count != 0)
                            {
                                destinationBlipId = Util.AddBlipForCoords(-308585968, jobDestinations[0], true);
                                Util.DisplayText("Proceed to the next delivery destination.");
                            }
                            else
                            {
                                inProgress = false;
                                isEnding = true;
                                destinationBlipId = Util.AddBlipForCoords(-308585968, vehStartLocation, true);
                                Util.DisplayText("Drop the wagon back off at the Saloon.");
                            }   
                        }
                    }
                    else
                    {
                        Util.DisplayText("Come to a complete stop to complete this delivery.", 500);
                    }
                }
            }
            
            // Check if ending
            if (isEnding)
            {
                VehicleChecks();

                // Check if at destination
                Vector3 vehiclePos = API.GetEntityCoords(vehicleId, true, true);
                if (API.GetDistanceBetweenCoords(vehiclePos.X, vehiclePos.Y, vehiclePos.Z, vehStartLocation.X, vehStartLocation.Y, vehStartLocation.Z, true) <= 2F)
                {
                    if (!API.IsPedInAnyVehicle(playerPedId, true))
                    {
                        isEnding = false;
                        CleanUp();
                        Util.DisplayText("Thank you for the quick work. Come by any time for another job.");
                        API.PlaySoundFrontend("REWARD_NEW_GUN", "HUD_REWARD_SOUNDSET", true, 0);
                        // API.MoneyIncrementCashBalance(500, 752097756);
                    }
                    else
                    {
                        if (API.GetEntityVelocity(playerPedId, 0) == Vector3.Zero)
                        {
                            // At destination
                            if (API.IsPedInAnyVehicle(playerPedId, true))
                            {
                                Util.DisplayHelpText("Exit the vehicle to complete the job");
                            }
                        }
                        else
                        {
                            Util.DisplayText("Come to a complete stop to complete this job.", 500);
                        }
                    }
                }
            }

            // Clean car up
            if (waitingForCarCleanup)
            {
                if (!API.IsEntityOnScreen(vehicleId) || API.IsEntityOccluded(vehicleId))
                {
                    waitingForCarCleanup = false;
                    API.DeleteEntity(ref vehicleId);
                    vehicleId = -1;
                }
            }
        }

        private void VehicleChecks()
        {
            if (!API.IsPedInAnyVehicle(API.PlayerPedId(), true))
            {
                Util.DisplayHelpText("Get back onto the wagon");
            }
            if (!HealthCheck())
            {
                waitingToStart = false;
                inProgress = false;
                isEnding = false;
                CleanUp();
            }
        }

        private bool HealthCheck()
        {
            return API.IsVehicleDriveable(vehicleId, 1, 1) || !API.IsPlayerDead(API.PlayerPedId());
        }

        private void CleanUp()
        {
            canStartMission = true;
            Util.worldMarkers.Add(startLocation);
            startBlipId = Util.AddBlipForCoords(2033377404, startLocation);
            API.SetBlipNameFromTextFile(startBlipId, "BLIP_SALOON");
            API.RemoveBlip(ref vehicleBlipId);
            API.RemoveBlip(ref destinationBlipId);
            API.SetVehicleAsNoLongerNeeded(ref vehicleId);
            waitingForCarCleanup = true;
        }

        private async void StartDelivery()
        {
            canStartMission = false;
            waitingToStart = true;
            Util.worldMarkers.Remove(startLocation);
            jobDestinations = new List<Vector3>(baseDestinations);
            API.RemoveBlip(ref startBlipId);
            API.PlaySoundFrontend("REWARD_NEW_GUN", "HUD_REWARD_SOUNDSET", true, 0);
            string cartModel = cartModels[random.Next(cartModels.Count)];
            await Util.LoadModel(cartModel);
            vehicleId = API.CreateVehicle((uint)API.GetHashKey(cartModel), vehStartLocation.X, vehStartLocation.Y, vehStartLocation.Z, 90F, true, true, false, true);
            API.SetEntityAsMissionEntity(vehicleId, true, true);
            vehicleBlipId = Util.AddBlipForEntity(vehicleId, -308585968);
            Util.DisplayText("Thanks for taking this job. The wagon is out back.");
        }
    }
}
