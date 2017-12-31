using ColossalFramework;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace RoadsUnited_Core
{
    public class RoadColorChanger : MonoBehaviour
    {
        public static Configuration config;

        public static void ReplaceLodAprAtlas()
        {
            Texture2D texture2D = new Texture2D(Singleton<NetManager>.instance.m_lodAprAtlas.width, Singleton<NetManager>.instance.m_lodAprAtlas.height);
            texture2D.anisoLevel = 8;
            for (int rgb_y = 0; rgb_y < texture2D.height; rgb_y++)
            {
                for (int rgb_x = 0; rgb_x < texture2D.width; rgb_x++)
                {
                    if (Singleton<NetManager>.instance.m_lodAprAtlas.GetPixel(rgb_x, rgb_y).b > 0f)
                    {
                        texture2D.SetPixel(rgb_x, rgb_y, new Color(Singleton<NetManager>.instance.m_lodAprAtlas.GetPixel(rgb_x, rgb_y).r, Singleton<NetManager>.instance.m_lodAprAtlas.GetPixel(rgb_x, rgb_y).g, 1f));
                    }
                    else
                    {
                        texture2D.SetPixel(rgb_x, rgb_y, Singleton<NetManager>.instance.m_lodAprAtlas.GetPixel(rgb_x, rgb_y));
                    }
                }
            }
            texture2D.Apply();
            Singleton<NetManager>.instance.m_lodAprAtlas = texture2D;
        }

      //public static void ReplaceLodRgbAtlas()
      //{
      //    Singleton<NetManager>.instance.m_lodRgbAtlas.(textures);
      //}


        public static void ChangeColor(float brightness, string prefab_road_name, string TextureDir)
        {
            for (uint i = 0; i < PrefabCollection<NetInfo>.LoadedCount(); i++)
            {
                var netInfo = PrefabCollection<NetInfo>.GetLoaded(i);
                if (netInfo == null) continue;
                if (netInfo.name.Equals(prefab_road_name))
                {
                    if (netInfo.m_color != null)
                        netInfo.m_color = new Color(brightness, brightness, brightness);
                }

                if (netInfo.name.Equals(prefab_road_name + " Slope"))
                {
                    if (netInfo.m_color != null)
                        netInfo.m_color = new Color(brightness, brightness, brightness);
                }

                if (netInfo.name.Equals(prefab_road_name + " Tunnel"))
                {
                    if (netInfo.m_color != null)
                        netInfo.m_color = new Color(brightness, brightness, brightness);
                }

                if (netInfo.name.Equals(prefab_road_name + " Elevated"))
                {
                    if (netInfo.m_color != null)
                        netInfo.m_color = new Color(brightness, brightness, brightness);
                }

                if (netInfo.name.Equals(prefab_road_name + " Bridge"))
                {
                    if (netInfo.m_color != null)
                        netInfo.m_color = new Color(brightness, brightness, brightness);
                }
            }
        }
        // RoadsUnited.RoadColourChanger
        public static void ChangeColorNetExt(float brightness, string Prefab_Class_Name, string TextureDir)
        {
            for (uint i = 0; i < PrefabCollection<NetInfo>.LoadedCount(); i++)
            {
                var netInfo = PrefabCollection<NetInfo>.GetLoaded(i);

                if (netInfo == null) continue;
                if (netInfo.m_class.name.Contains(Prefab_Class_Name))
                {
                    if (netInfo.m_color != null)
                    {
                        netInfo.m_color = new Color(brightness, brightness, brightness);
                    }
                }

            }
        }
    }
}
