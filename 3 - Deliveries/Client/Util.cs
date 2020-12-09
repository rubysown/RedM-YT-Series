using CitizenFX.Core;
using CitizenFX.Core.Native;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Client
{
    class Util : BaseScript
    {
        static int subtitleTimeEnd;
        static string subtitleText = null;
        public static List<Vector3> worldMarkers = new List<Vector3> { };

        [Tick]
        private static async Task OnTick()
        {
            if (subtitleText != null)
            {
                DrawText(subtitleText, 23, 0.5F, 0.85F, 0.50F, 0.40F, 255, 255, 255, 255);

                if (subtitleTimeEnd <= API.GetGameTimer())
                {
                    subtitleText = null;
                }
            }
            
            foreach(Vector3 pos in worldMarkers)
            {
                Function.Call((Hash)0x2A32FAA57B937173, -1795314153, pos.X, pos.Y, pos.Z, 0, 0, 0, 0, 0, 0, 1.0, 1.0, 0.9, 255, 255, 0, 155, 0, 0, 2, 0, 0, 0, 0);
            }
        }

        public static void EnableBlipGPS(int blipId)
        {
            Function.Call((Hash)0x662D364ABF16DE2F, blipId, 0x900A4D0A);
        }

        public static int AddBlipForCoords(int sprite, Vector3 pos, bool enableGps = false)
        {
            int blipId = Function.Call<int>((Hash)0x554D9D53F696D002, (uint)sprite, pos.X, pos.Y, pos.Z);
            if (enableGps)
            {
                EnableBlipGPS(blipId);
            }
            return blipId;
        }

        public static int AddBlipForEntity(int entity, int spriteId, bool enableGps = true)
        {
            int blipId = Function.Call<int>((Hash)0x23F74C2FDA6E7C61, spriteId, entity);
            if (enableGps)
            {
                EnableBlipGPS(blipId);
            }
            return blipId;
        }

        public static void DisplayText(string text, int duration = 5000)
        {
            subtitleTimeEnd = API.GetGameTimer() + duration;
            subtitleText = text;
        }

        public static void DisplayHelpText(string text)
        {
            DrawText(text, 23, 0.5F, 0.9F, 0.50F, 0.40F, 125, 255, 255, 255);
        }

        public static void DrawText(string text, int fontId, float x, float y, float scaleX, float scaleY, int r, int g, int b, int a)
        {
            // Draw Text
            API.SetTextScale(scaleX, scaleY);
            API.SetTextColor(r, g, b, a);
            API.SetTextCentre(true);
            Function.Call((Hash)0xADA9255D, fontId); // Loads the font requested
            API.DisplayText(Function.Call<long>(Hash._CREATE_VAR_STRING, 10, "LITERAL_STRING", text), x, y);

            // Draw Backdrop
            float lineLength = (float) text.Length / 100 * 0.66F;
            DrawTexture("boot_flow", "selection_box_bg_1d", x, y, lineLength, 0.035F, 0F, 0, 0, 0, 200);
        }

        public static async void DrawTexture(string textureDict, string textureName, float x, float y, float width, float height, float rotation, int r, int g, int b, int a)
        {
            if (!API.HasStreamedTextureDictLoaded(textureDict))
            {
                API.RequestStreamedTextureDict(textureDict, false);
                while (!API.HasStreamedTextureDictLoaded(textureDict))
                {
                    Debug.WriteLine($"{textureDict}.{textureName} is waiting to load...");
                    await BaseScript.Delay(100);
                }
            }
            API.DrawSprite(textureDict, textureName, x, y + 0.015F, width, height, rotation, r, g, b, a, true);
        }

        public static async Task<bool> LoadModel(string model)
        {
            int hash = API.GetHashKey(model);
            if (!API.IsModelValid((uint)hash))
            {
                Debug.WriteLine($"Model {model} is not valid and could not be loaded.");
                return false;
            }
            if (!API.HasModelLoaded((uint)hash))
            {
                API.RequestModel((uint)hash, true);
                while (!API.HasModelLoaded((uint)hash))
                {
                    Debug.WriteLine($"Model {model} is waiting to load...");
                    await BaseScript.Delay(100);
                }
            }
            return true;
        }

        public static void WorldPositionFetcher()
        {
            int playerPedId = API.PlayerPedId();
            Vector3 coords = API.GetEntityCoords(playerPedId, true, true);
            float h = API.GetEntityHeading(playerPedId);
            Debug.WriteLine($"X: {coords.X} Y: {coords.Y} Z: {coords.Z} H: {h}");
        }
    }
}
