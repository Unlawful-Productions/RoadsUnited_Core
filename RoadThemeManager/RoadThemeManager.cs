using ColossalFramework;
using ColossalFramework.Plugins;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace RoadsUnited_Core
{
    public class RoadThemeManager : Singleton<RoadThemeManager>
    {
        public bool isLoaded;
        public RoadThemePack activePack;

        public RoadThemePack ActivePack
        {
            get
            {
                return this.activePack;
            }
            set
            {
                this.activePack = value;
                if (this.activePack != null)
                {
                    ModLoader.config.texturePackPath = Singleton<RoadThemeManager>.instance.activePack.packPath;
                    ModLoader.SaveConfig();
                    RoadsUnited_Core.ApplyVanillaDictionary();
                    RoadsUnited_Core.ReplaceNetTextures();
                    if (ModLoader.config.selected_pack == 0)
                    {
                    }
                    else
                    {
                        RoadsUnited_CoreProps.ReplacePropTextures();
                    }
                    return;
                }
            }
        }

        public List<RoadThemePack> GetAvailablePacks()
        {
            List<RoadThemePack> list = new List<RoadThemePack>();
            foreach (PluginManager.PluginInfo current in Singleton<PluginManager>.instance.GetPluginsInfo())
            {
                if (current.isEnabled)
                {
                    string text = Path.Combine(current.modPath, "RoadsUnitedTheme.xml");
                    if (File.Exists(text))
                    {
                        RoadThemePack RoadThemePack = RoadThemePack.Deserialize(text);
                        if (RoadThemePack != null)
                        {
                            if (RoadThemePack.themeName == null)
                            {
                                RoadThemePack.themeName = current.name;
                            }
                            RoadThemePack.packPath = current.modPath;
                            list.Add(RoadThemePack);
                        }
                    }
                }
            }
            return list;
        }
    }
}
