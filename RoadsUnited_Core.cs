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

		public static void nodeTexReplace(uint i, int k, string cTexPath, string TexName) {
			string cTex = cTexPath;
			string cTexName = TexName;
			var netInfo = PrefabCollection<NetInfo>.GetLoaded(i);
			NetInfo.Node[] nodes = netInfo.m_nodes;
			NetInfo.Node node = nodes[k];

			if(File.Exists(Path.Combine(cTex, cTexName + "_MainTex.dds"))) {
				node.m_nodeMaterial.SetTexture("_MainTex", LoadTextureDDS(Path.Combine(cTex, cTexName + "_MainTex.dds")));
				if(File.Exists(Path.Combine(cTex, cTexName + "_APRMap.dds")))
					node.m_nodeMaterial.SetTexture("_APRMap", LoadTextureDDS(Path.Combine(cTex, cTexName + "_APRMap.dds")));
				if(File.Exists(Path.Combine(cTex, cTexName + "_APRMap.dds")))
					node.m_nodeMaterial.SetTexture("_APRMap", LoadTextureDDS(Path.Combine(cTex, cTexName + "_LOD_MainTex.dds")));
				if(File.Exists(Path.Combine(cTex, cTexName + "_APRMap.dds")))
					node.m_nodeMaterial.SetTexture("_APRMap", LoadTextureDDS(Path.Combine(cTex, cTexName + "_LOD_APRMap.dds")));
				if(File.Exists(Path.Combine(cTex, cTexName + "_APRMap.dds")))
					node.m_nodeMaterial.SetTexture("_APRMap", LoadTextureDDS(Path.Combine(cTex, cTexName + "_LOD_XYSMap.dds")));
				}
				node.m_lodRenderDistance = 2500;
			}

		public static void segmentTexReplace(uint i, int l, string cTexPath, string TexName) {
			string cTex = cTexPath;
			string cTexName = TexName;
			var netInfo = PrefabCollection<NetInfo>.GetLoaded(i);
			NetInfo.Segment[] segments = netInfo.m_segments;
			NetInfo.Segment segment = segments[l];

			if(File.Exists(Path.Combine(cTex, cTexName + "_MainTex.dds"))) {
				segment.m_segmentMaterial.SetTexture("_MainTex", LoadTextureDDS(Path.Combine(cTex, cTexName + "_MainTex.dds")));
				if(File.Exists(Path.Combine(cTex, cTexName + "_APRMap.dds")))
					segment.m_segmentMaterial.SetTexture("_APRMap", LoadTextureDDS(Path.Combine(cTex, cTexName + "_APRMap.dds")));
				if(File.Exists(Path.Combine(cTex, cTexName + "_APRMap.dds")))
					segment.m_segmentMaterial.SetTexture("_APRMap", LoadTextureDDS(Path.Combine(cTex, cTexName + "_LOD_MainTex.dds")));
				if(File.Exists(Path.Combine(cTex, cTexName + "_APRMap.dds")))
					segment.m_segmentMaterial.SetTexture("_APRMap", LoadTextureDDS(Path.Combine(cTex, cTexName + "_LOD_APRMap.dds")));
				if(File.Exists(Path.Combine(cTex, cTexName + "_APRMap.dds")))
					segment.m_segmentMaterial.SetTexture("_APRMap", LoadTextureDDS(Path.Combine(cTex, cTexName + "_LOD_XYSMap.dds")));
				}
			segment.m_lodRenderDistance = 2500;
			}

		public static void ReplaceNetTextures() {
			#region Variables
			string TrRailA = "tram-rail-double-wn-No Name";
			string TrRailB = "tram-rail-double-No Name";
			string main = "_MainTex";
			string apr = "_APRMap";
			string xys = "_XYSMap";
			string Main = "_MainTex.dds";
			string APR = "_APRMap.dds";
			string LOD = "_LOD_MainTex.dds";
			string Tex = ModLoader.Tex;
			string TramPath = ModLoader.currentTexturesPath_default + "/PropTextures/";
			string cTexDefault = ModLoader.currentTexturesPath_default;
			string cTexPathRT = cTexDefault + "/RoadTiny/";
			string cTexPathRS = cTexDefault + "/RoadSmall/";
			string cTexPathRM = cTexDefault + "/RoadMedium/";
			string cTexPathRL = cTexDefault + "/RoadLarge/";
			string cTexPathHW = cTexDefault + "/Highway/";
			#endregion

			if(ModLoader.config.texturePackPath != null) {
				ModLoader.currentTexturesPath_default = Path.Combine(ModLoader.config.texturePackPath, "BaseTextures");
				}

			for(uint i = 0;i < PrefabCollection<NetInfo>.LoadedCount();i++) {
				var netInfo = PrefabCollection<NetInfo>.GetLoaded(i);
				if(netInfo == null)
					continue;

				#region NExt
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
								if(File.Exists(Path.Combine(cTexPathRT, "RoadTiny2L_Node" + Main)))
									nodeTexReplace(i, k, cTexPathRT, "RoadTiny2L_Node");
								else {
									if(File.Exists(Path.Combine(cTexPathRT, "RoadTiny1L_Node" + Main)))
										nodeTexReplace(i, k, cTexPathRT, "RoadTiny1L_Node");
									}
								}
							if(netInfo.name.Equals("One-Lane Oneway With Parking")) {
								if(File.Exists(Path.Combine(cTexPathRT, "RoadTiny2L_Node" + Main)))
									nodeTexReplace(i, k, cTexPathRT, "RoadTiny2L_Node");
								else {
									if(File.Exists(Path.Combine(cTexPathRT, "RoadTiny1L_Node" + Main)))
										nodeTexReplace(i, k, cTexPathRT, "RoadTiny1L_Node");
									}
								}
							if(netInfo.name.Equals("One-Lane Oneway"))
								nodeTexReplace(i, k, cTexPathRT, "RoadTiny1L_Node");
							#endregion
							#region Avenues Nodes
							if(netInfo.name.Contains("Eight-Lane Avenue")) {
								if(netInfo.name.Equals("Eight-Lane Avenue Elevated"))
									nodeTexReplace(i, k, cTexPathRM, "RoadLarge8LElevated_Node");
								else if(netInfo.name.Equals("Eight-Lane Avenue Slope"))
									nodeTexReplace(i, k, cTexPathRM, "RoadLarge8LSlope_Node");
								else if(netInfo.name.Equals("Eight-Lane Avenue Tunnel"))
									nodeTexReplace(i, k, cTexPathRM, "RoadLarge8LTunnel_Node");
								else
									nodeTexReplace(i, k, cTexPathRM, "RoadLarge8LGnd_Node");
								}
							#endregion
							#region NExt Highways Nodes Default
							if(netInfo.name.Contains("Small Rural Highway"))
								nodeTexReplace(i, k, cTexPathHW, "Highway1L_Node");
							if(netInfo.name.Equals("Small Rural Highway Elevated"))
								nodeTexReplace(i, k, cTexPathHW, "Highway1L_Node");
							if((netInfo.name.Equals("Small Rural Highway Slope")) && (netInfo.name.Contains("Small")))
								nodeTexReplace(i, k, cTexPathHW, "Highway1LSlope_Node");
							if(netInfo.name.Equals("Small Rural Highway Tunnel"))
								nodeTexReplace(i, k, cTexPathHW, "Highway1LTunnel_Node");
							if(netInfo.name.Equals("Rural Highway"))
								nodeTexReplace(i, k, cTexPathHW, "Highway2L_Node");
							if(netInfo.name.Equals("Rural Highway Elevated"))
								nodeTexReplace(i, k, cTexPathHW, "Highway2L_Node");
							if(netInfo.name.Equals("Rural Highway Slope"))
								nodeTexReplace(i, k, cTexPathHW, "Highway2LSlope_Node");
							if(netInfo.name.Equals("Rural Highway Tunnel"))
								nodeTexReplace(i, k, cTexPathHW, "Highway2LTunnel_Node");
							if(netInfo.name.Equals("Four-Lane Highway"))
								nodeTexReplace(i, k, cTexPathHW, "Highway4LGnd_Node");
							if(netInfo.name.Equals("Four-Lane Highway Elevated"))
								nodeTexReplace(i, k, cTexPathHW, "Highway4LElevated_Node");
							if(netInfo.name.Equals("Four-Lane Highway Slope"))
								nodeTexReplace(i, k, cTexPathHW, "Highway4LSlope_Node");
							if(netInfo.name.Equals("Four-Lane Highway Tunnel"))
								nodeTexReplace(i, k, cTexPathHW, "Highway4LTunnel_Node");
							if(netInfo.name.Equals("Five-Lane Highway")) {
								if(File.Exists(Path.Combine(cTexPathHW, "Highway5L_Node" + Main)))
									nodeTexReplace(i, k, cTexPathHW, "Highway5L_Node");
								else
									nodeTexReplace(i, k, cTexPathHW, "Highway5LGnd_Node");
								}
							if(netInfo.name.Equals("Five-Lane Highway Elevated")) {
								if(File.Exists(Path.Combine(cTexPathHW, "Highway5L_Node" + Main)))
									nodeTexReplace(i, k, cTexPathHW, "Highway5L_Node");
								else
									nodeTexReplace(i, k, cTexPathHW, "Highway5LElevated_Node");
								}
							if(netInfo.name.Equals("Five-Lane Highway Slope"))
								nodeTexReplace(i, k, cTexPathHW, "Highway5LGnd_Node");
							if(netInfo.name.Equals("Five-Lane Highway Tunnel"))
								nodeTexReplace(i, k, cTexPathHW, "Highway5LTunnel_Node");
							if(netInfo.name.Equals("Large Highway"))
								nodeTexReplace(i, k, cTexPathHW, "Highway6L_Node");
							if(netInfo.name.Equals("Large Highway Elevated"))
								nodeTexReplace(i, k, cTexPathHW, "Highway6L_Node");
							if(netInfo.name.Equals("Large Highway Slope"))
								nodeTexReplace(i, k, cTexPathHW, "Highway6LGnd_Node");
							if(netInfo.name.Equals("Large Highway Tunnel"))
								nodeTexReplace(i, k, cTexPathHW, "Highway6LTunnel_Node");
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
								if(File.Exists(Path.Combine(cTexPathRT, "RoadTiny2L" + Main)))
									segmentTexReplace(i, l, cTexPathRT, "RoadTiny2L");
								else
									segmentTexReplace(i, l, cTexPathRT, "RoadTiny1L");
								}
							if(netInfo.name.Equals("One-Lane Oneway With Parking")) {
								if(File.Exists(Path.Combine(cTexPathRT, "RoadTiny2L" + Main)))
									segmentTexReplace(i, l, cTexPathRT, "RoadTiny2L");
								else
									segmentTexReplace(i, l, cTexPathRT, "RoadTiny1L");
								}
							if(netInfo.name.Equals("One-Lane Oneway"))
								segmentTexReplace(i, l, cTexPathRT, "RoadTiny1L");
							#endregion
							#region NExt SmallHeavyRoads Default
							if(netInfo.name.Contains("BasicRoadTL")) {
								if(segment.m_segmentMaterial.GetTexture(apr).name.Contains("Ground"))
									segmentTexReplace(i, l, cTexPathRS, "RoadSmallTLGnd");
								else if(segment.m_segmentMaterial.GetTexture(apr).name.Contains("Elevated"))
									segmentTexReplace(i, l, cTexPathRS, "RoadSmallTLElevated");
								else if(segment.m_segmentMaterial.GetTexture(apr).name.Contains("Tunnel"))
									segmentTexReplace(i, l, cTexPathRS, "RoadSmallTLTunnel");
								}
							if(netInfo.name.Contains("Small Avenue")) {
								if(segment.m_segmentMaterial.GetTexture(apr).name.Contains("Ground"))
									segmentTexReplace(i, l, cTexPathRS, "RoadSmall4LGnd");
								else if(segment.m_segmentMaterial.GetTexture(apr).name.Contains("Elevated"))
									segmentTexReplace(i, l, cTexPathRS, "RoadSmall4LElevated");
								else if(segment.m_segmentMaterial.GetTexture(apr).name.Contains("Tunnel"))
									segmentTexReplace(i, l, cTexPathRS, "RoadSmall4LTunnel");
								}
							if(netInfo.name.Contains("Oneway3L")) {
								if(segment.m_segmentMaterial.GetTexture(apr).name.Contains("Ground"))
									segmentTexReplace(i, l, cTexPathRS, "RoadSmallOneway3LGnd");
								else if(segment.m_segmentMaterial.GetTexture(apr).name.Contains("Elevated"))
									segmentTexReplace(i, l, cTexPathRS, "RoadSmallOneway3LElevated");
								else if(segment.m_segmentMaterial.GetTexture(apr).name.Contains("Tunnel"))
									segmentTexReplace(i, l, cTexPathRS, "RoadSmallOneway3LTunnel");
								}
							if(netInfo.name.Contains("Oneway4L")) {
								if(segment.m_segmentMaterial.GetTexture(apr).name.Contains("Ground"))
									segmentTexReplace(i, l, cTexPathRS, "RoadSmallOneway4LGnd");
								if(segment.m_segmentMaterial.GetTexture(apr).name.Contains("Elevated"))
									segmentTexReplace(i, l, cTexPathRS, "RoadSmallOneway4LElevated");
								if(segment.m_segmentMaterial.GetTexture(apr).name.Contains("Tunnel"))
									segmentTexReplace(i, l, cTexPathRS, "RoadSmallOneway4LTunnel");
								}
							if(netInfo.name.Contains("AsymRoadL1R2")) {
								if((netInfo.name.Equals("AsymRoadL1R2"))) {
									if(segment.m_segmentMaterial.GetTexture(apr).name.Contains("Inverted"))
										segmentTexReplace(i, l, cTexPathRS, "RoadSmallL1R2Gnd_Inv");
									else
										segmentTexReplace(i, l, cTexPathRS, "RoadSmallL1R2Gnd");
									}
								else {
									if((netInfo.name.Equals("AsymRoadL1R2 Elevated")) || (netInfo.name.Equals("AsymRoadL1R2 Bridge"))) {
										if(segment.m_segmentMaterial.GetTexture(apr).name.Contains("Inverted"))
											segmentTexReplace(i, l, cTexPathRS, "RoadSmallL1R2Elevated_Inv");
										else
											segmentTexReplace(i, l, cTexPathRS, "RoadSmallL1R2Elevated");
										}
									else if((netInfo.name.Equals("AsymRoadL1R2 Slope")) && (segment.m_mesh.name == "Slope")) {
										if(segment.m_segmentMaterial.GetTexture(apr).name.Contains("Inverted"))
											segmentTexReplace(i, l, cTexPathRS, "RoadSmallL1R2Gnd_Inv");
										else
											segmentTexReplace(i, l, cTexPathRS, "RoadSmallL1R2Gnd");
										}
									else if(netInfo.name.Equals("AsymRoadL1R2 Tunnel")) {
										if(segment.m_segmentMaterial.GetTexture(apr).name.Contains("Inverted"))
											segmentTexReplace(i, l, cTexPathRS, "RoadSmallL1R2Tunnel_Inv");
										else
											segmentTexReplace(i, l, cTexPathRS, "RoadSmallL1R2Tunnel");
										}
									}
								}
							if(netInfo.name.Contains("AsymRoadL1R3")) {
								if((netInfo.name.Equals("AsymRoadL1R3"))) {
									if(segment.m_segmentMaterial.GetTexture(apr).name.Contains("Inverted"))
										segmentTexReplace(i, l, cTexPathRS, "RoadSmallL1R3Gnd_Inv");
									else
										segmentTexReplace(i, l, cTexPathRS, "RoadSmallL1R3Gnd");
									}
								else {
									if((netInfo.name.Equals("AsymRoadL1R3 Elevated")) || (netInfo.name.Equals("AsymRoadL1R3 Bridge"))) {
										if(segment.m_segmentMaterial.GetTexture(apr).name.Contains("Inverted"))
											segmentTexReplace(i, l, cTexPathRS, "RoadSmallL1R3Elevated_Inv");
										else
											segmentTexReplace(i, l, cTexPathRS, "RoadSmallL1R3Elevated");
										}
									else if((netInfo.name.Equals("AsymRoadL1R3 Slope")) && (segment.m_mesh.name == "Slope")) {
										if(segment.m_segmentMaterial.GetTexture(apr).name.Contains("Inverted"))
											segmentTexReplace(i, l, cTexPathRS, "RoadSmallL1R3Gnd_Inv");
										else
											segmentTexReplace(i, l, cTexPathRS, "RoadSmallL1R3Gnd");
										}
									else if(netInfo.name.Equals("AsymRoadL1R3 Tunnel")) {
										if(segment.m_segmentMaterial.GetTexture(apr).name.Contains("Inverted"))
											segmentTexReplace(i, l, cTexPathRS, "RoadSmallL1R2Tunnel_Inv");
										else
											segmentTexReplace(i, l, cTexPathRS, "RoadSmallL1R2Tunnel");
										}
									}
								}
							#endregion
							#region NExt Avenues Default
							if(netInfo.name.Contains("Medium Avenue") && !netInfo.name.Contains("TL")) {
								if(segment.m_segmentMaterial.GetTexture(main).name.Contains("Ground"))
									segmentTexReplace(i, l, cTexPathRM, "RoadMedium4LGnd");
								else if(segment.m_segmentMaterial.GetTexture(main).name.Contains("Elevated"))
									segmentTexReplace(i, l, cTexPathRM, "RoadMedium4LElevated");
								else if(segment.m_segmentMaterial.GetTexture(main).name.Contains("Slope"))
									segmentTexReplace(i, l, cTexPathRM, "RoadMedium4LSlope");
								else if(segment.m_segmentMaterial.GetTexture(main).name.Contains("Tunnel"))
									segmentTexReplace(i, l, cTexPathRM, "RoadMedium4LTunnel");
								}
							if(netInfo.name.Contains("Medium Avenue") && netInfo.name.Contains("TL")) {
								if(segment.m_segmentMaterial.GetTexture(main).name.Contains("Ground"))
									segmentTexReplace(i, l, cTexPathRM, "RoadMedium4LTLGnd");
								else if(segment.m_segmentMaterial.GetTexture(main).name.Contains("Elevated"))
									segmentTexReplace(i, l, cTexPathRM, "RoadMedium4LTLElevated");
								else if(segment.m_segmentMaterial.GetTexture(main).name.Contains("Slope"))
									segmentTexReplace(i, l, cTexPathRM, "RoadMedium4LTLSlope");
								else if(segment.m_segmentMaterial.GetTexture(main).name.Contains("Tunnel"))
									segmentTexReplace(i, l, cTexPathRM, "RoadMedium4LTLTunnel");
								}
							if(netInfo.name.Contains("Eight-Lane Avenue")) {
								if((netInfo.name.Equals("Eight-Lane Avenue Elevated")) || (netInfo.name.ToLower().Contains("bridge")))
									segmentTexReplace(i, l, cTexPathRM, "RoadLarge8LElevated");
								else if(netInfo.name.Equals("Eight-Lane Avenue Slope"))
									segmentTexReplace(i, l, cTexPathRM, "RoadLarge8LSlope");
								else if(netInfo.name.Equals("Eight-Lane Avenue Tunnel"))
									segmentTexReplace(i, l, cTexPathRM, "RoadLarge8LTunnel");
								else
									segmentTexReplace(i, l, cTexPathRM, "RoadLarge8LGnd");
								}
							if(netInfo.name.Contains("AsymAvenueL2R3")) {
								if((netInfo.name.Equals("AsymAvenueL2R3"))) {
									if(segment.m_segmentMaterial.GetTexture(apr).name.Contains("Inverted"))
										segmentTexReplace(i, l, cTexPathRM, "RoadMediumL2R3Gnd_Inv");
									else
										segmentTexReplace(i, l, cTexPathRM, "RoadMediumL2R3Gnd");
									}
								else {
									if((netInfo.name.Equals("AsymAvenueL2R3 Elevated")) || (netInfo.name.Equals("AsymAvenueL2R3 Bridge"))) {
										if(segment.m_segmentMaterial.GetTexture(apr).name.Contains("Inverted"))
											segmentTexReplace(i, l, cTexPathRM, "RoadMediumL2R3Elevated_Inv");
										else
											segmentTexReplace(i, l, cTexPathRM, "RoadMediumL2R3Elevated");
										}
									else if((netInfo.name.Equals("AsymAvenueL2R3 Slope")) && (segment.m_mesh.name == "Slope")) {
										if(segment.m_segmentMaterial.GetTexture(apr).name.Contains("Inverted"))
											segmentTexReplace(i, l, cTexPathRM, "RoadMediumL2R3Gnd_Inv");
										else
											segmentTexReplace(i, l, cTexPathRM, "RoadMediumL2R3Gnd");
										}
									else if(netInfo.name.Equals("AsymAvenueL2R3 Tunnel")) {
										if(segment.m_segmentMaterial.GetTexture(apr).name.Contains("Inverted"))
											segmentTexReplace(i, l, cTexPathRM, "RoadMediumL2R3Tunnel_Inv");
										else
											segmentTexReplace(i, l, cTexPathRM, "RoadMediumL2R3Tunnel");
										}
									}
								}
							if(netInfo.name.Contains("AsymAvenueL2R4")) {
								if((netInfo.name.Equals("AsymAvenueL2R4"))) {
									if(segment.m_segmentMaterial.GetTexture(apr).name.Contains("Inverted"))
										segmentTexReplace(i, l, cTexPathRM, "RoadMediumL2R4Gnd_Inv");
									else
										segmentTexReplace(i, l, cTexPathRM, "RoadMediumL2R4Gnd");
									}
								else {
									if((netInfo.name.Equals("AsymAvenueL2R4 Elevated")) || (netInfo.name.Equals("AsymAvenueL2R4 Bridge"))) {
										if(segment.m_segmentMaterial.GetTexture(apr).name.Contains("Inverted"))
											segmentTexReplace(i, l, cTexPathRM, "RoadMediumL2R4Elevated_Inv");
										else
											segmentTexReplace(i, l, cTexPathRM, "RoadMediumL2R4Elevated");
										}
									else if((netInfo.name.Equals("AsymAvenueL2R4 Slope")) && (segment.m_mesh.name == "Slope")) {
										if(segment.m_segmentMaterial.GetTexture(apr).name.Contains("Inverted"))
											segmentTexReplace(i, l, cTexPathRM, "RoadMediumL2R4Gnd_Inv");
										else
											segmentTexReplace(i, l, cTexPathRM, "RoadMediumL2R4Gnd");
										}
									else if(netInfo.name.Equals("AsymAvenueL2R3 Tunnel")) {
										if(segment.m_segmentMaterial.GetTexture(apr).name.Contains("Inverted"))
											segmentTexReplace(i, l, cTexPathRM, "RoadMediumL2R4Tunnel_Inv");
										else
											segmentTexReplace(i, l, cTexPathRM, "RoadMediumL2R4Tunnel");
										}
									}
								}
							#endregion
							#region NExt Highways Default
							if((netInfo.name.Contains("Rural Highway")) && netInfo.name.Contains("Small")) {
								if(segment.m_mesh.name.Equals("Elevated"))
									segmentTexReplace(i, l, cTexPathHW, "Highway1L");
								else if(netInfo.name.Contains("Slope"))
									segmentTexReplace(i, l, cTexPathHW, "Highway1LSlope");
								else if(segment.m_mesh.name.Equals("Tunnel"))
									segmentTexReplace(i, l, cTexPathHW, "Highway1LTunnel");
								else if(!(netInfo.name.Equals("Rural Highway Tunnel") || netInfo.name.Equals("Rural Highway Slope")))
									segmentTexReplace(i, l, cTexPathHW, "Highway1L");
								}

							if(netInfo.name.Contains("Rural Highway") && !netInfo.name.Contains("Small")) {
								if(segment.m_mesh.name.Equals("Elevated"))
									segmentTexReplace(i, l, cTexPathHW, "Highway2L");
								else if(netInfo.name.Contains("Slope"))
									segmentTexReplace(i, l, cTexPathHW, "Highway2LSlope");
								else if(segment.m_mesh.name.Equals("Tunnel"))
									segmentTexReplace(i, l, cTexPathHW, "Highway2LTunnel");
								else if(!(netInfo.name.Equals("Small Rural Highway Slope") || netInfo.name.Equals("Small Rural Highway Tunnel")))
									segmentTexReplace(i, l, cTexPathHW, "Highway2L");
								}

							if(netInfo.name.Contains("Four-Lane Highway")) {
								if(netInfo.name.Equals("Four-Lane Highway Elevated"))
									segmentTexReplace(i, l, cTexPathHW, "Highway4LElevated");
								else if(netInfo.name.Equals("Four-Lane Highway Slope"))
									segmentTexReplace(i, l, cTexPathHW, "Highway4LSlope");
								else if(netInfo.name.Equals("Four-Lane Highway Tunnel"))
									segmentTexReplace(i, l, cTexPathHW, "Highway4LTunnel");
								else
									segmentTexReplace(i, l, cTexPathHW, "Highway4LGnd");
								}

							if(netInfo.name.Contains("Five-Lane Highway")) {
								if(netInfo.name.Equals("Five-Lane Highway Elevated"))
									segmentTexReplace(i, l, cTexPathHW, "Highway5L");
								else if(netInfo.name.Equals("Five-Lane Highway Slope"))
									segmentTexReplace(i, l, cTexPathHW, "Highway5LSlope");
								else if(netInfo.name.Equals("Five-Lane Highway Tunnel"))
									segmentTexReplace(i, l, cTexPathHW, "Highway5LTunnel");
								else
									segmentTexReplace(i, l, cTexPathHW, "Highway5L");
								}

							if(netInfo.name.Contains("Large Highway")) {
								if(netInfo.name.Equals("Large Highway Elevated"))
									segmentTexReplace(i, l, cTexPathHW, "Highway6L");
								else if(netInfo.name.Equals("Large Highway Slope"))
									segmentTexReplace(i, l, cTexPathHW, "Highway6LSlope");
								else if(netInfo.name.Equals("Large Highway Tunnel"))
									segmentTexReplace(i, l, cTexPathHW, "Highway6LTunnel");
								else
									segmentTexReplace(i, l, cTexPathHW, "Highway6L");
								}
							#endregion
							#region Small Busways
							if((netInfo.name.Contains("Small Busway")) && (!(netInfo.name.Contains("OneWay")))) {
								if(segment.m_mesh.name.Equals("SmallRoadSegment"))
									segmentTexReplace(i, l, cTexPathRS, "RoadSmallBusLaneGnd");
								else if(segment.m_mesh.name.Equals("SmallRoadSegmentBusSide"))
									segmentTexReplace(i, l, cTexPathRS, "RoadSmallBusLaneBusSide");
								else if(segment.m_mesh.name.Equals("SmallRoadSegmentBusBoth"))
									segmentTexReplace(i, l, cTexPathRS, "RoadSmallBusLaneBusBoth");
								else if(!(segment.m_mesh.name.Equals("SmallRoadSegment"))) {
									if(netInfo.name.Equals("Small Busway Elevated"))
										segmentTexReplace(i, l, cTexPathRS, "RoadSmallBusLaneElevated");
									else if(netInfo.name.Equals("Small Busway Slope"))
										segmentTexReplace(i, l, cTexPathRS, "RoadSmallBusLaneSlope");
									else if(netInfo.name.Equals("Small Busway Tunnel"))
										segmentTexReplace(i, l, cTexPathRS, "RoadSmallBusLaneTunnel");
									}
								}
							if((netInfo.name.Equals("Small Busway Decoration Grass")) || (netInfo.name.Equals("Small Busway Decoration Trees"))) {
								if(segment.m_mesh.name.Equals("SmallRoadSegment2"))
									segmentTexReplace(i, l, cTexPathRS, "RoadSmallBusLaneDeco");
								else if(segment.m_mesh.name.Equals("SmallRoadSegment2BusSide"))
									segmentTexReplace(i, l, cTexPathRS, "RoadSmallBusLaneDecoBusSide");
								else if(segment.m_mesh.name.Equals("SmallRoadSegment2BusBoth"))
									segmentTexReplace(i, l, cTexPathRS, "RoadSmallBusLaneDecoBusBoth");
								}
							if(netInfo.name.Contains("Small Busway OneWay")) {
								if(segment.m_mesh.name.Equals("SmallRoadSegment"))
									segmentTexReplace(i, l, cTexPathRS, "RoadSmallOnewayBusLaneGnd");
								else if(segment.m_mesh.name.Equals("SmallRoadSegmentBusSide"))
									segmentTexReplace(i, l, cTexPathRS, "RoadSmallOnewayBusLaneBusSide");
								else if(segment.m_mesh.name.Equals("SmallRoadSegmentBusBoth"))
									segmentTexReplace(i, l, cTexPathRS, "RoadSmallOnewayBusLaneBusBoth");
								else if(!(segment.m_mesh.name.Equals("SmallRoadSegment"))) {
									if(netInfo.name.Equals("Small Busway OneWay Elevated"))
										segmentTexReplace(i, l, cTexPathRS, "RoadSmallOnewayBusLaneElevated");
									else if(netInfo.name.Equals("Small Busway OneWay Slope"))
										segmentTexReplace(i, l, cTexPathRS, "RoadSmallOnewayBusLaneSlope");
									else if(netInfo.name.Equals("Small Busway OneWay Tunnel"))
										segmentTexReplace(i, l, cTexPathRS, "RoadSmallOnewayBusLaneTunnel");
									}
								}
							if((netInfo.name.Equals("Small Busway OneWay Decoration Grass")) || (netInfo.name.Equals("Small Busway OneWay Decoration Trees"))) {//mesh
								if(segment.m_mesh.name.Equals("SmallRoadSegment2"))
									segmentTexReplace(i, l, cTexPathRS, "RoadSmallOnewayBusLaneDeco");
								else if(segment.m_mesh.name.Equals("SmallRoadSegment2BusSide"))
									segmentTexReplace(i, l, cTexPathRS, "RoadSmallOnewayBusLaneDecoBusSide");
								else if(segment.m_mesh.name.Equals("SmallRoadSegment2BusBoth"))
									segmentTexReplace(i, l, cTexPathRS, "RoadSmallOnewayBusLaneDecoBusBoth");
								}
							#endregion
							#region Large Busways
							if(netInfo.name.Equals("Large Road With Bus Lanes")) {
								if(segment.m_mesh.name.Equals("RoadLageSegment"))
									segmentTexReplace(i, l, cTexPathRL, "RoadLargeBusLaneGnd");
								else if(segment.m_mesh.name.Equals("LargeRoadSegmentBusSide"))
									segmentTexReplace(i, l, cTexPathRL, "RoadLargeBusLaneBusSide");
								else if(segment.m_mesh.name.Equals("LargeRoadSegmentBusBoth"))
									segmentTexReplace(i, l, cTexPathRL, "RoadLargeBusLaneBusBoth");
								}
							if(netInfo.name.Equals("Large Road Elevated With Bus Lanes"))
								segmentTexReplace(i, l, cTexPathRL, "RoadLargeBusLaneElevated");
							if(netInfo.name.Equals("Large Road Slope With Bus Lanes"))
								segmentTexReplace(i, l, cTexPathRL, "RoadLargeBusLaneSlope");
							if(netInfo.name.Equals("Large Road Tunnel With Bus Lanes"))
								segmentTexReplace(i, l, cTexPathRL, "RoadLargeBusLaneTunnel");
							if(netInfo.name.Equals("Large Road Decoration Trees With Bus Lanes") ||
							   netInfo.name.Equals("Large Road Decoration Grass With Bus Lanes")) {
								if(segment.m_mesh.name.Equals("LargeRoadSegment2"))
									segmentTexReplace(i, l, cTexPathRL, "RoadLargeBusLaneDeco");
								else if(segment.m_mesh.name.Equals("LargeRoadSegment2BusSide"))
									segmentTexReplace(i, l, cTexPathRL, "RoadLargeBusLaneDecoBusSide");
								else if(segment.m_mesh.name.Equals("LargeRoadSegment2BusBoth"))
									segmentTexReplace(i, l, cTexPathRL, "RoadLargeBusLaneDecoBusBoth");
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
					(netInfo.name.Contains("Large Road") && netInfo.name.Contains("Bus Lane")))) {
					NetInfo.Node[] nodes = netInfo.m_nodes;
					#region nodes
					for(int k = 0;k < nodes.Length;k++) {
						NetInfo.Node node = nodes[k];
						if((node.m_nodeMaterial.GetTexture(main) != null) &&
							(!(node.m_nodeMaterial.name.Equals(TrRailA))) &&
							(!(node.m_nodeMaterial.name.Equals(TrRailB)))) {
							string nodeMaterialTexture_name = Path.Combine(ModLoader.currentTexturesPath_default, node.m_nodeMaterial.GetTexture("_MainTex").name + ".dds");
							string prefab_road_name = netInfo.name.Replace(" ", "_").ToLowerInvariant().Trim();
							if(File.Exists(prefab_road_name + "_" + nodeMaterialTexture_name))
								node.m_nodeMaterial.SetTexture("_MainTex", LoadTextureDDS(prefab_road_name + "_" + nodeMaterialTexture_name));
							node.m_lodRenderDistance = 2500;
							#region Small Roads
							if(netInfo.name.Contains("Basic Road")) {
								if(netInfo.name.Equals("Basic Road Elevated"))
									nodeTexReplace(i, k, cTexPathRS, "RoadSmallElevated_Node");
								else if((netInfo.name.Equals("Basic Road Decoration Grass")) || (netInfo.name.Equals("Basic Road Decoration Trees")))
									nodeTexReplace(i, k, cTexPathRS, "RoadSmallDeco_Node");
								else
									nodeTexReplace(i, k, cTexPathRS, "RoadSmallGnd_Node");
								}
							else if(netInfo.name.Contains("Oneway Road")) {
								if(netInfo.name.Equals("Oneway Road Elevated"))
									nodeTexReplace(i, k, cTexPathRS, "RoadSmallOnewayElevated_Node");
								else if((netInfo.name.Equals("Oneway Road Decoration Grass")) || (netInfo.name.Equals("Oneway Road Decoration Trees")))
									nodeTexReplace(i, k, cTexPathRS, "RoadSmallOnewayDeco_Node");
								else
									nodeTexReplace(i, k, cTexPathRS, "RoadSmallOnewayGnd_Node");
								}
							else if(netInfo.name.Equals("Basic Road Bicycle"))
								nodeTexReplace(i, k, cTexPathRS, "RoadSmallGnd_Node");
							else if(netInfo.name.Equals("Basic Road Elevated Bike"))
								nodeTexReplace(i, k, cTexPathRS, "RoadSmallBikeElevated_Node");
							else if((netInfo.name.Contains("Small Road Monorail")) && (!node.m_mesh.name.Contains("monorail"))) {
								if(netInfo.name.Equals("Small Road Monorail Elevated"))
									nodeTexReplace(i, k, cTexPathRS, "RoadSmallElevated_Node");
								else
									nodeTexReplace(i, k, cTexPathRS, "RoadSmallGnd_Node");
								}
							else if((netInfo.name.Contains("BasicRoadPntMdn") || netInfo.name.Contains("BasicRoadMdn"))&&!node.m_nodeMaterial.GetTexture(main).name.Contains("Median")) {
								//if(node.m_nodeMesh.name.Equals("Elevated_Node"))
								if(node.m_nodeMaterial.GetTexture(main).name.Contains("Elevated"))
									nodeTexReplace(i, k, cTexPathRS, "RoadSmallOnewayElevated_Node");
								//else if(node.m_nodeMesh.name.Equals("Slope_Node"))
								if(node.m_nodeMaterial.GetTexture(main).name.Contains("Tunnel"))
									nodeTexReplace(i, k, cTexPathRS, "RoadSmallMdnTunnel");
								//else if(node.m_nodeMesh.name.Equals("Tunnel_Node"))
								if(node.m_nodeMaterial.GetTexture(main).name.Contains("Ground"))
									nodeTexReplace(i, k, cTexPathRS, "RoadSmallOnewayGnd_Node");
								//else if(node.m_nodeMesh.name.Equals("SmallRoadSegment"))
									//nodeTexReplace(i, k, cTexPathRS, "RoadSmallMdnGnd_Node");
								}
							else if(netInfo.name.Contains("Oneway with bicycle lanes")){
								if(node.m_nodeMaterial.GetTexture(main).name.Contains("Elevated"))
									nodeTexReplace(i, k, cTexPathRS, "RoadSmallOnewayElevated_Node");
								if(node.m_nodeMaterial.GetTexture(main).name.Contains("Tunnel"))
									nodeTexReplace(i, k, cTexPathRS, "RoadSmallOnwayBikeTunnel");
								if(node.m_nodeMaterial.GetTexture(main).name.Contains("Ground"))
									nodeTexReplace(i, k, cTexPathRS, "RoadSmallOnewayGnd_Node");
								/*if(netInfo.name.Equals("Oneway with bicycle lanes"))
									nodeTexReplace(i, k, cTexPathRS, "RoadSmallOnewayGnd_Node");
								if(netInfo.name.Equals("Oneway with bicycle lanes"))
									if(node.m_nodeMaterial.GetTexture(main).name.Contains("Elevated"))
										nodeTexReplace(i, k, cTexPathRS, "RoadSmallOnewayElevated_Node");
								else if(netInfo.name.Equals("Oneway with bicycle lanes Slope")) {
									if(node.m_nodeMaterial.GetTexture(main).name.Contains("Tunnel"))
										nodeTexReplace(i, k, cTexPathRS, "RoadSmallOnwayBikeTunnel");
									else if(node.m_nodeMaterial.GetTexture(main).name.Contains("Slope"))
										nodeTexReplace(i, k, cTexPathRS, "RoadSmallOnwayBikeSlope");
									}*/
							}
							else if(netInfo.name.Equals("Oneway with bicycle lanes Tunnel"))
								nodeTexReplace(i, k, cTexPathRS, "RoadSmallOnwayBikeTunnel");

							else if(netInfo.name.Contains("Asymmetrical Three Lane Road")) {
								if(netInfo.name.Equals("Asymmetrical Three Lane Road Elevated"))
									nodeTexReplace(i, k, cTexPathRL, "RoadLargeElevated_Node");
								else if(netInfo.name.Equals("Asymmetrical Three Lane Road Slope"))
									nodeTexReplace(i, k, cTexPathRL, "RoadLargeDeco_Node");
								else
									nodeTexReplace(i, k, cTexPathRL, "RoadLargeGnd_Node");
								}
							#endregion
							#region Medium Roads
							if(netInfo.name.Contains("Medium Road") && !netInfo.name.Contains("Tram") && !node.m_mesh.name.Contains("monorail")) {
								if((netInfo.name.Contains("Medium Road Elevated")) && (!(netInfo.name.Contains("Bike"))))
									nodeTexReplace(i, k, cTexPathRM, "RoadMediumElevated_Node");
								else if((netInfo.name.Equals("Medium Road Decoration Grass")) || (netInfo.name.Equals("Medium Road Decoration Trees")))
									nodeTexReplace(i, k, cTexPathRM, "RoadMediumDeco_Node");
								else
									nodeTexReplace(i, k, cTexPathRM, "RoadMediumGnd_Node");
								}
							else if(netInfo.name.Equals("Medium Road Elevated Bike"))
								nodeTexReplace(i, k, cTexPathRM, "RoadMediumBikeElevated_Node");
							else if(netInfo.name.Contains("Avenue Large With Grass") && !netInfo.name.Contains("Buslanes"))
								nodeTexReplace(i, k, cTexPathRM, "RoadMediumAveGnd_Node");
							else if(netInfo.name.Contains("Avenue Large With Buslanes Grass"))
								nodeTexReplace(i, k, cTexPathRM, "RoadMediumAveBusLanesGnd_Node");
							else if(netInfo.name.Contains("Medium Road Monorail")) {
								if(netInfo.name.Contains("Elevated") && node.m_mesh.name.Equals("RoadMediumElevatedNode"))
									nodeTexReplace(i, k, cTexPathRM, "RoadMediumElevated_Node");
								else if(netInfo.name.Equals("Medium Road Monorail") && node.m_mesh.name.Equals("RoadMediumNode"))
									nodeTexReplace(i, k, cTexPathRM, "RoadMediumGnd_Node");
								}
							#endregion
							#region Large Roads
							if(netInfo.name.Contains("Large Road")) {
								if(netInfo.name.Equals("Large Road Elevated"))
									nodeTexReplace(i, k, cTexPathRL, "RoadLargeElevated_Node");
								else if((netInfo.name.Equals("Large Road Decoration Grass")) || (netInfo.name.Equals("Large Road Decoration Trees")))
									nodeTexReplace(i, k, cTexPathRL, "RoadLargeDeco_Node");
								else
									nodeTexReplace(i, k, cTexPathRL, "RoadLargeGnd_Node");
								}
							else if(netInfo.name.Equals("Large Road Elevated Bike"))
								nodeTexReplace(i, k, cTexPathRL, "RoadLargeBikeElevated_Node");
							else if(netInfo.name.Contains("Large Oneway")) {
								if(netInfo.name.Equals("Large Oneway Elevated"))
									nodeTexReplace(i, k, cTexPathRL, "RoadLargeOnewayElevated_Node");
								else if((netInfo.name.Equals("Large Oneway Decoration Grass")) || (netInfo.name.Equals("Large Oneway Decoration Trees")))
									nodeTexReplace(i, k, cTexPathRL, "RoadLargeOnewayDeco_Node");
								else if(netInfo.name.Equals("Large Oneway Road Slope"))
									nodeTexReplace(i, k, cTexPathRL, "RoadLargeOnewaySlope_Node");
								else
									nodeTexReplace(i, k, cTexPathRL, "RoadLargeOnewayGnd_Node");
								}
							#endregion
							#region Highway
							if(netInfo.name.Contains("Highway")) {
								if(!netInfo.name.Contains("Asym") && !netInfo.name.Contains("Highway2L2W")) {
									if(netInfo.name.Equals("HighwayRamp"))
										nodeTexReplace(i, k, cTexPathHW, "HighwayRamp_Node");
									else if(netInfo.name.Equals("HighwayRampElevated"))
										nodeTexReplace(i, k, cTexPathHW, "HighwayRamp_Node");
									else if(netInfo.name.Equals("HighwayRamp Slope"))
										nodeTexReplace(i, k, cTexPathHW, "HighwayRamp_Node");
									else if(netInfo.name.Equals("Highway Elevated"))
										nodeTexReplace(i, k, cTexPathHW, "Highway3L_Node");
									else if(netInfo.name.Equals("Highway Slope"))
										nodeTexReplace(i, k, cTexPathHW, "Highway3L_Node");
									else if(netInfo.name.Equals("Highway Barrier"))
										nodeTexReplace(i, k, cTexPathHW, "Highway3L_Node");
									else if(netInfo.name.Equals("Highway"))
										nodeTexReplace(i, k, cTexPathHW, "Highway3L_Node");
									else if((netInfo.name.Equals("Two Lane Highway Twoway")) ||
											(netInfo.name.Equals("Two Lane Highway Twoway Barrier")) ||
											(netInfo.name.Equals("Two Lane Highway Twoway Elevated")) ||
											(netInfo.name.Equals("Two Lane Highway Twoway Bridge")) ||
											(netInfo.name.Equals("Two Lane Highway Twoway Slope")) ||
											(netInfo.name.Equals("Two Lane Highway Twoway Tunnel")))
										nodeTexReplace(i, k, cTexPathHW, "Highway2L2W_Node");
									else if(netInfo.name.Equals("Two Lane Highway Elevated") ||
											netInfo.name.Equals("Two Lane Highway Bridge") ||
											(netInfo.name.Equals("Two Lane Highway Slope") && node.m_nodeMesh.name.Equals("two_lane_highway_tunnel_slope")) ||
											(netInfo.name.Equals("Two Lane Highway Tunnel") && node.m_nodeMesh.name.Equals("two_lane_highway_tunnel_slope")) ||
											netInfo.name.Equals("Two Lane Highway") ||
											netInfo.name.Equals("Two Lane Highway Barrier"))
										nodeTexReplace(i, k, cTexPathHW, "Highway2L2W_Node");
									else if(netInfo.name.Equals("Four Lane Highway") ||
											netInfo.name.Equals("Four Lane Highway Barrier") ||
											netInfo.name.Equals("Four Lane Highway Elevated") ||
											netInfo.name.Equals("Four Lane Highway Bridge") ||
											netInfo.name.Equals("Four Lane Highway Slope") ||
											netInfo.name.Equals("Four Lane Highway Tunnel"))
										nodeTexReplace(i, k, cTexPathHW, "Highway4LB_Node");
									}
								else {
									if(netInfo.name.Contains("L1R2")) {
										if(netInfo.name.Equals("AsymHighwayL1R2 Slope"))
											nodeTexReplace(i, k, cTexPathHW, "Highway3L2WSlope_Node");
										if(netInfo.name.Equals("AsymHighwayL1R2 Tunnel"))
											nodeTexReplace(i, k, cTexPathHW, "Highway3L2WTunnel_Node");
										if(node.m_nodeMesh.name.Contains("HighwayBridgeNode"))
											nodeTexReplace(i, k, cTexPathHW, "Highway3L2WElevated_Node");
										else if(netInfo.name.Equals("AsymHighwayL1R2"))
											nodeTexReplace(i, k, cTexPathHW, "Highway3L2WGnd_Node");
										}
									if(netInfo.name.Contains("Highway2L2W")) {
										if(netInfo.name.Equals("Highway2L2W"))
											nodeTexReplace(i, k, cTexPathHW, "Highway4L2WGnd");
										if(node.m_nodeMesh.name.Contains("HighwayBridgeNode"))
											nodeTexReplace(i, k, cTexPathHW, "Highway4L2WElevated");
										if(netInfo.name.Equals("Highway2L2W Slope"))
											nodeTexReplace(i, k, cTexPathHW, "Highway4L2WSlope");
										if(netInfo.name.Equals("Highway2L2W Tunnel"))
											nodeTexReplace(i, k, cTexPathHW, "Highway4L2WTunnel");
										}
									}
								}
							#endregion
							#region Trams
							if(netInfo.name.Equals("Basic Road Tram"))
								nodeTexReplace(i, k, cTexPathRS, "RoadSmallGnd_Node");
							else if(netInfo.name.Equals("Basic Road Elevated Tram"))
								nodeTexReplace(i, k, cTexPathRS, "RoadSmallElevated_Node");
							else if(netInfo.name.Equals("Basic Road Slope Tram"))
								nodeTexReplace(i, k, cTexPathRS, "RoadSmallGnd_Node");
							else if(netInfo.name.Equals("Oneway Road Tram"))
								nodeTexReplace(i, k, cTexPathRS, "RoadSmallOnewayGnd_Node");
							else if(netInfo.name.Equals("Oneway Road Elevated Tram"))
								nodeTexReplace(i, k, cTexPathRS, "RoadSmallOnewayElevated_Node");
							else if(netInfo.name.Equals("Oneway Road Slope Tram"))
								nodeTexReplace(i, k, cTexPathRS, "RoadSmallOnewayGnd_Node");
							else if(netInfo.name.Equals("Tram Track"))
								nodeTexReplace(i, k, cTexPathRS, "RoadSmallDeco_Node");
							else if(netInfo.name.Equals("Tram Track Elevated"))
								nodeTexReplace(i, k, cTexPathRS, "RoadSmallElevated_Node");
							else if(netInfo.name.Equals("Tram Track Slope"))
								nodeTexReplace(i, k, cTexPathRS, "RoadSmallDeco_Node");
							else if(netInfo.name.Equals("Oneway Tram Track"))
								nodeTexReplace(i, k, cTexPathRS, "RoadSmallDeco_Node");
							else if(netInfo.name.Equals("Oneway Tram Track Elevated"))
								nodeTexReplace(i, k, cTexPathRS, "RoadSmallElevated_Node");
							else if(netInfo.name.Equals("Oneway Tram Track Slope"))
								nodeTexReplace(i, k, cTexPathRS, "RoadSmallDeco_Node");
							if(netInfo.name.Equals("Medium Road Tram"))
								nodeTexReplace(i, k, cTexPathRM, "RoadMediumGnd_Node");
							else if(netInfo.name.Equals("Medium Road Elevated Tram"))
								nodeTexReplace(i, k, cTexPathRM, "RoadMediumTramElevated_Node");
							else if(netInfo.name.Equals("Medium Road Slope Tram"))
								nodeTexReplace(i, k, cTexPathRM, "RoadMediumTramGnd_Node");
							#endregion

							else {
								if(File.Exists(nodeMaterialTexture_name))
									node.m_nodeMaterial.SetTexture(main, LoadTextureDDS(nodeMaterialTexture_name));
								if((node.m_nodeMaterial.GetTexture(apr) != null) && (!(node.m_nodeMaterial.name.Equals(TrRailA))) && (!(node.m_nodeMaterial.name.Equals(TrRailB)))) {
									string nodeMaterialAPRMap_name = Path.Combine(ModLoader.currentTexturesPath_default, node.m_nodeMaterial.GetTexture("_APRMap").name + ".dds");
									if(File.Exists(nodeMaterialAPRMap_name))
										node.m_nodeMaterial.SetTexture("_APRMap", LoadTextureDDS(nodeMaterialAPRMap_name));
									}
								}
							}
						if((node.m_nodeMaterial.GetTexture(apr) != null) && (!(node.m_nodeMaterial.name.Contains("rail")))) {
							string nodeMaterialAPRMap_name = Path.Combine(cTexDefault, node.m_nodeMaterial.GetTexture(apr).name + ".dds");
							if(File.Exists(nodeMaterialAPRMap_name))
								node.m_nodeMaterial.SetTexture(apr, LoadTextureDDS(nodeMaterialAPRMap_name));
							}
						}
					#endregion
					#region segments
					NetInfo.Segment[] segments = netInfo.m_segments;
					for(int l = 0;l < segments.Length;l++) {
						NetInfo.Segment segment = segments[l];
						if((segment.m_segmentMaterial.GetTexture("_MainTex") != null) && (!(segment.m_segmentMaterial.name.Contains("rail"))) && (!segment.m_material.name.ToLower().Contains("cable"))) {
							string segmentMaterialTexture_name = Path.Combine(ModLoader.currentTexturesPath_default, segment.m_segmentMaterial.GetTexture("_MainTex").name + ".dds");
							#region Small Roads
							if(netInfo.name.Contains("Basic Road")) {
								if(segment.m_segmentMesh.name.Equals("SmallRoadSegment"))
									segmentTexReplace(i, l, cTexPathRS, "RoadSmallGnd");
								else if(segment.m_segmentMesh.name.Equals("SmallRoadSegmentBusSide"))
									segmentTexReplace(i, l, cTexPathRS, "RoadSmallBusSide");
								else if(segment.m_segmentMesh.name.Equals("SmallRoadSegmentBusBoth"))
									segmentTexReplace(i, l, cTexPathRS, "RoadSmallBusBoth");
								else if(!(segment.m_mesh.name.Equals("SmallRoadSegment"))) {
									if((netInfo.name.Equals("Basic Road Elevated")) || (netInfo.name.Equals("Basic Road Bridge")))
										segmentTexReplace(i, l, cTexPathRS, "RoadSmallElevated");
									else if(netInfo.name.Equals("Basic Road Slope"))
										segmentTexReplace(i, l, cTexPathRS, "RoadSmallSlope");
									}
								}
							if(netInfo.name.Contains("Oneway Road")) {
								if(segment.m_mesh.name.Equals("SmallRoadSegment"))
									segmentTexReplace(i, l, cTexPathRS, "RoadSmallOnewayGnd");
								else if(segment.m_mesh.name.Equals("SmallRoadSegmentBusSide"))
									segmentTexReplace(i, l, cTexPathRS, "RoadSmallOnewayBusSide");
								else if(segment.m_mesh.name.Equals("SmallRoadSegmentBusBoth"))
									segmentTexReplace(i, l, cTexPathRS, "RoadSmallOnewayBusBoth");
								else if(!(segment.m_mesh.name.Equals("SmallRoadSegment"))) {
									if((netInfo.name.Equals("Oneway Road Elevated")) || (netInfo.name.Equals("Oneway Road Bridge")))
										segmentTexReplace(i, l, cTexPathRS, "RoadSmallOnewayElevated");
									else if(netInfo.name.Equals("Oneway Road Slope"))
										segmentTexReplace(i, l, cTexPathRS, "RoadSmallOnewaySlope");
									}
								}
							if(netInfo.name.Equals("Basic Road Bicycle")) {
								if(segment.m_mesh.name.Equals("SmallRoadSegment"))
									segmentTexReplace(i, l, cTexPathRS, "RoadSmallBikeGnd");
								else if(segment.m_mesh.name.Equals("SmallRoadSegmentBusSide"))
									segmentTexReplace(i, l, cTexPathRS, "RoadSmallBikeBusSide");
								else if(segment.m_mesh.name.Equals("SmallRoadSegmentBusBoth"))
									segmentTexReplace(i, l, cTexPathRS, "RoadSmallBikeBusBoth");
								}
							if(netInfo.name.Equals("Basic Road Elevated Bike"))
								segmentTexReplace(i, l, cTexPathRS, "RoadSmallBikeElevated");
							if((netInfo.name.Equals("Basic Road Decoration Grass")) || (netInfo.name.Equals("Basic Road Decoration Trees"))) {
								if(segment.m_mesh.name.Equals("SmallRoadSegment2"))
									segmentTexReplace(i, l, cTexPathRS, "RoadSmallDeco");
								else if(segment.m_mesh.name.Equals("SmallRoadSegment2BusSide"))
									segmentTexReplace(i, l, cTexPathRS, "RoadSmallDecoBusSide");
								else if(segment.m_mesh.name.Equals("SmallRoadSegment2BusBoth"))
									segmentTexReplace(i, l, cTexPathRS, "RoadSmallDecoBusBoth");
								}
							if((netInfo.name.Equals("Oneway Road Decoration Grass")) || (netInfo.name.Equals("Oneway Road Decoration Trees"))) {
								if(segment.m_mesh.name.Equals("SmallRoadSegment2"))
									segmentTexReplace(i, l, cTexPathRS, "RoadSmallOnewayDeco");
								else if(segment.m_mesh.name.Equals("SmallRoadSegment2BusSide"))
									segmentTexReplace(i, l, cTexPathRS, "RoadSmallOnewayDecoBusBoth");
								else if(segment.m_mesh.name.Equals("SmallRoadSegment2BusBoth"))
									segmentTexReplace(i, l, cTexPathRS, "RoadSmallOnewayDecoBusBoth");
								}
							if(netInfo.name.Contains("Asymmetrical Three Lane Road")) {
								if(segment.m_segmentMesh.name.Equals("AsymmetricalThreeLaneSegment"))
									segmentTexReplace(i, l, cTexPathRL, "RoadLargeGnd");
								if((segment.m_segmentMesh.name.Equals("AsymmetricalThreeLaneGhostIsland")) || (segment.m_segmentMesh.name.Equals("AsymmetricalThreeLaneGhostIsland2")))
									segmentTexReplace(i, l, cTexPathRL, "AsymmetricalThreeLaneGhostIsland");
								if(segment.m_segmentMesh.name.Equals("AsymmetricalThreeLaneElevatedSegment"))
									segmentTexReplace(i, l, cTexPathRL, "RoadLargeElevated");
								if(segment.m_segmentMesh.name.Equals("AsymmetricalThreeLaneTunnelSlope") || segment.m_segmentMesh.name.Equals("AsymmetricalThreeLaneTunnelSlope2"))
									segmentTexReplace(i, l, cTexPathRL, "RoadLargeSlope");
								if((segment.m_segmentMesh.name.Equals("AsymmetricalThreeLaneElevatedGhostIsland")) || (segment.m_segmentMesh.name.Equals("AsymmetricalThreeLaneElevatedGhostIsland2")))
									segmentTexReplace(i, l, cTexPathRL, "AsymmetricalThreeLaneElevatedGhostIsland");
								}

							if(netInfo.name.Contains("Oneway with bicycle lanes")) {
								if(segment.m_segmentMaterial.GetTexture(apr).name.Contains("Ground"))
									segmentTexReplace(i, l, cTexPathRS, "RoadSmallOnwayBikeGnd");
								else if(segment.m_segmentMaterial.GetTexture(apr).name.Contains("Elevated"))
									segmentTexReplace(i, l, cTexPathRS, "RoadSmallOnwayBikeElevated");
								else if(segment.m_segmentMaterial.GetTexture(apr).name.Contains("Tunnel"))
									segmentTexReplace(i, l, cTexPathRS, "RoadSmallOnwayBikeTunnel");
								}
							if(netInfo.name.Contains("BasicRoadPntMdn") || netInfo.name.Contains("BasicRoadMdn")) {
								if((netInfo.name.Equals("BasicRoadPntMdn") ||
									netInfo.name.Equals("BasicRoadMdn") ||
									netInfo.name.Equals("BasicRoadMdn Decoration Grass") ||
									netInfo.name.Equals("BasicRoadMdn Decoration Trees")) && (
										segment.m_segmentMesh.name.Equals("SmallRoadSegment") ||
										segment.m_segmentMesh.name.Equals("SmallRoadSegmentBusSide") ||
										segment.m_segmentMesh.name.Equals("SmallRoadSegmentBusBoth")))
									segmentTexReplace(i, l, cTexPathRS, "RoadSmallMdnGnd");
								if((netInfo.name.Equals("BasicRoadPntMdn Elevated") || netInfo.name.Equals("BasicRoadMdn Elevated")) && segment.m_segmentMesh.name.Equals("Elevated"))
									segmentTexReplace(i, l, cTexPathRS, "RoadSmallMdnElevated");
								if((netInfo.name.Equals("BasicRoadPntMdn Slope") || netInfo.name.Equals("BasicRoadMdn Slope")) && segment.m_segmentMesh.name.Equals("Slope")) {
									if(File.Exists(Path.Combine(cTexPathRS, Tex + Main)))
										segmentTexReplace(i, l, cTexPathRS, "RoadSmallMdnSlope");
									else
										segmentTexReplace(i, l, cTexPathRS, "RoadSmallMdnGnd");
									}
								if((netInfo.name.Equals("BasicRoadPntMdn Tunnel") || netInfo.name.Equals("BasicRoadMdn Tunnel")) && segment.m_segmentMesh.name.Equals("Tunnel"))
									segmentTexReplace(i, l, cTexPathRS, "RoadSmallMdnTunnel");
								}
							if(netInfo.name.Contains("Small Road Monorail")) {
								if(segment.m_segmentMesh.name.Equals("SmallRoadSegment"))
									segmentTexReplace(i, l, cTexPathRS, "RoadSmallGnd");
								if(segment.m_segmentMesh.name.Equals("SmallRoadElevatedSegment"))
									segmentTexReplace(i, l, cTexPathRS, "RoadSmallElevated");
								}
							#endregion
							#region Medium Roads
							if((netInfo.name.Contains("Medium Road")) && (!netInfo.name.Contains("Monorail"))) {
								if(segment.m_mesh.name.Equals("RoadMediumSegment"))
									segmentTexReplace(i, l, cTexPathRM, "RoadMediumGnd");
								else if(segment.m_mesh.name.Equals("RoadMediumSegmentBusSide"))
									segmentTexReplace(i, l, cTexPathRM, "RoadMediumBusSide");
								else if(segment.m_mesh.name.Equals("RoadMediumSegmentBusBoth"))
									segmentTexReplace(i, l, cTexPathRM, "RoadMediumBusBoth");
								else if(!(segment.m_mesh.name.Equals("RoadMediumSegment"))) {
									if((netInfo.name.Equals("Medium Road Elevated")) || (netInfo.name.ToLower().Contains("bridge")))
										segmentTexReplace(i, l, cTexPathRM, "RoadMediumElevated");
									else if(netInfo.name.Equals("Medium Road Slope"))
										segmentTexReplace(i, l, cTexPathRM, "RoadMediumSlope");
									}
								}
							if((netInfo.name.Contains("Medium Road")) && (netInfo.name.Contains("Monorail"))) {
								if(segment.m_mesh.name.Equals("RoadMediumSegment"))
									segmentTexReplace(i, l, cTexPathRM, "RoadMediumGnd");
								else if(segment.m_mesh.name.Equals("RoadMediumSegmentBusSide"))
									segmentTexReplace(i, l, cTexPathRM, "RoadMediumBusSide");
								else if(segment.m_mesh.name.Equals("RoadMediumSegmentBusBoth"))
									segmentTexReplace(i, l, cTexPathRM, "RoadMediumBusBoth");
								else if(segment.m_mesh.name.Equals("RoadMediumElevatedSegment"))
									segmentTexReplace(i, l, cTexPathRM, "RoadMediumElevated");
								}
							if((netInfo.name.Equals("Medium Road Decoration Grass")) || (netInfo.name.Equals("Medium Road Decoration Trees"))) {
								if(segment.m_mesh.name.Equals("RoadMediumSegment"))
									segmentTexReplace(i, l, cTexPathRM, "RoadMediumDeco");
								else if(segment.m_mesh.name.Equals("RoadMediumSegmentBusSide"))
									segmentTexReplace(i, l, cTexPathRM, "RoadMediumDecoBusSide");
								else if(segment.m_mesh.name.Equals("RoadMediumSegmentBusBoth"))
									segmentTexReplace(i, l, cTexPathRM, "RoadMediumDecoBusBoth");
								}
							if(netInfo.name.Equals("Medium Road Bicycle")) {
								if(segment.m_mesh.name.Equals("RoadMediumSegment"))
									segmentTexReplace(i, l, cTexPathRM, "RoadMediumBikeGnd");
								else if(segment.m_mesh.name.Equals("RoadMediumSegmentBusSide"))
									segmentTexReplace(i, l, cTexPathRM, "RoadMediumBikeBusSide");
								else if(segment.m_mesh.name.Equals("RoadMediumSegmentBusBoth"))
									segmentTexReplace(i, l, cTexPathRM, "RoadMediumBikeBusBoth");
								}
							if((netInfo.name.Equals("Medium Road Elevated Bike")) || (netInfo.name.Equals("Medium Road Bridge Bike")))
								segmentTexReplace(i, l, cTexPathRM, "RoadMediumBikeElevated");
							if((netInfo.name.Contains("Avenue Large")) && (!netInfo.name.Contains("Buslanes"))) {
								if(segment.m_segmentMesh.name.Equals("AvenueLargeWithGrassSegment"))
									segmentTexReplace(i, l, cTexPathRM, "RoadMediumAveGnd");
								if(segment.m_segmentMesh.name.Equals("AvenueLargeWithGrassStopSingle"))
									segmentTexReplace(i, l, cTexPathRM, "RoadMediumAveBusSide");
								if(segment.m_segmentMesh.name.Equals("AvenueLargeWithGrassStopBoth"))
									segmentTexReplace(i, l, cTexPathRM, "RoadMediumAveBusSide");
								if((segment.m_segmentMesh.name.Equals("AvenueLargeWithGrassBridgeSuspension")) || (segment.m_segmentMesh.name.Equals("AvenueLargeWithGrassElevatedSegment")))
									segmentTexReplace(i, l, cTexPathRM, "RoadMediumAveElevated");
								if(segment.m_segmentMesh.name.Equals("AvenueLargeWithGrassTunnelSlope"))
									segmentTexReplace(i, l, cTexPathRM, "RoadMediumAveSlope");
								}
							if((netInfo.name.Contains("Avenue Large")) && (netInfo.name.Contains("Buslanes"))) {
								if(segment.m_segmentMesh.name.Equals("AvenueLargeWithBuslanesGrassSegment"))
									segmentTexReplace(i, l, cTexPathRM, "RoadMediumAveBusLanesGnd");
								else if(segment.m_segmentMesh.name.Equals("AvenueLargeWithBuslanesGrassStopSingle"))
									segmentTexReplace(i, l, cTexPathRM, "RoadMediumAveBusLanesBusSide");
								else if(segment.m_segmentMesh.name.Equals("AvenueLargeWithBuslanesGrassStopDouble"))
									segmentTexReplace(i, l, cTexPathRM, "RoadMediumAveBusLanesBusSide");
								if(segment.m_segmentMesh.name.Equals("AvenueLargeWithBuslanesGrassElevatedSegment"))
									segmentTexReplace(i, l, cTexPathRM, "RoadMediumAveBusLanesElevated");
								if(segment.m_segmentMesh.name.Equals("AvenueLargeWithBuslanesGrassTunnelSlope"))
									segmentTexReplace(i, l, cTexPathRM, "RoadMediumAveBusLanesSlope");
								}
							#endregion
							#region Large Roads
							if(netInfo.name.Contains("Large Road")) {
								Tex = "";
								if(segment.m_mesh.name.Equals("RoadLargeSegment"))
									segmentTexReplace(i, l, cTexPathRL, "RoadLargeGnd");
								else if(segment.m_mesh.name.Equals("LargeRoadSegmentBusSide"))
									segmentTexReplace(i, l, cTexPathRL, "RoadLargeBusSide");
								else if(segment.m_mesh.name.Equals("LargeRoadSegmentBusBoth"))
									segmentTexReplace(i, l, cTexPathRL, "RoadLargeBusBoth");
								else if(!(segment.m_mesh.name.Equals("RoadLargeSegment"))) {
									if((netInfo.name.Equals("Large Road Elevated")) || (netInfo.name.ToLower().Contains("bridge")))
										segmentTexReplace(i, l, cTexPathRL, "RoadLargeElevated");
									if(netInfo.name.Equals("Large Road Slope"))
										segmentTexReplace(i, l, cTexPathRL, "RoadLargeSlope");
									}
								}
							if((netInfo.name.Equals("Large Road Decoration Grass")) || (netInfo.name.Equals("Large Road Decoration Trees"))) {
								if(segment.m_mesh.name.Equals("LargeRoadSegment2"))
									segmentTexReplace(i, l, cTexPathRL, "RoadLargeDeco");
								else if(segment.m_mesh.name.Equals("LargeRoadSegment2BusSide"))
									segmentTexReplace(i, l, cTexPathRL, "RoadLargeDecoBusSide");
								else if(segment.m_mesh.name.Equals("LargeRoadSegment2BusBoth"))
									segmentTexReplace(i, l, cTexPathRL, "RoadLargeDecoBusBoth");
								}
							if(netInfo.name.Equals("Large Road Bicycle")) {
								if(segment.m_mesh.name.Equals("RoadLargeSegment"))
									segmentTexReplace(i, l, cTexPathRL, "RoadLargeBikeGnd");
								else if(segment.m_mesh.name.Equals("LargeRoadSegmentBusSide"))
									segmentTexReplace(i, l, cTexPathRL, "RoadLargeBikeBusSide");
								else if(segment.m_mesh.name.Equals("LargeRoadSegmentBusBoth"))
									segmentTexReplace(i, l, cTexPathRL, "RoadLargeBikeBusBoth");
								}
							if((netInfo.name.Equals("Large Road Elevated Bike")) || (netInfo.name.Equals("Large Road Bridge Bike")))
								segmentTexReplace(i, l, cTexPathRL, "RoadLargeBikeElevated");
							if(netInfo.name.Contains("Large Oneway")) {
								if(segment.m_mesh.name.Equals("RoadLargeSegment"))
									segmentTexReplace(i, l, cTexPathRL, "RoadLargeOnewayGnd");
								else if(segment.m_mesh.name.Equals("LargeRoadSegmentBusSide"))
									segmentTexReplace(i, l, cTexPathRL, "RoadLargeOnewayBusSide");
								else if(segment.m_mesh.name.Equals("LargeRoadSegmentBusBoth"))
									segmentTexReplace(i, l, cTexPathRL, "RoadLargeOnewayBusBoth");
								else if(!(segment.m_mesh.name.Equals("RoadLargeSegment"))) {
									if((netInfo.name.Equals("Large Oneway Elevated")) || (netInfo.name.ToLower().Contains("bridge")))
										segmentTexReplace(i, l, cTexPathRL, "RoadLargeOnewayElevated");
									}
								}
							if(netInfo.name.Equals("Large Oneway Road Slope"))
								segmentTexReplace(i, l, cTexPathRL, "RoadLargeOnewaySlope");
							if((netInfo.name.Equals("Large Oneway Decoration Grass")) || (netInfo.name.Equals("Large Oneway Decoration Trees"))) {
								if(segment.m_mesh.name.Equals("LargeRoadSegment2"))
									segmentTexReplace(i, l, cTexPathRL, "RoadLargeOnewayDeco");
								else if(segment.m_mesh.name.Equals("LargeRoadSegment2BusSide"))
									segmentTexReplace(i, l, cTexPathRL, "RoadLargeOnewayDecoBusSide");
								else if(segment.m_mesh.name.Equals("LargeRoadSegment2BusBoth"))
									segmentTexReplace(i, l, cTexPathRL, "RoadLargeOnewayDecoBusBoth");
								}
							#endregion
							#region Highway
							if(netInfo.name.Contains("Highway")) {
								if(!netInfo.name.Contains("Asym") && !netInfo.name.Contains("Highway2L2W")) {
									if((netInfo.name.Equals("HighwayRamp")) || (netInfo.name.Equals("HighwayRampElevated")))
										segmentTexReplace(i, l, cTexPathHW, "HighwayRamp");
									else if(netInfo.name.Equals("HighwayRamp Slope"))
										segmentTexReplace(i, l, cTexPathHW, "HighwayRampSlope");
									else if(netInfo.name.Equals("Highway Barrier"))
										segmentTexReplace(i, l, cTexPathHW, "Highway3L");
									else if((
											  netInfo.name.Equals("Highway Elevated")) || (
											  segment.m_mesh.name.Equals("HighwayBridgeSegment")) || (
											  segment.m_mesh.name.Equals("HighwayBaseSegment")) || (
											  segment.m_mesh.name.Equals("HighwayBarrierSegment")))
										segmentTexReplace(i, l, cTexPathHW, "Highway3L");
									else if((segment.m_mesh.name.Equals("highway-tunnel-segment")) || (segment.m_mesh.name.Equals("highway-tunnel-slope")))
										segmentTexReplace(i, l, cTexPathHW, "Highway3LSlope");
									else if((netInfo.name.Equals("Two Lane Highway Twoway")) || (netInfo.name.Equals("Two Lane Highway Twoway Barrier")))
										segmentTexReplace(i, l, cTexPathHW, "Highway2L2W");
									else if((netInfo.name.Equals("Two Lane Highway Twoway Elevated")) || (netInfo.name.Equals("Two Lane Highway Twoway Bridge")))
										segmentTexReplace(i, l, cTexPathHW, "Highway2L2W");
									else if((netInfo.name.Equals("Two Lane Highway Twoway Slope")) || (netInfo.name.Equals("Two Lane Highway Twoway Tunnel")))
										segmentTexReplace(i, l, cTexPathHW, "Highway2L2WSlope");
									else if((netInfo.name.Equals("Two Lane Highway")) || (netInfo.name.Equals("Two Lane Highway Barrier")))
										segmentTexReplace(i, l, cTexPathHW, "Highway2LB");
									else if((netInfo.name.Equals("Two Lane Highway Elevated")) || (netInfo.name.Equals("Two Lane Highway Bridge")))
										segmentTexReplace(i, l, cTexPathHW, "Highway2LB");
									else if((netInfo.name.Equals("Two Lane Highway Slope")) || (netInfo.name.Equals("Two Lane Highway Tunnel")))
										segmentTexReplace(i, l, cTexPathHW, "Highway2LBSlope");
									else if((netInfo.name.Equals("Four Lane Highway")) || (netInfo.name.Equals("Four Lane Highway Barrier")))
										segmentTexReplace(i, l, cTexPathHW, "Highway4LB");
									else if((netInfo.name.Equals("Four Lane Highway Elevated")) || (netInfo.name.Equals("Four Lane Highway Bridge")))
										segmentTexReplace(i, l, cTexPathHW, "Highway4LB");
									else if((netInfo.name.Equals("Four Lane Highway Slope")) || (netInfo.name.Equals("Four Lane Highway Tunnel")))
										segmentTexReplace(i, l, cTexPathHW, "Highway4LBSlope");
									}
								else {
									if(netInfo.name.Contains("L1R2")) {
										if(segment.m_mesh.name.Contains("Slope"))
											segmentTexReplace(i, l, cTexPathHW, "Highway3L2WSlope");
										if(segment.m_mesh.name.Equals("Tunnel"))
											segmentTexReplace(i, l, cTexPathHW, "Highway3L2WTunnel");
										if(segment.m_mesh.name.Equals("HighwayBridgeSegment"))
											segmentTexReplace(i, l, cTexPathHW, "Highway3L2WElevated");
										else if(segment.m_mesh.name.Equals("HighwayBaseSegment"))
											segmentTexReplace(i, l, cTexPathHW, "Highway3L2WGnd");
										}
									if(netInfo.name.Contains("Highway2L2W")) {
										if(segment.m_mesh.name.Equals("Ground"))
											segmentTexReplace(i, l, cTexPathHW, "Highway4L2WGnd");
										if(segment.m_mesh.name.Equals("HighwayBridgeSegment"))
											segmentTexReplace(i, l, cTexPathHW, "Highway4L2WElevated");
										if(segment.m_mesh.name.Equals("highway-tunnel-slope"))
											segmentTexReplace(i, l, cTexPathHW, "Highway4L2WSlope");
										if(segment.m_mesh.name.Equals("Tunnel"))
											segmentTexReplace(i, l, cTexPathHW, "Highway4L2WTunnel");
										}
									}
								}
							#endregion
							#region Trams
							if(netInfo.name.Equals("Basic Road Tram")) {
								if(segment.m_mesh.name.Equals("RoadSmallTramStopSingle"))
									segmentTexReplace(i, l, cTexPathRS, "RoadSmallBusBoth");
								else if(segment.m_mesh.name.Equals("RoadSmallTramStopDouble"))
									segmentTexReplace(i, l, cTexPathRS, "RoadSmallBusBoth");
								else if(segment.m_mesh.name.Equals("RoadSmallTramAndBusStop"))
									segmentTexReplace(i, l, cTexPathRS, "RoadSmallBusBoth");
								}
							else if((netInfo.name.Equals("Basic Road Elevated Tram")) || (netInfo.name.Equals("Basic Road Bridge Tram")))
								segmentTexReplace(i, l, cTexPathRS, "RoadSmallElevated");
							else if(netInfo.name.Equals("Basic Road Slope Tram"))
								segmentTexReplace(i, l, cTexPathRS, "RoadSmallSlope");
							else if(netInfo.name.Equals("Oneway Road Tram")) {
								if(segment.m_mesh.name.Equals("RoadSmallTramStopSingle"))
									segmentTexReplace(i, l, cTexPathRS, "RoadSmallOnewayBusBoth");
								else if(segment.m_mesh.name.Equals("SmallRoadSegmentBusSide"))
									segmentTexReplace(i, l, cTexPathRS, "RoadSmallOnewayBusBoth");
								}
							else if((netInfo.name.Equals("Oneway Road Elevated Tram")) || (netInfo.name.Equals("Oneway Road Bridge Tram")))
								segmentTexReplace(i, l, cTexPathRS, "RoadSmallOnewayElevated");
							else if(netInfo.name.Equals("Oneway Road Slope Tram"))
								segmentTexReplace(i, l, cTexPathRS, "RoadSmallOnewaySlope");
							else if(netInfo.name.Equals("Medium Road Tram")) {
								if(segment.m_mesh.name.Equals("RoadMediumTramSegment"))
									segmentTexReplace(i, l, cTexPathRM, "RoadMediumTramGnd");
								}
							else if((netInfo.name.Equals("Medium Road Elevated Tram")) || (netInfo.name.Equals("Medium Road Bridge Tram")))
								segmentTexReplace(i, l, cTexPathRM, "RoadMediumTramElevated");
							else if(netInfo.name.Equals("Medium Road Slope Tram"))
								segmentTexReplace(i, l, cTexPathRM, "RoadMediumTramSlope");
							else if(netInfo.name.Equals("Tram Track"))
								segmentTexReplace(i, l, cTexPathRS, "RoadSmallDeco");
							else if(netInfo.name.Equals("Tram Track Elevated"))
								segmentTexReplace(i, l, cTexPathRS, "RoadSmallOnewayElevated");
							else if(netInfo.name.Equals("Tram Track Slope"))
								segmentTexReplace(i, l, cTexPathRS, "RoadSmallOnewaySlope");
							else if(netInfo.name.Equals("Oneway Tram Track"))
								segmentTexReplace(i, l, cTexPathRS, "RoadSmallDeco");
							else if(netInfo.name.Equals("Oneway Tram Track Elevated"))
								segmentTexReplace(i, l, cTexPathRS, "RoadSmallOnewayElevated");
							else if(netInfo.name.Equals("Oneway Tram Track Slope"))
								segmentTexReplace(i, l, cTexPathRS, "RoadSmallOnewaySlope");
							#endregion
							#region Bus Lanes
							else if(netInfo.name.Equals("Medium Road Bus")) {
								if(segment.m_mesh.name.Equals("RoadMediumSegment"))
									segmentTexReplace(i, l, cTexPathRM, "RoadMediumBusLaneGnd");
								else if(segment.m_mesh.name.Equals("RoadMediumSegmentBusSide"))
									segmentTexReplace(i, l, cTexPathRM, "RoadMediumBusLaneBusSide");
								else if(segment.m_mesh.name.Equals("RoadMediumSegmentBusBoth"))
									segmentTexReplace(i, l, cTexPathRM, "RoadMediumBusLaneBusBoth");
								}
							else if((netInfo.name.Equals("Medium Road Elevated Bus")) || (netInfo.name.Equals("Medium Road Bridge Bus")))
								segmentTexReplace(i, l, cTexPathRM, "RoadMediumBusLaneElevated");
							else if(netInfo.name.Equals("Medium Road Slope Bus"))
								segmentTexReplace(i, l, cTexPathRM, "RoadMediumBusLaneSlope");
							else if(netInfo.name.Equals("Large Road Bus")) {
								if(segment.m_mesh.name.Equals("RoadLargeSegmentBusLane"))
									segmentTexReplace(i, l, cTexPathRL, "RoadLargeBusLaneGnd");
								else if(segment.m_mesh.name.Equals("RoadLargeSegmentBusSideBusLane"))
									segmentTexReplace(i, l, cTexPathRL, "RoadLargeBusLaneBusSide");
								else if(segment.m_mesh.name.Equals("LargeRoadSegmentBusBothBusLane"))
									segmentTexReplace(i, l, cTexPathRL, "RoadLargeBusLaneBusBoth");
								}
							else if((netInfo.name.Equals("Large Road Elevated Bus")) || (netInfo.name.Equals("Large Road Bridge Bus")))
								segmentTexReplace(i, l, cTexPathRL, "RoadLargeBusLaneElevated");
							else if(netInfo.name.Equals("Large Road Slope Bus"))
								segmentTexReplace(i, l, cTexPathRL, "RoadLargeBusLaneSlope");
							#endregion
							Debug.Log(segmentMaterialTexture_name);
							#region configsettings
							if(ModLoader.config.basic_road_parking == 1) {
								Tex = "RoadSmallGnd";
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
									}
								else if(segment.m_mesh.name.Equals("RoadMediumSegmentBusBoth")) {
									if(segmentMaterialTexture_name.Equals(Path.Combine(cTexDefault, ("RoadMediumSegment_d.dds"))) &&
									File.Exists(Path.Combine(cTexDefault, "RoadMedium_D_BusBoth_parking1.dds"))) {
										segmentMaterialTexture_name = Path.Combine(cTexDefault, "RoadMedium_D_BusBoth_parking1.dds");
										}
									}
								else if(segmentMaterialTexture_name.Equals(Path.Combine(cTexDefault, "RoadMediumSegment_d.dds")) &&
								File.Exists(Path.Combine(cTexDefault, "RoadMedium_D_parking1.dds"))) {
									segmentMaterialTexture_name = Path.Combine(cTexDefault, "RoadMedium_D_parking1.dds");
									}
								else if(segmentMaterialTexture_name.Equals(Path.Combine(cTexDefault, "RoadMedium_D.dds")) &&
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
									}
								else if(segment.m_mesh.name.Equals("RoadMediumSegmentBusBoth")) {
									if(segmentMaterialTexture_name.Equals(Path.Combine(cTexDefault, ("RoadMedium_D.dds"))) &&
									File.Exists(Path.Combine(cTexDefault, "RoadMedium_D_BusBoth_parking1.dds")))
										segmentMaterialTexture_name = Path.Combine(cTexDefault, "RoadMedium_D_BusBoth_parking1.dds");
									}
								else if(segmentMaterialTexture_name.Equals(Path.Combine(cTexDefault, "RoadMediumSegment_d.dds")) &&
								File.Exists(Path.Combine(cTexDefault, "RoadMedium_D_parking1.dds"))) {
									segmentMaterialTexture_name = Path.Combine(cTexDefault, "RoadMedium_D_parking1.dds");
									}
								else if(segmentMaterialTexture_name.Equals(Path.Combine(cTexDefault, "RoadMedium_D.dds")) &&
								File.Exists(Path.Combine(cTexDefault, "RoadMedium_D_parking1.dds"))) {
									segmentMaterialTexture_name = Path.Combine(cTexDefault, "RoadMedium_D_parking1.dds");
									}
								}
							if((ModLoader.config.medium_road_trees_parking == 1) && (netInfo.name.Contains("Trees"))) {
								if(segment.m_mesh.name.Equals("RoadMediumSegmentBusSide")) {
									if(segmentMaterialTexture_name.Equals(Path.Combine(cTexDefault, ("RoadMediumDeco_d.dds"))) &&
									File.Exists(Path.Combine(cTexDefault, "RoadMediumDeco_d_BusSide_parking1.dds")))
										segmentMaterialTexture_name = Path.Combine(cTexDefault, "RoadMediumDeco_d_BusSide_parking1.dds");
									}
								else if(segment.m_mesh.name.Equals("RoadMediumSegmentBusBoth") &&
								File.Exists(Path.Combine(cTexDefault, "RoadMediumDeco_d.dds"))) {
									segmentMaterialTexture_name = Path.Combine(cTexDefault, "RoadMediumDeco_d.dds");
									}
								else if(segmentMaterialTexture_name.Equals(Path.Combine(cTexDefault, "RoadMediumDeco_d.dds")) &&
								File.Exists(Path.Combine(cTexDefault, "RoadMediumDeco_d_parking1.dds")))
									segmentMaterialTexture_name = Path.Combine(cTexDefault, "RoadMediumDeco_d_parking1.dds");
								}
							if(ModLoader.config.medium_road_bus_parking == 1) {
								if(segment.m_mesh.name.Equals("RoadMediumSegmentBusSide")) {
									if(segmentMaterialTexture_name.Equals(Path.Combine(cTexDefault, ("RoadMediumBusLane.dds"))) &&
									File.Exists(Path.Combine(cTexDefault, "RoadMediumBusLane_BusSide_parking1.dds")))
										segmentMaterialTexture_name = Path.Combine(cTexDefault, "RoadMediumBusLane_BusSide_parking1.dds");
									}
								else if(segment.m_mesh.name.Equals("RoadMediumSegmentBusBoth")) {
									if(segmentMaterialTexture_name.Equals(Path.Combine(cTexDefault, ("RoadMediumBusLane.dds"))) &&
									File.Exists(Path.Combine(cTexDefault, "RoadMediumBusLane_BusBoth_parking1.dds")))
										segmentMaterialTexture_name = Path.Combine(cTexDefault, "RoadMediumBusLane_BusBoth_parking1.dds");
									}
								else if(segmentMaterialTexture_name.Equals(Path.Combine(cTexDefault, "RoadMediumBusLane.dds")) &&
								File.Exists(Path.Combine(cTexDefault, "RoadMediumBusLane_parking1.dds"))) {
									segmentMaterialTexture_name = Path.Combine(cTexDefault, "RoadMediumBusLane_parking1.dds");
									}
								}
							if(ModLoader.config.large_road_parking == 1) {
								if(segment.m_mesh.name.Equals("LargeRoadSegmentBusSide")) {
									if(segmentMaterialTexture_name.Equals(Path.Combine(cTexDefault, ("RoadLargeSegment_d.dds"))) &&
									File.Exists(Path.Combine(cTexDefault, "RoadLargeSegment_d_BusSide_parking1.dds")))
										segmentMaterialTexture_name = Path.Combine(cTexDefault, "RoadLargeSegment_d_BusSide_parking1.dds");
									}
								else if(segment.m_mesh.name.Equals("LargeRoadSegmentBusBoth")) {
									if(segmentMaterialTexture_name.Equals(Path.Combine(cTexDefault, ("RoadLargeSegment_d.dds"))) &&
									File.Exists(Path.Combine(cTexDefault, "RoadLargeSegment_d_BusBoth_parking1.dds"))) //might be changed back to default segment
										segmentMaterialTexture_name = Path.Combine(cTexDefault, "RoadLargeSegment_d_BusBoth_parking1.dds");
									}
								else if(segmentMaterialTexture_name.Equals(Path.Combine(cTexDefault, "RoadLargeSegment_d.dds")) &&
								File.Exists(Path.Combine(cTexDefault, "RoadLargeSegment_d_parking1.dds"))) {
									segmentMaterialTexture_name = Path.Combine(cTexDefault, "RoadLargeSegment_d_parking1.dds");
									}
								}
							if(ModLoader.config.large_oneway_parking == 1) {
								if(segment.m_mesh.name.Equals("LargeRoadSegmentBusSide")) {
									if(segmentMaterialTexture_name.Equals(Path.Combine(cTexDefault, ("RoadLargeOnewaySegment_d.dds"))) &&
									File.Exists(Path.Combine(cTexDefault, "RoadLargeOnewaySegment_d_BusSide_parking1.dds")))
										segmentMaterialTexture_name = Path.Combine(cTexDefault, "RoadLargeOnewaySegment_d_BusSide_parking1.dds");
									}
								else if(segmentMaterialTexture_name.Equals(Path.Combine(cTexDefault, "RoadLargeOnewaySegment_d.dds")) &&
								File.Exists(Path.Combine(cTexDefault, "RoadLargeOnewaySegment_d_parking1.dds"))) {
									segmentMaterialTexture_name = Path.Combine(cTexDefault, "RoadLargeOnewaySegment_d_parking1.dds");
									}
								}
							if(ModLoader.config.large_road_bus_parking == 1) {
								if(segment.m_mesh.name.Equals("LargeRoadSegmentBusSideBusLane")) {
									if(segmentMaterialTexture_name.Equals(Path.Combine(cTexDefault, ("RoadLargeBuslane_D.dds"))) &&
									File.Exists(Path.Combine(cTexDefault, "RoadLargeBuslane_D_BusSide_parking1.dds")))
										segmentMaterialTexture_name = Path.Combine(cTexDefault, "RoadLargeBuslane_D_BusSide_parking1.dds");
									}
								else if(segment.m_mesh.name.Equals("LargeRoadSegmentBusBothBusLane")) {
									if(segmentMaterialTexture_name.Equals(Path.Combine(cTexDefault, ("RoadLargeBuslane_D.dds"))) &&
									File.Exists(Path.Combine(cTexDefault, "RoadLargeBuslane_D_BusBoth_parking1.dds"))) //might be changed back to default segment
										segmentMaterialTexture_name = Path.Combine(cTexDefault, "RoadLargeBuslane_D_BusBoth_parking1.dds");
									}
								else if(segmentMaterialTexture_name.Equals(Path.Combine(cTexDefault, "RoadLargeBuslane_D.dds")) &&
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
					#endregion
				}
			}
		}
	}
