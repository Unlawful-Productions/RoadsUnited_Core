using ColossalFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;
using System.Text.RegularExpressions;

namespace RoadsUnited_Core {

	public class RoadsUnited_Core: MonoBehaviour {
		public static Configuration config;
		public static Dictionary<string, Texture2D> vanillaPrefabProperties = new Dictionary<string, Texture2D>();

		private static Texture2D defaultmap;
		private static Texture2D acimap;
		private static Texture2D aprmap;
		private static Dictionary<string, Texture2D> textureCache = new Dictionary<string, Texture2D>();

		public static Texture2D LoadTextureDDS(string fullPath) {
			// Testen ob Textur bereits geladen, in dem Fall geladene Textur zurückgeben
			Texture2D texture;
			if(textureCache.TryGetValue(fullPath, out texture))
				return texture;
			// Nein? Textur laden
			var numArray = File.ReadAllBytes(fullPath);
			var width = BitConverter.ToInt32(numArray, 16);
			var height = BitConverter.ToInt32(numArray, 12);
			texture = new Texture2D(width, height, TextureFormat.DXT5, true);
			var list = new List<byte>();
			for(int index = 0;index < numArray.Length;++index) {
				if(index > (int)sbyte.MaxValue)
					list.Add(numArray[index]);
				}
			texture.LoadRawTextureData(list.ToArray());
			texture.name = Path.GetFileName(fullPath);
			texture.anisoLevel = 8;
			texture.Apply();
			textureCache.Add(fullPath, texture); // Neu geladene Textur in den Cache packen
			return texture;
			}
		//deactivated for now
		public static Texture2D LoadTexture(string fullPath) {
			Texture2D texture2D = new Texture2D(1, 1);
			if(textureCache.TryGetValue(fullPath, out texture2D))
				return texture2D;
			texture2D.LoadImage(File.ReadAllBytes(fullPath));
			texture2D.name = Path.GetFileName(fullPath);
			texture2D.anisoLevel = 8;
			texture2D.Compress(true);
			return texture2D;
			}

		public static void CreateVanillaDictionary() {
			for(uint i = 0;i < PrefabCollection<NetInfo>.LoadedCount();i++) {
				var netInfo = PrefabCollection<NetInfo>.GetLoaded(i);
				if(netInfo == null)
					continue;
				string prefab_road_name = netInfo.name.Replace(" ", "_").ToLowerInvariant().Trim();
				NetInfo.Node[] nodes = netInfo.m_nodes;
				for(int k = 0;k < nodes.Length;k++) {
					NetInfo.Node node = nodes[k];
					if(!(
						netInfo.m_class.name.Contains("Heating Pipe") ||
						netInfo.m_class.name.Contains("Water") ||
						netInfo.m_class.name.Contains("Train") ||
						netInfo.m_class.name.Contains("Metro") ||
						netInfo.m_class.name.Contains("Transport") ||
						netInfo.m_class.name.Contains("Bus Line") ||
						netInfo.m_class.name.Contains("Airplane") ||
						netInfo.m_class.name.Contains("Ship")
						)) {
						if((node.m_nodeMaterial != null) && (!(node.m_nodeMaterial.name.Contains("rail")))) {
							vanillaPrefabProperties.Add(prefab_road_name + "_node_" + (k) + "_nodeMaterial" + "_MainTex", node.m_nodeMaterial.GetTexture("_MainTex") as Texture2D);
							vanillaPrefabProperties.Add(prefab_road_name + "_node_" + (k) + "_nodeMaterial" + "_APRMap", node.m_nodeMaterial.GetTexture("_APRMap") as Texture2D);
							}
						}
					}
				NetInfo.Segment[] segments = netInfo.m_segments;
				for(int l = 0;l < segments.Length;l++) {
					NetInfo.Segment segment = segments[l];
					if(!(
						//netInfo.m_class.name.Contains("NExt") ||
						netInfo.m_class.name.Contains("Heating Pipe") ||
						netInfo.m_class.name.Contains("Water") ||
						netInfo.m_class.name.Contains("Train") ||
						netInfo.m_class.name.Contains("Metro") ||
						netInfo.m_class.name.Contains("Transport") ||
						netInfo.m_class.name.Contains("Bus Line") ||
						netInfo.m_class.name.Contains("Airplane") ||
						netInfo.m_class.name.Contains("Ship")
						)) {
						if((segment.m_segmentMaterial != null) && (!(segment.m_segmentMaterial.name.Contains("rail")))) {
							vanillaPrefabProperties.Add(prefab_road_name + "_segment_" + (l) + "_segmentMaterial" + "_MainTex", segment.m_segmentMaterial.GetTexture("_MainTex") as Texture2D);
							vanillaPrefabProperties.Add(prefab_road_name + "_segment_" + (l) + "_segmentMaterial" + "_APRMap", segment.m_segmentMaterial.GetTexture("_APRMap") as Texture2D);
							}
						}
					}
				}

			PropCollection[] array = UnityEngine.Object.FindObjectsOfType<PropCollection>();
			for(int i = 0;i < array.Length;i++) {
				PropCollection propCollection = array[i];
				try {
					PropInfo[] prefabs = propCollection.m_prefabs;
					for(int j = 0;j < prefabs.Length;j++) {
						PropInfo propInfo = prefabs[j];
						string str = propInfo.name;
						//if (propInfo.m_lodMaterialCombined != null)
						if(propInfo.m_lodMaterialCombined.GetTexture("_MainTex").name != null) {
							vanillaPrefabProperties.Add(str + "_prop_" + "_MainTex", propInfo.m_lodMaterialCombined.GetTexture("_MainTex") as Texture2D);
							vanillaPrefabProperties.Add(str + "_prop_" + "_ACIMap", propInfo.m_lodMaterialCombined.GetTexture("_ACIMap") as Texture2D);
							}
						}
					}
				catch(Exception) { }
				}
			}

		public static void ApplyVanillaDictionary() {
			for(uint i = 0;i < PrefabCollection<NetInfo>.LoadedCount();i++) {
				var netInfo = PrefabCollection<NetInfo>.GetLoaded(i);
				if(netInfo == null)
					continue;
				string prefab_road_name = netInfo.name.Replace(" ", "_").ToLowerInvariant().Trim();
				NetInfo.Node[] nodes = netInfo.m_nodes;
				for(int k = 0;k < nodes. Length;k++) {
					NetInfo.Node node = nodes[k];
					if(!(
						//netInfo.m_class.name.Contains("NExt") ||
						netInfo.m_class.name.Contains("Heating Pipe") ||
						netInfo.m_class.name.Contains("Water") ||
						netInfo.m_class.name.Contains("Train") ||
						netInfo.m_class.name.Contains("Metro") ||
						netInfo.m_class.name.Contains("Transport") ||
						netInfo.m_class.name.Contains("Bus Line") ||
						netInfo.m_class.name.Contains("Airplane") ||
						netInfo.m_class.name.Contains("Ship")
						)) {
						if((node.m_nodeMaterial != null) && (!(node.m_nodeMaterial.name.Contains("rail")))) {
							if(vanillaPrefabProperties.TryGetValue(prefab_road_name + "_node_" + (k) + "_nodeMaterial" + "_MainTex", out defaultmap))
								node.m_nodeMaterial.SetTexture("_MainTex", defaultmap);
							if(vanillaPrefabProperties.TryGetValue(prefab_road_name + "_node_" + (k) + "_nodeMaterial" + "_APRMap", out aprmap))
								node.m_nodeMaterial.SetTexture("_APRMap", aprmap);
							}
						}
					}

				NetInfo.Segment[] segments = netInfo.m_segments;
				for(int l = 0;l < segments.Length;l++) {
					NetInfo.Segment segment = segments[l];
					if(!(
						//netInfo.m_class.name.Contains("NExt") ||
						netInfo.m_class.name.Contains("Heating Pipe") || netInfo.m_class.name.Contains("Water") ||
						netInfo.m_class.name.Contains("Train") ||
						netInfo.m_class.name.Contains("Metro") ||
						netInfo.m_class.name.Contains("Transport") ||
						netInfo.m_class.name.Contains("Bus Line") ||
						netInfo.m_class.name.Contains("Airplane") ||
						netInfo.m_class.name.Contains("Ship")
						)) {
						if((segment.m_segmentMaterial != null) && (!(segment.m_segmentMaterial.name.Contains("rail")))) {
							if(vanillaPrefabProperties.TryGetValue(prefab_road_name + "_segment_" + (l) + "_segmentMaterial" + "_MainTex", out defaultmap))
								segment.m_segmentMaterial.SetTexture("_MainTex", defaultmap);
							if(vanillaPrefabProperties.TryGetValue(prefab_road_name + "_segment_" + (l) + "_segmentMaterial" + "_APRMap", out aprmap))
								segment.m_segmentMaterial.SetTexture("_APRMap", aprmap);
							}
						}
					}
				}
			PropCollection[] array = UnityEngine.Object.FindObjectsOfType<PropCollection>();
			for(int i = 0;i < array.Length;i++) {
				PropCollection propCollection = array[i];
				try {
					PropInfo[] prefabs = propCollection.m_prefabs;
					for(int j = 0;j < prefabs.Length;j++) {
						PropInfo propInfo = prefabs[j];
						string str = propInfo.name;
						//if (propInfo.m_lodMaterialCombined != null)
						if(propInfo.m_lodMaterialCombined.GetTexture("_MainTex").name != null) {
							if(vanillaPrefabProperties.TryGetValue(str + "_prop_" + "_MainTex", out defaultmap))
								propInfo.m_lodMaterialCombined.SetTexture("_MainTex", defaultmap);
							if(vanillaPrefabProperties.TryGetValue(str + "_prop_" + "_ACIMap", out acimap))
								propInfo.m_lodMaterialCombined.SetTexture("_ACIMap", acimap);
							}
						}
					}
				catch(Exception) { }
				}
			}

		public static void ApplyVanillaRoadDictionary() {
			for(uint i = 0;i < PrefabCollection<NetInfo>.LoadedCount();i++) {
				var netInfo = PrefabCollection<NetInfo>.GetLoaded(i);

				if(netInfo == null)
					continue;
				string prefab_road_name = netInfo.name.Replace(" ", "_").ToLowerInvariant().Trim();
				NetInfo.Node[] nodes = netInfo.m_nodes;
				for(int k = 0;k < nodes.Length;k++) {
					NetInfo.Node node = nodes[k];
					if(!(
						//netInfo.m_class.name.Contains("NExt") ||
						netInfo.m_class.name.Contains("Heating Pipe") ||
						netInfo.m_class.name.Contains("Water") ||
						netInfo.m_class.name.Contains("Train") ||
						netInfo.m_class.name.Contains("Metro") ||
						netInfo.m_class.name.Contains("Transport") ||
						netInfo.m_class.name.Contains("Bus Line") ||
						netInfo.m_class.name.Contains("Airplane") ||
						netInfo.m_class.name.Contains("Ship")
						)) {
						if((node.m_nodeMaterial != null) && (!(node.m_nodeMaterial.name.Contains("rail")))) {
							if(vanillaPrefabProperties.TryGetValue(prefab_road_name + "_node_" + (k) + "_nodeMaterial" + "_MainTex", out defaultmap))
								node.m_nodeMaterial.SetTexture("_MainTex", defaultmap);
							if(vanillaPrefabProperties.TryGetValue(prefab_road_name + "_node_" + (k) + "_nodeMaterial" + "_APRMap", out aprmap))
								node.m_nodeMaterial.SetTexture("_APRMap", aprmap);
							}
						}
					}

				NetInfo.Segment[] segments = netInfo.m_segments;
				for(int l = 0;l < segments.Length;l++) {
					NetInfo.Segment segment = segments[l];
					if(!(
						//netInfo.m_class.name.Contains("NExt") ||
						netInfo.m_class.name.Contains("Heating Pipe") || netInfo.m_class.name.Contains("Water") ||
						netInfo.m_class.name.Contains("Train") ||
						netInfo.m_class.name.Contains("Metro") ||
						netInfo.m_class.name.Contains("Transport") ||
						netInfo.m_class.name.Contains("Bus Line") ||
						netInfo.m_class.name.Contains("Airplane") ||
						netInfo.m_class.name.Contains("Ship")
						)) {
						if((segment.m_segmentMaterial != null) && (!(segment.m_segmentMaterial.name.Contains("rail")))) {
							if(vanillaPrefabProperties.TryGetValue(prefab_road_name + "_segment_" + (l) + "_segmentMaterial" + "_MainTex", out defaultmap))
								segment.m_segmentMaterial.SetTexture("_MainTex", defaultmap);
							if(vanillaPrefabProperties.TryGetValue(prefab_road_name + "_segment_" + (l) + "_segmentMaterial" + "_APRMap", out aprmap))
								segment.m_segmentMaterial.SetTexture("_APRMap", aprmap);
							}
						}
					}
				}
			}

		public static void ReplaceNetTextures() {
			//Texture Category
			string main = "_MainTex";
			string apr = "_APRMap";
			string xys	=	"_XYSMap";
			
			//Road Sizes
			string RT = "RoadTiny";
			string RS = "RoadSmall";
			string RM = "RoadMedium";
			string RL = "RoadLarge";
			string HW = "Highway";
			
			//Modifiers 1
			string A3 = "L1R2";
			string A4 = "L1R3";
			string A5 = "L2R3";			//*New
			string A6 = "L2R4";			//*New
			string Av = "Ave";
			string Bk = "Bike";
			string Bl = "BusLane";
			string Bs = "BusLanes";		//*New
			string Gh = "Ghost";		//*New
			string Lg = "Large";		//*New
			string Md = "Mdn";			//*New
			string Mr = "Monorail";		//*New
			string Ow = "Oneway";
			string Tw = "Twoway";
			string PM = "PntMdn";		//*New
			string St = "Stock";
			string Tr = "Tram";
			
			//Ground Roads
			string Gd = "Gnd";
			string Dc = "Deco";
			
			//Transit Stops
			string B1 = "BusSide";
			string B2 = "BusBoth";
			
			//Different Heights
			string El = "Elevated";
			string Sl = "Slope";
			string Tl = "Tunnel";
			
			//Node
			string Nd = "_Node";
			
			//Tram Fix
			string TrRailA = "tram-rail-double-wn-No Name";
			string TrRailB = "tram-rail-double-No Name";
			
			//File Suffixes
			string Main = "_MainTex.dds";
			string APR = "_APRMap.dds";
			string LOD = "_LOD_MainTex.dds";
			//string LODAPR	=   "_LOD_APRMap.dds";
			//string LODXYS	=   "_LOD_XYSMap.dds";

			string Tex2A = ModLoader.Tex2A;
			string Tex2B = ModLoader.Tex2B;
			string Tex2C = ModLoader.Tex2C;
			string Tex = ModLoader.Tex;
			string cTexPath = "";
			string TramPath = ModLoader.currentTexturesPath_default + "/PropTextures/";
			string cTexDefault = ModLoader.currentTexturesPath_default;
			string cTexPathRT = ModLoader.currentTexturesPath_default + "/" + RT + "/";
			string cTexPathRS = ModLoader.currentTexturesPath_default + "/" + RS + "/";
			string cTexPathRM = ModLoader.currentTexturesPath_default + "/" + RM + "/";
			string cTexPathRL = ModLoader.currentTexturesPath_default + "/" + RL + "/";
			string cTexPathHW = ModLoader.currentTexturesPath_default + "/" + HW + "/";

			if(ModLoader.config.texturePackPath != null) {
				ModLoader.currentTexturesPath_default = Path.Combine(ModLoader.config.texturePackPath, "BaseTextures");
				}

			for(uint i = 0;i < PrefabCollection<NetInfo>.LoadedCount();i++) {//
				var netInfo = PrefabCollection<NetInfo>.GetLoaded(i);
				if(netInfo == null)
					continue;

				#region NExt Nodes + Segments

				if(netInfo.m_class.name.Contains("NExt") ||
					netInfo.name.Contains("Busway") ||
					(netInfo.name.Contains("Large Road") &&
					netInfo.name.Contains("Bus Lane"))) {

					#region NExt Nodes

					NetInfo.Node[] nodes = netInfo.m_nodes;
					for(int k = 0;k < nodes.Length;k++) {
						NetInfo.Node node = nodes[k];
						if((node.m_nodeMaterial.GetTexture(main) != null) && (!(node.m_nodeMaterial.name.Contains("rail")))) {
							//Added additional path + filename structure
							#region NExt TinyRoads Default

							if(netInfo.name.Equals("Two-Lane Alley")) {
								Tex = RT + "2L" + Nd;
								if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "Alley2L_Ground_Node_MainTex.dds"))) {
									node.m_nodeMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "Alley2L_Ground_Node_MainTex.dds")));
									if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "Alley2L_Ground_Node_APRMap.dds")))
										node.m_nodeMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "Alley2L_Ground_Node_APRMap.dds")));
									} else if(File.Exists(Path.Combine(cTexPathRT, Tex + Main))) {
									node.m_nodeMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRT, Tex + Main)));
									if(File.Exists(Path.Combine(cTexPathRT, Tex + APR)))
										node.m_nodeMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRT, Tex + APR)));
									} else {
									Tex = RT + "1L" + Nd;
									if(File.Exists(Path.Combine(cTexPathRT, Tex + Main)))
										node.m_nodeMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRT, Tex + Main)));
									if(File.Exists(Path.Combine(cTexPathRT, Tex + APR)))
										node.m_nodeMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRT, Tex + APR)));
									}
								node.m_lodRenderDistance = 2500;
								}

							if(netInfo.name.Equals("One-Lane Oneway")) {
								Tex = RT + "1L" + Nd;
								if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "OneWay1L_Ground_Node_MainTex.dds"))) {
									node.m_nodeMaterial.SetTexture("_MainTex", LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "OneWay1L_Ground_Node_MainTex.dds")));
									if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "OneWay1L_Ground_Node_APRMap.dds")))
										node.m_nodeMaterial.SetTexture("_APRMap", LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "OneWay1L_Ground_Node_APRMap.dds")));
									} else if(File.Exists(Path.Combine(cTexPathRT, Tex + Main))) {
									node.m_nodeMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRT, Tex + Main)));
									if(File.Exists(Path.Combine(cTexPathRT, Tex + APR)))
										node.m_nodeMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRT, Tex + APR)));
									} else {
									if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, Tex + Main)))
										node.m_nodeMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRT, Tex + Main)));
									if(File.Exists(Path.Combine(cTexPathRT, Tex + APR)))
										node.m_nodeMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRT, Tex + APR)));
									}
								node.m_lodRenderDistance = 2500;
								}

							#endregion

							#region Avenues Nodes
							if(netInfo.name.Contains("Eight-Lane Avenue")) {
								Tex = RL + "8L";
								if(netInfo.name.Equals("Eight-Lane Avenue Elevated")) {
									Tex += El + Nd;
									if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "LargeAvenue8LM_Elevated_Node_MainTex.dds")))
										node.m_nodeMaterial.SetTexture("_MainTex", LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "LargeAvenue8LM_Elevated_Node_MainTex.dds")));
									else if(File.Exists(Path.Combine(cTexPathRL, Tex + Main))) {
										node.m_nodeMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRL, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPathRL, Tex + APR)))
											node.m_nodeMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRL, Tex + APR)));
										}
									} else if(netInfo.name.Equals("Eight-Lane Avenue Slope")) {
									Tex += Sl + Nd;
									if(File.Exists(Path.Combine(cTexPathRL, Tex + Main))) {
										node.m_nodeMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRL, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPathRL, Tex + APR)))
											node.m_nodeMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRL, Tex + APR)));
										}
									} else if(netInfo.name.Equals("Eight-Lane Avenue Tunnel")) {
									Tex += Tl + Nd;
									if(File.Exists(Path.Combine(cTexPathRL, Tex + Main))) {
										node.m_nodeMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRL, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPathRL, Tex + APR)))
											node.m_nodeMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRL, Tex + APR)));
										}
									} else {
									Tex += Gd + Nd;
									if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "LargeAvenue8LM_Ground_Node_MainTex.dds")))
										node.m_nodeMaterial.SetTexture("_MainTex", LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "LargeAvenue8LM_Ground_Node_MainTex.dds")));
									else if(File.Exists(Path.Combine(cTexPathRL, Tex + Main))) {
										node.m_nodeMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRL, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPathRL, Tex + APR)))
											node.m_nodeMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRL, Tex + APR)));
										}
									}
								node.m_lodRenderDistance = 2500;
								}
							#endregion

							#region NExt Highways Nodes Default
							//Highway 1L
							if(netInfo.name.Contains("Small Rural Highway")) {
								Tex = HW + "1L" + Nd;
								if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "Highway2L_Ground_Node_MainTex.dds"))) /* not an error, it uses the 2l node tex*/
									node.m_nodeMaterial.SetTexture("_MainTex", LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "Highway2L_Ground_Node_MainTex.dds")));
								else if(File.Exists(Path.Combine(cTexPathHW, Tex + Main))) {
									node.m_nodeMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + Main)));
									if(File.Exists(Path.Combine(cTexPathHW, Tex + APR)))
										node.m_nodeMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + APR)));
									}
								node.m_lodRenderDistance = 2500;
								}
							if(netInfo.name.Equals("Small Rural Highway Elevated")) {
								Tex = HW + "1L" + Nd;
								if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "Highway2L_Ground_Node_MainTex.dds")))
									node.m_nodeMaterial.SetTexture("_MainTex", LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "Highway2L_Ground_Node_MainTex.dds")));
								else if(File.Exists(Path.Combine(cTexPathHW, Tex + Main))) {
									node.m_nodeMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + Main)));
									if(File.Exists(Path.Combine(cTexPathHW, Tex + APR)))
										node.m_nodeMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + APR)));
									}
								node.m_lodRenderDistance = 2500;
								}
							if((netInfo.name.Equals("Small Rural Highway Slope")) && (netInfo.name.Contains("Small"))) {
								Tex = HW + "1L" + Sl + Nd;
								if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "Highway1L_Slope_Node_MainTex.dds")))
									node.m_nodeMaterial.SetTexture("_MainTex", LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "Highway1L_Slope_Node_MainTex.dds")));
								else if(File.Exists(Path.Combine(cTexPathHW, Tex + Main))) {
									node.m_nodeMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + Main)));
									if(File.Exists(Path.Combine(cTexPathHW, Tex + APR)))
										node.m_nodeMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + APR)));
									}
								node.m_lodRenderDistance = 2500;
								}
							if(netInfo.name.Equals("Small Rural Highway Tunnel")) {
								Tex = HW + "1L" + Tl + Nd;
								if(File.Exists(Path.Combine(cTexPathHW, Tex + Main))) {
									node.m_nodeMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + Main)));
									if(File.Exists(Path.Combine(cTexPathHW, Tex + APR)))
										node.m_nodeMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + APR)));
									}
								node.m_lodRenderDistance = 2500;
								}
							//Highway 2L
							if(netInfo.name.Equals("Rural Highway")) {
								Tex = HW + "2L" + Nd;
								if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "Highway2L_Ground_Node_MainTex.dds")))
									node.m_nodeMaterial.SetTexture("_MainTex", LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "Highway2L_Ground_Node_MainTex.dds")));
								else if(File.Exists(Path.Combine(cTexPathHW, Tex + Main))) {
									node.m_nodeMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + Main)));
									if(File.Exists(Path.Combine(cTexPathHW, Tex + APR)))
										node.m_nodeMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + APR)));
									}
								node.m_lodRenderDistance = 2500;
								}
							if(netInfo.name.Equals("Rural Highway Elevated")) {
								Tex = HW + "2L" + Nd;
								if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "Highway2L_Ground_Node_MainTex.dds")))
									node.m_nodeMaterial.SetTexture("_MainTex", LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "Highway2L_Ground_Node_MainTex.dds")));
								else if(File.Exists(Path.Combine(cTexPathHW, Tex + Main))) {
									node.m_nodeMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + Main)));
									if(File.Exists(Path.Combine(cTexPathHW, Tex + APR)))
										node.m_nodeMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + APR)));
									}
								node.m_lodRenderDistance = 2500;
								}
							if(netInfo.name.Equals("Rural Highway Slope")) {
								Tex = HW + "2L" + Sl + Nd;
								if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "Highway2L_Slope_Node_MainTex.dds")))
									node.m_nodeMaterial.SetTexture("_MainTex", LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "Highway2L_Slope_Node_MainTex.dds")));
								else if(File.Exists(Path.Combine(cTexPathHW, Tex + Main))) {
									node.m_nodeMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + Main)));
									if(File.Exists(Path.Combine(cTexPathHW, Tex + APR)))
										node.m_nodeMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + APR)));
									}
								node.m_lodRenderDistance = 2500;
								}
							if(netInfo.name.Equals("Rural Highway Tunnel")) {
								Tex = HW + "2L" + Tl + Nd;
								if(File.Exists(Path.Combine(cTexPathHW, Tex + Main))) {
									node.m_nodeMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + Main)));
									if(File.Exists(Path.Combine(cTexPathHW, Tex + APR)))
										node.m_nodeMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + APR)));
									}
								node.m_lodRenderDistance = 2500;
								}

							if(netInfo.name.Equals("Four-Lane Highway")) {
								Tex = HW + "4L" + Gd + Nd;
								if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "Highway4L_Ground_Node_MainTex.dds")))
									node.m_nodeMaterial.SetTexture("_MainTex", LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "Highway4L_Ground_Node_MainTex.dds")));
								else if(File.Exists(Path.Combine(cTexPathHW, Tex + Main))) {
									node.m_nodeMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + Main)));
									if(File.Exists(Path.Combine(cTexPathHW, Tex + APR)))
										node.m_nodeMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + APR)));
									}
								node.m_lodRenderDistance = 2500;
								}
							if(netInfo.name.Equals("Four-Lane Highway Elevated")) {
								Tex = HW + "4L" + El + Nd;
								if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "Highway4L_Elevated_Node_MainTex.dds")))
									node.m_nodeMaterial.SetTexture("_MainTex", LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "Highway4L_Elevated_Node_MainTex.dds")));
								else if(File.Exists(Path.Combine(cTexPathHW, Tex + Main))) {
									node.m_nodeMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + Main)));
									if(File.Exists(Path.Combine(cTexPathHW, Tex + APR)))
										node.m_nodeMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + APR)));
									}
								node.m_lodRenderDistance = 2500;
								}
							if(netInfo.name.Equals("Four-Lane Highway Slope")) {
								Tex = HW + "4L" + Sl + Nd;
								if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "Highway4L_Slope_Node_MainTex.dds")))
									node.m_nodeMaterial.SetTexture("_MainTex", LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "Highway4L_Slope_Node_MainTex.dds")));
								else if(File.Exists(Path.Combine(cTexPathHW, Tex + Main))) {
									node.m_nodeMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + Main)));
									if(File.Exists(Path.Combine(cTexPathHW, Tex + APR)))
										node.m_nodeMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + APR)));
									}
								node.m_lodRenderDistance = 2500;
								}
							if(netInfo.name.Equals("Four-Lane Highway Tunnel")) {
								Tex = HW + "4L" + Tl + Nd;
								if(File.Exists(Path.Combine(cTexPathHW, Tex + Main))) {
									node.m_nodeMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + Main)));
									if(File.Exists(Path.Combine(cTexPathHW, Tex + APR)))
										node.m_nodeMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + APR)));
									}
								node.m_lodRenderDistance = 2500;
								}

							if(netInfo.name.Equals("Five-Lane Highway")) {
								Tex = HW + "5L" + Nd;
								if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "Highway5L_Ground_Node_MainTex.dds")))
									node.m_nodeMaterial.SetTexture("_MainTex", LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "Highway5L_Ground_Node_MainTex.dds")));
								else if(File.Exists(Path.Combine(cTexPathHW, Tex + Main))) {
									node.m_nodeMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + Main)));
									Tex = HW + "5L" + Gd + Nd;
									if(File.Exists(Path.Combine(cTexPathHW, Tex + APR)))
										node.m_nodeMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + APR)));
									}
								node.m_lodRenderDistance = 2500;
								}
							if(netInfo.name.Equals("Five-Lane Highway Elevated")) {
								Tex = HW + "5L" + Nd;
								if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "Highway5L_Ground_Node_MainTex.dds")))
									node.m_nodeMaterial.SetTexture("_MainTex", LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "Highway5L_Ground_Node_MainTex.dds")));
								else if(File.Exists(Path.Combine(cTexPathHW, Tex + Main))) {
									node.m_nodeMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + Main)));
									Tex = HW + "5L" + El + Nd;
									if(File.Exists(Path.Combine(cTexPathHW, Tex + APR)))
										node.m_nodeMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + APR)));
									}
								node.m_lodRenderDistance = 2500;
								}
							if(netInfo.name.Equals("Five-Lane Highway Slope")) {
								Tex = HW + "5L" + Gd + Nd;
								if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "Highway5L_Slope_Node_MainTex.dds")))
									node.m_nodeMaterial.SetTexture("_MainTex", LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "Highway5L_Slope_Node_MainTex.dds")));
								else if(File.Exists(Path.Combine(cTexPathHW, Tex + Main))) {
									node.m_nodeMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + Main)));
									if(File.Exists(Path.Combine(cTexPathHW, Tex + APR)))
										node.m_nodeMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + APR)));
									}
								node.m_lodRenderDistance = 2500;
								}
							if(netInfo.name.Equals("Five-Lane Highway Tunnel")) {
								Tex = HW + "5L" + Tl + Nd;
								if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "Highway5L_Tunnel_Node_MainTex.dds")))
									node.m_nodeMaterial.SetTexture("_MainTex", LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "Highway5L_Tunnel_Node_MainTex.dds")));
								else if(File.Exists(Path.Combine(cTexPathHW, Tex + Main))) {
									node.m_nodeMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + Main)));
									if(File.Exists(Path.Combine(cTexPathHW, Tex + APR)))
										node.m_nodeMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + APR)));
									}
								node.m_lodRenderDistance = 2500;
								}

							if(netInfo.name.Equals("Large Highway")) {
								Tex = HW + "6L" + Nd;
								if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "Highway6L_Ground_Node_MainTex.dds")))
									node.m_nodeMaterial.SetTexture("_MainTex", LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "Highway6L_Ground_Node_MainTex.dds")));
								else if(File.Exists(Path.Combine(cTexPathHW, Tex + Main))) {
									node.m_nodeMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + Main)));
									Tex = HW + "6L" + Gd + Nd;
									if(File.Exists(Path.Combine(cTexPathHW, Tex + APR)))
										node.m_nodeMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + APR)));
									}
								node.m_lodRenderDistance = 2500;
								}
							if(netInfo.name.Equals("Large Highway Elevated")) {
								Tex = HW + "6L" + Nd;
								if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "Highway6L_Ground_Node_MainTex.dds")))
									node.m_nodeMaterial.SetTexture("_MainTex", LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "Highway6L_Ground_Node_MainTex.dds")));
								else if(File.Exists(Path.Combine(cTexPathHW, Tex + Main))) {
									node.m_nodeMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + Main)));
									Tex = HW + "6L" + El + Nd;
									if(File.Exists(Path.Combine(cTexPathHW, Tex + APR)))
										node.m_nodeMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + APR)));
									}
								node.m_lodRenderDistance = 2500;
								}
							if(netInfo.name.Equals("Large Highway Slope")) {
								Tex = HW + "6L" + Gd + Nd;
								if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "Highway6L_Slope_Node_MainTex.dds")))
									node.m_nodeMaterial.SetTexture("_MainTex", LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "Highway6L_Slope_Node_MainTex.dds")));
								else if(File.Exists(Path.Combine(cTexPathHW, Tex + Main))) {
									node.m_nodeMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + Main)));
									if(File.Exists(Path.Combine(cTexPathHW, Tex + APR)))
										node.m_nodeMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + APR)));
									}
								node.m_lodRenderDistance = 2500;
								}
							if(netInfo.name.Equals("Large Highway Tunnel")) {
								Tex = HW + "6L" + Tl + Nd;
								if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "Highway5L_Tunnel_Node_MainTex.dds")))
									node.m_nodeMaterial.SetTexture("_MainTex", LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "Highway5L_Tunnel_Node_MainTex.dds")));
								else if(File.Exists(Path.Combine(cTexPathHW, Tex + Main))) {
									node.m_nodeMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + Main)));
									if(File.Exists(Path.Combine(cTexPathHW, Tex + APR)))
										node.m_nodeMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + APR)));
									}
								node.m_lodRenderDistance = 2500;
								}

							#endregion

							#region NExt Highways Nodes APRMaps

							if(netInfo.name.Contains("Rural Highway") && netInfo.name.Contains("Small")) {
								if(node.m_nodeMaterial.GetTexture("_APRMap").name.Contains("Ground"))
									if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "Highway1L_Ground_Node_APRMap.dds")))
										node.m_nodeMaterial.SetTexture("_APRMap", LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "Highway1L_Ground_Node_APRMap.dds")));

								if(node.m_nodeMaterial.GetTexture("_APRMap").name.Contains("Elevated"))
									if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "Highway1L_Ground_Node_APRMap.dds")))
										node.m_nodeMaterial.SetTexture("_APRMap", LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "Highway1L_Ground_Node_APRMap.dds")));

								if(node.m_nodeMaterial.GetTexture("_APRMap").name.Contains("Slope"))
									if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "Highway1L_Slope_Node_APRMap.dds")))
										node.m_nodeMaterial.SetTexture("_APRMap", LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "Highway1L_Slope_Node_APRMap.dds")));

								node.m_lodRenderDistance = 2500;
								}

							if(netInfo.name.Contains("Rural Highway") && !netInfo.name.Contains("Small")) {
								if(node.m_nodeMaterial.GetTexture("_APRMap").name.Contains("Ground"))
									if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "Highway2L_Ground_Node_APRMap.dds")))
										node.m_nodeMaterial.SetTexture("_APRMap", LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "Highway2L_Ground_Node_APRMap.dds")));

								if(node.m_nodeMaterial.GetTexture("_APRMap").name.Contains("Elevated"))
									if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "Highway2L_Elevated_Node_APRMap.dds")))
										node.m_nodeMaterial.SetTexture("_APRMap", LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "Highway2L_Elevated_Node_APRMap.dds")));

								if(node.m_nodeMaterial.GetTexture("_APRMap").name.Contains("Slope"))
									if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "Highway2L_Slope_Node_APRMap.dds")))
										node.m_nodeMaterial.SetTexture("_APRMap", LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "Highway2L_Slope_Node_APRMap.dds")));

								node.m_lodRenderDistance = 2500;
								}

							if(netInfo.name.Contains("Four-Lane Highway")) {
								if(node.m_nodeMaterial.GetTexture("_APRMap").name.Contains("Ground"))
									if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "Highway4L_Ground_Node_APRMap.dds")))
										node.m_nodeMaterial.SetTexture("_APRMap", LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "Highway4L_Ground_Node_APRMap.dds")));

								if(node.m_nodeMaterial.GetTexture("_APRMap").name.Contains("Elevated"))
									if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "Highway4L_Elevated_Node_APRMap.dds")))
										node.m_nodeMaterial.SetTexture("_APRMap", LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "Highway4L_Elevated_Node_APRMap.dds")));

								if(node.m_nodeMaterial.GetTexture("_APRMap").name.Contains("Slope"))
									if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "Highway4L_Slope_Node_APRMap.dds")))
										node.m_nodeMaterial.SetTexture("_APRMap", LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "Highway4L_Slope_Node_APRMap.dds")));

								node.m_lodRenderDistance = 2500;
								}

							if(netInfo.name.Contains("Five-Lane Highway")) {
								if(node.m_nodeMaterial.GetTexture("_APRMap").name.Contains("Ground"))
									if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "Highway5L_Ground_Node_APRMap.dds")))
										node.m_nodeMaterial.SetTexture("_APRMap", LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "Highway5L_Ground_Node_APRMap.dds")));

								if(node.m_nodeMaterial.GetTexture("_APRMap").name.Contains("Elevated"))
									if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "Highway5L_Elevated_Node_APRMap.dds")))
										node.m_nodeMaterial.SetTexture("_APRMap", LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "Highway5L_Elevated_Node_APRMap.dds")));

								if(node.m_nodeMaterial.GetTexture("_APRMap").name.Contains("Slope"))
									if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "Highway5L_Slope_Node_APRMap.dds")))
										node.m_nodeMaterial.SetTexture("_APRMap", LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "Highway5L_Slope_Node_APRMap.dds")));

								node.m_lodRenderDistance = 2500;
								}

							if(netInfo.name.Contains("Large Highway")) {
								if(node.m_nodeMaterial.GetTexture("_APRMap").name.Contains("Ground"))
									if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "Highway6L_Ground_Node_APRMap.dds")))
										node.m_nodeMaterial.SetTexture("_APRMap", LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "Highway6L_Ground_Node_APRMap.dds")));

								if(node.m_nodeMaterial.GetTexture("_APRMap").name.Contains("Elevated"))
									if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "Highway6L_Elevated_Node_APRMap.dds")))
										node.m_nodeMaterial.SetTexture("_APRMap", LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "Highway6L_Elevated_Node_APRMap.dds")));

								if(node.m_nodeMaterial.GetTexture("_APRMap").name.Contains("Slope"))
									if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "Highway6L_Slope_Node_APRMap.dds")))
										node.m_nodeMaterial.SetTexture("_APRMap", LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "Highway6L_Slope_Node_APRMap.dds")));

								node.m_lodRenderDistance = 2500;
								}
							#endregion
							}
						}
					#endregion

					#region NExt Segments

					NetInfo.Segment[] segments = netInfo.m_segments;
					for(int l = 0;l < segments.Length;l++) {
						NetInfo.Segment segment = segments[l];

						if((segment.m_segmentMaterial.GetTexture("_MainTex") != null) && (!segment.m_material.name.ToLower().Contains("cable"))) {

							#region NExt TinyRoads Default

							if(netInfo.name.Equals("Two-Lane Alley")) {
								Tex = RT + "2L";
								if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "Alley2L_Ground_Segment_MainTex.dds")))
									segment.m_segmentMaterial.SetTexture("_MainTex", LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "Alley2L_Ground_Segment_MainTex.dds")));
								else if(File.Exists(Path.Combine(cTexPathRT, Tex + Main))) {
									segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRT, Tex + Main)));
									if(File.Exists(Path.Combine(cTexPathRT, Tex + APR)))
										segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRT, Tex + APR)));
									} else {
									Tex = RT + "1L";
									if(File.Exists(Path.Combine(cTexPathRT, Tex + Main)))
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRT, Tex + Main)));
									if(File.Exists(Path.Combine(cTexPathRT, Tex + APR)))
										segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRT, Tex + APR)));
									}
								segment.m_lodRenderDistance = 2500;
								}

							if(netInfo.name.Equals("One-Lane Oneway")) {
								Tex = RT + "1L";
								if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "OneWay1L_Ground_Segment_MainTex.dds")))
									segment.m_segmentMaterial.SetTexture("_MainTex", LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "OneWay1L_Ground_Segment_MainTex.dds")));
								else if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "Alley2L_Ground_Segment_MainTex.dds")))
									segment.m_segmentMaterial.SetTexture("_MainTex", LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "Alley2L_Ground_Segment_MainTex.dds")));
								else if(File.Exists(Path.Combine(cTexPathRT, Tex + Main))) {
									segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRT, Tex + Main)));
									if(File.Exists(Path.Combine(cTexPathRT, Tex + APR)))
										segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRT, Tex + APR)));
									}
								segment.m_lodRenderDistance = 2500;
								}

							#endregion

							#region NExt SmallHeavyRoads Default
							if(netInfo.name.Contains("BasicRoadTL")) {//mesh//small-tunnel-segment||highway-tunnel-slope||slope
								if(segment.m_segmentMaterial.GetTexture(apr).name.Contains("Ground")) {
									Tex = RS + "TL" + Gd;
									if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "BasicRoadTL_Ground_Segment_MainTex.dds")))
										segment.m_segmentMaterial.SetTexture("_MainTex", LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "BasicRoadTL_Ground_Segment_MainTex.dds")));
									if(File.Exists(Path.Combine(cTexPathRS, Tex + Main)))
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + Main)));
									if(File.Exists(Path.Combine(cTexPathRS, Tex + APR)))
										segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + APR)));
									}
								else if(segment.m_segmentMaterial.GetTexture(apr).name.Contains("Elevated")) {
									Tex = RS + "TL" + El;
									if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "BasicRoadTL_Elevated_Segment_MainTex.dds")))
										segment.m_segmentMaterial.SetTexture("_MainTex", LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "BasicRoadTL_Elevated_Segment_MainTex.dds")));
									if(File.Exists(Path.Combine(cTexPathRS, Tex + Main)))
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + Main)));
									if(File.Exists(Path.Combine(cTexPathRS, Tex + APR)))
										segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + APR)));
									}
								else if(segment.m_segmentMaterial.GetTexture(apr).name.Contains("Tunnel")) {
									Tex = RS + "TL" + Tl;
									if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "BasicRoadTL_Tunnel_Segment_MainTex.dds")))
										segment.m_segmentMaterial.SetTexture("_MainTex", LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "BasicRoadTL_Tunnel_Segment_MainTex.dds")));
									if(File.Exists(Path.Combine(cTexPathRS, Tex + Main)))
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + Main)));
									if(File.Exists(Path.Combine(cTexPathRS, Tex + APR)))
										segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + APR)));
									}

								segment.m_lodRenderDistance = 2500;
								}
							if(netInfo.name.Contains("Small Avenue")) {
								if(segment.m_segmentMaterial.GetTexture(apr).name.Contains("Ground")) {
									Tex = RS + "4L" + Gd;
									if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "SmallAvenue4L_Ground_Segment_MainTex.dds")))
										segment.m_segmentMaterial.SetTexture("_MainTex", LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "SmallAvenue4L_Ground_Segment_MainTex.dds")));
									if(File.Exists(Path.Combine(cTexPathRS, Tex + Main)))
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + Main)));
									if(File.Exists(Path.Combine(cTexPathRS, Tex + APR)))
										segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + APR)));
									} else if(segment.m_segmentMaterial.GetTexture(apr).name.Contains("Elevated")) {
									Tex = RS + "4L" + El;
									if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "SmallAvenue4L_Elevated_Segment_MainTex.dds")))
										segment.m_segmentMaterial.SetTexture("_MainTex", LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "SmallAvenue4L_Elevated_Segment_MainTex.dds")));
									if(File.Exists(Path.Combine(cTexPathRS, Tex + Main)))
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + Main)));
									if(File.Exists(Path.Combine(cTexPathRS, Tex + APR)))
										segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + APR)));
									} else if(segment.m_segmentMaterial.GetTexture(apr).name.Contains("Tunnel")) {
									Tex = RS + "4L" + Tl;
									if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "SmallAvenue4L_Tunnel_Segment_MainTex.dds")))
										segment.m_segmentMaterial.SetTexture("_MainTex", LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "SmallAvenue4L_Tunnel_Segment_MainTex.dds")));
									if(File.Exists(Path.Combine(cTexPathRS, Tex + Main)))
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + Main)));
									if(File.Exists(Path.Combine(cTexPathRS, Tex + APR)))
										segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + APR)));
									}
								segment.m_lodRenderDistance = 2500;
								}
							if(netInfo.name.Contains("Oneway3L")) {//mesh
								if(segment.m_segmentMaterial.GetTexture(apr).name.Contains("Ground")) {
									Tex = RS + Ow + "3L" + Gd;
									if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "OneWay3L_Ground_Segment_MainTex.dds")))
										segment.m_segmentMaterial.SetTexture("_MainTex", LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "OneWay3L_Ground_Segment_MainTex.dds")));
									if(File.Exists(Path.Combine(cTexPathRS, Tex + Main)))
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + Main)));
									if(File.Exists(Path.Combine(cTexPathRS, Tex + APR)))
										segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + APR)));
									} else if(segment.m_segmentMaterial.GetTexture(apr).name.Contains("Elevated")) {
									Tex = RS + Ow + "3L" + El;
									if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "OneWay3L_Elevated_Segment_MainTex.dds")))
										segment.m_segmentMaterial.SetTexture("_MainTex", LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "OneWay3L_Elevated_Segment_MainTex.dds")));
									if(File.Exists(Path.Combine(cTexPathRS, Tex + Main)))
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + Main)));
									if(File.Exists(Path.Combine(cTexPathRS, Tex + APR)))
										segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + APR)));
									} else if(segment.m_segmentMaterial.GetTexture(apr).name.Contains("Tunnel")) {
									Tex = RS + Ow + "3L" + Tl;
									if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "OneWay3L_Tunnel_Segment_MainTex.dds")))
										segment.m_segmentMaterial.SetTexture("_MainTex", LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "OneWay3L_Tunnel_Segment_MainTex.dds")));
									if(File.Exists(Path.Combine(cTexPathRS, Tex + Main)))
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + Main)));
									if(File.Exists(Path.Combine(cTexPathRS, Tex + APR)))
										segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + APR)));
									}
								segment.m_lodRenderDistance = 2500;
								}
							if(netInfo.name.Contains("Oneway4L")) {
								if(segment.m_segmentMaterial.GetTexture(apr).name.Contains("Ground")) {
									Tex = RS + Ow + "4L" + Gd;
									if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "OneWay4L_Ground_Segment_MainTex.dds")))
										segment.m_segmentMaterial.SetTexture("_MainTex", LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "OneWay4L_Ground_Segment_MainTex.dds")));
									if(File.Exists(Path.Combine(cTexPathRS, Tex + Main)))
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + Main)));
									if(File.Exists(Path.Combine(cTexPathRS, Tex + APR)))
										segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + APR)));
									}

								if(segment.m_segmentMaterial.GetTexture(apr).name.Contains("Elevated")) {
									Tex = RS + Ow + "4L" + El;
									if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "OneWay4L_Elevated_Segment_MainTex.dds")))
										segment.m_segmentMaterial.SetTexture("_MainTex", LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "OneWay4L_Elevated_Segment_MainTex.dds")));
									if(File.Exists(Path.Combine(cTexPathRS, Tex + Main)))
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + Main)));
									if(File.Exists(Path.Combine(cTexPathRS, Tex + APR)))
										segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + APR)));
									}

								if(segment.m_segmentMaterial.GetTexture(apr).name.Contains("Tunnel")) {
									Tex = RS + Ow + "4L" + Tl;
									if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "OneWay4L_Tunnel_Segment_MainTex.dds")))
										segment.m_segmentMaterial.SetTexture("_MainTex", LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "OneWay4L_Tunnel_Segment_MainTex.dds")));
									if(File.Exists(Path.Combine(cTexPathRS, Tex + Main)))
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + Main)));
									if(File.Exists(Path.Combine(cTexPathRS, Tex + APR)))
										segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + APR)));
									}
								segment.m_lodRenderDistance = 2500;
								}
							if(netInfo.name.Contains("AsymRoadL1R2")) {
								if((netInfo.name.Equals("AsymRoadL1R2")))//&& (!((netInfo.name.Contains("Elevated")) || (netInfo.name.Contains("Bridge")) || (netInfo.name.Contains("Slope")) || (netInfo.name.Contains("Tunnel")))))
								{
									Tex = RS + A3 + Gd;
									if(segment.m_segmentMaterial.GetTexture(apr).name.Contains("Inverted")) {
										Tex += "_Inv";
										if(File.Exists(Path.Combine(cTexDefault, Tex + Main))) {
											segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexDefault, Tex + Main)));
											if(File.Exists(Path.Combine(cTexDefault, Tex + APR)))
												segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexDefault, Tex + APR)));
											} else if(File.Exists(Path.Combine(cTexPathRS, Tex + Main))) {
											segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + Main)));
											if(File.Exists(Path.Combine(cTexPathRS, Tex + APR)))
												segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + APR)));
											}
										} else {
										if(File.Exists(Path.Combine(cTexDefault, Tex + Main))) {
											segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexDefault, Tex + Main)));
											if(File.Exists(Path.Combine(cTexDefault, Tex + APR)))
												segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexDefault, Tex + APR)));
											} else if(File.Exists(Path.Combine(cTexPathRS, Tex + Main))) {
											segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + Main)));
											if(File.Exists(Path.Combine(cTexPathRS, Tex + APR)))
												segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + APR)));
											}
										}
									} else {
									if((netInfo.name.Equals("AsymRoadL1R2 Elevated")) || (netInfo.name.Equals("AsymRoadL1R2 Bridge"))) {
										if(segment.m_segmentMaterial.GetTexture(apr).name.Contains("Inverted")) {
											Tex = RS + A3 + El + "_Inv";
											if(File.Exists(Path.Combine(cTexDefault, Tex + Main))) {
												segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexDefault, Tex + Main)));
												if(File.Exists(Path.Combine(cTexDefault, Tex + APR)))
													segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexDefault, Tex + APR)));
												} else if(File.Exists(Path.Combine(cTexPathRS, Tex + Main))) {
												segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + Main)));
												if(File.Exists(Path.Combine(cTexPathRS, Tex + APR)))
													segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + APR)));
												}
											} else {
											Tex = RS + A3 + El;
											if(File.Exists(Path.Combine(cTexDefault, Tex + Main))) {
												segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexDefault, Tex + Main)));
												if(File.Exists(Path.Combine(cTexDefault, Tex + APR)))
													segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexDefault, Tex + APR)));
												} else if(File.Exists(Path.Combine(cTexPathRS, Tex + Main))) {
												segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + Main)));
												if(File.Exists(Path.Combine(cTexPathRS, Tex + APR)))
													segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + APR)));
												}
											}
										} else if((netInfo.name.Equals("AsymRoadL1R2 Slope")) && (segment.m_mesh.name == "Slope")) {
										if(segment.m_segmentMaterial.GetTexture(apr).name.Contains("Inverted")) {
											Tex = RS + A3 + Gd + "_Inv";
											if(File.Exists(Path.Combine(cTexDefault, Tex + Main))) {
												segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexDefault, Tex + Main)));
												if(File.Exists(Path.Combine(cTexDefault, Tex + APR)))
													segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexDefault, Tex + APR)));
												} else if(File.Exists(Path.Combine(cTexPathRS, Tex + Main))) {
												segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + Main)));
												if(File.Exists(Path.Combine(cTexPathRS, Tex + APR)))
													segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + APR)));
												}
											} else {
											Tex = RS + A3 + Gd;
											if(File.Exists(Path.Combine(cTexDefault, Tex + Main))) {
												segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexDefault, Tex + Main)));
												if(File.Exists(Path.Combine(cTexDefault, Tex + APR)))
													segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexDefault, Tex + APR)));
												} else if(File.Exists(Path.Combine(cTexPathRS, Tex + Main))) {
												segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + Main)));
												if(File.Exists(Path.Combine(cTexPathRS, Tex + APR)))
													segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + APR)));
												}
											}
										} else if(netInfo.name.Equals("AsymRoadL1R2 Tunnel")) {
										if(segment.m_segmentMaterial.GetTexture(apr).name.Contains("Inverted")) {
											Tex = RS + A3 + Tl + "_Inv";
											if(File.Exists(Path.Combine(cTexDefault, Tex + Main))) {
												segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexDefault, Tex + Main)));
												if(File.Exists(Path.Combine(cTexDefault, Tex + APR)))
													segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexDefault, Tex + APR)));
												} else if(File.Exists(Path.Combine(cTexPathRS, Tex + Main))) {
												segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + Main)));
												if(File.Exists(Path.Combine(cTexPathRS, Tex + APR)))
													segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + APR)));
												}
											} else {
											Tex = RS + A3 + Tl;
											if(File.Exists(Path.Combine(cTexDefault, Tex + Main))) {
												segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexDefault, Tex + Main)));
												if(File.Exists(Path.Combine(cTexDefault, Tex + APR)))
													segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexDefault, Tex + APR)));
												} else if(File.Exists(Path.Combine(cTexPathRS, Tex + Main))) {
												segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + Main)));
												if(File.Exists(Path.Combine(cTexPathRS, Tex + APR)))
													segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + APR)));
												}
											}
										}
									}
								segment.m_lodRenderDistance = 2500;
								}
							if(netInfo.name.Contains("AsymRoadL1R3")) {
								if((netInfo.name.Equals("AsymRoadL1R3")))
								{
									Tex = RS + A4 + Gd;
									if(segment.m_segmentMaterial.GetTexture(apr).name.Contains("Inverted")) {
										Tex += "_Inv";
										if(File.Exists(Path.Combine(cTexDefault, Tex + Main))) {
											segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexDefault, Tex + Main)));
											if(File.Exists(Path.Combine(cTexDefault, Tex + APR)))
												segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexDefault, Tex + APR)));
											} else if(File.Exists(Path.Combine(cTexPathRS, Tex + Main))) {
											segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + Main)));
											if(File.Exists(Path.Combine(cTexPathRS, Tex + APR)))
												segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + APR)));
											}
										} else {
										if(File.Exists(Path.Combine(cTexDefault, Tex + Main))) {
											segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexDefault, Tex + Main)));
											if(File.Exists(Path.Combine(cTexDefault, Tex + APR)))
												segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexDefault, Tex + APR)));
											} else if(File.Exists(Path.Combine(cTexPathRS, Tex + Main))) {
											segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + Main)));
											if(File.Exists(Path.Combine(cTexPathRS, Tex + APR)))
												segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + APR)));
											}
										}
									} else {
									if((netInfo.name.Equals("AsymRoadL1R3 Elevated")) || (netInfo.name.Equals("AsymRoadL1R3 Bridge"))) {
										if(segment.m_segmentMaterial.GetTexture(apr).name.Contains("Inverted")) {
											Tex = RS + A4 + El + "_Inv";
											if(File.Exists(Path.Combine(cTexDefault, Tex + Main))) {
												segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexDefault, Tex + Main)));
												if(File.Exists(Path.Combine(cTexDefault, Tex + APR)))
													segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexDefault, Tex + APR)));
												} else if(File.Exists(Path.Combine(cTexPathRS, Tex + Main))) {
												segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + Main)));
												if(File.Exists(Path.Combine(cTexPathRS, Tex + APR)))
													segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + APR)));
												}
											} else {
											Tex = RS + A4 + El;
											if(File.Exists(Path.Combine(cTexDefault, Tex + Main))) {
												segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexDefault, Tex + Main)));
												if(File.Exists(Path.Combine(cTexDefault, Tex + APR)))
													segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexDefault, Tex + APR)));
												} else if(File.Exists(Path.Combine(cTexPathRS, Tex + Main))) {
												segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + Main)));
												if(File.Exists(Path.Combine(cTexPathRS, Tex + APR)))
													segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + APR)));
												}
											}
										} else if((netInfo.name.Equals("AsymRoadL1R3 Slope")) && (segment.m_mesh.name == "Slope")) {
										if(segment.m_segmentMaterial.GetTexture(apr).name.Contains("Inverted")) {
											Tex = RS + A4 + Gd + "_Inv";
											if(File.Exists(Path.Combine(cTexDefault, Tex + Main))) {
												segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexDefault, Tex + Main)));
												if(File.Exists(Path.Combine(cTexDefault, Tex + APR)))
													segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexDefault, Tex + APR)));
												} else if(File.Exists(Path.Combine(cTexPathRS, Tex + Main))) {
												segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + Main)));
												if(File.Exists(Path.Combine(cTexPathRS, Tex + APR)))
													segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + APR)));
												}
											} else {
											Tex = RS + A4 + Gd;
											if(File.Exists(Path.Combine(cTexDefault, Tex + Main))) {
												segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexDefault, Tex + Main)));
												if(File.Exists(Path.Combine(cTexDefault, Tex + APR)))
													segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexDefault, Tex + APR)));
												} else if(File.Exists(Path.Combine(cTexPathRS, Tex + Main))) {
												segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + Main)));
												if(File.Exists(Path.Combine(cTexPathRS, Tex + APR)))
													segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + APR)));
												}
											}
										} else if(netInfo.name.Equals("AsymRoadL1R3 Tunnel")) {
										if(segment.m_segmentMaterial.GetTexture(apr).name.Contains("Inverted")) {
											Tex = RS + A3 + Tl + "_Inv";
											if(File.Exists(Path.Combine(cTexDefault, Tex + Main))) {
												segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexDefault, Tex + Main)));
												if(File.Exists(Path.Combine(cTexDefault, Tex + APR)))
													segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexDefault, Tex + APR)));
												} else if(File.Exists(Path.Combine(cTexPathRS, Tex + Main))) {
												segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + Main)));
												if(File.Exists(Path.Combine(cTexPathRS, Tex + APR)))
													segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + APR)));
												}
											} else {
											Tex = RS + A3 + Tl;
											if(File.Exists(Path.Combine(cTexDefault, Tex + Main))) {
												segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexDefault, Tex + Main)));
												if(File.Exists(Path.Combine(cTexDefault, Tex + APR)))
													segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexDefault, Tex + APR)));
												} else if(File.Exists(Path.Combine(cTexPathRS, Tex + Main))) {
												segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + Main)));
												if(File.Exists(Path.Combine(cTexPathRS, Tex + APR)))
													segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + APR)));
												}
											}
										}
									}
								segment.m_lodRenderDistance = 2500;
								}
							#endregion

							#region NExt Avenues Default

							if(netInfo.name.Contains("Medium Avenue") && !netInfo.name.Contains("TL")) {
								if(segment.m_segmentMaterial.GetTexture(main).name.Contains("Ground")) {
									Tex = RM + "4L" + Gd;
									if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "MediumAvenue4L_Ground_Segment_MainTex.dds"))) {
										segment.m_segmentMaterial.SetTexture("_MainTex", LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "MediumAvenue4L_Ground_Segment_MainTex.dds")));
										if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "RoadLargeSegment-default-apr.dds")))
											segment.m_segmentMaterial.SetTexture("_APRMap", LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "RoadLargeSegment-default-apr.dds")));
										else if(File.Exists(Path.Combine(ModLoader.APRMaps_Path, "RoadLargeSegment-default-apr.dds")))
											segment.m_segmentMaterial.SetTexture("_APRMap", LoadTextureDDS(Path.Combine(ModLoader.APRMaps_Path, "RoadLargeSegment-default-apr.dds")));
										} else if(File.Exists(Path.Combine(cTexPathRM, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRM, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPathRM, Tex + APR)))
											segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRM, Tex + APR)));
										}
									} else if(segment.m_segmentMaterial.GetTexture(main).name.Contains("Elevated")) {
									Tex = RM + "4L" + El;
									if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "MediumAvenue4L_Elevated_Segment_MainTex.dds")))
										segment.m_segmentMaterial.SetTexture("_MainTex", LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "MediumAvenue4L_Elevated_Segment_MainTex.dds")));
									else if(File.Exists(Path.Combine(cTexPathRM, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRM, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPathRM, Tex + APR)))
											segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRM, Tex + APR)));
										}
									} else if(segment.m_segmentMaterial.GetTexture(main).name.Contains("Slope")) {
									Tex = RM + "4L" + Sl;
									if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "MediumAvenue4L_Slope_Segment_MainTex.dds")))
										segment.m_segmentMaterial.SetTexture("_MainTex", LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "MediumAvenue4L_Slope_Segment_MainTex.dds")));
									else if(File.Exists(Path.Combine(cTexPathRM, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRM, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPathRM, Tex + APR)))
											segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRM, Tex + APR)));
										}
									} else if(segment.m_segmentMaterial.GetTexture(main).name.Contains("Tunnel")) {
									Tex = RM + "4L" + Tl;
									if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "MediumAvenue4L_Tunnel_Segment_MainTex.dds")))
										segment.m_segmentMaterial.SetTexture("_MainTex", LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "MediumAvenue4L_Tunnel_Segment_MainTex.dds")));
									else if(File.Exists(Path.Combine(cTexPathRM, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRM, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPathRM, Tex + APR)))
											segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRM, Tex + APR)));
										}
									}
								segment.m_lodRenderDistance = 2500;
								}

							if(netInfo.name.Contains("Medium Avenue") && netInfo.name.Contains("TL")) {
								if(segment.m_segmentMaterial.GetTexture(main).name.Contains("Ground")) {
									Tex = RM + "4LTL" + Gd;
									if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "MediumAvenue4LTL_Ground_Segment_MainTex.dds"))) {
										segment.m_segmentMaterial.SetTexture("_MainTex", LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "MediumAvenue4LTL_Ground_Segment_MainTex.dds")));
										if(segment.m_segmentMaterial.GetTexture("_APRMap").name.Contains("Ground")) {
											if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "RoadLargeSegment-default-apr.dds")))
												segment.m_segmentMaterial.SetTexture("_APRMap", LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "RoadLargeSegment-default-apr.dds")));
											else
												if(File.Exists(Path.Combine(ModLoader.APRMaps_Path, "RoadLargeSegment-default-apr.dds")))
													segment.m_segmentMaterial.SetTexture("_APRMap", LoadTextureDDS(Path.Combine(ModLoader.APRMaps_Path, "RoadLargeSegment-default-apr.dds")));
											}
										} else if(File.Exists(Path.Combine(cTexPathRM, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRM, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPathRM, Tex + APR)))
											segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRM, Tex + APR)));
										}
									} else if(segment.m_segmentMaterial.GetTexture(main).name.Contains("Elevated")) {
									Tex = RM + "4LTL" + El;
									if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "MediumAvenue4LTL_Elevated_Segment_MainTex.dds"))) {
										segment.m_segmentMaterial.SetTexture("_MainTex", LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "MediumAvenue4LTL_Elevated_Segment_MainTex.dds")));
										} else if(File.Exists(Path.Combine(cTexPathRM, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRM, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPathRM, Tex + APR)))
											segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRM, Tex + APR)));
										}
									} else if(segment.m_segmentMaterial.GetTexture(main).name.Contains("Slope")) {
									Tex = RM + "4LTL" + Sl;
									if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "MediumAvenue4LTL_Slope_Segment_MainTex.dds")))
										segment.m_segmentMaterial.SetTexture("_MainTex", LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "MediumAvenue4LTL_Slope_Segment_MainTex.dds")));
									else if(File.Exists(Path.Combine(cTexPathRM, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRM, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPathRM, Tex + APR)))
											segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRM, Tex + APR)));
										}
									} else if(segment.m_segmentMaterial.GetTexture(main).name.Contains("Tunnel")) {
									Tex = RM + "4LTL" + Tl;
									if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "MediumAvenue4LTL_Tunnel_Segment_MainTex.dds")))
										segment.m_segmentMaterial.SetTexture("_MainTex", LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "MediumAvenue4LTL_Tunnel_Segment_MainTex.dds")));
									else if(File.Exists(Path.Combine(cTexPathRM, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRM, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPathRM, Tex + APR)))
											segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRM, Tex + APR)));
										}
									}
								segment.m_lodRenderDistance = 2500;
								}

							if(netInfo.name.Contains("Eight-Lane Avenue")) {
								Tex = RL + "8L";
								segment.m_lodRenderDistance = 2500;
								if((netInfo.name.Equals("Eight-Lane Avenue Elevated")) || (netInfo.name.ToLower().Contains("bridge"))) {
									Tex += El;
									if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "LargeAvenue8LM_Elevated_Segment_MainTex.dds")))
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "LargeAvenue8LM_Elevated_Segment_MainTex.dds")));
									else if(File.Exists(Path.Combine(cTexPathRL, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRL, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPathRL, Tex + APR)))
											segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRL, Tex + APR)));
										}
									} else if(netInfo.name.Equals("Eight-Lane Avenue Slope")) {
									Tex += Sl;
									if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "LargeAvenue8LM_Slope_Segment_MainTex.dds")))
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "LargeAvenue8LM_Slope_Segment_MainTex.dds")));
									else if(File.Exists(Path.Combine(cTexPathRL, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRL, Tex + Main)));
										//if(File.Exists(Path.Combine(cTexPathRL,Tex+APR)))
										//segment.m_segmentMaterial.SetTexture(apr,LoadTextureDDS(Path.Combine(cTexPathRL,Tex+APR)));
										}
									} else if(netInfo.name.Equals("Eight-Lane Avenue Tunnel")) {
									Tex += Tl;
									if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "LargeAvenue8LM_Tunnel_Segment_MainTex.dds")))
										segment.m_segmentMaterial.SetTexture("_MainTex", LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "LargeAvenue8LM_Tunnel_Segment_MainTex.dds")));
									else if(File.Exists(Path.Combine(cTexPathRL, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRL, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPathRL, Tex + APR)))
											segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRL, Tex + APR)));
										}
									} else {
									Tex += Gd;
									if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "LargeAvenue8LM_Ground_Segment_MainTex.dds")))
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "LargeAvenue8LM_Ground_Segment_MainTex.dds")));
									else if(File.Exists(Path.Combine(cTexPathRL, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRL, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPathRL, Tex + APR)))
											segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRL, Tex + APR)));
										}
									}
								}
					
							if(netInfo.name.Contains("AsymAvenueL2R3")) {
								if((netInfo.name.Equals("AsymAvenueL2R3")))
								{
									Tex = RM + A5  + Gd;
									if(segment.m_segmentMaterial.GetTexture(apr).name.Contains("Inverted")) {
										Tex += "_Inv";
										if(File.Exists(Path.Combine(cTexDefault, Tex + Main))) {
											segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexDefault, Tex + Main)));
											if(File.Exists(Path.Combine(cTexDefault, Tex + APR)))
												segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexDefault, Tex + APR)));
											}
										else if(File.Exists(Path.Combine(cTexPathRS, Tex + Main))) {
											segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + Main)));
											if(File.Exists(Path.Combine(cTexPathRS, Tex + APR)))
												segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + APR)));
											}
										}
									else {
										if(File.Exists(Path.Combine(cTexDefault, Tex + Main))) {
											segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexDefault, Tex + Main)));
											if(File.Exists(Path.Combine(cTexDefault, Tex + APR)))
												segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexDefault, Tex + APR)));
											} else if(File.Exists(Path.Combine(cTexPathRS, Tex + Main))) {
											segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + Main)));
											if(File.Exists(Path.Combine(cTexPathRS, Tex + APR)))
												segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + APR)));
											}
										}
									}
								else {
									if((netInfo.name.Equals("AsymAvenueL2R3 Elevated")) || (netInfo.name.Equals("AsymAvenueL2R3 Bridge"))) {
										if(segment.m_segmentMaterial.GetTexture(apr).name.Contains("Inverted")) {
											Tex = RM + A5 + El + "_Inv";
											if(File.Exists(Path.Combine(cTexDefault, Tex + Main))) {
												segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexDefault, Tex + Main)));
												if(File.Exists(Path.Combine(cTexDefault, Tex + APR)))
													segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexDefault, Tex + APR)));
												} else if(File.Exists(Path.Combine(cTexPathRS, Tex + Main))) {
												segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + Main)));
												if(File.Exists(Path.Combine(cTexPathRS, Tex + APR)))
													segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + APR)));
												}
											}
										else {
											Tex = RM + A5 + El;
											if(File.Exists(Path.Combine(cTexDefault, Tex + Main))) {
												segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexDefault, Tex + Main)));
												if(File.Exists(Path.Combine(cTexDefault, Tex + APR)))
													segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexDefault, Tex + APR)));
												} else if(File.Exists(Path.Combine(cTexPathRS, Tex + Main))) {
												segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + Main)));
												if(File.Exists(Path.Combine(cTexPathRS, Tex + APR)))
													segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + APR)));
												}
											}
										}
									else if((netInfo.name.Equals("AsymAvenueL2R3 Slope")) && (segment.m_mesh.name == "Slope")) {
										if(segment.m_segmentMaterial.GetTexture(apr).name.Contains("Inverted")) {
											Tex = RM + A5 + Gd + "_Inv";
											if(File.Exists(Path.Combine(cTexDefault, Tex + Main))) {
												segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexDefault, Tex + Main)));
												if(File.Exists(Path.Combine(cTexDefault, Tex + APR)))
													segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexDefault, Tex + APR)));
												} else if(File.Exists(Path.Combine(cTexPathRS, Tex + Main))) {
												segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + Main)));
												if(File.Exists(Path.Combine(cTexPathRS, Tex + APR)))
													segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + APR)));
												}
											}
										else {
											Tex = RM + A5 + Gd;
											if(File.Exists(Path.Combine(cTexDefault, Tex + Main))) {
												segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexDefault, Tex + Main)));
												if(File.Exists(Path.Combine(cTexDefault, Tex + APR)))
													segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexDefault, Tex + APR)));
												} else if(File.Exists(Path.Combine(cTexPathRS, Tex + Main))) {
												segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + Main)));
												if(File.Exists(Path.Combine(cTexPathRS, Tex + APR)))
													segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + APR)));
												}
											}
										}
									else if(netInfo.name.Equals("AsymAvenueL2R3 Tunnel")) {
										if(segment.m_segmentMaterial.GetTexture(apr).name.Contains("Inverted")) {
											Tex = RM + A5 + Tl + "_Inv";
											if(File.Exists(Path.Combine(cTexDefault, Tex + Main))) {
												segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexDefault, Tex + Main)));
												if(File.Exists(Path.Combine(cTexDefault, Tex + APR)))
													segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexDefault, Tex + APR)));
												} else if(File.Exists(Path.Combine(cTexPathRS, Tex + Main))) {
												segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + Main)));
												if(File.Exists(Path.Combine(cTexPathRS, Tex + APR)))
													segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + APR)));
												}
											} else {
											Tex = RM + A5 + Tl;
											if(File.Exists(Path.Combine(cTexDefault, Tex + Main))) {
												segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexDefault, Tex + Main)));
												if(File.Exists(Path.Combine(cTexDefault, Tex + APR)))
													segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexDefault, Tex + APR)));
												} else if(File.Exists(Path.Combine(cTexPathRS, Tex + Main))) {
												segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + Main)));
												if(File.Exists(Path.Combine(cTexPathRS, Tex + APR)))
													segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + APR)));
												}
											}
										}
									}
								segment.m_lodRenderDistance = 2500;
								}
							
							if(netInfo.name.Contains("AsymAvenueL2R4")) {
								if((netInfo.name.Equals("AsymAvenueL2R4")))
								{
									Tex = RM + A6 + Gd;
									if(segment.m_segmentMaterial.GetTexture(apr).name.Contains("Inverted")) {
										Tex += "_Inv";
										if(File.Exists(Path.Combine(cTexDefault, Tex + Main))) {
											segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexDefault, Tex + Main)));
											if(File.Exists(Path.Combine(cTexDefault, Tex + APR)))
												segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexDefault, Tex + APR)));
											} else if(File.Exists(Path.Combine(cTexPathRS, Tex + Main))) {
											segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + Main)));
											if(File.Exists(Path.Combine(cTexPathRS, Tex + APR)))
												segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + APR)));
											}
										} else {
										if(File.Exists(Path.Combine(cTexDefault, Tex + Main))) {
											segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexDefault, Tex + Main)));
											if(File.Exists(Path.Combine(cTexDefault, Tex + APR)))
												segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexDefault, Tex + APR)));
											} else if(File.Exists(Path.Combine(cTexPathRS, Tex + Main))) {
											segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + Main)));
											if(File.Exists(Path.Combine(cTexPathRS, Tex + APR)))
												segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + APR)));
											}
										}
									} else {
									if((netInfo.name.Equals("AsymAvenueL2R4 Elevated")) || (netInfo.name.Equals("AsymAvenueL2R4 Bridge"))) {
										if(segment.m_segmentMaterial.GetTexture(apr).name.Contains("Inverted")) {
											Tex = RM + A6 + El + "_Inv";
											if(File.Exists(Path.Combine(cTexDefault, Tex + Main))) {
												segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexDefault, Tex + Main)));
												if(File.Exists(Path.Combine(cTexDefault, Tex + APR)))
													segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexDefault, Tex + APR)));
												} else if(File.Exists(Path.Combine(cTexPathRS, Tex + Main))) {
												segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + Main)));
												if(File.Exists(Path.Combine(cTexPathRS, Tex + APR)))
													segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + APR)));
												}
											} else {
											Tex = RM + A6  + El;
											if(File.Exists(Path.Combine(cTexDefault, Tex + Main))) {
												segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexDefault, Tex + Main)));
												if(File.Exists(Path.Combine(cTexDefault, Tex + APR)))
													segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexDefault, Tex + APR)));
												} else if(File.Exists(Path.Combine(cTexPathRS, Tex + Main))) {
												segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + Main)));
												if(File.Exists(Path.Combine(cTexPathRS, Tex + APR)))
													segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + APR)));
												}
											}
										} else if((netInfo.name.Equals("AsymAvenueL2R4 Slope")) && (segment.m_mesh.name == "Slope")) {
										if(segment.m_segmentMaterial.GetTexture(apr).name.Contains("Inverted")) {
											Tex = RM + A6  + Gd + "_Inv";
											if(File.Exists(Path.Combine(cTexDefault, Tex + Main))) {
												segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexDefault, Tex + Main)));
												if(File.Exists(Path.Combine(cTexDefault, Tex + APR)))
													segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexDefault, Tex + APR)));
												} else if(File.Exists(Path.Combine(cTexPathRS, Tex + Main))) {
												segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + Main)));
												if(File.Exists(Path.Combine(cTexPathRS, Tex + APR)))
													segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + APR)));
												}
											} else {
											Tex = RM + A6 + Gd;
											if(File.Exists(Path.Combine(cTexDefault, Tex + Main))) {
												segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexDefault, Tex + Main)));
												if(File.Exists(Path.Combine(cTexDefault, Tex + APR)))
													segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexDefault, Tex + APR)));
												} else if(File.Exists(Path.Combine(cTexPathRS, Tex + Main))) {
												segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + Main)));
												if(File.Exists(Path.Combine(cTexPathRS, Tex + APR)))
													segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + APR)));
												}
											}
										} else if(netInfo.name.Equals("AsymAvenueL2R3 Tunnel")) {
										if(segment.m_segmentMaterial.GetTexture(apr).name.Contains("Inverted")) {
											Tex = RM + A6  + Tl + "_Inv";
											if(File.Exists(Path.Combine(cTexDefault, Tex + Main))) {
												segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexDefault, Tex + Main)));
												if(File.Exists(Path.Combine(cTexDefault, Tex + APR)))
													segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexDefault, Tex + APR)));
												} else if(File.Exists(Path.Combine(cTexPathRS, Tex + Main))) {
												segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + Main)));
												if(File.Exists(Path.Combine(cTexPathRS, Tex + APR)))
													segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + APR)));
												}
											} else {
											Tex = RM + A6  + Tl;
											if(File.Exists(Path.Combine(cTexDefault, Tex + Main))) {
												segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexDefault, Tex + Main)));
												if(File.Exists(Path.Combine(cTexDefault, Tex + APR)))
													segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexDefault, Tex + APR)));
												} else if(File.Exists(Path.Combine(cTexPathRS, Tex + Main))) {
												segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + Main)));
												if(File.Exists(Path.Combine(cTexPathRS, Tex + APR)))
													segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + APR)));
												}
											}
										}
									}
								segment.m_lodRenderDistance = 2500;
								}
							#endregion

							#region NExt Highways Default

							if((netInfo.name.Contains("Rural Highway")) && netInfo.name.Contains("Small")) {
								if(netInfo.name.Equals("Rural Highway Elevated")) {
									Tex = HW + "1L";
									if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "Highway1L_Ground_Segment_MainTex.dds")))
										segment.m_segmentMaterial.SetTexture("_MainTex", LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "Highway1L_Ground_Segment_MainTex.dds")));
									else if(File.Exists(Path.Combine(cTexPathHW, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + Main)));
										}
									} else if(netInfo.name.Equals("Rural Highway Slope")) {
									Tex = HW + "1L" + Sl;
									if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "Highway1L_Slope_Segment_MainTex.dds")))
										segment.m_segmentMaterial.SetTexture("_MainTex", LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "Highway1L_Slope_Segment_MainTex.dds")));
									else if(File.Exists(Path.Combine(cTexPathHW, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + Main)));
										}
									} else if(netInfo.name.Equals("Rural Highway Tunnel")) {
									Tex = HW + "1L" + Tl;
									if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "Highway1L_Tunnel_Segment_MainTex.dds")))
										segment.m_segmentMaterial.SetTexture("_MainTex", LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "Highway1L_Tunnel_Segment_MainTex.dds")));
									else if(File.Exists(Path.Combine(cTexPathHW, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + Main)));
										}
									} else {
									Tex = HW + "1L";
									if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "Highway1L_Ground_Segment_MainTex.dds")))
										segment.m_segmentMaterial.SetTexture("_MainTex", LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "Highway1L_Ground_Segment_MainTex.dds")));
									else if(File.Exists(Path.Combine(cTexPathHW, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + Main)));
										}
									}
								segment.m_lodRenderDistance = 2500;
								}

							if(netInfo.name.Contains("Rural Highway") && !netInfo.name.Contains("Small")) {
								if(netInfo.name.Equals("Small Rural Highway Elevated")) {
									Tex = HW + "2L";
									if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "Highway2L_Ground_Segment_MainTex.dds")))
										segment.m_segmentMaterial.SetTexture("_MainTex", LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "Highway2L_Ground_Segment_MainTex.dds")));
									else if(File.Exists(Path.Combine(cTexPathHW, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + Main)));
										}
									} else if(netInfo.name.Equals("Small Rural Highway Slope")) {
								Tex = HW + "2L" + Sl;
								if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "Highway2L_Slope_Segment_MainTex.dds")))
									segment.m_segmentMaterial.SetTexture("_MainTex", LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "Highway2L_Slope_Segment_MainTex.dds")));
								else if(File.Exists(Path.Combine(cTexPathHW, Tex + Main))) {
									segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + Main)));
									}
								} else if(netInfo.name.Equals("Small Rural Highway Tunnel")) {
									Tex = HW + "2L" + Tl;
									if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "Highway2L_Tunnel_Segment_MainTex.dds")))
										segment.m_segmentMaterial.SetTexture("_MainTex", LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "Highway2L_Tunnel_Segment_MainTex.dds")));
									else if(File.Exists(Path.Combine(cTexPathHW, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + Main)));
										}
								} else {
									Tex = HW + "2L";
									if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "Highway2L_Ground_Segment_MainTex.dds")))
										segment.m_segmentMaterial.SetTexture("_MainTex", LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "Highway2L_Ground_Segment_MainTex.dds")));
									else if(File.Exists(Path.Combine(cTexPathHW, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + Main)));
										}
									}

								segment.m_lodRenderDistance = 2500;
								}

							if(netInfo.name.Contains("Four-Lane Highway")) {
								if(netInfo.name.Equals("Four-Lane Highway Elevated")) {
									Tex = HW + "4L" + El;
									if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "Highway4L_Elevated_Segment_MainTex.dds")))
										segment.m_segmentMaterial.SetTexture("_MainTex", LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "Highway4L_Elevated_Segment_MainTex.dds")));
									else if(File.Exists(Path.Combine(cTexPathHW, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + Main)));
										}
									} else if(netInfo.name.Equals("Four-Lane Highway Slope")) {
									Tex = HW + "4L" + Sl;
									if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "Highway4L_Slope_Segment_MainTex.dds")))
										segment.m_segmentMaterial.SetTexture("_MainTex", LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "Highway4L_Slope_Segment_MainTex.dds")));
									else if(File.Exists(Path.Combine(cTexPathHW, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + Main)));
										}
									} else if(netInfo.name.Equals("Four-Lane Highway Tunnel")) {
									Tex = HW + "4L" + Tl;
									if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "Highway4L_Tunnel_Segment_MainTex.dds")))
										segment.m_segmentMaterial.SetTexture("_MainTex", LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "Highway4L_Tunnel_Segment_MainTex.dds")));
									else if(File.Exists(Path.Combine(cTexPathHW, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + Main)));
										}
									} else {
									Tex = HW + "4L" + Gd;
									if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "Highway4L_Ground_Segment_MainTex.dds")))
										segment.m_segmentMaterial.SetTexture("_MainTex", LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "Highway4L_Ground_Segment_MainTex.dds")));
									else if(File.Exists(Path.Combine(cTexPathHW, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + Main)));
										}
									}
								segment.m_lodRenderDistance = 2500;
								}

							if(netInfo.name.Contains("Five-Lane Highway")) {
								if(netInfo.name.Equals("Five-Lane Highway Elevated")) {
									Tex = HW + "5L";
									if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "Highway5L_Ground_Segment_MainTex.dds")))
										segment.m_segmentMaterial.SetTexture("_MainTex", LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "Highway5L_Ground_Segment_MainTex.dds")));
									else if(File.Exists(Path.Combine(cTexPathHW, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + Main)));
										}
									} else if(netInfo.name.Equals("Five-Lane Highway Slope")) {
									Tex = HW + "5L" + Sl;
									if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "Highway5L_Slope_Segment_MainTex.dds")))
										segment.m_segmentMaterial.SetTexture("_MainTex", LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "Highway5L_Slope_Segment_MainTex.dds")));
									else if(File.Exists(Path.Combine(cTexPathHW, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + Main)));
										}
									} else if(netInfo.name.Equals("Five-Lane Highway Tunnel")) {
									Tex = HW + "5L" + Tl;
									if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "Highway5L_Tunnel_Segment_MainTex.dds")))
										segment.m_segmentMaterial.SetTexture("_MainTex", LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "Highway5L_Tunnel_Segment_MainTex.dds")));
									else if(File.Exists(Path.Combine(cTexPathHW, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + Main)));
										}
									} else {
									Tex = HW + "5L";
									if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "Highway5L_Ground_Segment_MainTex.dds")))
										segment.m_segmentMaterial.SetTexture("_MainTex", LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "Highway5L_Ground_Segment_MainTex.dds")));
									else if(File.Exists(Path.Combine(cTexPathHW, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + Main)));
										}
									}
								segment.m_lodRenderDistance = 2500;
								}

							if(netInfo.name.Contains("Large Highway")) {
								if(netInfo.name.Equals("Large Highway Elevated")) {
									Tex = HW + "6L";
									if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "Highway6L_Ground_Segment_MainTex.dds")))
										segment.m_segmentMaterial.SetTexture("_MainTex", LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "Highway6L_Ground_Segment_MainTex.dds")));
									else if(File.Exists(Path.Combine(cTexPathHW, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + Main)));
										}
									} else if(netInfo.name.Equals("Large Highway Slope")) {
									Tex = HW + "6L" + Sl;
									if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "Highway6L_Slope_Segment_MainTex.dds")))
										segment.m_segmentMaterial.SetTexture("_MainTex", LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "Highway6L_Slope_Segment_MainTex.dds")));
									else if(File.Exists(Path.Combine(cTexPathHW, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + Main)));
										}
									} else if(netInfo.name.Equals("Large Highway Tunnel")) {
									Tex = HW + "6L" + Tl;
									if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "Highway6L_Tunnel_Segment_MainTex.dds")))
										segment.m_segmentMaterial.SetTexture("_MainTex", LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "Highway6L_Tunnel_Segment_MainTex.dds")));
									else if(File.Exists(Path.Combine(cTexPathHW, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + Main)));
										}
									} else {
									Tex = HW + "6L";
									if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "Highway6L_Ground_Segment_MainTex.dds")))
										segment.m_segmentMaterial.SetTexture("_MainTex", LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "Highway6L_Ground_Segment_MainTex.dds")));
									else if(File.Exists(Path.Combine(cTexPathHW, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + Main)));
										}
									}

								segment.m_lodRenderDistance = 2500;
								}
							#endregion

							#region Small Busways
							if((netInfo.name.Contains("Small Busway")) && (!(netInfo.name.Contains("OneWay")))) {//mesh
								if(segment.m_mesh.name.Equals("SmallRoadSegment")) {
									Tex = RS + Bl + Gd;
									if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "Busway2L_Ground_Segment_MainTex.dds")))
										segment.m_segmentMaterial.SetTexture("_MainTex", LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "Busway2L_Ground_Segment_MainTex.dds")));
									else if(File.Exists(Path.Combine(cTexPathRS, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPathRS, Tex + APR)))
											segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + APR)));
										}
									} else if(segment.m_mesh.name.Equals("SmallRoadSegmentBusSide")) {
									Tex = RS + Bl + B1;
									if(File.Exists(Path.Combine(cTexPathRS, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPathRS, Tex + APR)))
											segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + APR)));
										}
									} else if(segment.m_mesh.name.Equals("SmallRoadSegmentBusBoth")) {
									Tex = RS + Bl + B2;
									if(File.Exists(Path.Combine(cTexPathRS, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPathRS, Tex + APR)))
											segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + APR)));
										}
									} else if(!(segment.m_mesh.name.Equals("SmallRoadSegment"))) {
									if(netInfo.name.Equals("Small Busway Elevated")) {
										Tex = RS + Bl + El;
										if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "Busway2L_Elevated_Segment_MainTex.dds")))
											segment.m_segmentMaterial.SetTexture("_MainTex", LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "Busway2L_Elevated_Segment_MainTex.dds")));
										else if(File.Exists(Path.Combine(cTexPathRS, Tex + Main))) {
											segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + Main)));
											if(File.Exists(Path.Combine(cTexPathRS, Tex + APR)))
												segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + APR)));
											}
										} else if(netInfo.name.Equals("Small Busway Slope")) {
										Tex = RS + Bl + Sl;
										if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "Busway2L_Slope_Segment_MainTex.dds")))
											segment.m_segmentMaterial.SetTexture("_MainTex", LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "Busway2L_Slope_Segment_MainTex.dds")));
										else if(File.Exists(Path.Combine(cTexPathRS, Tex + Main))) {
											segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + Main)));
											if(File.Exists(Path.Combine(cTexPathRS, Tex + APR)))
												segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + APR)));
											}
										} else if(netInfo.name.Equals("Small Busway Tunnel"))
										Tex = RS + Bl + Tl;
									if(File.Exists(Path.Combine(cTexPathRS, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPathRS, Tex + APR)))
											segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + APR)));
										}
									}
								segment.m_lodRenderDistance = 2500;
								}
							if((netInfo.name.Equals("Small Busway Decoration Grass")) || (netInfo.name.Equals("Small Busway Decoration Trees"))) {//mesh
								if(segment.m_mesh.name.Equals("SmallRoadSegment2")) {
									Tex = RS + Bl + Dc;
									if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "Busway2L_DecoGrass_Ground_Segment_MainTex.dds")))
										segment.m_segmentMaterial.SetTexture("_MainTex", LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "Busway2L_DecoGrass_Ground_Segment_MainTex.dds")));
									else if(File.Exists(Path.Combine(cTexPathRS, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPathRS, Tex + APR)))
											segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + APR)));
										}
									} else if(segment.m_mesh.name.Equals("SmallRoadSegment2BusSide")) {
									Tex = RS + Bl + Dc + B1;
									if(File.Exists(Path.Combine(cTexPathRS, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPathRS, Tex + APR)))
											segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + APR)));
										}
									} else if(segment.m_mesh.name.Equals("SmallRoadSegment2BusBoth")) {
									Tex = RS + Bl + Dc + B2;
									if(File.Exists(Path.Combine(cTexPathRS, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPathRS, Tex + APR)))
											segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + APR)));
										}
									}
								segment.m_lodRenderDistance = 2500;
								}
							if(netInfo.name.Contains("Small Busway OneWay")) {//mesh
								if(segment.m_mesh.name.Equals("SmallRoadSegment")) {
									Tex = RS + Ow + Bl + Gd;
									if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "Busway2L1W_Ground_Segment_MainTex.dds")))
										segment.m_segmentMaterial.SetTexture("_MainTex", LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "Busway2L1W_Ground_Segment_MainTex.dds")));
									else if(File.Exists(Path.Combine(cTexPathRS, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPathRS, Tex + APR)))
											segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + APR)));
										}
									} else if(segment.m_mesh.name.Equals("SmallRoadSegmentBusSide")) {
									Tex = RS + Ow + Bl + B1;
									if(File.Exists(Path.Combine(cTexPathRS, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPathRS, Tex + APR)))
											segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + APR)));
										}
									} else if(segment.m_mesh.name.Equals("SmallRoadSegmentBusBoth")) {
									Tex = RS + Ow + Bl + B2;
									if(File.Exists(Path.Combine(cTexPathRS, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPathRS, Tex + APR)))
											segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + APR)));
										}
									} else if(!(segment.m_mesh.name.Equals("SmallRoadSegment"))) {
									if(netInfo.name.Equals("Small Busway OneWay Elevated")) {
										Tex = RS + Ow + Bl + El;
										if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "Busway2L1W_Elevated_Segment_MainTex.dds")))
											segment.m_segmentMaterial.SetTexture("_MainTex", LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "Busway2L1W_Elevated_Segment_MainTex.dds")));
										else if(File.Exists(Path.Combine(cTexPathRS, Tex + Main))) {
											segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + Main)));
											if(File.Exists(Path.Combine(cTexPathRS, Tex + APR)))
												segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + APR)));
											}
										} else if(netInfo.name.Equals("Small Busway OneWay Slope")) {
										Tex = RS + Ow + Bl + Sl;
										if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "Busway2L1W_Slope_Segment_MainTex.dds")))
											segment.m_segmentMaterial.SetTexture("_MainTex", LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "Busway2L1W_Slope_Segment_MainTex.dds")));
										else if(File.Exists(Path.Combine(cTexPathRS, Tex + Main))) {
											segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + Main)));
											if(File.Exists(Path.Combine(cTexPathRS, Tex + APR)))
												segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + APR)));
											}
										} else if(netInfo.name.Equals("Small Busway OneWay Tunnel")) {
										Tex = RS + Ow + Bl + Tl;
										if(File.Exists(Path.Combine(cTexPathRS, Tex + Main))) {
											segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + Main)));
											if(File.Exists(Path.Combine(cTexPathRS, Tex + APR)))
												segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + APR)));
											}
										}
									}
								segment.m_lodRenderDistance = 2500;
								}
							if((netInfo.name.Equals("Small Busway OneWay Decoration Grass")) || (netInfo.name.Equals("Small Busway OneWay Decoration Trees"))) {//mesh
								if(segment.m_mesh.name.Equals("SmallRoadSegment2")) {
									Tex = RS + Ow + Bl + Dc;
									if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "Busway2L1W_DecoGrass_Ground_Segment_MainTex.dds"))) {
										segment.m_segmentMaterial.SetTexture("_MainTex", LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "Busway2L1W_DecoGrass_Ground_Segment_MainTex.dds")));
										segment.m_lodRenderDistance = 2500;
										} else if(File.Exists(Path.Combine(cTexPathRS, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPathRS, Tex + APR)))
											segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + APR)));
										}
									} else if(segment.m_mesh.name.Equals("SmallRoadSegment2BusSide")) {
									Tex = RS + Ow + Dc + B1;
									if(File.Exists(Path.Combine(cTexPathRS, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPathRS, Tex + APR)))
											segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + APR)));
										}
									} else if(segment.m_mesh.name.Equals("SmallRoadSegment2BusBoth")) {
									Tex = RS + Ow + Dc + B2;
									if(File.Exists(Path.Combine(cTexPathRS, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPathRS, Tex + APR)))
											segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + APR)));
										}
									}
								segment.m_lodRenderDistance = 2500;
								}

							#endregion

							#region Large Busways
							if(netInfo.name.Equals("Large Road With Bus Lanes")) {//mesh
								if(segment.m_mesh.name.Equals("RoadLageSegment")) {
									Tex = RL + Bl + Gd;
									if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "RoadLargeBuslane_D.dds"))) {
										segment.m_segmentMaterial.SetTexture("_MainTex", LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "RoadLargeBuslane_D.dds")));
										} else if(File.Exists(Path.Combine(cTexPathRL, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRL, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPathRL, Tex + APR)))
											segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRL, Tex + APR)));
										}
									} else if(segment.m_mesh.name.Equals("LargeRoadSegmentBusSide")) {
									Tex = RL + Bl + B1;
									if(File.Exists(Path.Combine(cTexPathRL, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRL, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPathRL, Tex + APR)))
											segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRL, Tex + APR)));
										}
									} else if(segment.m_mesh.name.Equals("LargeRoadSegmentBusBoth")) {
									Tex = RL + Bl + B2;
									if(File.Exists(Path.Combine(cTexPathRL, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRL, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPathRL, Tex + APR)))
											segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRL, Tex + APR)));
										}
									}
								segment.m_lodRenderDistance = 2500;
								}
							if(netInfo.name.Equals("Large Road Elevated With Bus Lanes")) {
								Tex = RL + Bl + El;
								if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "RoadLargeElevatedBus_D.dds"))) {
									segment.m_segmentMaterial.SetTexture("_MainTex", LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "RoadLargeElevatedBus_D.dds")));
									} else if(File.Exists(Path.Combine(cTexPathRL, Tex + Main))) {
									segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRL, Tex + Main)));
									if(File.Exists(Path.Combine(cTexPathRL, Tex + APR)))
										segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRL, Tex + APR)));
									}
								segment.m_lodRenderDistance = 2500;
								}
							if(netInfo.name.Equals("Large Road Slope With Bus Lanes")) {
								Tex = RL + Bl + Sl;
								if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "large-tunnelBus_d.dds"))) {
									segment.m_segmentMaterial.SetTexture("_MainTex", LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "large-tunnelBus_d.dds")));
									segment.m_lodRenderDistance = 2500;
									} else if(File.Exists(Path.Combine(cTexPathRL, Tex + Main))) {
									segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRL, Tex + Main)));
									if(File.Exists(Path.Combine(cTexPathRL, Tex + APR)))
										segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRL, Tex + APR)));
									}
								segment.m_lodRenderDistance = 2500;
								}
							if(netInfo.name.Equals("Large Road Tunnel With Bus Lanes")) {
								Tex = RL + Bl + Tl;
								if(File.Exists(Path.Combine(cTexPathRL, Tex + Main))) {
									segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRL, Tex + Main)));
									if(File.Exists(Path.Combine(cTexPathRL, Tex + APR)))
										segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRL, Tex + APR)));
									}
								segment.m_lodRenderDistance = 2500;
								}
							if((netInfo.name.Equals("Large Road Decoration Trees With Bus Lanes")) || (netInfo.name.Equals("Large Road Decoration Grass With Bus Lanes"))) {//mesh
								if(segment.m_mesh.name.Equals("LargeRoadSegment2")) {
									Tex = RL + Bl + Dc;
									if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "Busway6L_DecoGrass_Ground_Segment_MainTex.dds"))) {
										segment.m_segmentMaterial.SetTexture("_MainTex", LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "Busway6L_DecoGrass_Ground_Segment_MainTex.dds")));
										if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "RoadLargeSegment-default-apr.dds"))) {
											segment.m_segmentMaterial.SetTexture("_APRMap", LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "RoadLargeSegment-default-apr.dds")));
											}
										if(File.Exists(Path.Combine(ModLoader.APRMaps_Path, "RoadLargeSegment-default-apr.dds"))) {
											segment.m_segmentMaterial.SetTexture("_APRMap", LoadTextureDDS(Path.Combine(ModLoader.APRMaps_Path, "RoadLargeSegment-default-apr.dds")));
											}
										if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "RoadLargeSegment-default-apr.dds"))) {
											segment.m_segmentMaterial.SetTexture("_APRMap", LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "RoadLargeSegment-default-apr.dds")));
											}
										if(File.Exists(Path.Combine(ModLoader.APRMaps_Path, "RoadLargeSegment-default-apr.dds"))) {
											segment.m_segmentMaterial.SetTexture("_APRMap", LoadTextureDDS(Path.Combine(ModLoader.APRMaps_Path, "RoadLargeSegment-default-apr.dds")));
											}
										} else if(File.Exists(Path.Combine(cTexPathRL, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRL, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPathRL, Tex + APR)))
											segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRL, Tex + APR)));
										}
									} else if(segment.m_mesh.name.Equals("LargeRoadSegment2BusSide")) {
									Tex = RL + Bl + Dc + B1;
									if(File.Exists(Path.Combine(cTexPathRL, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRL, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPathRL, Tex + APR)))
											segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRL, Tex + APR)));
										}
									} else if(segment.m_mesh.name.Equals("LargeRoadSegment2BusBoth")) {
									Tex = RL + Bl + Dc + B2;
									if(File.Exists(Path.Combine(cTexPathRL, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRL, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPathRL, Tex + APR)))
											segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRL, Tex + APR)));
										}
									}
								segment.m_lodRenderDistance = 2500;
								}
							#endregion

							}

						if(segment.m_segmentMaterial.GetTexture(apr) != null) {
							#region SmallHeavyRoads APRMaps

							if(netInfo.name.Contains("BasicRoadTL")) {
								if(segment.m_segmentMaterial.GetTexture(apr).name.Contains("Ground"))
									if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "BasicRoadTL_Ground_Segment_APRMap.dds")))
										segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "BasicRoadTL_Ground_Segment_APRMap.dds")));
									else if(File.Exists(Path.Combine(ModLoader.APRMaps_Path, "BasicRoadTL_Ground_Segment_APRMap.dds")))
										segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(ModLoader.APRMaps_Path, "BasicRoadTL_Ground_Segment_APRMap.dds")));

								if(segment.m_segmentMaterial.GetTexture(apr).name.Contains("Elevated"))
									if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "BasicRoadTL_Elevated_Segment_APRMap.dds")))
										segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "BasicRoadTL_Elevated_Segment_APRMap.dds")));
									else if(File.Exists(Path.Combine(ModLoader.APRMaps_Path, "BasicRoadTL_Elevated_Segment_APRMap.dds")))
										segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(ModLoader.APRMaps_Path, "BasicRoadTL_Elevated_Segment_APRMap.dds")));

								if(segment.m_segmentMaterial.GetTexture(apr).name.Contains("Tunnel"))
									if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "BasicRoadTL_Tunnel_Segment_APRMap.dds")))
										segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "BasicRoadTL_Tunnel_Segment_APRMap.dds")));
									else if(File.Exists(Path.Combine(ModLoader.APRMaps_Path, "BasicRoadTL_Tunnel_Segment_APRMap.dds")))
										segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(ModLoader.APRMaps_Path, "BasicRoadTL_Tunnel_Segment_APRMap.dds")));

								segment.m_lodRenderDistance = 2500;
								}

							if(netInfo.name.Contains("Oneway3L")) {

								if(segment.m_segmentMaterial.GetTexture(apr).name.Contains("Ground"))
									if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "OneWay3L_Ground_Segment_APRMap.dds")))
										segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "OneWay3L_Ground_Segment_APRMap.dds")));
									else if(File.Exists(Path.Combine(ModLoader.APRMaps_Path, "OneWay3L_Ground_Segment_APRMap.dds")))
										segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(ModLoader.APRMaps_Path, "OneWay3L_Ground_Segment_APRMap.dds")));

								//   if ((netInfo.name.Contains("Elevated") || (netInfo.name.Contains("Bridge"))))
								if(segment.m_segmentMaterial.GetTexture(apr).name.Contains("Elevated"))
									if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "OneWay3L_Elevated_Segment_APRMap.dds")))
										segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "OneWay3L_Elevated_Segment_APRMap.dds")));
									else if(File.Exists(Path.Combine(ModLoader.APRMaps_Path, "OneWay3L_Elevated_Segment_APRMap.dds")))
										segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(ModLoader.APRMaps_Path, "OneWay3L_Elevated_Segment_APRMap.dds")));

								//  if ((netInfo.name.Contains("Slope") || (netInfo.name.Contains("Tunnel"))))
								if(segment.m_segmentMaterial.GetTexture(apr).name.Contains("Tunnel"))
									if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "OneWay3L_Tunnel_Segment_APRMap.dds")))
										segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "OneWay3L_Tunnel_Segment_APRMap.dds")));
									else if(File.Exists(Path.Combine(ModLoader.APRMaps_Path, "OneWay3L_Tunnel_Segment_APRMap.dds")))
										segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(ModLoader.APRMaps_Path, "OneWay3L_Tunnel_Segment_APRMap.dds")));

								segment.m_lodRenderDistance = 2500;
								}

							if(netInfo.name.Contains("Oneway4L")) {
								if(segment.m_segmentMaterial.GetTexture(apr).name.Contains("Ground"))
									if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "OneWay4L_Ground_Segment_APRMap.dds")))
										segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "OneWay4L_Ground_Segment_APRMap.dds")));
									else if(File.Exists(Path.Combine(ModLoader.APRMaps_Path, "OneWay4L_Ground_Segment_APRMap.dds")))
										segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(ModLoader.APRMaps_Path, "OneWay4L_Ground_Segment_APRMap.dds")));

								if(segment.m_segmentMaterial.GetTexture(apr).name.Contains("Elevated"))
									if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "OneWay4L_Elevated_Segment_APRMap.dds")))
										segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "OneWay4L_Elevated_Segment_APRMap.dds")));
									else if(File.Exists(Path.Combine(ModLoader.APRMaps_Path, "OneWay4L_Elevated_Segment_APRMap.dds")))
										segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(ModLoader.APRMaps_Path, "OneWay4L_Elevated_Segment_APRMap.dds")));

								if(segment.m_segmentMaterial.GetTexture(apr).name.Contains("Tunnel"))
									if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "OneWay4L_Tunnel_Segment_APRMap.dds")))
										segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(ModLoader.APRMaps_Path, "OneWay4L_Tunnel_Segment_APRMap.dds")));
									else if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "OneWay4L_Tunnel_Segment_APRMap.dds")))
										segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(ModLoader.APRMaps_Path, "OneWay4L_Tunnel_Segment_APRMap.dds")));

								segment.m_lodRenderDistance = 2500;
								}

							if(netInfo.name.Contains("Small Avenue")) {
								if(segment.m_segmentMaterial.GetTexture(apr).name.Contains("Ground"))
									if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "SmallAvenue4L_Ground_Segment_APRMap.dds")))
										segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "SmallAvenue4L_Ground_Segment_APRMap.dds")));
									else if(File.Exists(Path.Combine(ModLoader.APRMaps_Path, "SmallAvenue4L_Ground_Segment_APRMap.dds")))
										segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(ModLoader.APRMaps_Path, "SmallAvenue4L_Ground_Segment_APRMap.dds")));

								if(segment.m_segmentMaterial.GetTexture(apr).name.Contains("Elevated"))
									if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "SmallAvenue4L_Elevated_Segment_APRMap.dds")))
										segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "SmallAvenue4L_Elevated_Segment_APRMap.dds")));
									else if(File.Exists(Path.Combine(ModLoader.APRMaps_Path, "SmallAvenue4L_Elevated_Segment_APRMap.dds")))
										segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(ModLoader.APRMaps_Path, "SmallAvenue4L_Elevated_Segment_APRMap.dds")));

								if(segment.m_segmentMaterial.GetTexture(apr).name.Contains("Tunnel"))
									if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "SmallAvenue4L_Tunnel_Segment_APRMap.dds")))
										segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "SmallAvenue4L_Tunnel_Segment_APRMap.dds")));
									else if(File.Exists(Path.Combine(ModLoader.APRMaps_Path, "SmallAvenue4L_Tunnel_Segment_APRMap.dds")))
										segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(ModLoader.APRMaps_Path, "SmallAvenue4L_Tunnel_Segment_APRMap.dds")));

								segment.m_lodRenderDistance = 2500;
								}
							#endregion

							#region Avenues APRMaps
							if(netInfo.name.Contains("Eight-Lane Avenue")) {
								if(segment.m_segmentMaterial.GetTexture(apr).name.Contains("Ground"))
									if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "LargeAvenue8LM_Ground_Segment_APRMap.dds")))
										segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "LargeAvenue8LM_Ground_Segment_APRMap.dds")));
									else if(File.Exists(Path.Combine(ModLoader.APRMaps_Path, "LargeAvenue8LM_Ground_Segment_APRMap.dds")))
										segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(ModLoader.APRMaps_Path, "LargeAvenue8LM_Ground_Segment_APRMap.dds")));

								if(segment.m_segmentMaterial.GetTexture(apr).name.Contains("Elevated"))
									if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "LargeAvenue8LM_Elevated_Segment_APRMap.dds")))
										segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "LargeAvenue8LM_Elevated_Segment_APRMap.dds")));
									else if(File.Exists(Path.Combine(ModLoader.APRMaps_Path, "LargeAvenue8LM_Elevated_Segment_APRMap.dds")))
										segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(ModLoader.APRMaps_Path, "LargeAvenue8LM_Elevated_Segment_APRMap.dds")));


								if(segment.m_segmentMaterial.GetTexture(apr).name.Contains("Slope"))
									if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "LargeAvenue8LM_Slope_Segment_APRMap.dds")))
										segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "LargeAvenue8LM_Slope_Segment_APRMap.dds")));
									else if(File.Exists(Path.Combine(ModLoader.APRMaps_Path, "LargeAvenue8LM_Slope_Segment_APRMap.dds")))
										segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(ModLoader.APRMaps_Path, "LargeAvenue8LM_Slope_Segment_APRMap.dds")));

								if(segment.m_segmentMaterial.GetTexture(apr).name.Contains("Tunnel"))
									if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "LargeAvenue8LM_Tunnel_Segment_APRMap.dds")))
										segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "LargeAvenue8LM_Tunnel_Segment_APRMap.dds")));
									else if(File.Exists(Path.Combine(ModLoader.APRMaps_Path, "LargeAvenue8LM_Tunnel_Segment_APRMap.dds")))
										segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(ModLoader.APRMaps_Path, "LargeAvenue8LM_Tunnel_Segment_APRMap.dds")));

								segment.m_lodRenderDistance = 2500;
								}
							#endregion

							#region NExt Highways APRMaps
							if(netInfo.name.Contains("Rural Highway") && netInfo.name.Contains("Small")) {
								if(segment.m_segmentMaterial.GetTexture(apr).name.Contains("Ground"))
									if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "Highway1L_Ground_Segment_APRMap.dds")))
										segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "Highway1L_Ground_Segment_APRMap.dds")));
									else if(File.Exists(Path.Combine(ModLoader.APRMaps_Path, "Highway1L_Ground_Segment_APRMap.dds")))
										segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(ModLoader.APRMaps_Path, "Highway1L_Ground_Segment_APRMap.dds")));

								if(segment.m_segmentMaterial.GetTexture(apr).name.Contains("Elevated"))
									if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "Highway1L_Ground_Segment_APRMap.dds")))
										segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "Highway1L_Ground_Segment_APRMap.dds")));
									else if(File.Exists(Path.Combine(ModLoader.APRMaps_Path, "Highway1L_Ground_Segment_APRMap.dds")))
										segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(ModLoader.APRMaps_Path, "Highway1L_Ground_Segment_APRMap.dds")));

								if(segment.m_segmentMaterial.GetTexture(apr).name.Contains("Slope"))
									if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "Highway1L_Slope_Segment_APRMap.dds")))
										segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "Highway1L_Slope_Segment_APRMap.dds")));
									else if(File.Exists(Path.Combine(ModLoader.APRMaps_Path, "Highway1L_Slope_Segment_APRMap.dds")))
										segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(ModLoader.APRMaps_Path, "Highway1L_Slope_Segment_APRMap.dds")));

								if(segment.m_segmentMaterial.GetTexture(apr).name.Contains("Tunnel"))
									if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "Highway1L_Tunnel_Segment_APRMap.dds")))
										segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "Highway1L_Tunnel_Segment_APRMap.dds")));
									else if(File.Exists(Path.Combine(ModLoader.APRMaps_Path, "Highway1L_Tunnel_Segment_APRMap.dds")))
										segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(ModLoader.APRMaps_Path, "Highway1L_Tunnel_Segment_APRMap.dds")));

								segment.m_lodRenderDistance = 2500;
								}

							if(netInfo.name.Contains("Rural Highway") && !netInfo.name.Contains("Small")) {
								if(segment.m_segmentMaterial.GetTexture(apr).name.Contains("Ground"))
									if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "Highway2L_Ground_Segment_APRMap.dds")))
										segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "Highway2L_Ground_Segment_APRMap.dds")));
									else if(File.Exists(Path.Combine(ModLoader.APRMaps_Path, "Highway2L_Ground_Segment_APRMap.dds")))
										segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(ModLoader.APRMaps_Path, "Highway2L_Ground_Segment_APRMap.dds")));

								if(segment.m_segmentMaterial.GetTexture(apr).name.Contains("Elevated"))
									if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "Highway2L_Ground_Segment_APRMap.dds")))
										segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "Highway2L_Ground_Segment_APRMap.dds")));
									else if(File.Exists(Path.Combine(ModLoader.APRMaps_Path, "Highway2L_Ground_Segment_APRMap.dds")))
										segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(ModLoader.APRMaps_Path, "Highway2L_Ground_Segment_APRMap.dds")));

								if(segment.m_segmentMaterial.GetTexture(apr).name.Contains("Slope"))
									if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "Highway2L_Slope_Segment_APRMap.dds")))
										segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "Highway2L_Slope_Segment_APRMap.dds")));
									else if(File.Exists(Path.Combine(ModLoader.APRMaps_Path, "Highway2L_Slope_Segment_APRMap.dds")))
										segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(ModLoader.APRMaps_Path, "Highway2L_Slope_Segment_APRMap.dds")));

								if(segment.m_segmentMaterial.GetTexture(apr).name.Contains("Tunnel"))
									if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "Highway2L_Tunnel_Segment_APRMap.dds")))
										segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "Highway2L_Tunnel_Segment_APRMap.dds")));
									else if(File.Exists(Path.Combine(ModLoader.APRMaps_Path, "Highway2L_Tunnel_Segment_APRMap.dds")))
										segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(ModLoader.APRMaps_Path, "Highway2L_Tunnel_Segment_APRMap.dds")));

								segment.m_lodRenderDistance = 2500;
								}

							if(netInfo.name.Contains("Four-Lane Highway")) {
								if(segment.m_segmentMaterial.GetTexture(apr).name.Contains("Ground"))
									if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "Highway4L_Ground_Segment_APRMap.dds")))
										segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "Highway4L_Ground_Segment_APRMap.dds")));
									else if(File.Exists(Path.Combine(ModLoader.APRMaps_Path, "Highway4L_Ground_Segment_APRMap.dds")))
										segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(ModLoader.APRMaps_Path, "Highway4L_Ground_Segment_APRMap.dds")));

								if(segment.m_segmentMaterial.GetTexture(apr).name.Contains("Elevated"))
									if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "Highway4L_Elevated_Segment_APRMap.dds")))
										segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "Highway4L_Elevated_Segment_APRMap.dds")));
									else if(File.Exists(Path.Combine(ModLoader.APRMaps_Path, "Highway4L_Elevated_Segment_APRMap.dds")))
										segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(ModLoader.APRMaps_Path, "Highway4L_Elevated_Segment_APRMap.dds")));

								if(segment.m_segmentMaterial.GetTexture(apr).name.Contains("Slope"))
									if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "Highway4L_Slope_Segment_APRMap.dds")))
										segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "Highway4L_Slope_Segment_APRMap.dds")));
									else if(File.Exists(Path.Combine(ModLoader.APRMaps_Path, "Highway4L_Slope_Segment_APRMap.dds")))
										segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(ModLoader.APRMaps_Path, "Highway4L_Slope_Segment_APRMap.dds")));

								if(segment.m_segmentMaterial.GetTexture(apr).name.Contains("Tunnel"))
									if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "Highway4L_Tunnel_Segment_APRMap.dds")))
										segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "Highway4L_Tunnel_Segment_APRMap.dds")));
									else if(File.Exists(Path.Combine(ModLoader.APRMaps_Path, "Highway4L_Tunnel_Segment_APRMap.dds")))
										segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(ModLoader.APRMaps_Path, "Highway4L_Tunnel_Segment_APRMap.dds")));

								segment.m_lodRenderDistance = 2500;
								}

							if(netInfo.name.Contains("Five-Lane Highway")) {
								if(segment.m_segmentMaterial.GetTexture(apr).name.Contains("Ground"))
									if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "Highway5L_Ground_Segment_APRMap.dds")))
										segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "Highway5L_Ground_Segment_APRMap.dds")));
									else if(File.Exists(Path.Combine(ModLoader.APRMaps_Path, "Highway5L_Ground_Segment_APRMap.dds")))
										segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(ModLoader.APRMaps_Path, "Highway5L_Ground_Segment_APRMap.dds")));

								if(segment.m_segmentMaterial.GetTexture(apr).name.Contains("Elevated"))
									if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "Highway5L_Ground_Segment_APRMap.dds")))
										segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "Highway5L_Ground_Segment_APRMap.dds")));
									else if(File.Exists(Path.Combine(ModLoader.APRMaps_Path, "Highway5L_Ground_Segment_APRMap.dds")))
										segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(ModLoader.APRMaps_Path, "Highway5L_Ground_Segment_APRMap.dds")));

								if(segment.m_segmentMaterial.GetTexture(apr).name.Contains("Slope"))
									if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "Highway5L_Slope_Segment_APRMap.dds")))
										segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "Highway5L_Slope_Segment_APRMap.dds")));
									else if(File.Exists(Path.Combine(ModLoader.APRMaps_Path, "Highway5L_Slope_Segment_APRMap.dds")))
										segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(ModLoader.APRMaps_Path, "Highway5L_Slope_Segment_APRMap.dds")));

								if(segment.m_segmentMaterial.GetTexture(apr).name.Contains("Tunnel"))
									if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "Highway5L_Tunnel_Segment_APRMap.dds")))
										segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "Highway5L_Tunnel_Segment_APRMap.dds")));
									else if(File.Exists(Path.Combine(ModLoader.APRMaps_Path, "Highway5L_Tunnel_Segment_APRMap.dds")))
										segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(ModLoader.APRMaps_Path, "Highway5L_Tunnel_Segment_APRMap.dds")));

								segment.m_lodRenderDistance = 2500;
								}

							if(netInfo.name.Contains("Large Highway")) {
								if(segment.m_segmentMaterial.GetTexture(apr).name.Contains("Ground"))
									if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "Highway6L_Ground_Segment_APRMap.dds")))
										segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "Highway6L_Ground_Segment_APRMap.dds")));
									else if(File.Exists(Path.Combine(ModLoader.APRMaps_Path, "Highway6L_Ground_Segment_APRMap.dds")))
										segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(ModLoader.APRMaps_Path, "Highway6L_Ground_Segment_APRMap.dds")));

								if(segment.m_segmentMaterial.GetTexture(apr).name.Contains("Elevated"))
									if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "Highway6L_Ground_Segment_APRMap.dds")))
										segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "Highway6L_Ground_Segment_APRMap.dds")));
									else if(File.Exists(Path.Combine(ModLoader.APRMaps_Path, "Highway6L_Ground_Segment_APRMap.dds")))
										segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(ModLoader.APRMaps_Path, "Highway6L_Ground_Segment_APRMap.dds")));

								if(segment.m_segmentMaterial.GetTexture(apr).name.Contains("Slope"))
									if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "Highway6L_Slope_Segment_APRMap.dds")))
										segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "Highway6L_Slope_Segment_APRMap.dds")));
									else if(File.Exists(Path.Combine(ModLoader.APRMaps_Path, "Highway6L_Slope_Segment_APRMap.dds")))
										segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(ModLoader.APRMaps_Path, "Highway6L_Slope_Segment_APRMap.dds")));

								if(segment.m_segmentMaterial.GetTexture(apr).name.Contains("Tunnel"))
									if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "Highway6L_Tunnel_Segment_APRMap.dds")))
										segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "Highway6L_Tunnel_Segment_APRMap.dds")));
									else if(File.Exists(Path.Combine(ModLoader.APRMaps_Path, "Highway6L_Tunnel_Segment_APRMap.dds")))
										segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(ModLoader.APRMaps_Path, "Highway6L_Tunnel_Segment_APRMap.dds")));

								segment.m_lodRenderDistance = 2500;
								}
							#endregion

							}
						}

					#endregion
					}
				#endregion
				if(!(
					netInfo.m_class.name.Contains("NExt") ||
					netInfo.m_class.name.Contains("Water") ||
					netInfo.m_class.name.Contains("Train") ||
					netInfo.m_class.name.Contains("Metro") ||
					netInfo.m_class.name.Contains("Transport") ||
					netInfo.m_class.name.Contains("Bus Line") ||
					netInfo.m_class.name.Contains("Airplane") ||
					netInfo.m_class.name.Contains("Ship") ||
					netInfo.name.Contains("Busway") ||
					(netInfo.name.Contains("Large Road") && netInfo.name.Contains("Bus Lane"))
					))

				//Only Roads specifically
                {
					NetInfo.Node[] nodes = netInfo.m_nodes;
					//nodes
					for(int k = 0;k < nodes.Length;k++) {
						NetInfo.Node node = nodes[k];
						if((node.m_nodeMaterial.GetTexture(main) != null) &&
							(!(node.m_nodeMaterial.name.Equals(TrRailA))) &&
							(!(node.m_nodeMaterial.name.Equals(TrRailB)))) {
							string nodeMaterialTexture_name = Path.Combine(ModLoader.currentTexturesPath_default, node.m_nodeMaterial.GetTexture("_MainTex").name + ".dds");
							string prefab_road_name = netInfo.name.Replace(" ", "_").ToLowerInvariant().Trim();

							if(File.Exists(prefab_road_name + "_" + nodeMaterialTexture_name))
								node.m_nodeMaterial.SetTexture("_MainTex", LoadTextureDDS(prefab_road_name + "_" + nodeMaterialTexture_name));

							Tex = RS + Gd + Nd;
							node.m_lodRenderDistance = 2500;
							if(File.Exists(Path.Combine(cTexPathRS, Tex + Main))) {
								#region Small Roads
								if(netInfo.name.Contains("Basic Road")) {//mesh
									Tex = RS;
									if(netInfo.name.Equals("Basic Road Elevated")) {
										Tex = RS + El + Nd;
										if(File.Exists(Path.Combine(cTexPathRS, Tex + Main))) {
											node.m_nodeMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + Main)));
											if(File.Exists(Path.Combine(cTexPathRS, Tex + APR))) {
												node.m_nodeMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + APR)));
												}
											}
										}
									else if((netInfo.name.Equals("Basic Road Decoration Grass")) || (netInfo.name.Equals("Basic Road Decoration Trees"))) {//mesh
										Tex = RS + Dc + Nd;
										if(File.Exists(Path.Combine(cTexPathRS, Tex + Main))) {
											node.m_nodeMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + Main)));
											if(File.Exists(Path.Combine(cTexPathRS, Tex + APR)))
												node.m_nodeMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + APR)));
											}
										}
									else {
										Tex = RS + Gd + Nd;
										if(File.Exists(Path.Combine(cTexPathRS, Tex + Main))) {
											node.m_nodeMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + Main)));
											if(File.Exists(Path.Combine(cTexPathRS, Tex + APR))) {
												node.m_nodeMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + APR)));
												}
											}
										}
									}
								else if(netInfo.name.Contains("Oneway Road")) {//mesh
									Tex = RS + Ow;
									if(netInfo.name.Equals("Oneway Road Elevated")) {
										Tex += El + Nd;
										if(File.Exists(Path.Combine(cTexPathRS, Tex + Main))) {
											node.m_nodeMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + Main)));
											if(File.Exists(Path.Combine(cTexPathRS, Tex + APR))) {
												node.m_nodeMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + APR)));
												}
											}
										}
									else if((netInfo.name.Equals("Oneway Road Decoration Grass")) || (netInfo.name.Equals("Oneway Road Decoration Trees"))) {//mesh
										Tex = RS + Ow + Dc + Nd;
										if(File.Exists(Path.Combine(cTexPathRS, Tex + Main))) {
											node.m_nodeMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + Main)));
											if(File.Exists(Path.Combine(cTexPathRS, Tex + APR)))
												node.m_nodeMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + APR)));
											}
										}
									else {
										Tex += Gd + Nd;
										if(File.Exists(Path.Combine(cTexPathRS, Tex + Main))) {
											node.m_nodeMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + Main)));
											if(File.Exists(Path.Combine(cTexPathRS, Tex + APR))) {
												node.m_nodeMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + APR)));
												}
											}
										}
									}
								else if(netInfo.name.Equals("Basic Road Bicycle")) {//mesh
									Tex = RS + Gd + Nd;
									if(File.Exists(Path.Combine(cTexPathRS, Tex + Main))) {
										node.m_nodeMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPathRS, Tex + APR)))
											node.m_nodeMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + APR)));
										}
									}
								else if(netInfo.name.Equals("Basic Road Elevated Bike")) {
									Tex = RS + Bk + El + Nd;
									if(File.Exists(Path.Combine(cTexPathRS, Tex + Main))) {
										node.m_nodeMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPathRS, Tex + APR)))
											node.m_nodeMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + APR)));
										}
									}
								else if((netInfo.name.Contains("Small Road Monorail")) && (!node.m_mesh.name.Contains("monorail"))) {
									if(netInfo.name.Equals("Small Road Monorail Elevated")) {
										Tex = RS + El + Nd;
										if(File.Exists(Path.Combine(cTexPathRS, Tex + Main))) {
											node.m_nodeMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + Main)));
											if(File.Exists(Path.Combine(cTexPathRS, Tex + APR)))
												node.m_nodeMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + APR)));
											}
										}
									else{
										Tex = RS + Gd + Nd;
										node.m_nodeMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPathRS, Tex + APR)))
											node.m_nodeMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + APR)));
										}
									}
								else if(netInfo.name.Contains("Asymmetrical Three Lane Road")) {
									Tex = RL;
									if(netInfo.name.Equals("Asymmetrical Three Lane Road Elevated")) {
										Tex += El + Nd;
										if(File.Exists(Path.Combine(cTexPathRL, Tex + Main))) {
											node.m_nodeMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRL, Tex + Main)));
											if(File.Exists(Path.Combine(cTexPathRL, Tex + APR))) {
												node.m_nodeMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRL, Tex + APR)));
												}
											}
										}
									else if(netInfo.name.Equals("Asymmetrical Three Lane Road Slope")) {
										Tex += Sl + Nd;
										if(File.Exists(Path.Combine(cTexPathRL, Tex + Main))) {
											node.m_nodeMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRL, Tex + Main)));
											if(File.Exists(Path.Combine(cTexPathRL, Tex + APR))) {
												node.m_nodeMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRL, Tex + APR)));
												}
											}
										}
									else {
										Tex += Gd + Nd;
										if(File.Exists(Path.Combine(cTexPathRL, Tex + Main))) {
											node.m_nodeMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRL, Tex + Main)));
											if(File.Exists(Path.Combine(cTexPathRL, Tex + APR))) {
												node.m_nodeMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRL, Tex + APR)));
												}
											}
										}
									}

								#endregion
								//RoadMedium
								#region Medium Roads
								else if(netInfo.name.Contains("Medium Road") && !netInfo.name.Contains("Tram") && !node.m_mesh.name.Contains("monorail")) {//mesh
									Tex = RM;
									if((netInfo.name.Contains("Medium Road Elevated")) && (!(netInfo.name.Contains("Bike")))) {
										Tex += El + Nd;
										if(File.Exists(Path.Combine(cTexPathRM, Tex + Main))) {
											node.m_nodeMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRM, Tex + Main)));
											if(File.Exists(Path.Combine(cTexPathRM, Tex + APR)))
												node.m_nodeMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRM, Tex + APR)));
											}
										}
									else if((netInfo.name.Equals("Medium Road Decoration Grass")) || (netInfo.name.Equals("Medium Road Decoration Trees"))) {//mesh
										Tex = RM + Dc + Nd;
										if(File.Exists(Path.Combine(cTexPathRM, Tex + Main))) {
											node.m_nodeMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRM, Tex + Main)));
											if(File.Exists(Path.Combine(cTexPathRM, Tex + APR)))
												node.m_nodeMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRM, Tex + APR)));
											}
										}
									else {
										Tex += Gd + Nd;
										if(File.Exists(Path.Combine(cTexPathRM, Tex + Main))) {
											node.m_nodeMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRM, Tex + Main)));
											if(File.Exists(Path.Combine(cTexPathRM, Tex + APR)))
												node.m_nodeMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRM, Tex + APR)));
											}
										}
									}
								else if(netInfo.name.Equals("Medium Road Elevated Bike")) {
									Tex = RM + Bk + El + Nd;
									if(File.Exists(Path.Combine(cTexPathRM, Tex + Main))) {
										node.m_nodeMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRM, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPathRM, Tex + APR)))
											node.m_nodeMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRM, Tex + APR)));
										}
									}
								else if(netInfo.name.Contains("Avenue Large With Grass") && !netInfo.name.Contains("Buslanes")) {
									Tex = RM + Av + Gd + Nd;
									if(File.Exists(Path.Combine(cTexPathRM, Tex + Main))) {
										node.m_nodeMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRM, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPathRM, Tex + APR)))
											node.m_nodeMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRM, Tex + APR)));
										}
									}
								else if(netInfo.name.Contains("Avenue Large With Buslanes Grass")) {
									Tex = RM + Av + Bs + Gd + Nd;
									if(File.Exists(Path.Combine(cTexPathRM, Tex + Main))) {
										node.m_nodeMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRM, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPathRM, Tex + APR)))
											node.m_nodeMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRM, Tex + APR)));
										}
									}
								else if(netInfo.name.Contains("Medium Road Monorail")) {
									Tex = RM;
									if(netInfo.name.Contains("Elevated") && node.m_mesh.name.Equals("RoadMediumElevatedNode")) {
										Tex += El + Nd;
										if(File.Exists(Path.Combine(cTexPathRM, Tex + Main))) {
											node.m_nodeMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRM, Tex + Main)));
											if(File.Exists(Path.Combine(cTexPathRM, Tex + APR)))
												node.m_nodeMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRM, Tex + APR)));
											}
										}
									else if(netInfo.name.Equals("Medium Road Monorail") && node.m_mesh.name.Equals("RoadMediumNode")) {
										Tex += Gd + Nd;
										if(File.Exists(Path.Combine(cTexPathRM, Tex + Main))) {
											node.m_nodeMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRM, Tex + Main)));
											if(File.Exists(Path.Combine(cTexPathRM, Tex + APR)))
												node.m_nodeMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRM, Tex + APR)));
											}
										}
									}
								#endregion
								//RoadLarge
								#region Large Roads
								if(netInfo.name.Contains("Large Road")) {//mesh
									Tex = RL;
									if(netInfo.name.Equals("Large Road Elevated")) {
										Tex += El + Nd;
										if(File.Exists(Path.Combine(cTexPathRL, Tex + Main))) {
											node.m_nodeMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRL, Tex + Main)));
											if(File.Exists(Path.Combine(cTexPathRL, Tex + APR)))
												node.m_nodeMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRL, Tex + APR)));
											}
										}
									else if((netInfo.name.Equals("Large Road Decoration Grass")) || (netInfo.name.Equals("Large Road Decoration Trees"))) {//mesh
										Tex = RL + Dc + Nd;
										if(File.Exists(Path.Combine(cTexPathRL, Tex + Main))) {
											node.m_nodeMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRL, Tex + Main)));
											if(File.Exists(Path.Combine(cTexPathRL, Tex + APR)))
												node.m_nodeMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRL, Tex + APR)));
											}
										}
									else {
										Tex += Gd + Nd;
										if(File.Exists(Path.Combine(cTexPathRL, Tex + Main))) {
											node.m_nodeMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRL, Tex + Main)));
											if(File.Exists(Path.Combine(cTexPathRL, Tex + APR)))
												node.m_nodeMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRL, Tex + APR)));
											}
										}
									}
								else if(netInfo.name.Equals("Large Road Elevated Bike")) {
									Tex = RL + Bk + El + Nd;
									if(File.Exists(Path.Combine(cTexPathRL, Tex + Main))) {
										node.m_nodeMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRL, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPathRL, Tex + APR)))
											node.m_nodeMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRL, Tex + APR)));
										}
									}
								else if(netInfo.name.Contains("Large Oneway")) {//mesh
									Tex = RL + Ow;
									if(netInfo.name.Equals("Large Oneway Elevated")) {
										Tex += El + Nd;
										if(File.Exists(Path.Combine(cTexPathRL, Tex + Main))) {
											node.m_nodeMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRL, Tex + Main)));
											if(File.Exists(Path.Combine(cTexPathRL, Tex + APR)))
												node.m_nodeMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRL, Tex + APR)));
											}
										}
									else if((netInfo.name.Equals("Large Oneway Decoration Grass")) || (netInfo.name.Equals("Large Oneway Decoration Trees"))) {//mesh
										Tex = RL + Ow + Dc + Nd;
										if(File.Exists(Path.Combine(cTexPathRL, Tex + Main))) {
											node.m_nodeMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRL, Tex + Main)));
											if(File.Exists(Path.Combine(cTexPathRL, Tex + APR)))
												node.m_nodeMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRL, Tex + APR)));
											}
										}
									else if(netInfo.name.Equals("Large Oneway Road Slope")) {
										Tex = RL + Ow + Sl + Nd;
										if(File.Exists(Path.Combine(cTexPathRL, Tex + Main))) {
											node.m_nodeMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRL, Tex + Main)));
											if(File.Exists(Path.Combine(cTexPathRL, Tex + APR)))
												node.m_nodeMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRL, Tex + APR)));
											}
										}
									else {
										Tex += Gd + Nd;
										if(File.Exists(Path.Combine(cTexPathRL, Tex + Main))) {
											node.m_nodeMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRL, Tex + Main)));
											if(File.Exists(Path.Combine(cTexPathRL, Tex + APR)))
												node.m_nodeMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRL, Tex + APR)));
											}
										}
									}
								#endregion
								//Highway
								#region Highway
								else if(netInfo.name.Contains("Highway")) {
									if(!netInfo.name.Contains("Asym")&&!netInfo.name.Contains("Highway2L2W")) {
									if(netInfo.name.Equals("HighwayRamp")) {
										Tex = HW + "Ramp" + Nd;
										if(File.Exists(Path.Combine(cTexPathHW, Tex + Main))) {
											node.m_nodeMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + Main)));
											if(File.Exists(Path.Combine(cTexPathHW, Tex + APR)))
												node.m_nodeMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + APR)));
											}
										}
									else if(netInfo.name.Equals("HighwayRampElevated")) {
										Tex = HW + "Ramp" + Nd;
										if(File.Exists(Path.Combine(cTexPathHW, Tex + Main))) {
											node.m_nodeMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + Main)));
											if(File.Exists(Path.Combine(cTexPathHW, Tex + APR)))
												node.m_nodeMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + APR)));
											}
										}
									else if(netInfo.name.Equals("HighwayRamp Slope")) {
										Tex = HW + "Ramp" + Nd;
										if(File.Exists(Path.Combine(cTexPathHW, Tex + Main))) {
											node.m_nodeMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + Main)));
											if(File.Exists(Path.Combine(cTexPathHW, Tex + APR)))
												node.m_nodeMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + APR)));
											}
										}
									else if(netInfo.name.Equals("Highway Elevated")) {
										Tex = HW + "3L" + Nd;
										if(File.Exists(Path.Combine(cTexPathHW, Tex + Main))) {
											node.m_nodeMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + Main)));
											if(File.Exists(Path.Combine(cTexPathHW, Tex + APR)))
												node.m_nodeMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + APR)));
											}
										}
									else if(netInfo.name.Equals("Highway Slope")) {
										Tex = HW + "3L" + Nd;
										if(File.Exists(Path.Combine(cTexPathHW, Tex + Main))) {
											node.m_nodeMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + Main)));
											if(File.Exists(Path.Combine(cTexPathHW, Tex + APR)))
												node.m_nodeMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + APR)));
											}
										}
									else if(netInfo.name.Equals("Highway Barrier")) {
										Tex = HW + "3L" + Nd;
										if(File.Exists(Path.Combine(cTexPathHW, Tex + Main))) {
											node.m_nodeMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + Main)));
											if(File.Exists(Path.Combine(cTexPathHW, Tex + APR)))
												node.m_nodeMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + APR)));
											}
										}
									else if(netInfo.name.Equals("Highway")) {
										Tex = HW + "3L" + Nd;
										if(File.Exists(Path.Combine(cTexPathHW, Tex + Main))) {
											node.m_nodeMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + Main)));
											if(File.Exists(Path.Combine(cTexPathHW, Tex + APR)))
												node.m_nodeMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + APR)));
											}
										}

									//2L2W
									else if((netInfo.name.Equals("Two Lane Highway Twoway")) || 
										    (netInfo.name.Equals("Two Lane Highway Twoway Barrier")) || 
											(netInfo.name.Equals("Two Lane Highway Twoway Elevated")) || 
											(netInfo.name.Equals("Two Lane Highway Twoway Bridge")) ||
											(netInfo.name.Equals("Two Lane Highway Twoway Slope")) || 
											(netInfo.name.Equals("Two Lane Highway Twoway Tunnel"))
										) {
										Tex = HW + "2L2W" + Nd;
										if(File.Exists(Path.Combine(cTexPathHW, Tex + Main))) {
										node.m_nodeMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + Main)));
											//APR
											if(File.Exists(Path.Combine(cTexPathHW, Tex + APR)))
												node.m_nodeMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + APR)));
											}
										}
									else if(netInfo.name.Equals("Two Lane Highway Elevated") || 
											netInfo.name.Equals("Two Lane Highway Bridge") ||
											(netInfo.name.Equals("Two Lane Highway Slope") && node.m_nodeMesh.name.Equals("two_lane_highway_tunnel_slope")) ||
											(netInfo.name.Equals("Two Lane Highway Tunnel") && node.m_nodeMesh.name.Equals("two_lane_highway_tunnel_slope")) ||
											netInfo.name.Equals("Two Lane Highway") || 
											netInfo.name.Equals("Two Lane Highway Barrier")) {
										Tex = HW + "2L2W" + Nd;
										if(File.Exists(Path.Combine(cTexPathHW, Tex + Main))) {
										node.m_nodeMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + Main)));
											//APR
											if(File.Exists(Path.Combine(cTexPathHW, Tex + APR)))
												node.m_nodeMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + APR)));
											}
										}
										//4L
									else if(netInfo.name.Equals("Four Lane Highway") || 
											netInfo.name.Equals("Four Lane Highway Barrier") ||
											netInfo.name.Equals("Four Lane Highway Elevated") || 
											netInfo.name.Equals("Four Lane Highway Bridge") ||
											netInfo.name.Equals("Four Lane Highway Slope") || 
											netInfo.name.Equals("Four Lane Highway Tunnel")) {
										Tex = HW + "4LB" + Nd;
										if(File.Exists(Path.Combine(cTexPathHW, Tex + Main))) {
										node.m_nodeMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + Main)));
											//APR
											if(File.Exists(Path.Combine(cTexPathHW, Tex + APR)))
												node.m_nodeMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + APR)));
											}

										}
								}else{//These are the NExt Roads, past few updates changed the way the roads were added in.
									if(netInfo.name.Contains("L1R2")) {
										Tex = HW + "3L2W";
										if(netInfo.name.Equals("AsymHighwayL1R2 Slope")) {
										Tex += Sl;
											if(File.Exists(Path.Combine(cTexPathHW, Tex + Main))) {
												node.m_nodeMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + Nd + Main)));
												//APR
												if(File.Exists(Path.Combine(cTexPathHW, Tex + APR)))
													node.m_nodeMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + Nd + APR)));
												}
											}
										if(netInfo.name.Equals("AsymHighwayL1R2 Tunnel")) {
											Tex += Tl;
											if(File.Exists(Path.Combine(cTexPathHW, Tex + Main))) {
												node.m_nodeMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + Nd + Main)));
												//APR
												if(File.Exists(Path.Combine(cTexPathHW, Tex + APR)))
													node.m_nodeMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + Nd + APR)));
												}
											}
										if(node.m_nodeMesh.name.Contains("HighwayBridgeNode")) {
											Tex += El;
											if(File.Exists(Path.Combine(cTexPathHW, Tex + Main))) {
												node.m_nodeMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + Nd + Main)));
												//APR
												if(File.Exists(Path.Combine(cTexPathHW, Tex + APR)))
													node.m_nodeMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + Nd + APR)));
												}
											}
										else if(netInfo.name.Equals("AsymHighwayL1R2")) {
										Tex += Gd;
										if(File.Exists(Path.Combine(cTexPathHW, Tex + Main))) {
											node.m_nodeMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + Nd + Main)));
											//APR
											if(File.Exists(Path.Combine(cTexPathHW, Tex + APR)))
												node.m_nodeMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + Nd + APR)));
											}
										}
										}
									if(netInfo.name.Contains("Highway2L2W")) {
										Tex = HW + "4L2W";
										if(netInfo.name.Equals("Highway2L2W")) {
											Tex += Gd;
											if(File.Exists(Path.Combine(cTexPathHW, Tex + Main))) {
												node.m_nodeMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + Nd + Main)));
												//APR
												if(File.Exists(Path.Combine(cTexPathHW, Tex + APR)))
													node.m_nodeMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + Nd + APR)));
												}
											}
										if(node.m_nodeMesh.name.Contains("HighwayBridgeNode")) {
											Tex += El;
											if(File.Exists(Path.Combine(cTexPathHW, Tex + Main))) {
												node.m_nodeMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + Nd + Main)));
												//APR
												if(File.Exists(Path.Combine(cTexPathHW, Tex + APR)))
													node.m_nodeMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + Nd + APR)));
												}
											}
										if(netInfo.name.Equals("Highway2L2W Slope")) {
											Tex += Sl;
											if(File.Exists(Path.Combine(cTexPathHW, Tex + Main))) {
												node.m_nodeMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + Nd + Main)));
												//APR
												if(File.Exists(Path.Combine(cTexPathHW, Tex + APR)))
													node.m_nodeMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + Nd + APR)));
												}
											}
										if(netInfo.name.Equals("Highway2L2W Tunnel")) {
											Tex += Tl;
											if(File.Exists(Path.Combine(cTexPathHW, Tex + Main))) {
												node.m_nodeMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + Nd + Main)));
												//APR
												if(File.Exists(Path.Combine(cTexPathHW, Tex + APR)))
													node.m_nodeMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + Nd + APR)));
												}
											}
										}
									}
								}
								#endregion
								//Tram
								#region Trams
								else if(netInfo.name.Equals("Basic Road Tram")) {//mesh
									Tex = RS + Gd + Nd;
									if(File.Exists(Path.Combine(cTexPathRS, Tex + Main))) {
										node.m_nodeMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPathRS, Tex + APR)))
											node.m_nodeMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + APR)));
										}
									}
								else if(netInfo.name.Equals("Basic Road Elevated Tram")) {
									Tex = RS + El + Nd;
									if(File.Exists(Path.Combine(cTexPathRS, Tex + Main))) {
										node.m_nodeMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPathRS, Tex + APR)))
											node.m_nodeMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + APR)));
										}
									}
								else if(netInfo.name.Equals("Basic Road Slope Tram")) {
									Tex = RS + Gd + Nd;
									if(File.Exists(Path.Combine(cTexPathRS, Tex + Main))) {
										node.m_nodeMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPathRS, Tex + APR)))
											node.m_nodeMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + APR)));
										}
									}
								else if(netInfo.name.Equals("Oneway Road Tram")) {
									Tex = RS + Ow + Gd + Nd;
									if(File.Exists(Path.Combine(cTexPathRS, Tex + Main))) {
										node.m_nodeMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPathRS, Tex + APR)))
											node.m_nodeMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + APR)));
										}
									}
								else if(netInfo.name.Equals("Oneway Road Elevated Tram")) {
									Tex = RS + Ow + El + Nd;
									if(File.Exists(Path.Combine(cTexPathRS, Tex + Main))) {
										node.m_nodeMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPathRS, Tex + APR)))
											node.m_nodeMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + APR)));
										}
									}
								else if(netInfo.name.Equals("Oneway Road Slope Tram")) {
									Tex = RS + Ow + Gd + Nd;
									if(File.Exists(Path.Combine(cTexPathRS, Tex + Main))) {
										node.m_nodeMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPathRS, Tex + APR)))
											node.m_nodeMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + APR)));
										}
									}
								else if(netInfo.name.Equals("Medium Road Tram")) {//mesh
									Tex = RM + Tr + Gd + Nd;
									if(File.Exists(Path.Combine(cTexPathRM, Tex + Main))) {
										node.m_nodeMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRM, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPathRM, Tex + APR)))
											node.m_nodeMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRM, Tex + APR)));
										}
									}
								else if(netInfo.name.Equals("Medium Road Elevated Tram")) {
									Tex = RM + Tr + El + Nd;
									if(File.Exists(Path.Combine(cTexPathRM, Tex + Main))) {
										node.m_nodeMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRM, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPathRM, Tex + APR)))
											node.m_nodeMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRM, Tex + APR)));
										}
									}
								else if(netInfo.name.Equals("Medium Road Slope Tram")) {
									Tex = RM + Tr + Gd + Nd;
									if(File.Exists(Path.Combine(cTexPathRM, Tex + Main))) {
										node.m_nodeMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRM, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPathRM, Tex + APR)))
											node.m_nodeMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRM, Tex + APR)));
										}
									}
								else if(netInfo.name.Equals("Tram Track")) {//mesh
									Tex = RS + Dc + Nd;
									if(File.Exists(Path.Combine(cTexPathRS, Tex + Main))) {
										node.m_nodeMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPathRS, Tex + APR)))
											node.m_nodeMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + APR)));
										}
									}
								else if(netInfo.name.Equals("Tram Track Elevated")) {//mesh
									Tex = RS + El + Nd;
									if(File.Exists(Path.Combine(cTexPathRS, Tex + Main))) {
										node.m_nodeMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPathRS, Tex + APR)))
											node.m_nodeMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + APR)));
										}
									}
								else if(netInfo.name.Equals("Tram Track Slope")) {//mesh
									Tex = RS + Dc + Nd;
									if(File.Exists(Path.Combine(cTexPathRS, Tex + Main))) {
										node.m_nodeMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPathRS, Tex + APR)))
											node.m_nodeMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + APR)));
										}
									}
								else if(netInfo.name.Equals("Oneway Tram Track")) {//mesh
									Tex = RS + Dc + Nd;
									if(File.Exists(Path.Combine(cTexPathRS, Tex + Main))) {
										node.m_nodeMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPathRS, Tex + APR)))
											node.m_nodeMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + APR)));
										}
									}
								else if(netInfo.name.Equals("Oneway Tram Track Elevated")) {//mesh
									Tex = RS + El + Nd;
									if(File.Exists(Path.Combine(cTexPathRS, Tex + Main))) {
										node.m_nodeMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPathRS, Tex + APR)))
											node.m_nodeMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + APR)));
										}
									}
								else if(netInfo.name.Equals("Oneway Tram Track Slope")) {//mesh
									Tex = RS + Dc + Nd;
									if(File.Exists(Path.Combine(cTexPathRS, Tex + Main))) {
										node.m_nodeMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPathRS, Tex + APR)))
											node.m_nodeMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + APR)));
										}
									}
								#endregion
								} else {
								if(File.Exists(nodeMaterialTexture_name))
									node.m_nodeMaterial.SetTexture(main, LoadTextureDDS(nodeMaterialTexture_name));
								if((node.m_nodeMaterial.GetTexture(apr) != null) && (!(node.m_nodeMaterial.name.Equals(TrRailA))) && (!(node.m_nodeMaterial.name.Equals(TrRailB)))) {
									string nodeMaterialAPRMap_name = Path.Combine(ModLoader.currentTexturesPath_default, node.m_nodeMaterial.GetTexture("_APRMap").name + ".dds");
									if(File.Exists(nodeMaterialAPRMap_name))
										node.m_nodeMaterial.SetTexture("_APRMap", LoadTextureDDS(nodeMaterialAPRMap_name));
									}
								}
							//node.m_lodMesh=null;
							}
						if((node.m_nodeMaterial.GetTexture(apr) != null) && (!(node.m_nodeMaterial.name.Contains("rail")))) {
							string nodeMaterialAPRMap_name = Path.Combine(cTexDefault, node.m_nodeMaterial.GetTexture(apr).name + ".dds");

							if(File.Exists(nodeMaterialAPRMap_name))
								node.m_nodeMaterial.SetTexture(apr, LoadTextureDDS(nodeMaterialAPRMap_name));
							}
						}
					// Look for segments
					NetInfo.Segment[] segments = netInfo.m_segments;
					for(int l = 0;l < segments.Length;l++) {
						NetInfo.Segment segment = segments[l];
						if((segment.m_segmentMaterial.GetTexture("_MainTex") != null) && (!(segment.m_segmentMaterial.name.Contains("rail"))) && (!segment.m_material.name.ToLower().Contains("cable"))) {
							string segmentMaterialTexture_name = Path.Combine(ModLoader.currentTexturesPath_default, segment.m_segmentMaterial.GetTexture("_MainTex").name + ".dds");
							#region Small Roads
							if(netInfo.name.Contains("Basic Road")) {
								Tex = RS;
								if(segment.m_segmentMesh.name.Equals("SmallRoadSegment")) {
									Tex += Gd;
									if(File.Exists(Path.Combine(cTexPathRS, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPathRS, Tex + APR)))
											segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + APR)));

										} else if(segmentMaterialTexture_name.Equals(Path.Combine(cTexDefault, "RoadSmall_D.dds")))
										segmentMaterialTexture_name = Path.Combine(cTexDefault, "RoadSmall_D.dds");
									} else if(segment.m_segmentMesh.name.Equals("SmallRoadSegmentBusSide")) {
									Tex += B1;
									if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "RoadSmall_D_BusSide.dds")))
										segmentMaterialTexture_name = Path.Combine(ModLoader.currentTexturesPath_default, "RoadSmall_D_BusSide.dds");
									else if(File.Exists(Path.Combine(cTexPathRS, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPathRS, Tex + APR)))
											segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + APR)));
										}
									} else if(segment.m_segmentMesh.name.Equals("SmallRoadSegmentBusBoth")) {
									Tex += B2;
									if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "RoadSmall_D_BusBoth.dds")))
										segmentMaterialTexture_name = Path.Combine(ModLoader.currentTexturesPath_default, "RoadSmall_D_BusBoth.dds");
									else if(File.Exists(Path.Combine(cTexPathRS, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPathRS, Tex + APR)))
											segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + APR)));
										}
									} else if(!(segment.m_mesh.name.Equals("SmallRoadSegment"))) {
									if((netInfo.name.Equals("Basic Road Elevated")) || (netInfo.name.Equals("Basic Road Bridge"))) {
										Tex += El;
										if(File.Exists(Path.Combine(cTexPathRS, Tex + Main))) {
											segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + Main)));
											if(File.Exists(Path.Combine(cTexPathRS, Tex + APR)))
												segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + APR)));
											}
										} else if(netInfo.name.Equals("Basic Road Slope")) {
										Tex += Sl;
										if(File.Exists(Path.Combine(cTexPathRS, Tex + Main))) {
											segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + Main)));
											if(File.Exists(Path.Combine(cTexPathRS, Tex + APR)))
												segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + APR)));
											}
										}
									}
								}
							if(netInfo.name.Contains("Oneway Road")) {
								Tex = RS + Ow;
								if(segment.m_mesh.name.Equals("SmallRoadSegment")) {
									Tex = RS + Ow + Gd;
									if(File.Exists(Path.Combine(cTexDefault, "Oneway_RoadSmallSegment.dds")))
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexDefault, "Oneway_RoadSmallSegment.dds")));
									else if(File.Exists(Path.Combine(cTexPathRS, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPathRS, Tex + APR)))
											segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + APR)));
										}
									} else if(segment.m_mesh.name.Equals("SmallRoadSegmentBusSide")) {
									Tex = RS + Ow + B1;
									if(File.Exists(Path.Combine(cTexDefault, "Oneway_RoadSmallSegment_BusSide.dds")))
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "Oneway_RoadSmallSegment_BusSide.dds")));
									else if(File.Exists(Path.Combine(cTexPathRS, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPathRS, Tex + APR)))
											segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + APR)));
										}
									} else if(segment.m_mesh.name.Equals("SmallRoadSegmentBusBoth")) {
									Tex = RS + Ow + B2;
									if(File.Exists(Path.Combine(cTexDefault, "Oneway_RoadSmallSegment_BusBoth.dds")))
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexDefault, "Oneway_RoadSmallSegment_BusBoth.dds")));
									else if(File.Exists(Path.Combine(cTexPathRS, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPathRS, Tex + APR)))
											segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + APR)));
										}
									} else if(!(segment.m_mesh.name.Equals("SmallRoadSegment"))) {
									if((netInfo.name.Equals("Oneway Road Elevated")) || (netInfo.name.Equals("Oneway Road Bridge"))) {
										Tex = RS + Ow + El;
										if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "Oneway_RoadSmallElevatedSegment_D.dds")))
											segment.m_segmentMaterial.SetTexture("_MainTex", LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "Oneway_RoadSmallElevatedSegment_D.dds")));
										else if(File.Exists(Path.Combine(cTexPathRS, Tex + Main))) {
											segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + Main)));
											if(File.Exists(Path.Combine(cTexPathRS, Tex + APR)))
												segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + APR)));
											}
										} else if(netInfo.name.Equals("Oneway Road Slope")) {
										Tex = RS + Ow + Sl;
										if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "Oneway_small-tunnel_d.dds")))
											segment.m_segmentMaterial.SetTexture("_MainTex", LoadTextureDDS(Path.Combine(ModLoader.currentTexturesPath_default, "Oneway_small-tunnel_d.dds")));
										else if(File.Exists(Path.Combine(cTexPathRS, Tex + Main))) {
											segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + Main)));
											if(File.Exists(Path.Combine(cTexPathRS, Tex + APR)))
												segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + APR)));
											}
										}
									}
								}
							if(netInfo.name.Equals("Basic Road Bicycle")) {
								if(segment.m_mesh.name.Equals("SmallRoadSegment")) {
									Tex = RS + Bk + Gd;
									if(File.Exists(Path.Combine(cTexPathRS, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPathRS, Tex + APR)))
											segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + APR)));
										}
									} else if(segment.m_mesh.name.Equals("SmallRoadSegmentBusSide")) {
									Tex = RS + Bk + B1;
									if(ModLoader.config.basic_road_parking == 1) {
										if(segmentMaterialTexture_name.Equals(Path.Combine(cTexDefault, ("RoadSmall_D_BusSide.dds"))))
											segmentMaterialTexture_name = Path.Combine(cTexDefault, "RoadSmall_D_BusSide_parking1.dds");
										} else if(File.Exists(Path.Combine(cTexPathRS, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPathRS, Tex + APR)))
											segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + APR)));
										}
									} else if(segment.m_mesh.name.Equals("SmallRoadSegmentBusBoth")) {
									Tex = RS + Bk + B2;
									if(File.Exists(Path.Combine(cTexPathRS, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPathRS, Tex + APR)))
											segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + APR)));
										}
									}
								}
							if(netInfo.name.Equals("Basic Road Elevated Bike")) {
								Tex = RS + Bk + El;
								if(File.Exists(Path.Combine(cTexPathRS, Tex + Main))) {
									segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + Main)));
									if(File.Exists(Path.Combine(cTexPathRS, Tex + APR)))
										segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + APR)));
									}
								}
							if((netInfo.name.Equals("Basic Road Decoration Grass")) || (netInfo.name.Equals("Basic Road Decoration Trees"))) {

								if(segment.m_mesh.name.Equals("SmallRoadSegment2")) {
									Tex = RS + Dc;
									if(File.Exists(Path.Combine(cTexPathRS, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPathRS, Tex + APR)))
											segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + APR)));
										}
									} else if(segment.m_mesh.name.Equals("SmallRoadSegment2BusSide")) {
									Tex = RS + Dc + B1;
									if(File.Exists(Path.Combine(cTexDefault, "SmallRoadSegmentDeco_BusSide.dds")))
										segmentMaterialTexture_name = Path.Combine(cTexDefault, "SmallRoadSegmentDeco_BusSide.dds");
									else if(File.Exists(Path.Combine(cTexPathRS, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPathRS, Tex + APR)))
											segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + APR)));
										}
									} else if(segment.m_mesh.name.Equals("SmallRoadSegment2BusBoth")) {
									Tex = RS + Dc + B2;
									if(File.Exists(Path.Combine(cTexDefault, "SmallRoadSegmentDeco_BusSide.dds")))
										segmentMaterialTexture_name = Path.Combine(cTexDefault, "SmallRoadSegmentDeco_BusBoth.dds");
									else if(File.Exists(Path.Combine(cTexPathRS, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPathRS, Tex + APR)))
											segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + APR)));
										}
									}
								}
							if((netInfo.name.Equals("Oneway Road Decoration Grass")) || (netInfo.name.Equals("Oneway Road Decoration Trees"))) {
								if(segment.m_mesh.name.Equals("SmallRoadSegment2")) {
									Tex = RS + Ow + Dc;
									if(File.Exists(Path.Combine(cTexDefault, "Oneway_SmallRoadSegmentDeco.dds")))
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexDefault, "Oneway_SmallRoadSegmentDeco.dds")));
									else if(File.Exists(Path.Combine(cTexPathRS, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPath, Tex + APR)))
											segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + APR)));
										}
									} else if(segment.m_mesh.name.Equals("SmallRoadSegment2BusSide")) {
									Tex = RS + Ow + Dc + B2;
									if(File.Exists(Path.Combine(cTexDefault, "Oneway_SmallRoadSegmentDeco_BusSide.dds")))
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexDefault, "Oneway_SmallRoadSegmentDeco_BusSide.dds")));
									else if(File.Exists(Path.Combine(cTexPathRS, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPathRS, Tex + APR)))
											segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + APR)));
										}
									} else if(segment.m_mesh.name.Equals("SmallRoadSegment2BusBoth")) {
									Tex = RS + Ow + Dc + B2;
									if(File.Exists(Path.Combine(cTexDefault, "Oneway_SmallRoadSegmentDeco_BusBoth.dds")))
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexDefault, "Oneway_SmallRoadSegmentDeco_BusBoth.dds")));
									else if(File.Exists(Path.Combine(cTexPath, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPath, Tex + APR)))
											segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + APR)));
										}
									}
								}
							if(netInfo.name.Contains("Asymmetrical Three Lane Road")) {
								if(segment.m_segmentMesh.name.Equals("AsymmetricalThreeLaneSegment")) {
									Tex = RL + Gd;
									if(File.Exists(Path.Combine(cTexPathRL, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRL, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPathRL, Tex + APR)))
											segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRL, Tex + APR)));
										}
									} 
								if((segment.m_segmentMesh.name.Equals("AsymmetricalThreeLaneGhostIsland"))||(segment.m_segmentMesh.name.Equals("AsymmetricalThreeLaneGhostIsland2"))) {
									Tex = "AsymmetricalThreeLaneGhostIsland";
									if(File.Exists(Path.Combine(cTexPathRS, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPathRS, Tex + APR)))
											segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + APR)));
										}
									}
								if(segment.m_segmentMesh.name.Equals("AsymmetricalThreeLaneElevatedSegment")) {
									Tex = RL + El;
									if(File.Exists(Path.Combine(cTexPathRL, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRL, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPathRL, Tex + APR)))
											segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRL, Tex + APR)));
										}
									}
								if(segment.m_segmentMesh.name.Equals("AsymmetricalThreeLaneTunnelSlope") || segment.m_segmentMesh.name.Equals("AsymmetricalThreeLaneTunnelSlope2")) {
									Tex = RL + Sl;
									if(File.Exists(Path.Combine(cTexPathRL, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRL, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPathRL, Tex + APR)))
											segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRL, Tex + APR)));
										}
									}
								if((segment.m_segmentMesh.name.Equals("AsymmetricalThreeLaneElevatedGhostIsland")) || (segment.m_segmentMesh.name.Equals("AsymmetricalThreeLaneElevatedGhostIsland2"))) {
								Tex = "AsymmetricalThreeLaneElevatedGhostIsland";
									if(File.Exists(Path.Combine(cTexPathRS, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPath, Tex + APR)))
											segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + APR)));
										}
									}
								}
							if(netInfo.name.Contains("BasicRoadPntMdn")) {
								Tex = RS + Md;
								if(netInfo.name.Equals("BasicRoadPntMdn")) {
									Tex += Gd;
									if(File.Exists(Path.Combine(cTexPathRS, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPath, Tex + APR)))
											segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + APR)));
										}
									}
								if(netInfo.name.Equals("BasicRoadPntMdn Elevated")) {
									Tex += El;
									if(File.Exists(Path.Combine(cTexPathRS, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPath, Tex + APR)))
											segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + APR)));
										}
									}
								if(netInfo.name.Equals("BasicRoadPntMdn	Slope")) {
									Tex += Sl;
									if(File.Exists(Path.Combine(cTexPathRS, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPath, Tex + APR)))
											segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + APR)));
										}
									else {
										Tex = RS+Md+Gd;
										if(File.Exists(Path.Combine(cTexPathRS, Tex + Main))) {
											segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + Main)));
											if(File.Exists(Path.Combine(cTexPath, Tex + APR)))
												segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + APR)));
											}
										}
									}
								if(netInfo.name.Equals("BasicRoadPntMdn Tunnel")) {
									Tex += Tl;
									if(File.Exists(Path.Combine(cTexPathRS, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPath, Tex + APR)))
											segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + APR)));
										}
									}
								}
							if(netInfo.name.Contains("Small Road Monorail")) {
							if(segment.m_segmentMesh.name.Equals("SmallRoadSegment")) {
								Tex = RS + Gd;
								if(File.Exists(Path.Combine(cTexPathRS, Tex + Main))) {
									segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + Main)));
									if(File.Exists(Path.Combine(cTexPath, Tex + APR)))
										segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + APR)));
									}
								}
							if(segment.m_segmentMesh.name.Equals("SmallRoadElevatedSegment")) {
								Tex = RS + El;
								if(File.Exists(Path.Combine(cTexPathRS, Tex + Main))) {
									segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + Main)));
									if(File.Exists(Path.Combine(cTexPath, Tex + APR)))
										segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + APR)));
									}
								}
							}
							
							#endregion
							//RoadMedium
							#region Medium Roads
							if((netInfo.name.Contains("Medium Road"))&&(!netInfo.name.Contains("Monorail"))) {//mesh
								Tex = RM;
								if(segment.m_mesh.name.Equals("RoadMediumSegment")) {
									Tex += Gd;
									if(File.Exists(Path.Combine(cTexPathRM, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRM, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPathRM, Tex + APR)))
											segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRM, Tex + APR)));
										}
									} else if(segment.m_mesh.name.Equals("RoadMediumSegmentBusSide")) {
									Tex += B1;
									if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "RoadMedium_D_BusSide.dds")))
										segmentMaterialTexture_name = Path.Combine(ModLoader.currentTexturesPath_default, "RoadMedium_D_BusSide.dds");
									else if(File.Exists(Path.Combine(cTexPathRM, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRM, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPathRM, Tex + APR)))
											segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRM, Tex + APR)));
										}
									} else if(segment.m_mesh.name.Equals("RoadMediumSegmentBusBoth")) {
									Tex += B2;
									if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "RoadMedium_D_BusBoth.dds")))
										segmentMaterialTexture_name = Path.Combine(ModLoader.currentTexturesPath_default, "RoadMedium_D_BusBoth.dds");
									else if(File.Exists(Path.Combine(cTexPathRM, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRM, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPathRM, Tex + APR)))
											segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRM, Tex + APR)));
										}
									} else if(!(segment.m_mesh.name.Equals("RoadMediumSegment"))) {
									if((netInfo.name.Equals("Medium Road Elevated")) || (netInfo.name.ToLower().Contains("bridge"))) {
										Tex += El;
										if(File.Exists(Path.Combine(cTexPathRM, Tex + Main))) {
											segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRM, Tex + Main)));
											if(File.Exists(Path.Combine(cTexPathRM, Tex + APR)))
												segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRM, Tex + APR)));
											if(File.Exists(Path.Combine(cTexPathRM, Tex + LOD)))
												segment.m_lodMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRM, Tex + LOD)));
											}
										}
									if(netInfo.name.Equals("Medium Road Slope")) {
										Tex += Sl;
										if(File.Exists(Path.Combine(cTexPathRM, Tex + Main))) {
											segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRM, Tex + Main)));
											if(File.Exists(Path.Combine(cTexPathRM, Tex + APR)))
												segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRM, Tex + APR)));
											}
										}
									}
								}
							if((netInfo.name.Contains("Medium Road")) && (netInfo.name.Contains("Monorail"))) {//mesh
								Tex = RM;
								if(segment.m_mesh.name.Equals("RoadMediumSegment")) {
									Tex += Gd;
									if(File.Exists(Path.Combine(cTexPathRM, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRM, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPathRM, Tex + APR)))
											segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRM, Tex + APR)));
										}
									}
								else if(segment.m_mesh.name.Equals("RoadMediumSegmentBusSide")) {
									Tex += B1;
									if(File.Exists(Path.Combine(cTexPathRM, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRM, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPathRM, Tex + APR)))
											segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRM, Tex + APR)));
										}
									}
								else if(segment.m_mesh.name.Equals("RoadMediumSegmentBusBoth")) {
									Tex += B2;
									if(File.Exists(Path.Combine(cTexPathRM, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRM, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPathRM, Tex + APR)))
											segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRM, Tex + APR)));
										}
									}
								else if(segment.m_mesh.name.Equals("RoadMediumElevatedSegment")) {
									Tex += El;
									if(File.Exists(Path.Combine(cTexPathRM, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRM, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPathRM, Tex + APR)))
											segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRM, Tex + APR)));
										if(File.Exists(Path.Combine(cTexPathRM, Tex + LOD)))
											segment.m_lodMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRM, Tex + LOD)));
										}
									}
								}
							if((netInfo.name.Equals("Medium Road Decoration Grass")) || (netInfo.name.Equals("Medium Road Decoration Trees"))) {//mesh
								Tex = RM + Dc;
								if(segment.m_mesh.name.Equals("RoadMediumSegment")) {
									if(File.Exists(Path.Combine(cTexPathRM, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRM, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPathRM, Tex + APR)))
											segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRM, Tex + APR)));
										}
									} else if(segment.m_mesh.name.Equals("RoadMediumSegmentBusSide")) {
									Tex += B1;
									if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "RoadMediumDeco_d_BusSide.dds")))
										segmentMaterialTexture_name = Path.Combine(ModLoader.currentTexturesPath_default, "RoadMediumDeco_d_BusSide.dds");
									else if(File.Exists(Path.Combine(cTexPathRM, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRM, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPathRM, Tex + APR)))
											segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRM, Tex + APR)));
										}
									} else if(segment.m_mesh.name.Equals("RoadMediumSegmentBusBoth")) {
									Tex += B2;
									if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "RoadMediumDeco_d_BusBoth.dds")))
										segmentMaterialTexture_name = Path.Combine(ModLoader.currentTexturesPath_default, "RoadMediumDeco_d_BusBoth.dds");
									else if(File.Exists(Path.Combine(cTexPathRM, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRM, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPathRM, Tex + APR)))
											segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRM, Tex + APR)));
										}
									}
								}
							if(netInfo.name.Equals("Medium Road Bicycle")) {//mesh
								Tex = RM + Bk;
								if(segment.m_mesh.name.Equals("RoadMediumSegment")) {
									Tex += Gd;
									if(File.Exists(Path.Combine(cTexPathRM, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRM, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPathRM, Tex + APR)))
											segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRM, Tex + APR)));
										}
									} else if(segment.m_mesh.name.Equals("RoadMediumSegmentBusSide")) {
									Tex += B1;
									if(File.Exists(Path.Combine(cTexPathRM, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRM, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPathRM, Tex + APR)))
											segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRM, Tex + APR)));
										}
									} else if(segment.m_mesh.name.Equals("RoadMediumSegmentBusBoth")) {
									Tex += B2;
									if(File.Exists(Path.Combine(cTexPathRM, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRM, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPathRM, Tex + APR)))
											segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRM, Tex + APR)));
										}
									}
								}
							if((netInfo.name.Equals("Medium Road Elevated Bike")) ||
								(netInfo.name.Equals("Medium Road Bridge Bike"))) {
								Tex = RM + Bk + El;
								if(File.Exists(Path.Combine(cTexPathRM, Tex + Main))) {
									segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRM, Tex + Main)));
									if(File.Exists(Path.Combine(cTexPathRM, Tex + APR)))
										segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRM, Tex + APR)));
									}
								}
							//Avenue Large
							if((netInfo.name.Contains("Avenue Large")) && (!netInfo.name.Contains("Buslanes"))) {
							Tex = RM + Av;
								if(segment.m_segmentMesh.name.Equals("AvenueLargeWithGrassSegment")) {
									Tex += Gd;
									//Tex = "RoadMediumAveGnd";
									if(File.Exists(Path.Combine(cTexPathRM, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRM, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPathRM, Tex + APR)))
											segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRM, Tex + APR)));
										}
									}
								else if(segment.m_segmentMesh.name.Equals("AvenueLargeWithGrassStopSingle")) {
										Tex += B1;
										if(File.Exists(Path.Combine(cTexPathRM, Tex + Main))) {
											segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRM, Tex + Main)));
											if(File.Exists(Path.Combine(cTexPathRM, Tex + APR)))
												segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRM, Tex + APR)));
										}
									}
								else if(segment.m_segmentMesh.name.Equals("AvenueLargeWithGrassStopBoth")) {
									Tex += B1;
									if(File.Exists(Path.Combine(cTexPathRM, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRM, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPathRM, Tex + APR)))
											segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRM, Tex + APR)));
										}
									}
								if((segment.m_segmentMesh.name.Equals("AvenueLargeWithGrassBridgeSuspension")) || (segment.m_segmentMesh.name.Equals("AvenueLargeWithGrassElevatedSegment"))) {
									Tex += El;
									if(File.Exists(Path.Combine(cTexPathRM, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRM, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPathRM, Tex + APR)))
											segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRM, Tex + APR)));
										}
									else{
									Tex = RM + Av + Gd;
										if(File.Exists(Path.Combine(cTexPathRM, Tex + Main))) {
											segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRM, Tex + Main)));
											if(File.Exists(Path.Combine(cTexPathRM, Tex + APR)))
												segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRM, Tex + APR)));
											}
										}
									}
								if(segment.m_segmentMesh.name.Equals("AvenueLargeWithGrassTunnelSlope")) {
									Tex += Sl;
									if(File.Exists(Path.Combine(cTexPathRM, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRM, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPathRM, Tex + APR)))
											segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRM, Tex + APR)));
										}
									}
								}
							if((netInfo.name.Contains("Avenue Large")) && (netInfo.name.Contains("Buslanes"))) {
								Tex = RM + Av + Bs;
								if(segment.m_segmentMesh.name.Equals("AvenueLargeWithBuslanesGrassSegment")) {
									Tex += Gd;
									if(File.Exists(Path.Combine(cTexPathRM, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRM, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPathRM, Tex + APR)))
											segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRM, Tex + APR)));
										}
									}
								else if(segment.m_segmentMesh.name.Equals("AvenueLargeWithBuslanesGrassStopSingle")) {
									Tex += B1;
									if(File.Exists(Path.Combine(cTexPathRM, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRM, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPathRM, Tex + APR)))
											segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRM, Tex + APR)));
										}
									}
								else if(segment.m_segmentMesh.name.Equals("AvenueLargeWithBuslanesGrassStopDouble")) {
									Tex += B1;
									if(File.Exists(Path.Combine(cTexPathRM, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRM, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPathRM, Tex + APR)))
											segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRM, Tex + APR)));
										}
									}
								if(segment.m_segmentMesh.name.Equals("AvenueLargeWithBuslanesGrassElevatedSegment")) {
									Tex += El;
									if(File.Exists(Path.Combine(cTexPathRM, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRM, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPathRM, Tex + APR)))
											segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRM, Tex + APR)));
										}
									else {
										Tex = RM + Av + Bs + Gd;
										if(File.Exists(Path.Combine(cTexPathRM, Tex + Main))) {
											segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRM, Tex + Main)));
											if(File.Exists(Path.Combine(cTexPathRM, Tex + APR)))
												segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRM, Tex + APR)));
											}
										}
									}
								if(segment.m_segmentMesh.name.Equals("AvenueLargeWithBuslanesGrassTunnelSlope")) {
									Tex += Sl;
									if(File.Exists(Path.Combine(cTexPathRM, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRM, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPathRM, Tex + APR)))
											segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRM, Tex + APR)));
										}
									}
								}
							#endregion
							//RoadLarge
							#region Large Roads
							if(netInfo.name.Contains("Large Road")) {//mesh
								Tex = RL;
								if(segment.m_mesh.name.Equals("RoadLargeSegment")) {
									Tex += Gd;
									if(File.Exists(Path.Combine(cTexPathRL, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRL, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPathRL, Tex + APR)))
											segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRL, Tex + APR)));
										}
									} else if(segment.m_mesh.name.Equals("LargeRoadSegmentBusSide")) {
									Tex += B1;
									if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "RoadLargeSegment_d_BusSide.dds")))
										segmentMaterialTexture_name = Path.Combine(ModLoader.currentTexturesPath_default, "RoadLargeSegment_d_BusSide.dds");
									else if(File.Exists(Path.Combine(cTexPathRL, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRL, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPathRL, Tex + APR)))
											segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRL, Tex + APR)));
										}
									} else if(segment.m_mesh.name.Equals("LargeRoadSegmentBusBoth")) {
									Tex += B2;
									if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "RoadLargeSegment_d_BusBoth.dds")))
										segmentMaterialTexture_name = Path.Combine(ModLoader.currentTexturesPath_default, "RoadLargeSegment_d_BusBoth.dds");
									else if(File.Exists(Path.Combine(cTexPathRL, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRL, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPathRL, Tex + APR)))
											segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRL, Tex + APR)));
										}
									} else if(!(segment.m_mesh.name.Equals("RoadLargeSegment"))) {
									if((netInfo.name.Equals("Large Road Elevated")) || (netInfo.name.ToLower().Contains("bridge"))) {
										Tex += El;
										if(File.Exists(Path.Combine(cTexPathRL, Tex + Main))) {
											segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRL, Tex + Main)));
											if(File.Exists(Path.Combine(cTexPathRL, Tex + APR)))
												segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRL, Tex + APR)));
											}
										}
									if(netInfo.name.Equals("Large Road Slope")) {
										Tex += Sl;
										if(File.Exists(Path.Combine(cTexPathRL, Tex + Main))) {
											segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRL, Tex + Main)));
											if(File.Exists(Path.Combine(cTexPathRL, Tex + APR)))
												segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRL, Tex + APR)));
											}
										}
									}
								}
							if((netInfo.name.Equals("Large Road Decoration Grass")) || (netInfo.name.Equals("Large Road Decoration Trees"))) {//mesh
								Tex = RL + Dc;
								if(segment.m_mesh.name.Equals("LargeRoadSegment2")) {
									if(File.Exists(Path.Combine(cTexPathRL, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRL, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPathRL, Tex + APR)))
											segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRL, Tex + APR)));
										if(File.Exists(Path.Combine(cTexPathRL, Tex + LOD)))
											segment.m_lodMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRL, Tex + LOD)));
										}
									} else if(segment.m_mesh.name.Equals("LargeRoadSegment2BusSide")) {
									Tex += B1;
									if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "RoadLargeOnewaySegment_d_BusSide.dds")))
										segmentMaterialTexture_name = Path.Combine(ModLoader.currentTexturesPath_default, "RoadLargeOnewaySegment_d_BusSide.dds");
									else if(File.Exists(Path.Combine(cTexPathRL, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRL, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPathRL, Tex + APR)))
											segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRL, Tex + APR)));
										if(File.Exists(Path.Combine(cTexPathRL, Tex + LOD)))
											segment.m_lodMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRL, Tex + LOD)));
										}
									} else if(segment.m_mesh.name.Equals("LargeRoadSegment2BusBoth")) {
									Tex += B2;
									if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "RoadLargeSegmentDecoBusBoth_d.dds")))
										segmentMaterialTexture_name = Path.Combine(ModLoader.currentTexturesPath_default, "RoadLargeSegmentDecoBusBoth_d.dds");
									if(File.Exists(Path.Combine(cTexPathRL, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRL, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPathRL, Tex + APR)))
											segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRL, Tex + APR)));
										if(File.Exists(Path.Combine(cTexPathRL, Tex + LOD)))
											segment.m_lodMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRL, Tex + LOD)));
										}
									}
								}
							if(netInfo.name.Equals("Large Road Bicycle")) {//mesh
								Tex = RL + Bk;
								if(segment.m_mesh.name.Equals("RoadLargeSegment")) {
									Tex += Gd;
									if(File.Exists(Path.Combine(cTexPathRL, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRL, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPathRL, Tex + APR)))
											segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRL, Tex + APR)));
										if(File.Exists(Path.Combine(cTexPathRL, Tex + LOD)))
											segment.m_lodMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRL, Tex + LOD)));
										}
									} else if(segment.m_mesh.name.Equals("LargeRoadSegmentBusSide")) {
									Tex += B1;
									if(File.Exists(Path.Combine(cTexPathRL, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRL, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPathRL, Tex + APR)))
											segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRL, Tex + APR)));
										if(File.Exists(Path.Combine(cTexPathRL, Tex + LOD)))
											segment.m_lodMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRL, Tex + LOD)));
										}
									} else if(segment.m_mesh.name.Equals("LargeRoadSegmentBusBoth")) {
									Tex += B2;
									if(File.Exists(Path.Combine(cTexPathRL, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRL, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPathRL, Tex + APR)))
											segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRL, Tex + APR)));
										if(File.Exists(Path.Combine(cTexPathRL, Tex + LOD)))
											segment.m_lodMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRL, Tex + LOD)));
										}
									}
								}
							if((netInfo.name.Equals("Large Road Elevated Bike")) ||
								(netInfo.name.Equals("Large Road Bridge Bike"))) {
								Tex = RL + Bk + El;
								if(File.Exists(Path.Combine(cTexPathRL, Tex + Main))) {
									segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRL, Tex + Main)));
									if(File.Exists(Path.Combine(cTexPathRL, Tex + APR)))
										segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRL, Tex + APR)));
									if(File.Exists(Path.Combine(cTexPathRL, Tex + LOD)))
										segment.m_lodMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRL, Tex + LOD)));
									}
								}
							if(netInfo.name.Contains("Large Oneway")) {//mesh
								Tex = RL + Ow;
								if(segment.m_mesh.name.Equals("RoadLargeSegment")) {
									Tex += Gd;
									if(File.Exists(Path.Combine(cTexPathRL, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRL, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPathRL, Tex + APR)))
											segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRL, Tex + APR)));
										if(File.Exists(Path.Combine(cTexPathRL, Tex + LOD)))
											segment.m_lodMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRL, Tex + LOD)));
										}
									} else if(segment.m_mesh.name.Equals("LargeRoadSegmentBusSide")) {
									Tex += B1;
									if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "RoadLargeOnewaySegment_d_BusSide.dds")))
										segmentMaterialTexture_name = Path.Combine(ModLoader.currentTexturesPath_default, "RoadLargeOnewaySegment_d_BusSide.dds");
									else if(File.Exists(Path.Combine(cTexPathRL, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRL, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPathRL, Tex + APR)))
											segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRL, Tex + APR)));
										if(File.Exists(Path.Combine(cTexPathRL, Tex + LOD)))
											segment.m_lodMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRL, Tex + LOD)));
										}
									} else if(segment.m_mesh.name.Equals("LargeRoadSegmentBusBoth")) {
									Tex += B2;
									if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "RoadLargeOnewaySegment_d_BusBoth.dds")))
										segmentMaterialTexture_name = Path.Combine(ModLoader.currentTexturesPath_default, "RoadLargeOnewaySegment_d_BusBoth.dds");
									else if(File.Exists(Path.Combine(cTexPathRL, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRL, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPathRL, Tex + APR)))
											segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRL, Tex + APR)));
										if(File.Exists(Path.Combine(cTexPathRL, Tex + LOD)))
											segment.m_lodMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRL, Tex + LOD)));
										}
									} else if(!(segment.m_mesh.name.Equals("RoadLargeSegment"))) {
									if((netInfo.name.Equals("Large Oneway Elevated")) || (netInfo.name.ToLower().Contains("bridge"))) {
										Tex += El;
										if(File.Exists(Path.Combine(cTexPathRL, Tex + Main))) {
											segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRL, Tex + Main)));
											if(File.Exists(Path.Combine(cTexPathRL, Tex + APR)))
												segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRL, Tex + APR)));
											if(File.Exists(Path.Combine(cTexPathRL, Tex + LOD)))
												segment.m_lodMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRL, Tex + LOD)));
											}
										}
									}
								}
							if(netInfo.name.Equals("Large Oneway Road Slope")) {
								Tex = RL + Ow + Sl;
								if(File.Exists(Path.Combine(cTexPathRL, Tex + Main))) {
									segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRL, Tex + Main)));
									if(File.Exists(Path.Combine(cTexPathRL, Tex + APR)))
										segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRL, Tex + APR)));
									}
								}
							if((netInfo.name.Equals("Large Oneway Decoration Grass")) || (netInfo.name.Equals("Large Oneway Decoration Trees"))) {
								Tex = RL + Ow + Dc;
								if(segment.m_mesh.name.Equals("LargeRoadSegment2")) {
									if(File.Exists(Path.Combine(cTexPathRL, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRL, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPathRL, Tex + APR)))
											segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRL, Tex + APR)));
										}
									} else if(segment.m_mesh.name.Equals("LargeRoadSegment2BusSide")) {
									Tex += B1;
									if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "RoadLargeSegmentDecoBusSide_d.dds")))
										segmentMaterialTexture_name = Path.Combine(ModLoader.currentTexturesPath_default, "RoadLargeSegmentDecoBusSide_d.dds");
									if(File.Exists(Path.Combine(cTexPathRL, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRL, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPathRL, Tex + APR)))
											segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRL, Tex + APR)));
										}
									} else if(segment.m_mesh.name.Equals("LargeRoadSegment2BusBoth")) {
									Tex += B2;
									if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "RoadLargeSegmentDecoBusBoth_d.dds")))
										segmentMaterialTexture_name = Path.Combine(ModLoader.currentTexturesPath_default, "RoadLargeSegmentDecoBusBoth_d.dds");
									else if(File.Exists(Path.Combine(cTexPathRL, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRL, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPathRL, Tex + APR)))
											segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRL, Tex + APR)));
										}
									}
								}
							#endregion
								//Highway
							#region Highway
							else if(netInfo.name.Contains("Highway")) {//&& !netInfo.name.Contains("Asym")) {
							if(!netInfo.name.Contains("Asym")&&!netInfo.name.Contains("Highway2L2W")) {
								if((netInfo.name.Equals("HighwayRamp")) || (netInfo.name.Equals("HighwayRampElevated"))) {//mesh
									Tex = HW + "Ramp";
									if(File.Exists(Path.Combine(cTexPathHW, Tex + Main)))
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + Main)));
										{
										if(File.Exists(Path.Combine(cTexPathHW, Tex + APR)))
											segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + APR)));
										}
									}
								else if(netInfo.name.Equals("HighwayRamp Slope")) {//mesh
									Tex = HW + "Ramp" + Sl;
									if(File.Exists(Path.Combine(cTexPathHW, Tex + Main)))
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + Main)));
										{
										if(File.Exists(Path.Combine(cTexPathHW, Tex + APR)))
											segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + APR)));
										}
									}
								else if(netInfo.name.Equals("Highway Barrier")) {
									Tex = HW + "3L";
										{
										if(File.Exists(Path.Combine(cTexPathHW, Tex + Main)))
											segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPathHW, Tex + APR)))
											segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + APR)));
										}
									}
								else if((
										  netInfo.name.Equals("Highway Elevated")) || (
										  segment.m_mesh.name.Equals("HighwayBridgeSegment")) || (
										  segment.m_mesh.name.Equals("HighwayBaseSegment")) || (
										  segment.m_mesh.name.Equals("HighwayBarrierSegment"))) {
									Tex = HW + "3L";
									if(File.Exists(Path.Combine(cTexPathHW, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + Main)));
										//APR
										if(File.Exists(Path.Combine(cTexPathHW, Tex + APR)))
											segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + APR)));
										}
									}
								else if((segment.m_mesh.name.Equals("highway-tunnel-segment")) || (segment.m_mesh.name.Equals("highway-tunnel-slope"))) {
									Tex = HW + "3L" + Sl;
									if(File.Exists(Path.Combine(cTexPathHW, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + Main)));
										//APR
										if(File.Exists(Path.Combine(cTexPathHW, Tex + APR)))
											segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + APR)));
										}
									}

								//2L2W
								else if((netInfo.name.Equals("Two Lane Highway Twoway")) || (netInfo.name.Equals("Two Lane Highway Twoway Barrier"))) {
									Tex = HW + "2L2W";
									if(File.Exists(Path.Combine(cTexPathHW, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + Main)));
										//APR
										if(File.Exists(Path.Combine(cTexPathHW, Tex + APR)))
											segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + APR)));
										}
									}
								else if((netInfo.name.Equals("Two Lane Highway Twoway Elevated")) || (netInfo.name.Equals("Two Lane Highway Twoway Bridge"))) {
									Tex = HW + "2L2W";
									if(File.Exists(Path.Combine(cTexPathHW, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + Main)));
										//APR
										if(File.Exists(Path.Combine(cTexPathHW, Tex + APR)))
											segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + APR)));
										}
									}
								else if((netInfo.name.Equals("Two Lane Highway Twoway Slope")) || (netInfo.name.Equals("Two Lane Highway Twoway Tunnel"))) {
									Tex = HW + "2L2W" + Sl;
									if(File.Exists(Path.Combine(cTexPathHW, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + Main)));
										//APR
										if(File.Exists(Path.Combine(cTexPathHW, Tex + APR)))
											segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + APR)));
										}
									}
								//2L
								else if((netInfo.name.Equals("Two Lane Highway")) || (netInfo.name.Equals("Two Lane Highway Barrier"))) {
									Tex = HW + "2LB";
									if(File.Exists(Path.Combine(cTexPathHW, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + Main)));
										//APR
										if(File.Exists(Path.Combine(cTexPathHW, Tex + APR)))
											segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + APR)));
										}

									}
								else if((netInfo.name.Equals("Two Lane Highway Elevated")) || (netInfo.name.Equals("Two Lane Highway Bridge"))) {
									Tex = HW + "2LB";
									if(File.Exists(Path.Combine(cTexPathHW, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + Main)));
										//APR
										if(File.Exists(Path.Combine(cTexPathHW, Tex + APR)))
											segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + APR)));
										}
									}

								else if((netInfo.name.Equals("Two Lane Highway Slope")) || (netInfo.name.Equals("Two Lane Highway Tunnel"))) {
									Tex = HW + "2LB" + Sl;
									if(File.Exists(Path.Combine(cTexPathHW, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + Main)));
										//APR
										if(File.Exists(Path.Combine(cTexPathHW, Tex + APR)))
											segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + APR)));
										}
									}
								//4L
								else if((netInfo.name.Equals("Four Lane Highway")) || (netInfo.name.Equals("Four Lane Highway Barrier"))) {
									Tex = HW + "4LB";
									if(File.Exists(Path.Combine(cTexPathHW, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + Main)));
										//APR
										if(File.Exists(Path.Combine(cTexPathHW, Tex + APR)))
											segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + APR)));
										}

									}
								else if((netInfo.name.Equals("Four Lane Highway Elevated")) || (netInfo.name.Equals("Four Lane Highway Bridge"))) {
									Tex = HW + "4LB";
									if(File.Exists(Path.Combine(cTexPathHW, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + Main)));
										//APR
										if(File.Exists(Path.Combine(cTexPathHW, Tex + APR)))
											segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + APR)));
										}

									}
								else if((netInfo.name.Equals("Four Lane Highway Slope")) || (netInfo.name.Equals("Four Lane Highway Tunnel"))) {
									Tex = HW + "4LB" + Sl;
									if(File.Exists(Path.Combine(cTexPathHW, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + Main)));
										//APR
										if(File.Exists(Path.Combine(cTexPathHW, Tex + APR)))
											segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + APR)));
										}
									}

							}else{
								if(netInfo.name.Contains("L1R2")) {
									Tex = HW + "3L2W";
									if(segment.m_mesh.name.Contains("Slope")) {
									Tex += Sl;
										if(File.Exists(Path.Combine(cTexPathHW, Tex + Main))) {
											segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + Main)));
											//APR
											if(File.Exists(Path.Combine(cTexPathHW, Tex + APR)))
												segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + APR)));
											}
										}
									if(segment.m_mesh.name.Equals("Tunnel")) {
										Tex += Tl;
										if(File.Exists(Path.Combine(cTexPathHW, Tex + Main))) {
											segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + Main)));
											//APR
											if(File.Exists(Path.Combine(cTexPathHW, Tex + APR)))
												segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + APR)));
											}
										}
									if(segment.m_mesh.name.Equals("HighwayBridgeSegment")) {
										Tex += El;
										if(File.Exists(Path.Combine(cTexPathHW, Tex + Main))) {
											segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + Main)));
											//APR
											if(File.Exists(Path.Combine(cTexPathHW, Tex + APR)))
												segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + APR)));
											}
										}
									else if(segment.m_mesh.name.Equals("HighwayBaseSegment")){
										Tex += Gd;
										if(File.Exists(Path.Combine(cTexPathHW, Tex + Main))) {
											segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + Main)));
											//APR
											if(File.Exists(Path.Combine(cTexPathHW, Tex + APR)))
												segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + APR)));
											}
										}
									}
								if(netInfo.name.Contains("Highway2L2W")) {
									Tex = HW + "4L2W";
									if(segment.m_mesh.name.Equals("Ground")) {
										Tex += Gd;
										if(File.Exists(Path.Combine(cTexPathHW, Tex + Main))) {
											segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + Main)));
											//APR
											if(File.Exists(Path.Combine(cTexPathHW, Tex + APR)))
												segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + APR)));
											}
										}
									if(segment.m_mesh.name.Equals("HighwayBridgeSegment")) {
										Tex += El;
										if(File.Exists(Path.Combine(cTexPathHW, Tex + Main))) {
											segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + Main)));
											//APR
											if(File.Exists(Path.Combine(cTexPathHW, Tex + APR)))
												segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + APR)));
											}
										}
									if(segment.m_mesh.name.Equals("highway-tunnel-slope")) {
										Tex += Sl;
										if(File.Exists(Path.Combine(cTexPathHW, Tex + Main))) {
											segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + Main)));
											//APR
											if(File.Exists(Path.Combine(cTexPathHW, Tex + APR)))
												segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + APR)));
											}
										}
									if(segment.m_mesh.name.Equals("Tunnel")) {
										Tex += Tl;
										if(File.Exists(Path.Combine(cTexPathHW, Tex + Main))) {
											segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + Main)));
											//APR
											if(File.Exists(Path.Combine(cTexPathHW, Tex + APR)))
												segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathHW, Tex + APR)));
											}
										}
									}
								}
							}
							#endregion
							//Tram
							#region Trams
							if(netInfo.name.Equals("Basic Road Tram")) {//mesh
								if(segment.m_mesh.name.Equals("RoadSmallTramStopSingle")) {
									Tex = RS + B2;
									if(File.Exists(Path.Combine(cTexPathRS, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPathRS, Tex + APR)))
											segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + APR)));
										}
									} else if(segment.m_mesh.name.Equals("RoadSmallTramStopDouble")) {
									Tex = RS + B2;
									if(File.Exists(Path.Combine(cTexPathRS, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPathRS, Tex + APR)))
											segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + APR)));
										}
									} else if(segment.m_mesh.name.Equals("RoadSmallTramAndBusStop")) {
									Tex = RS + B2;
									if(File.Exists(Path.Combine(cTexPathRS, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPathRS, Tex + APR)))
											segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + APR)));
										}
									}
								} else if((netInfo.name.Equals("Basic Road Elevated Tram")) || (netInfo.name.Equals("Basic Road Bridge Tram"))) {
								Tex = RS + El;
								if(File.Exists(Path.Combine(cTexPathRS, Tex + Main))) {
									segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + Main)));
									if(File.Exists(Path.Combine(cTexPathRS, Tex + APR)))
										segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + APR)));
									}
								} else if(netInfo.name.Equals("Basic Road Slope Tram")) {
								Tex = RS + Sl;
								if(File.Exists(Path.Combine(cTexPathRS, Tex + Main))) {
									segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + Main)));
									if(File.Exists(Path.Combine(cTexPathRS, Tex + APR)))
										segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + APR)));
									}
								} else if(netInfo.name.Equals("Oneway Road Tram")) {
								Tex = RS + Ow;
								if(segment.m_mesh.name.Equals("RoadSmallTramStopSingle")) {
									Tex = RS + Ow + B2;
									if(File.Exists(Path.Combine(cTexPathRS, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPathRS, Tex + APR)))
											segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + APR)));
										}
									} else if(segment.m_mesh.name.Equals("SmallRoadSegmentBusSide")) {
									Tex = RS + Ow + B2;
									if(File.Exists(Path.Combine(cTexPathRS, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPathRS, Tex + APR)))
											segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + APR)));
										}
									}
								} else if((netInfo.name.Equals("Oneway Road Elevated Tram")) || (netInfo.name.Equals("Oneway Road Bridge Tram"))) {
								Tex = RS + Ow + El;
								if(File.Exists(Path.Combine(cTexPathRS, Tex + Main))) {
									segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + Main)));
									if(File.Exists(Path.Combine(cTexPathRS, Tex + APR)))
										segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + APR)));
									}
								} else if(netInfo.name.Equals("Oneway Road Slope Tram")) {
								Tex = RS + Ow + Sl;
								if(File.Exists(Path.Combine(cTexPathRS, Tex + Main))) {
									segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + Main)));
									if(File.Exists(Path.Combine(cTexPathRS, Tex + APR)))
										segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + APR)));
									}
								} else if(netInfo.name.Equals("Medium Road Tram")) {//mesh
								if(segment.m_mesh.name.Equals("RoadMediumTramSegment")) {
									Tex = RM + Tr + Gd;
									if(File.Exists(Path.Combine(cTexPathRM, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRM, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPathRM, Tex + APR)))
											segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRM, Tex + APR)));
										}
									}
								} else if((netInfo.name.Equals("Medium Road Elevated Tram")) || (netInfo.name.Equals("Medium Road Bridge Tram"))) {
								Tex = RM + Tr + El;
								if(File.Exists(Path.Combine(cTexPathRM, Tex + Main))) {
									segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRM, Tex + Main)));
									if(File.Exists(Path.Combine(cTexPathRM, Tex + APR)))
										segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRM, Tex + APR)));
									}
								} else if(netInfo.name.Equals("Medium Road Slope Tram")) {
								Tex = RM + Tr + Sl;
								if(File.Exists(Path.Combine(cTexPathRM, Tex + Main))) {
									segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRM, Tex + Main)));
									if(File.Exists(Path.Combine(cTexPathRM, Tex + APR)))
										segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRM, Tex + APR)));
									}
								} else if(netInfo.name.Equals("Tram Track")) {//mesh
								Tex = RS + Dc;
								if(File.Exists(Path.Combine(cTexPathRS, Tex + Main))) {
									segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + Main)));
									if(File.Exists(Path.Combine(cTexPathRS, Tex + APR)))
										segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + APR)));
									}
								} else if(netInfo.name.Equals("Tram Track Elevated")) {
								Tex = RS + Ow + El;
								if(File.Exists(Path.Combine(cTexPathRS, Tex + Main))) {
									segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + Main)));
									if(File.Exists(Path.Combine(cTexPathRS, Tex + APR)))
										segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + APR)));
									}
								} else if(netInfo.name.Equals("Tram Track Slope")) {
								Tex = RS + Ow + Sl;
								if(File.Exists(Path.Combine(cTexPathRS, Tex + Main))) {
									segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + Main)));
									if(File.Exists(Path.Combine(cTexPathRS, Tex + APR)))
										segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + APR)));
									}
								} else if(netInfo.name.Equals("Oneway Tram Track")) {//mesh
								Tex = RS + Dc;
								if(File.Exists(Path.Combine(cTexPathRS, Tex + Main))) {
									segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + Main)));
									if(File.Exists(Path.Combine(cTexPathRS, Tex + APR)))
										segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + APR)));
									}
								} else if(netInfo.name.Equals("Oneway Tram Track Elevated")) {
								Tex = RS + Ow + El;
								if(File.Exists(Path.Combine(cTexPathRS, Tex + Main))) {
									segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + Main)));
									if(File.Exists(Path.Combine(cTexPathRS, Tex + APR)))
										segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + APR)));
									}
								} else if(netInfo.name.Equals("Oneway Tram Track Slope")) {
								Tex = RS + Ow + Sl;
								if(File.Exists(Path.Combine(cTexPathRS, Tex + Main))) {
									segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + Main)));
									if(File.Exists(Path.Combine(cTexPathRS, Tex + APR)))
										segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRS, Tex + APR)));
									}
								}
							#endregion
								//Bus
							#region Bus Lanes
								//Medium Road Bus
								  else if(netInfo.name.Equals("Medium Road Bus")) {//mesh
								if(segment.m_mesh.name.Equals("RoadMediumSegment")) {
									Tex = RM + Bl + Gd;
									if(File.Exists(Path.Combine(cTexPathRM, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRM, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPathRM, Tex + APR)))
											segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRM, Tex + APR)));
										}
									} else if(segment.m_mesh.name.Equals("RoadMediumSegmentBusSide")) {
									Tex = RM + Bl + B1;
									if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "RoadMediumBusLane_BusSide.dds")))
										segmentMaterialTexture_name = Path.Combine(ModLoader.currentTexturesPath_default, "RoadMediumBusLane_BusSide.dds");
									else if(File.Exists(Path.Combine(cTexPathRM, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRM, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPathRM, Tex + APR)))
											segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRM, Tex + APR)));
										}
									} else if(segment.m_mesh.name.Equals("RoadMediumSegmentBusBoth")) {
									Tex = RM + Bl + B2;
									if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "RoadMediumBusLane_BusBoth.dds")))
										segmentMaterialTexture_name = Path.Combine(ModLoader.currentTexturesPath_default, "RoadMediumBusLane_BusBoth.dds");
									else if(File.Exists(Path.Combine(cTexPathRM, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRM, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPathRM, Tex + APR)))
											segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRM, Tex + APR)));
										}
									}
								} else if((netInfo.name.Equals("Medium Road Elevated Bus")) || (netInfo.name.Equals("Medium Road Bridge Bus"))) {
								Tex = RM + Bl + El;
								if(File.Exists(Path.Combine(cTexPathRM, Tex + Main))) {
									segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRM, Tex + Main)));
									if(File.Exists(Path.Combine(cTexPathRM, Tex + APR)))
										segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRM, Tex + APR)));
									}
								} else if(netInfo.name.Equals("Medium Road Slope Bus")) {
								Tex = RM + Bl + Sl;
								if(File.Exists(Path.Combine(cTexPathRM, Tex + Main))) {
									segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRM, Tex + Main)));
									if(File.Exists(Path.Combine(cTexPathRM, Tex + APR)))
										segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRM, Tex + APR)));
									}
								} else if(netInfo.name.Equals("Large Road Bus")) {//mesh
								if(segment.m_mesh.name.Equals("RoadLargeSegmentBusLane")) {
									Tex = RL + Bl + Gd;
									if(File.Exists(Path.Combine(cTexPathRL, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRL, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPathRL, Tex + APR)))
											segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRL, Tex + APR)));
										}
									} else if(segment.m_mesh.name.Equals("RoadLargeSegmentBusSideBusLane")) {
									Tex = RL + Bl + B1;
									if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "RoadLargeBuslane_D_BusSide.dds")))
										segmentMaterialTexture_name = Path.Combine(ModLoader.currentTexturesPath_default, "RoadLargeBuslane_D_BusSide.dds");
									else if(File.Exists(Path.Combine(cTexPathRL, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRL, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPathRL, Tex + APR)))
											segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRL, Tex + APR)));
										}
									} else if(segment.m_mesh.name.Equals("LargeRoadSegmentBusBothBusLane")) {
									Tex = RL + Bl + B2;
									if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "RoadLargeBuslane_D_BusBoth.dds")))
										segmentMaterialTexture_name = Path.Combine(ModLoader.currentTexturesPath_default, "RoadLargeBuslane_D_BusBoth.dds");
									else if(File.Exists(Path.Combine(cTexPathRL, Tex + Main))) {
										segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRL, Tex + Main)));
										if(File.Exists(Path.Combine(cTexPathRL, Tex + APR)))
											segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRL, Tex + APR)));
										}
									}
								} else if((netInfo.name.Equals("Large Road Elevated Bus")) || (netInfo.name.Equals("Large Road Bridge Bus"))) {
								Tex = RL + Bl + El;
								if(File.Exists(Path.Combine(cTexPathRL, Tex + Main))) {
									segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRL, Tex + Main)));
									if(File.Exists(Path.Combine(cTexPathRL, Tex + APR)))
										segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRL, Tex + APR)));
									}
								} else if(netInfo.name.Equals("Large Road Slope Bus")) {
								Tex = RL + Bl + Sl;
								if(File.Exists(Path.Combine(cTexPathRL, Tex + Main))) {
									segment.m_segmentMaterial.SetTexture(main, LoadTextureDDS(Path.Combine(cTexPathRL, Tex + Main)));
									if(File.Exists(Path.Combine(cTexPathRL, Tex + APR)))
										segment.m_segmentMaterial.SetTexture(apr, LoadTextureDDS(Path.Combine(cTexPathRL, Tex + APR)));
									}
								}
							#endregion
							Debug.Log(segmentMaterialTexture_name);
							#region configsettings
							if(ModLoader.config.basic_road_parking == 1) {
								Tex = RS + Gd;
								if(segmentMaterialTexture_name.Equals(Path.Combine(cTexDefault, "RoadSmall_D.dds")))
									segmentMaterialTexture_name = Path.Combine(cTexDefault, "RoadSmall_D_parking1.dds");

								if(segmentMaterialTexture_name.Equals(Path.Combine(cTexPathRS, Tex + Main)))
									segmentMaterialTexture_name = Path.Combine(cTexPathRS, Tex + Main);

								if(segmentMaterialTexture_name.Equals(Path.Combine(cTexDefault, ("RoadSmall_D_BusSide.dds"))))
									segmentMaterialTexture_name = Path.Combine(cTexDefault, "RoadSmall_D_BusSide_parking1.dds");
								}
							if((ModLoader.config.medium_road_parking == 1) && (!(netInfo.name.Contains("Grass") || netInfo.name.Contains("Trees")))) {
								if(segment.m_mesh.name.Equals("RoadMediumSegmentBusSide")) {
									if(segmentMaterialTexture_name.Equals(Path.Combine(cTexDefault, ("RoadMediumSegment_d.dds"))) &&
									File.Exists(Path.Combine(cTexDefault, "RoadMedium_D_BusSide_parking1.dds"))) {
										segmentMaterialTexture_name = Path.Combine(cTexDefault, "RoadMedium_D_BusSide_parking1.dds");
										}
									} else if(segment.m_mesh.name.Equals("RoadMediumSegmentBusBoth")) {
									if(segmentMaterialTexture_name.Equals(Path.Combine(cTexDefault, ("RoadMediumSegment_d.dds"))) &&
									File.Exists(Path.Combine(cTexDefault, "RoadMedium_D_BusBoth_parking1.dds"))) {
										segmentMaterialTexture_name = Path.Combine(cTexDefault, "RoadMedium_D_BusBoth_parking1.dds");
										}
									} else if(segmentMaterialTexture_name.Equals(Path.Combine(cTexDefault, "RoadMediumSegment_d.dds")) &&
									  File.Exists(Path.Combine(cTexDefault, "RoadMedium_D_parking1.dds"))) {
									segmentMaterialTexture_name = Path.Combine(cTexDefault, "RoadMedium_D_parking1.dds");
									} else if(segmentMaterialTexture_name.Equals(Path.Combine(cTexDefault, "RoadMedium_D.dds")) &&
									  File.Exists(Path.Combine(cTexDefault, "RoadMedium_D_parking1.dds"))) {
									segmentMaterialTexture_name = Path.Combine(cTexDefault, "RoadMedium_D_parking1.dds");
									}
								}
							if((ModLoader.config.medium_road_grass_parking == 1) && (netInfo.name.Contains("Grass"))) {
								if(segment.m_mesh.name.Equals("RoadMediumSegmentBusSide")) {
									if(segmentMaterialTexture_name.Equals(Path.Combine(cTexDefault, ("RoadMedium_D.dds"))) &&
									File.Exists(Path.Combine(cTexDefault, "RoadMedium_D_BusSide_parking1.dds"))) {
										segmentMaterialTexture_name = Path.Combine(cTexDefault, "RoadMedium_D_BusSide_parking1.dds");
										}
									} else if(segment.m_mesh.name.Equals("RoadMediumSegmentBusBoth")) {
									if(segmentMaterialTexture_name.Equals(Path.Combine(cTexDefault, ("RoadMedium_D.dds"))) &&
									File.Exists(Path.Combine(cTexDefault, "RoadMedium_D_BusBoth_parking1.dds")))
										segmentMaterialTexture_name = Path.Combine(cTexDefault, "RoadMedium_D_BusBoth_parking1.dds");
									} else if(segmentMaterialTexture_name.Equals(Path.Combine(cTexDefault, "RoadMediumSegment_d.dds")) &&
									  File.Exists(Path.Combine(cTexDefault, "RoadMedium_D_parking1.dds"))) {
									segmentMaterialTexture_name = Path.Combine(cTexDefault, "RoadMedium_D_parking1.dds");
									} else if(segmentMaterialTexture_name.Equals(Path.Combine(cTexDefault, "RoadMedium_D.dds")) &&
									  File.Exists(Path.Combine(cTexDefault, "RoadMedium_D_parking1.dds"))) {
									segmentMaterialTexture_name = Path.Combine(cTexDefault, "RoadMedium_D_parking1.dds");
									}
								}
							if((ModLoader.config.medium_road_trees_parking == 1) && (netInfo.name.Contains("Trees"))) {
								if(segment.m_mesh.name.Equals("RoadMediumSegmentBusSide")) {
									if(segmentMaterialTexture_name.Equals(Path.Combine(cTexDefault, ("RoadMediumDeco_d.dds"))) &&
									File.Exists(Path.Combine(cTexDefault, "RoadMediumDeco_d_BusSide_parking1.dds")))
										segmentMaterialTexture_name = Path.Combine(cTexDefault, "RoadMediumDeco_d_BusSide_parking1.dds");
									} else if(segment.m_mesh.name.Equals("RoadMediumSegmentBusBoth") &&
									  File.Exists(Path.Combine(cTexDefault, "RoadMediumDeco_d.dds"))) {
									segmentMaterialTexture_name = Path.Combine(cTexDefault, "RoadMediumDeco_d.dds");
									} else if(segmentMaterialTexture_name.Equals(Path.Combine(cTexDefault, "RoadMediumDeco_d.dds")) &&
									  File.Exists(Path.Combine(cTexDefault, "RoadMediumDeco_d_parking1.dds")))
									segmentMaterialTexture_name = Path.Combine(cTexDefault, "RoadMediumDeco_d_parking1.dds");
								}
							if(ModLoader.config.medium_road_bus_parking == 1) {
								if(segment.m_mesh.name.Equals("RoadMediumSegmentBusSide")) {
									if(segmentMaterialTexture_name.Equals(Path.Combine(cTexDefault, ("RoadMediumBusLane.dds"))) &&
									File.Exists(Path.Combine(cTexDefault, "RoadMediumBusLane_BusSide_parking1.dds")))
										segmentMaterialTexture_name = Path.Combine(cTexDefault, "RoadMediumBusLane_BusSide_parking1.dds");
									} else if(segment.m_mesh.name.Equals("RoadMediumSegmentBusBoth")) {
									if(segmentMaterialTexture_name.Equals(Path.Combine(cTexDefault, ("RoadMediumBusLane.dds"))) &&
									File.Exists(Path.Combine(cTexDefault, "RoadMediumBusLane_BusBoth_parking1.dds")))
										segmentMaterialTexture_name = Path.Combine(cTexDefault, "RoadMediumBusLane_BusBoth_parking1.dds");
									} else if(segmentMaterialTexture_name.Equals(Path.Combine(cTexDefault, "RoadMediumBusLane.dds")) &&
									  File.Exists(Path.Combine(cTexDefault, "RoadMediumBusLane_parking1.dds"))) {
									segmentMaterialTexture_name = Path.Combine(cTexDefault, "RoadMediumBusLane_parking1.dds");
									}
								}
							if(ModLoader.config.large_road_parking == 1) {
								if(segment.m_mesh.name.Equals("LargeRoadSegmentBusSide")) {
									if(segmentMaterialTexture_name.Equals(Path.Combine(cTexDefault, ("RoadLargeSegment_d.dds"))) &&
									File.Exists(Path.Combine(cTexDefault, "RoadLargeSegment_d_BusSide_parking1.dds")))
										segmentMaterialTexture_name = Path.Combine(cTexDefault, "RoadLargeSegment_d_BusSide_parking1.dds");
									} else if(segment.m_mesh.name.Equals("LargeRoadSegmentBusBoth")) {
									if(segmentMaterialTexture_name.Equals(Path.Combine(cTexDefault, ("RoadLargeSegment_d.dds"))) &&
									File.Exists(Path.Combine(cTexDefault, "RoadLargeSegment_d_BusBoth_parking1.dds"))) //might be changed back to default segment
										segmentMaterialTexture_name = Path.Combine(cTexDefault, "RoadLargeSegment_d_BusBoth_parking1.dds");
									} else if(segmentMaterialTexture_name.Equals(Path.Combine(cTexDefault, "RoadLargeSegment_d.dds")) &&
									  File.Exists(Path.Combine(cTexDefault, "RoadLargeSegment_d_parking1.dds"))) {
									segmentMaterialTexture_name = Path.Combine(cTexDefault, "RoadLargeSegment_d_parking1.dds");
									}
								}
							if(ModLoader.config.large_oneway_parking == 1) {
								if(segment.m_mesh.name.Equals("LargeRoadSegmentBusSide")) {
									if(segmentMaterialTexture_name.Equals(Path.Combine(cTexDefault, ("RoadLargeOnewaySegment_d.dds"))) &&
									File.Exists(Path.Combine(cTexDefault, "RoadLargeOnewaySegment_d_BusSide_parking1.dds")))
										segmentMaterialTexture_name = Path.Combine(cTexDefault, "RoadLargeOnewaySegment_d_BusSide_parking1.dds");
									} else if(segmentMaterialTexture_name.Equals(Path.Combine(cTexDefault, "RoadLargeOnewaySegment_d.dds")) &&
									  File.Exists(Path.Combine(cTexDefault, "RoadLargeOnewaySegment_d_parking1.dds"))) {
									segmentMaterialTexture_name = Path.Combine(cTexDefault, "RoadLargeOnewaySegment_d_parking1.dds");
									}
								}
							if(ModLoader.config.large_road_bus_parking == 1) {
								if(segment.m_mesh.name.Equals("LargeRoadSegmentBusSideBusLane")) {
									if(segmentMaterialTexture_name.Equals(Path.Combine(cTexDefault, ("RoadLargeBuslane_D.dds"))) &&
									File.Exists(Path.Combine(cTexDefault, "RoadLargeBuslane_D_BusSide_parking1.dds")))
										segmentMaterialTexture_name = Path.Combine(cTexDefault, "RoadLargeBuslane_D_BusSide_parking1.dds");
									} else if(segment.m_mesh.name.Equals("LargeRoadSegmentBusBothBusLane")) {
									if(segmentMaterialTexture_name.Equals(Path.Combine(cTexDefault, ("RoadLargeBuslane_D.dds"))) &&
									File.Exists(Path.Combine(cTexDefault, "RoadLargeBuslane_D_BusBoth_parking1.dds"))) //might be changed back to default segment
										segmentMaterialTexture_name = Path.Combine(cTexDefault, "RoadLargeBuslane_D_BusBoth_parking1.dds");
									} else if(segmentMaterialTexture_name.Equals(Path.Combine(cTexDefault, "RoadLargeBuslane_D.dds")) &&
									  File.Exists(Path.Combine(cTexDefault, "RoadLargeBuslane_D_parking1.dds"))) {
									segmentMaterialTexture_name = Path.Combine(cTexDefault, "RoadLargeBuslane_D_parking1.dds");
									}
								}
							#endregion
							//Replace the default segment textures
							if(File.Exists(segmentMaterialTexture_name))
								segment.m_segmentMaterial.SetTexture("_MainTex", LoadTextureDDS(segmentMaterialTexture_name));
							}

						if(segment.m_segmentMaterial.GetTexture("_APRMap") != null) {
							string segmentMaterialAPRMap_name = Path.Combine(ModLoader.currentTexturesPath_default, segment.m_segmentMaterial.GetTexture("_APRMap").name + ".dds");
							Debug.Log(segmentMaterialAPRMap_name);
							//APRS!!!!!
								{
								if((
									segment.m_segmentMaterial.GetTexture("_APRMap").name.Equals("LargeRoadSegmentBusSide-BikeLane-apr") ||
									segment.m_segmentMaterial.GetTexture("_APRMap").name.Equals("LargeRoadSegmentBusBoth-BikeLane-apr")
									))
									if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "RoadLargeSegment-BikeLane-apr.dds")))
										segmentMaterialAPRMap_name = Path.Combine(ModLoader.currentTexturesPath_default, "RoadLargeSegment-BikeLane-apr.dds");
									else if(File.Exists(Path.Combine(ModLoader.APRMaps_Path, "RoadLargeSegment-BikeLane-apr.dds")))
										segmentMaterialAPRMap_name = Path.Combine(ModLoader.APRMaps_Path, "RoadLargeSegment-BikeLane-apr.dds");
								if((
									segment.m_segmentMaterial.GetTexture("_APRMap").name.Equals("LargeRoadSegmentBusSide-LargeRoadSegmentBusSide-apr") ||
									segment.m_segmentMaterial.GetTexture("_APRMap").name.Equals("LargeRoadSegmentBusBoth-LargeRoadSegmentBusBoth-apr")
									))
									if(File.Exists(Path.Combine(ModLoader.currentTexturesPath_default, "RoadLargeSegment-default-apr.dds")))
										segmentMaterialAPRMap_name = Path.Combine(ModLoader.currentTexturesPath_default, "RoadLargeSegment-default-apr.dds");
									else if(File.Exists(Path.Combine(ModLoader.APRMaps_Path, "RoadLargeSegment-default-apr.dds")))
										segmentMaterialAPRMap_name = Path.Combine(ModLoader.APRMaps_Path, "RoadLargeSegment-default-apr.dds");

								if(File.Exists(segmentMaterialAPRMap_name))
									segment.m_segmentMaterial.SetTexture("_APRMap", LoadTextureDDS(segmentMaterialAPRMap_name));
								}
							}
						segment.m_lodRenderDistance = 2500;
						}
					}
				}
			}
		}
	}

