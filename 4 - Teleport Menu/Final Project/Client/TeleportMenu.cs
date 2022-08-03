using CitizenFX.Core;
using CitizenFX.Core.Native;
using MenuAPI;
using System.Collections.Generic;

namespace Client
{
    class TeleportMenu
    {
        public static void CreateMenu()
        {
            Menu = new Menu("Teleport Menu");
            MenuController.MenuToggleKey = 0;
            MenuController.AddMenu(Menu);

            foreach(KeyValuePair<string, Dictionary<string, Vector3>> entry in Locations)
            {
                CreateSubMenu(entry.Key, entry.Value);
            }
        }

        public static void OpenMenu()
        {
            if (Menu == default(Menu)) CreateMenu();
            Menu.OpenMenu();
        }

        private static Menu CreateSubMenu(string name, Dictionary<string, Vector3> locations)
        {
            Menu submenu = new Menu("Teleport Menu", name);
            MenuController.AddSubmenu(Menu, submenu);
            MenuItem menuButton = new MenuItem(name) { RightIcon = MenuItem.Icon.ARROW_RIGHT };
            Menu.AddMenuItem(menuButton);
            MenuController.BindMenuItem(Menu, submenu, menuButton);

            foreach(KeyValuePair<string, Vector3> entry in locations)
            {
                submenu.AddMenuItem(new MenuItem(entry.Key));
            }

            submenu.OnItemSelect += (_menu, _item, _index) =>
            {
                int playerPedId = API.PlayerPedId();
                Vector3 target = locations[_item.Text];
                int targetId;
                if (API.IsPedInAnyVehicle(playerPedId, true))
                {
                    targetId = API.GetLastDrivenVehicle();
                } else
                {
                    targetId = API.IsPedOnFoot(playerPedId) ? playerPedId : API.GetMount(playerPedId);
                }
                API.SetEntityCoords(targetId, target.X, target.Y, target.Z, true, true, true, false);
                submenu.CloseMenu();
            };

            return submenu;
        }

        private static Menu Menu { get; set; }
        public static Dictionary<string, Dictionary<string, Vector3>> Locations = new Dictionary<string, Dictionary<string, Vector3>>() {
            {"Northern Zones", new Dictionary<string, Vector3>(){
                    {"Grizzlies Train Station", new Vector3(571.46f, 1709.60f, 187.55f) },
                    {"Barrow Lagoon", new Vector3(-1022.5f, 1682.60f, 242.16f) },
                    {"Mount Hagen", new Vector3(-1504.35f, 1246.98f, 313.68f) },
                    {"Frozen Couple", new Vector3(-1596.30f, 1838.17f, 299.44f) }
                }
            },
            {"Northeastern Zones", new Dictionary<string, Vector3>(){
                    {"Van Horn", new Vector3(2983.45f, 430.15f, 51.17f) },
                    {"Van Horn Lighthouse", new Vector3(3034.83f, 434.75f, 63.76f) },
                    {"Crashed Boat", new Vector3(3587.61f, 1412.36f, 41.64f) },
                    {"Annesburg", new Vector3(2904.36f, 1248.80f, 44.87f) },
                    {"Annesburg Mines", new Vector3(2811.51f, 1350.15f, 71.40f) },
                    {"Willard's Rest / Waterfalls", new Vector3(2962.59f, 2206, 165.28f) },
                    {"Doverhill (Tesla Coil)", new Vector3(2528.70f, 2289.21f, 176.55f) },
                    {"Ambarino Sign", new Vector3(1977, 1782.97f, 191.95f) },
                    {"Veteran's Homestead", new Vector3(1695.53f, 1517.75f, 146.75f) },
                    {"Beaver Hollow", new Vector3(2401.96f, 1237.94f, 102.53f) },
                    {"Colter", new Vector3(1342.37f, 2429.93f, 470.78f) }
                }
            },
            {"Eastern Zones", new Dictionary<string, Vector3>(){
                    {"Emerald Ranch", new Vector3(1418.27f, 316.26f, 88.48f) },
                    {"Emerald Ranch Fence", new Vector3(1417.81f, 268, 89.61f) },
                    {"UFO Cult House", new Vector3(1469.52f, 803.95f, 100.25f) },
                    {"Rhodes", new Vector3(1232.2f, -1251, 73.67f) },
                    {"Rhodes Sheriff Station", new Vector3(1359.57f, -1301.45f, 77.76f) },
                    {"Braithwaite Manor", new Vector3(1010.88f, -1741.42f, 46.57f) },
                    {"Caliga Hall", new Vector3(1748.7f, -1373, 44) },
                    {"Lagras", new Vector3(2105.41f, -682.16f, 42.26f) },
                    {"Clemens Point", new Vector3(843, -1029.49f, 51) },
                    {"Shady Belle", new Vector3(1763.23f, -1772.33f, 50.68f) }
                }
            },
            {"Northwestern Zones", new Dictionary<string, Vector3>(){
                    {"Monto's Rest", new Vector3(-1389.25f, -235, 98.85f) },
                    {"Fort Wallace", new Vector3(361.91f, 1461.29f, 179.19f) },
                    {"Valentine", new Vector3(-231.33f, 703.26f, 113.74f) },
                    {"Valentine Sheriff Station", new Vector3(-278.21f, 807, 119.38f) },
                    {"Strawberry", new Vector3(-1731.42f, -412.89f, 154.86f) },
                    {"Strawberry Sheriff Station", new Vector3(-1809.93f, -349.49f, 164.65f) },
                    {"Blackwater", new Vector3(-746.55f, -1321.26f, 42.99f) },
                    {"Wapiti Reservation", new Vector3(448.93f, 2239.86f, 248.44f) },
                    {"Beecher's Hope", new Vector3(-1641.79f, -1399.67f, 82.80f) },
                    {"Horseshoe Overlook", new Vector3(69.79f, 109.28f, 103.53f) },
                }
            },
            {"Southwestern Zones", new Dictionary<string, Vector3>(){
                    {"Tanner's Reach", new Vector3(-2370.17f, -1595.12f, 154) },
                    {"Swadlass Point", new Vector3(-2573.16f, -1371.53f, 149.71f) },
                    {"Upper Montana River", new Vector3(-2003.78f, -1058, 77) },
                    {"Manzanita Post", new Vector3(-1962.67f, -1614.49f, 116) },
                    {"Native Attack Site", new Vector3(-2037.57f, -1917, 110) },
                    {"Macfarlane's Ranch", new Vector3(-2333, -2350.93f, 63.20f) },
                    {"Roots Chapel", new Vector3(-3322.75f, -2833.4f, -6.23f) },
                    {"Armadillo", new Vector3(-3665.94f, -2612.44f, -14) },
                    {"Benedict Point", new Vector3(-5245.89f, -3470.67f, -22) },
                    {"Tumbleweed", new Vector3(-5517.37f, -2936.82f, -2.21f) },
                    {"Rathskeller Fork", new Vector3(-5165, -2143.5f, 11.77f) },
                    {"Ridgewood Farm", new Vector3(-4823.54f, -2711.38f, -13.74f) },
                    {"Mercer Station", new Vector3(-4352.89f, -3052.94f, -11.1f) },
                    {"Fort Mercer", new Vector3(-4217.85f, -3514.63f, 36.98f) },
                    {"Gaptooth Breach", new Vector3(-6000.76f, -3319.31f, -21.72f) }
                }
            },
            {"Saint Denis", new Dictionary<string, Vector3>(){
                    {"Entrance", new Vector3(2209.55f, -1346.31f, 45.27f) },
                    {"Police HQ", new Vector3(2519.43f, -1309.52f, 48.98f) },
                    {"Cornwall Freight", new Vector3(2327.49f, -1502.42f, 46.15f) },
                    {"Paperboy", new Vector3(2683.75f, -1401.46f, 46.36f) },
                    {"Post Office", new Vector3(2747.49f, -1403.77f, 46.19f) },
                    {"Schiffer Brewery", new Vector3(2626.12f, -1219.23f, 53.24f) },
                    {"Holdern Barber", new Vector3(2663.26f, -1182.40f, 53.18f) },
                    {"Theatre Raleur", new Vector3(2531.46f, -1271.88f, 49.19f) },
                    {"Park Gazebo (Dominos)", new Vector3(2520, -1250, 50) },
                    {"Milliners (Clothes)", new Vector3(2552, -1176.57f, 53.31f) },
                    {"Dr. Joseph R Barnes MD (Doctor)", new Vector3(2725, -1240.16f, 49.92f) },
                    {"Kuo Chao and Co (Guns)", new Vector3(2724, -1283.32f, 49.40f) },
                    {"Horner & Co General Store", new Vector3(2819.6f, -1311.31f, 46.73f) },
                    {"Trapper", new Vector3(2831.33f, -1227.61f, 47.64f) },
                    {"Fence", new Vector3(2849.29f, -1203, 47.69f) },
                    {"Doyle's Tavern", new Vector3(2792.43f, -1176, 47.94f) },
                    {"Marcel Beliveau (Photography)", new Vector3(2734.56f, -1111.32f, 48.88f) },
                    {"Fontana Theatre", new Vector3(2683, -1365.80f, 47.46f) },
                    {"Sisika Penitentiary", new Vector3(3348.68f, -638, 44.96f) },
                    {"Sisika Island", new Vector3(3136.86f, -767.73f, 44.81f) }
                }
            },
            {"Remote Locations", new Dictionary<string, Vector3>(){
                    {"Guarma Beach", new Vector3(1997.57f, -4499.80f, 41.77f) },
                    {"Guarma Fort", new Vector3(999.91f, -6749.73f, 63.12f) },
                    {"Mexico", new Vector3(2174.26f, -3382.99f, 61.21f) }
                }
            },
            {"Unfinished Locations", new Dictionary<string, Vector3>(){
                    {"Greenery", new Vector3(285.46f, 3279.31f, 284.63f) },
                    {"Large Rockbed", new Vector3(729.11f, 3606.84f, 254) },
                    {"Gaptooth Breach Mineshaft", new Vector3(-5980.29f, -3119.83f, -31) },
                    {"Edge of the Universe", new Vector3(898.13f, 4093, 286.84f) },
                    {"Sand Mountain", new Vector3(4260.69f, 3583.58f, 45.71f) },
                    {"Underwater Trees", new Vector3(4456.24f, 2491.35f, 40.23f) },
                    {"Beach", new Vector3(4235.68f, 2051, 43.71f) },
                    {"Floating Forest", new Vector3(5650.82f, 423.69f, 161) },
                }
            }
        };
    }
}
