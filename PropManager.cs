using ICities;
using System.IO;
using UnityEngine;
using RoadsUnited_Core;
using System;

namespace RoadsUnited_Core
{
    public class RoadsUnited_CoreProps : MonoBehaviour
    {
        public static void ReplacePropTextures()
		{
			string Tex=ModLoader.Tex;
			string cTexPathProp=ModLoader.currentTexturesPath_default+"/PropTextures/";

            PropCollection[] array = UnityEngine.Object.FindObjectsOfType<PropCollection>();
            for (int i = 0; i < array.Length; i++)
            {
                PropCollection propCollection = array[i];
                try
                {
                    PropInfo[] prefabs = propCollection.m_prefabs;
                    for (int j = 0; j < prefabs.Length; j++)
                    {
                        PropInfo propInfo = prefabs[j];

                        string defaultname = null;

                        if (propInfo.m_lodMaterialCombined.GetTexture("_MainTex").name != null)
                        {
                            defaultname = propInfo.m_lodMaterialCombined.GetTexture("_MainTex").name;
                        }

                        if (defaultname != null)
                        {
                            string propLodTexture = null;
                            string propLodACIMapTexture = null;

							//if(defaultname=="BusLaneText")
							//{
								//propLodTexture = Path.Combine(ModLoader.currentTexturesPath_default, "BusLane.dds");
								//propLodACIMapTexture=Path.Combine(ModLoader.currentTexturesPath_default,"BusLane-aci.dds");
							//}
							//else
							//{
								//propLodTexture=Path.Combine(ModLoader.currentTexturesPath_default,defaultname+".dds");
								//propLodACIMapTexture = Path.Combine(ModLoader.currentTexturesPath_default, defaultname + "-aci.dds");
							//}

                            if (defaultname == "BusLaneText")
                            {
								propLodTexture=Path.Combine(cTexPathProp,"BusLane.dds");
								propLodACIMapTexture=Path.Combine(cTexPathProp,"BusLane-aci.dds");
                            }
                            else
							{
								propLodTexture=Path.Combine(cTexPathProp,defaultname+".dds");
								propLodACIMapTexture=Path.Combine(cTexPathProp,defaultname+"-aci.dds");
                            }

                            if (File.Exists(propLodTexture))
                            {
                                //only the m_lodMaterialCombined texture is visible
                                propInfo.m_lodMaterialCombined.SetTexture("_MainTex", RoadsUnited_Core.LoadTextureDDS(propLodTexture));
                            }
                            if (File.Exists(propLodACIMapTexture))
                            {
                                propInfo.m_lodMaterialCombined.SetTexture("_ACIMap", RoadsUnited_Core.LoadTextureDDS(propLodACIMapTexture));
                            }
                        }
                    }
                }
                catch (Exception)
                {
                }
            }
        }

        public static void ChangeArrowProp()
        {
            for (uint i = 0; i < PrefabCollection<PropInfo>.LoadedCount(); i++)
            {
                var prefab = PrefabCollection<PropInfo>.GetLoaded(i);

                if (prefab == null) continue;


                PropInfo propInfo = prefab;

                if (propInfo.name.Equals("Road Arrow LFR"))
                {
                    if ((ModLoader.config.disable_optional_arrow_lfr == true))
                    {
                        propInfo.m_maxRenderDistance = 0f;
                        propInfo.m_maxScale = 0f;
                        propInfo.m_minScale = 0f;
                    }
                    else
                    {
                        propInfo.m_maxRenderDistance = 1000f;
                        propInfo.m_maxScale = 1f;
                        propInfo.m_minScale = 1f;
                    }
                }

                if (propInfo.name.Equals("Road Arrow LR"))
                {
                    if ((ModLoader.config.disable_optional_arrow_lr == true))
                    {
                        propInfo.m_maxRenderDistance = 0f;
                        propInfo.m_maxScale = 0f;
                        propInfo.m_minScale = 0f;
                    }
                    else
                    {
                        propInfo.m_maxRenderDistance = 1000f;
                        propInfo.m_maxScale = 1f;
                        propInfo.m_minScale = 1f;
                    }
                }

            }


        }

        // Not working; romoves all the arrows and manholes, if lane props get chenged, they change everywhere.

        //NetCollection[] array3 = UnityEngine.Object.FindObjectsOfType<NetCollection>();
        //NetCollection[] array4 = array3;
        //for (int k = 0; k < array4.Length; k++)
        //{
        //    NetCollection netCollection = array4[k];
        //    bool flag6 = netCollection != null;
        //    if (flag6)
        //    {
        //        NetInfo[] prefabs2 = netCollection.m_prefabs;
        //        for (int l = 0; l < prefabs2.Length; l++)
        //        {
        //            NetInfo netInfo = prefabs2[l];
        //            bool flag7 = netInfo != null;
        //            if (flag7)
        //            {
        //                bool flag8 = netInfo.m_class.name.Equals("Highway");
        //                if (flag8)
        //                {
        //                    NetInfo.Lane[] lanes = netInfo.m_lanes;
        //                    for (int m = 0; m < lanes.Length; m++)
        //                    {
        //                        NetInfo.Lane lane = lanes[m];
        //                        bool flag9 = lane != null;
        //                        if (flag9)
        //                        {
        //                            FastList<NetLaneProps.Prop> fastList = new FastList<NetLaneProps.Prop>();
        //                            NetLaneProps.Prop[] props = lane.m_laneProps.m_props;
        //                            for (int n = 0; n < props.Length; n++)
        //                            {
        //                                NetLaneProps.Prop prop = props[n];
        //                                bool flag10 = prop != null;
        //                                {
        //                                    if (ModLoader.config.disable_optional_arrows)
        //                                    {
        //                                        bool flag11 = !prop.m_prop.name.Equals("Road Arrow F") && !prop.m_prop.name.Equals("Manhole");
        //                                        if (flag11)
        //                                        {
        //                                            fastList.Add(prop);
        //                                        }
        //                                    }
        //                                    else
        //                                    {
        //                                        bool flag12 = !prop.m_prop.name.Equals("Manhole");
        //                                        if (flag12)
        //                                        {
        //                                            fastList.Add(prop);
        //                                        }
        //                                    }
        //                                }
        //                            }
        //                            lane.m_laneProps.m_props = fastList.ToArray();
        //                        }
        //                    }
        //                }
        //                bool flag13 = netInfo.m_class.name.Contains("Elevated") || netInfo.m_class.name.Contains("Bridge");
        //                if (flag13)
        //                {
        //                    NetInfo.Lane[] lanes2 = netInfo.m_lanes;
        //                    for (int num = 0; num < lanes2.Length; num++)
        //                    {
        //                        NetInfo.Lane lane2 = lanes2[num];
        //                        bool flag14 = lane2 != null && lane2.m_laneProps != null;
        //                        if (flag14)
        //                        {
        //                            FastList<NetLaneProps.Prop> fastList2 = new FastList<NetLaneProps.Prop>();
        //                            NetLaneProps.Prop[] props2 = lane2.m_laneProps.m_props;
        //                            for (int num2 = 0; num2 < props2.Length; num2++)
        //                            {
        //                                NetLaneProps.Prop prop2 = props2[num2];
        //                                bool flag15 = prop2 != null && !prop2.m_prop.name.Equals("Manhole");
        //                                if (flag15)
        //                                {
        //                                    fastList2.Add(prop2);
        //                                }
        //                            }
        //                            lane2.m_laneProps.m_props = fastList2.ToArray();
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}
    }

}

