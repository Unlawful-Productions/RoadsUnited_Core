using ColossalFramework.IO;
using ColossalFramework.PlatformServices;
using ICities;
using System.IO;
using UnityEngine;
using ColossalFramework.UI;
using ColossalFramework;
using ColossalFramework.Plugins;

namespace RoadsUnited_Core
{
    public class ModLoader : LoadingExtensionBase
    {
		//GameObject hookGo;
		//Hook4 hook;

        public static Configuration config;

        public static readonly string configFile = "RoadsUnitedCorePlusConfig.xml";

        public static string getModPath()
        {
            string text = ".";
			PublishedFileId[] subscribedItems = PlatformService.workshop.GetSubscribedItems();
            for (int i = 0; i < subscribedItems.Length; i++)
            {
                PublishedFileId id = subscribedItems[i];
				if(id.AsUInt64 == 726005715)
                {
					text = PlatformService.workshop.GetSubscribedItemPath(id);
                    Debug.Log("Roads United Core +: Workshop path: " + text);
                    break;
                }
            }
            string text2 = DataLocation.modsPath + "/RoadsUnited_Core";
            Debug.Log("Roads United Core +: " + text2);
            string result;
            if (Directory.Exists(text2))
            {
                Debug.Log("Roads United Core +: Local path exists, looking for assets here: " + text2);
                result = text2;
            }
            else
            {
                result = text;
            }
            return result;
        }

		public static string modPath=getModPath();
		public static string currentTexturesPath_default="None";
		public static string APRMaps_Path=Path.Combine(modPath,"APRMaps");
		public static string Tex="";
		public static string Tex2A="";
		public static string Tex2B="";
		public static string Tex2C="";

		public RoadsUnited_Core textureManager;
        
        public override void OnCreated(ILoading loading)
        {
            base.OnCreated(loading);
            config = Configuration.Deserialize(configFile);
            if (config == null)
            {
                config = new Configuration();
            }
            SaveConfig();
        }

        public override void OnLevelLoaded(LoadMode mode)
        {
            base.OnLevelLoaded(mode);
            string modPath = getModPath();
			//hookGo = new GameObject("RoadsUnited_Core hook");
			//hook = hookGo.AddComponent<Hook4>();
			if (ModLoader.config.create_vanilla_dictionary == true)
			{
                bool isEmpty;
                using (var dictionaryEnum = RoadsUnited_Core.vanillaPrefabProperties.GetEnumerator())
                    isEmpty = !dictionaryEnum.MoveNext();
                if (isEmpty)
                {
                    RoadsUnited_Core.CreateVanillaDictionary();
                }
			}


            if (ModLoader.config.use_custom_textures == true)
            {
                RoadsUnited_Core.ReplaceNetTextures();
          //      RoadsUnited_Core.ReplaceNetTexturesPNG();

            }

            #region.RoadColorChanger

            if (ModLoader.config.use_custom_colors == true)
            {
                RoadColorChanger.ChangeColor(ModLoader.config.small_road_brightness, "Basic Road", ModLoader.modPath);
                RoadColorChanger.ChangeColor(ModLoader.config.small_road_brightness, "Basic Road Elevated", ModLoader.modPath);
                RoadColorChanger.ChangeColor(ModLoader.config.small_road_brightness, "Basic Road Bridge", ModLoader.modPath);

                RoadColorChanger.ChangeColor(ModLoader.config.small_road_brightness, "Basic Road Bicycle", ModLoader.modPath);
                RoadColorChanger.ChangeColor(ModLoader.config.small_road_brightness, "Basic Road Elevated Bike", ModLoader.modPath);
                RoadColorChanger.ChangeColor(ModLoader.config.small_road_brightness, "Basic Road Bridge Bike", ModLoader.modPath);

                RoadColorChanger.ChangeColor(ModLoader.config.small_road_brightness, "Basic Road Tram", ModLoader.modPath);
                RoadColorChanger.ChangeColor(ModLoader.config.small_road_brightness, "Basic Road Elevated Tram", ModLoader.modPath);
                RoadColorChanger.ChangeColor(ModLoader.config.small_road_brightness, "Basic Road Bridge Tram", ModLoader.modPath);
                RoadColorChanger.ChangeColor(ModLoader.config.small_road_brightness, "Basic Road Slope Tram", ModLoader.modPath);
                RoadColorChanger.ChangeColor(ModLoader.config.small_road_brightness, "Basic Road Tunnel Tram", ModLoader.modPath);



                RoadColorChanger.ChangeColor(ModLoader.config.small_road_decoration, "Basic Road Decoration Grass", ModLoader.modPath);
                RoadColorChanger.ChangeColor(ModLoader.config.small_road_decoration, "Basic Road Decoration Trees", ModLoader.modPath);


                RoadColorChanger.ChangeColor(ModLoader.config.small_road_brightness, "Oneway Road", ModLoader.modPath);
                RoadColorChanger.ChangeColor(ModLoader.config.small_road_brightness, "Oneway Road Elevated", ModLoader.modPath);
                RoadColorChanger.ChangeColor(ModLoader.config.small_road_brightness, "Oneway Road Bridge", ModLoader.modPath);
                RoadColorChanger.ChangeColor(ModLoader.config.small_road_decoration, "Oneway Road Decoration Grass", ModLoader.modPath);
                RoadColorChanger.ChangeColor(ModLoader.config.small_road_decoration, "Oneway Road Decoration Trees", ModLoader.modPath);

                RoadColorChanger.ChangeColor(ModLoader.config.small_road_brightness, "Oneway Road Tram", ModLoader.modPath);
                RoadColorChanger.ChangeColor(ModLoader.config.small_road_brightness, "Oneway Road Elevated Tram", ModLoader.modPath);
                RoadColorChanger.ChangeColor(ModLoader.config.small_road_brightness, "Oneway Road Bridge Tram", ModLoader.modPath);
                RoadColorChanger.ChangeColor(ModLoader.config.small_road_brightness, "Oneway Road Slope Tram", ModLoader.modPath);
                RoadColorChanger.ChangeColor(ModLoader.config.small_road_brightness, "Oneway Road Tunnel Tram", ModLoader.modPath);

                RoadColorChanger.ChangeColor(ModLoader.config.medium_road_brightness, "Medium Road", ModLoader.modPath);
                RoadColorChanger.ChangeColor(ModLoader.config.medium_road_brightness, "Medium Road Elevated", ModLoader.modPath);
                RoadColorChanger.ChangeColor(ModLoader.config.medium_road_brightness, "Medium Road Bridge", ModLoader.modPath);

                RoadColorChanger.ChangeColor(ModLoader.config.medium_road_brightness, "Medium Road Bicycle", ModLoader.modPath);
                RoadColorChanger.ChangeColor(ModLoader.config.medium_road_brightness, "Medium Road Elevated Bike", ModLoader.modPath);
                RoadColorChanger.ChangeColor(ModLoader.config.medium_road_brightness, "Medium Road Bridge Bike", ModLoader.modPath);

                RoadColorChanger.ChangeColor(ModLoader.config.medium_road_brightness, "Medium Road Tram", ModLoader.modPath);
                RoadColorChanger.ChangeColor(ModLoader.config.medium_road_brightness, "Medium Road Elevated Tram", ModLoader.modPath);
                RoadColorChanger.ChangeColor(ModLoader.config.medium_road_brightness, "Medium Road Bridge Tram", ModLoader.modPath);
                RoadColorChanger.ChangeColor(ModLoader.config.medium_road_brightness, "Medium Road Slope Tram", ModLoader.modPath);
                RoadColorChanger.ChangeColor(ModLoader.config.medium_road_brightness, "Medium Road Tunnel Tram", ModLoader.modPath);

                RoadColorChanger.ChangeColor(ModLoader.config.medium_road_decoration_brightness, "Medium Road Decoration Grass", ModLoader.modPath);
                RoadColorChanger.ChangeColor(ModLoader.config.medium_road_decoration_brightness, "Medium Road Decoration Trees", ModLoader.modPath);

                RoadColorChanger.ChangeColor(ModLoader.config.medium_road_brightness, "Medium Road Bus", ModLoader.modPath);
                RoadColorChanger.ChangeColor(ModLoader.config.medium_road_brightness, "Medium Road Elevated Bus", ModLoader.modPath);
                RoadColorChanger.ChangeColor(ModLoader.config.medium_road_brightness, "Medium Road Bridge Bus", ModLoader.modPath);
                RoadColorChanger.ChangeColor(ModLoader.config.medium_road_brightness, "Medium Road Slope Bus", ModLoader.modPath);
                RoadColorChanger.ChangeColor(ModLoader.config.medium_road_brightness, "Medium Road Tunnel Bus", ModLoader.modPath);

                RoadColorChanger.ChangeColor(ModLoader.config.large_road_brightness, "Large Road", ModLoader.modPath);
                RoadColorChanger.ChangeColor(ModLoader.config.large_road_brightness, "Large Road Elevated", ModLoader.modPath);
                RoadColorChanger.ChangeColor(ModLoader.config.large_road_brightness, "Large Road Bridge", ModLoader.modPath);

                RoadColorChanger.ChangeColor(ModLoader.config.large_road_decoration_brightness, "Large Road Decoration Grass", ModLoader.modPath);
                RoadColorChanger.ChangeColor(ModLoader.config.large_road_decoration_brightness, "Large Road Decoration Trees", ModLoader.modPath);

                RoadColorChanger.ChangeColor(ModLoader.config.large_road_brightness, "Large Road Bicycle", ModLoader.modPath);
                RoadColorChanger.ChangeColor(ModLoader.config.large_road_brightness, "Large Road Elevated Bike", ModLoader.modPath);
                RoadColorChanger.ChangeColor(ModLoader.config.large_road_brightness, "Large Road Bridge Bike", ModLoader.modPath);

                RoadColorChanger.ChangeColor(ModLoader.config.large_road_brightness, "Large Road Bus", ModLoader.modPath);
                RoadColorChanger.ChangeColor(ModLoader.config.large_road_brightness, "Large Road Elevated Bus", ModLoader.modPath);
                RoadColorChanger.ChangeColor(ModLoader.config.large_road_brightness, "Large Road Bridge Bus", ModLoader.modPath);
                RoadColorChanger.ChangeColor(ModLoader.config.large_road_brightness, "Large Road Slope Bus", ModLoader.modPath);
                RoadColorChanger.ChangeColor(ModLoader.config.large_road_brightness, "Large Road Tunnel Bus", ModLoader.modPath);
				
                RoadColorChanger.ChangeColor(ModLoader.config.large_road_brightness, "Large Oneway", ModLoader.modPath);
                RoadColorChanger.ChangeColor(ModLoader.config.large_road_brightness, "Large Oneway Road", ModLoader.modPath); // RCC adds Slope + Tunnel
                RoadColorChanger.ChangeColor(ModLoader.config.large_road_brightness, "Large Oneway Elevated", ModLoader.modPath);
                RoadColorChanger.ChangeColor(ModLoader.config.large_road_brightness, "Large Oneway Bridge", ModLoader.modPath);

                RoadColorChanger.ChangeColor(ModLoader.config.large_road_decoration_brightness, "Large Oneway Decoration Grass", ModLoader.modPath);
                RoadColorChanger.ChangeColor(ModLoader.config.large_road_decoration_brightness, "Large Oneway Decoration Trees", ModLoader.modPath);

                RoadColorChanger.ChangeColor(ModLoader.config.highway_brightness, "HighwayRamp", ModLoader.modPath);
                RoadColorChanger.ChangeColor(ModLoader.config.highway_brightness, "HighwayRampElevated", ModLoader.modPath);
                RoadColorChanger.ChangeColor(ModLoader.config.highway_brightness, "HighwayRamp Slope", ModLoader.modPath);
                RoadColorChanger.ChangeColor(ModLoader.config.highway_brightness, "HighwayRamp Tunnel", ModLoader.modPath);

                RoadColorChanger.ChangeColor(ModLoader.config.highway_brightness, "Highway", ModLoader.modPath);
                RoadColorChanger.ChangeColor(ModLoader.config.highway_brightness, "Highway Slope", ModLoader.modPath);
                RoadColorChanger.ChangeColor(ModLoader.config.highway_brightness, "Highway Tunnel", ModLoader.modPath);
                RoadColorChanger.ChangeColor(ModLoader.config.highway_brightness, "Highway Elevated", ModLoader.modPath);
                RoadColorChanger.ChangeColor(ModLoader.config.highway_brightness, "Highway Bridge", ModLoader.modPath);
                RoadColorChanger.ChangeColor(ModLoader.config.highway_brightness, "Highway Barrier", ModLoader.modPath);

                RoadColorChanger.ChangeColorNetExt(ModLoader.config.small_road_brightness, "NExt2LAlley", ModLoader.modPath);
                RoadColorChanger.ChangeColorNetExt(ModLoader.config.small_road_brightness, "NExt1LOneway", ModLoader.modPath);
                RoadColorChanger.ChangeColorNetExt(ModLoader.config.small_road_brightness, "NExtSmall3LRoad", ModLoader.modPath);
                RoadColorChanger.ChangeColorNetExt(ModLoader.config.small_road_brightness, "NExtSmall4LRoad", ModLoader.modPath);
                RoadColorChanger.ChangeColorNetExt(ModLoader.config.small_road_brightness, "Small Avenue", ModLoader.modPath);
                RoadColorChanger.ChangeColorNetExt(ModLoader.config.small_road_brightness, "Oneway3L", ModLoader.modPath);
                RoadColorChanger.ChangeColorNetExt(ModLoader.config.small_road_brightness, "Oneway4L", ModLoader.modPath);
                RoadColorChanger.ChangeColorNetExt(ModLoader.config.medium_road_brightness, "NExtMediumRoad", ModLoader.modPath);
                RoadColorChanger.ChangeColorNetExt(ModLoader.config.medium_road_brightness, "NExtMediumRoadTunnel", ModLoader.modPath);
                RoadColorChanger.ChangeColorNetExt(ModLoader.config.medium_road_brightness, "NExtMediumRoadTL", ModLoader.modPath);
                RoadColorChanger.ChangeColorNetExt(ModLoader.config.medium_road_brightness, "NExtMediumRoadTLTunnel", ModLoader.modPath);
                RoadColorChanger.ChangeColorNetExt(ModLoader.config.large_road_brightness, "NExtLargeRoad", ModLoader.modPath);
                RoadColorChanger.ChangeColorNetExt(ModLoader.config.large_road_brightness, "NExtLargeRoadTunnel", ModLoader.modPath);
                RoadColorChanger.ChangeColorNetExt(ModLoader.config.large_road_brightness, "NExtLargeRoadTL", ModLoader.modPath);
                RoadColorChanger.ChangeColorNetExt(ModLoader.config.large_road_brightness, "NExtLargeRoadTLTunnel", ModLoader.modPath);

                RoadColorChanger.ChangeColorNetExt(ModLoader.config.large_road_brightness, "NExtXLargeRoad", ModLoader.modPath);
                RoadColorChanger.ChangeColorNetExt(ModLoader.config.large_road_brightness, "NExtXLargeRoadTunnel", ModLoader.modPath);

                RoadColorChanger.ChangeColorNetExt(ModLoader.config.highway_national_brightness, "NExtHighway1L", ModLoader.modPath);
                RoadColorChanger.ChangeColorNetExt(ModLoader.config.highway_national_brightness, "NExtHighwayTunnel1LTunnel", ModLoader.modPath);
                RoadColorChanger.ChangeColorNetExt(ModLoader.config.highway_brightness, "NExtHighway2L", ModLoader.modPath);
                RoadColorChanger.ChangeColorNetExt(ModLoader.config.highway_brightness, "NExtHighwayTunnel2LTunnel", ModLoader.modPath);
                RoadColorChanger.ChangeColorNetExt(ModLoader.config.highway_brightness, "NExtHighway4L", ModLoader.modPath);
                RoadColorChanger.ChangeColorNetExt(ModLoader.config.highway_brightness, "NExtHighwayTunnel4LTunnel", ModLoader.modPath);
                RoadColorChanger.ChangeColorNetExt(ModLoader.config.highway_brightness, "NExtHighway5L", ModLoader.modPath);
                RoadColorChanger.ChangeColorNetExt(ModLoader.config.highway_brightness, "NExtHighwayTunnel5LTunnel", ModLoader.modPath);
                RoadColorChanger.ChangeColorNetExt(ModLoader.config.highway_brightness, "NExtHighway6L", ModLoader.modPath);
                RoadColorChanger.ChangeColorNetExt(ModLoader.config.highway_brightness, "NExtHighwayTunnel6LTunnel", ModLoader.modPath);



                RoadColorChanger.ChangeColor(ModLoader.config.small_road_brightness, "Small Busway", ModLoader.modPath);
                RoadColorChanger.ChangeColor(ModLoader.config.small_road_decoration, "Small Busway Decoration Grass", ModLoader.modPath);
                RoadColorChanger.ChangeColor(ModLoader.config.small_road_decoration, "Small Busway Decoration Trees", ModLoader.modPath);

                RoadColorChanger.ChangeColor(ModLoader.config.small_road_brightness, "Small Busway Oneway", ModLoader.modPath);
                RoadColorChanger.ChangeColor(ModLoader.config.small_road_decoration, "Small Busway Oneway Decoration Grass", ModLoader.modPath);
                RoadColorChanger.ChangeColor(ModLoader.config.small_road_decoration, "Small Busway Oneway Decoration Trees", ModLoader.modPath);

                RoadColorChanger.ChangeColor(ModLoader.config.large_road_brightness, "Large Road With Bus Lanes", ModLoader.modPath);
                RoadColorChanger.ChangeColor(ModLoader.config.large_road_decoration_brightness, "Large Road Decoration Grass With Bus Lanes", ModLoader.modPath);
                RoadColorChanger.ChangeColor(ModLoader.config.large_road_decoration_brightness, "Large Road Decoration Trees With Bus Lanes", ModLoader.modPath);
                RoadColorChanger.ChangeColor(ModLoader.config.large_road_brightness, "Large Road Elevated With Bus Lanes", ModLoader.modPath);
                RoadColorChanger.ChangeColor(ModLoader.config.large_road_brightness, "Large Road Bridge With Bus Lanes", ModLoader.modPath);
                RoadColorChanger.ChangeColor(ModLoader.config.large_road_brightness, "Large Road Tunnel With Bus Lanes", ModLoader.modPath);
                RoadColorChanger.ChangeColor(ModLoader.config.large_road_brightness, "Large Road Slope With Bus Lanes", ModLoader.modPath);



                RoadColorChanger.ReplaceLodAprAtlas();
            }
            #endregion


            RoadsUnited_CoreProps.ChangeArrowProp();

            RoadsUnited_CoreProps.ReplacePropTextures();

        }

        public override void OnLevelUnloading()
        {
            base.OnLevelUnloading();
            RoadsUnited_Core.ApplyVanillaDictionary();
            RoadsUnited_Core.vanillaPrefabProperties.Clear();
			//hook.DisableHook();
			//GameObject.Destroy(hookGo);
			//hook = null;
        }
        
        public static void SaveConfig()
        {
            Configuration.Serialize(ModLoader.configFile, ModLoader.config);
        }

        public override void OnReleased()
        {

        }

    }
}
