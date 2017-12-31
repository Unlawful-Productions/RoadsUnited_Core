using ColossalFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace RoadsUnited_Core
{
	public class Hook4: MonoBehaviour
	{
		public bool hookEnabled = false;
		private Dictionary<MethodInfo, RedirectCallsState> redirects = new Dictionary<MethodInfo, RedirectCallsState>();//create new dictionary
		//public static Material invertedBridgeMat;

		public void Update()//enables constant state to refresh hook
		{
			bool flag = !this.hookEnabled;//flag equals opposite of hookEnabled, [true]
			if(flag)//=true
			{
				this.EnableHook();
			}
		}

		public void EnableHook()
		{
			var allFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;
			var method = typeof(NetSegment).GetMethods(allFlags).Single(c => c.Name == "RenderInstance" && c.GetParameters().Length == 3);
			redirects.Add(method, RedirectionHelper.RedirectCalls(method, typeof(Hook4).GetMethod("RenderInstanceSegment", allFlags)));

			method = typeof(NetSegment).GetMethods(allFlags).Single(c => c.Name == "RenderLod");
			redirects.Add(method, RedirectionHelper.RedirectCalls(method, typeof(Hook4).GetMethod("RenderInstanceSegment", allFlags)));

			method = typeof(NetNode).GetMethods(allFlags).Single(c => c.Name == "RenderInstance" && c.GetParameters().Length == 3);
			redirects.Add(method, RedirectionHelper.RedirectCalls(method, typeof(Hook4).GetMethods(allFlags).Single(c => c.Name == "RenderInstanceNode" && c.GetParameters().Length == 3)));

			method = typeof(NetNode).GetMethods(allFlags).Single(c => c.Name == "RenderLod");
			redirects.Add(method, RedirectionHelper.RedirectCalls(method, typeof(Hook4).GetMethods(allFlags).Single(c => c.Name == "RenderInstanceNode" && c.GetParameters().Length == 3)));

			hookEnabled = true;
		}

		public void DisableHook()
		{
			if(!hookEnabled)//in the context of hook being false, that is if hookenabled!=false, continue
			{
				return;
			}
			foreach(var kvp in redirects)
			{
				RedirectionHelper.RevertRedirect(kvp.Key, kvp.Value);//kvp==key value pair
			}
			redirects.Clear();//clear out dictionary
			hookEnabled = false;//hook disabled
		}

		private MethodInfo GetMethod(string name, uint argCount)
		{
			MethodInfo[] methods = typeof(NetNode).GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			MethodInfo[] array = methods;
			MethodInfo result;
			for(int i = 0; i < array.Length; i++)
			{
				MethodInfo methodInfo = array[i];
				bool flag = methodInfo.Name == name && (long)methodInfo.GetParameters().Length == (long)((ulong)argCount);
				if(flag)
				{
					result = methodInfo;
					return result;
				}
			}
			result = null;
			return result;
		}

		private void RefreshJunctionData(NetNode netnode, ushort nodeID, NetInfo info, uint instanceIndex)//RefreshJunctionData_1	/ copy the method from ingame
		{//RefreshJunctionData(nodeID, info, instanceIndex)
			MethodInfo method = this.GetMethod("RefreshJunctionData", 3u);
			object[] parameters = new object[]
            {
                nodeID,
                info,
                instanceIndex
            };
			method.Invoke(netnode, parameters);
		}

		private void RefreshBendData(NetNode netnode, ushort nodeID, NetInfo info, uint instanceIndex, ref RenderManager.Instance data)//copy it
		{//			 RefreshBendData(netnode, nodeID, info, instanceIndex, data)
			MethodInfo method = this.GetMethod("RefreshBendData", 4u);
			object[] array = new object[]
            {
                nodeID,
                info,
                instanceIndex,
                data
            };
			method.Invoke(netnode, array);
			data = (RenderManager.Instance)array[3];
		}

		private void RefreshJunctionData(NetNode netnode, ushort nodeID, int segmentIndex, ushort nodeSegmentID, Vector3 centerPos, ref uint instanceIndex, ref RenderManager.Instance data)
		{
			NetManager instance = Singleton<NetManager>.instance;
			data.m_position = netnode.m_position;
			data.m_rotation = Quaternion.identity;
			data.m_initialized = true;
			float vScale = 0.05f;
			Vector3 startPosition1a = Vector3.zero;
			Vector3 startPosition1b = Vector3.zero;
			Vector3 startDirection1c = Vector3.zero;
			Vector3 startDirection1d = Vector3.zero;
			Vector3 end_CornerPos1a = Vector3.zero;
			Vector3 end_CornerPos1b = Vector3.zero;
			Vector3 end_CornerPos1c = Vector3.zero;
			Vector3 end_CornerPos1d = Vector3.zero;
			Vector3 end_CornerDir1a = Vector3.zero;
			Vector3 end_CornerDir1b = Vector3.zero;
			Vector3 end_CornerDir1c = Vector3.zero;
			Vector3 end_CornerDir1d = Vector3.zero;
			Vector3 middlePosition1a;
			Vector3 middlePosition1b;
			Vector3 middlePosition1c;
			Vector3 middlePosition1d;
			Vector3 middlePosition2a;
			Vector3 middlePosition2b;
			Vector3 middlePosition2c;
			Vector3 middlePosition2d;

			NetSegment netSegment = instance.m_segments.m_buffer[(int)nodeSegmentID];
			NetInfo info = netSegment.Info;
			ItemClass connectionClass = info.GetConnectionClass();
			Vector3 segmentdirection = (nodeID != netSegment.m_startNode) ? netSegment.m_endDirection : netSegment.m_startDirection;
			float num = -4f;
			float num2 = -4f;
			ushort segmentID1a = 0;
			ushort segmentID1b = 0;
			int num8;
			for(int i = 0; i < 8; i = num8)
			{
				ushort segment = netnode.GetSegment(i);
				bool flag = segment != 0 && segment != nodeSegmentID;
				if(flag)
				{
					ItemClass connectionClass2 = instance.m_segments.m_buffer[(int)segment].Info.GetConnectionClass();
					bool roadflag = connectionClass.m_service == connectionClass2.m_service;
					if(roadflag)//=true
					{
						NetSegment netSegment2 = instance.m_segments.m_buffer[(int)segment];
						Vector3 segmentdirection2 = (nodeID != netSegment2.m_startNode) ? netSegment2.m_endDirection : netSegment2.m_startDirection;
						float num5 = (float)((double)segmentdirection.x * (double)segmentdirection2.x + (double)segmentdirection.z * (double)segmentdirection2.z);
						bool flag3 = (double)segmentdirection2.z/*   */* (double)segmentdirection.x - (double)segmentdirection2.x * (double)segmentdirection.z < 0.0;
						if(flag3)//=true
						{
							bool flag4 = (double)num5 > (double)num;
							if(flag4)//=true
							{
								num = num5;
								segmentID1a = segment;
							}
							float num6 = -2f - num5;
							bool flag5 = (double)num6 > (double)num2;
							if(flag5)//=true
							{
								num2 = num6;
								segmentID1b = segment;
							}
						}
						else
						{
							bool flag6 = (double)num5 > (double)num2;
							if(flag6)//=true
							{
								num2 = num5;
								segmentID1b = segment;
							}
							float num7 = -2f - num5;
							bool flag7 = (double)num7 > (double)num;
							if(flag7)//=true
							{
								num = num7;
								segmentID1a = segment;
							}
						}
					}
				}
				num8 = i + 1;
			}
			bool start1a = netSegment.m_startNode == nodeID;
			bool smooth1;

			netSegment.CalculateCorner(nodeSegmentID, true, start1a, false, out startPosition1a, out startDirection1c, out smooth1);
			netSegment.CalculateCorner(nodeSegmentID, true, start1a, true, out startPosition1b, out startDirection1d, out smooth1);
			//CalculateCorner(ushort segmentID,bool heightOffset,bool start,bool leftSide,out Vector3 cornerPos,out Vector3 cornerDirection,out bool smooth);
			bool flag9 = segmentID1a != 0 && segmentID1b > 0;
			if(flag9)
			{
				float num9 = (float)((double)info.m_pavementWidth / (double)info.m_halfWidth * 0.5);
				float y = 1f;
				bool flag10 = segmentID1a > 0;
				if(flag10)
				{
					NetSegment netSegment3 = instance.m_segments.m_buffer[(int)segmentID1a];
					NetInfo info2 = netSegment3.Info;
					bool start1b = netSegment3.m_startNode == nodeID;
					netSegment3.CalculateCorner(segmentID1a, true, start1b, true, out end_CornerPos1a, out end_CornerDir1a, out smooth1);
					netSegment3.CalculateCorner(segmentID1a, true, start1b, false, out end_CornerPos1b, out end_CornerDir1b, out smooth1);
					float num10 = (float)((double)info2.m_pavementWidth / (double)info2.m_halfWidth * 0.5);
					num9 = (float)(((double)num9 + (double)num10) * 0.5);
					y = (float)(2.0 * (double)info.m_halfWidth / ((double)info.m_halfWidth + (double)info2.m_halfWidth));
				}
				float num11 = (float)((double)info.m_pavementWidth / (double)info.m_halfWidth * 0.5);
				float w = 1f;
				bool flag11 = segmentID1b > 0;
				if(flag11)
				{
					NetSegment netSegment4 = instance.m_segments.m_buffer[(int)segmentID1b];
					NetInfo info3 = netSegment4.Info;
					bool start1c = netSegment4.m_startNode == nodeID;
					netSegment4.CalculateCorner(segmentID1b, true, start1c, true, out end_CornerPos1c, out end_CornerDir1c, out smooth1);
					netSegment4.CalculateCorner(segmentID1b, true, start1c, false, out end_CornerPos1d, out end_CornerDir1d, out smooth1);
					float num12 = (float)((double)info3.m_pavementWidth / (double)info3.m_halfWidth * 0.5);
					num11 = (float)(((double)num11 + (double)num12) * 0.5);
					w = (float)(2.0 * (double)info.m_halfWidth / ((double)info.m_halfWidth + (double)info3.m_halfWidth));
				}
				NetSegment.CalculateMiddlePoints(startPosition1a, -startDirection1c, end_CornerPos1a, -end_CornerDir1a, true, true, out middlePosition1a, out middlePosition2a);
				NetSegment.CalculateMiddlePoints(startPosition1b, -startDirection1d, end_CornerPos1b, -end_CornerDir1b, true, true, out middlePosition1b, out middlePosition2b);
				NetSegment.CalculateMiddlePoints(startPosition1a, -startDirection1c, end_CornerPos1c, -end_CornerDir1c, true, true, out middlePosition1c, out middlePosition2c);
				NetSegment.CalculateMiddlePoints(startPosition1b, -startDirection1d, end_CornerPos1d, -end_CornerDir1d, true, true, out middlePosition1d, out middlePosition2d);

				data.m_dataMatrix0 = NetSegment.CalculateControlMatrix(/*--------*/startPosition1a, middlePosition1a, middlePosition2a, end_CornerPos1a, startPosition1a, middlePosition1a, middlePosition2a, end_CornerPos1a, netnode.m_position, vScale);
				data.m_extraData.m_dataMatrix2 = NetSegment.CalculateControlMatrix(startPosition1b, middlePosition1b, middlePosition2b, end_CornerPos1b, startPosition1b, middlePosition1b, middlePosition2b, end_CornerPos1b, netnode.m_position, vScale);
				data.m_extraData.m_dataMatrix3 = NetSegment.CalculateControlMatrix(startPosition1a, middlePosition1c, middlePosition2c, end_CornerPos1c, startPosition1a, middlePosition1c, middlePosition2c, end_CornerPos1c, netnode.m_position, vScale);
				data.m_dataMatrix1 = NetSegment.CalculateControlMatrix(/*--------*/startPosition1b, middlePosition1d, middlePosition2d, end_CornerPos1d, startPosition1b, middlePosition1d, middlePosition2d, end_CornerPos1d, netnode.m_position, vScale);

				data.m_dataVector0 = new Vector4(0.5f / info.m_halfWidth,
											   1f / info.m_segmentLength,
											   (float)(0.5 - (double)info.m_pavementWidth / (double)info.m_halfWidth * 0.5),
											   (float)((double)info.m_pavementWidth / (double)info.m_halfWidth * 0.5));

				data.m_dataVector1 = centerPos - data.m_position;
				data.m_dataVector1.w = ((float)(
											 ((double)data.m_dataMatrix0.m33
											   + (double)data.m_extraData.m_dataMatrix2.m33
											   + (double)data.m_extraData.m_dataMatrix3.m33
											   + (double)data.m_dataMatrix1.m33)
										   * 0.25));

				data.m_dataVector2 = new Vector4(num9,
											   y,
											   num11,
											   w);

				data.m_extraData.m_dataVector4 = RenderManager.GetColorLocation(86016u + (uint)nodeID);
			}
			else
			{
				centerPos.x = (float)(((double)startPosition1a.x + (double)startPosition1b.x) * 0.5);
				centerPos.z = (float)(((double)startPosition1a.z + (double)startPosition1b.z) * 0.5);

				Vector3 endPos1a = startPosition1b;
				Vector3 endPos1b = startPosition1a;

				Vector3 a = startDirection1d;
				Vector3 a2 = startDirection1c;
				float d = Mathf.Min(info.m_halfWidth * 1.333333f, 16f);

				Vector3 middlePosition1e = startPosition1a - startDirection1c * d;
				Vector3 middlePosition1f = startPosition1b + startDirection1d * d;
				Vector3 middlePosition1g = startPosition1a + startDirection1c * d;
				Vector3 middlePosition1h = startPosition1b - startDirection1d * d;

				Vector3 middlePosition2e = endPos1a/*---*/- a/*-----*/* d;
				Vector3 middlePosition2f = endPos1b/*---*/+ a2/*----*/* d;
				Vector3 middlePosition2g = endPos1a/*---*/+ a/*-----*/* d;
				Vector3 middlePosition2h = endPos1b/*---*/- a2/*----*/* d;
				data.m_dataMatrix0 = NetSegment.CalculateControlMatrix(/*--------*/startPosition1a, middlePosition1e, middlePosition2e, endPos1a, startPosition1a, middlePosition1e, middlePosition2e, endPos1a, netnode.m_position, vScale);
				data.m_extraData.m_dataMatrix2 = NetSegment.CalculateControlMatrix(startPosition1b, middlePosition1f, middlePosition2f, endPos1b, startPosition1b, middlePosition1f, middlePosition2f, endPos1b, netnode.m_position, vScale);
				data.m_extraData.m_dataMatrix3 = NetSegment.CalculateControlMatrix(startPosition1a, middlePosition1g, middlePosition2g, endPos1a, startPosition1a, middlePosition1g, middlePosition2g, endPos1a, netnode.m_position, vScale);
				data.m_dataMatrix1 = NetSegment.CalculateControlMatrix(/*--------*/startPosition1b, middlePosition1h, middlePosition2h, endPos1b, startPosition1b, middlePosition1h, middlePosition2h, endPos1b, netnode.m_position, vScale);

				data.m_dataMatrix0.SetRow(3, data.m_dataMatrix0.GetRow(3) + new Vector4(/*filler--------------*/0.2f, 0.2f, 0.2f, 0.2f));
				data.m_extraData.m_dataMatrix2.SetRow(3, data.m_extraData.m_dataMatrix2.GetRow(3) + new Vector4(0.2f, 0.2f, 0.2f, 0.2f));
				data.m_extraData.m_dataMatrix3.SetRow(3, data.m_extraData.m_dataMatrix3.GetRow(3) + new Vector4(0.2f, 0.2f, 0.2f, 0.2f));
				data.m_dataMatrix1.SetRow(3, data.m_dataMatrix1.GetRow(3) + new Vector4(/*filler--------------*/0.2f, 0.2f, 0.2f, 0.2f));

				data.m_dataVector0 = new Vector4(0.5f / info.m_halfWidth,
											   1f / info.m_segmentLength,
											   (float)(0.5 - (double)info.m_pavementWidth / (double)info.m_halfWidth * 0.5),
											   (float)((double)info.m_pavementWidth / (double)info.m_halfWidth * 0.5));
				data.m_dataVector1 = centerPos - data.m_position;
				data.m_dataVector1.w = (float)(((double)data.m_dataMatrix0.m33
											 + (double)data.m_extraData.m_dataMatrix2.m33
											 + (double)data.m_extraData.m_dataMatrix3.m33
											 + (double)data.m_dataMatrix1.m33) * 0.25);
				data.m_dataVector2 = new Vector4(
												(float)
													((double)info.m_pavementWidth
													/ (double)info.m_halfWidth
													* 0.5),
												1f,
												(float)
													((double)info.m_pavementWidth
													/ (double)info.m_halfWidth
													* 0.5),
												1f);
				data.m_extraData.m_dataVector4 = RenderManager.GetColorLocation(86016u + (uint)nodeID);
			}
			data.m_dataInt0 = segmentIndex;
			data.m_dataColor0 = info.m_color;
			data.m_dataColor0.a = 0f;
			bool requireSurfaceMaps = info.m_requireSurfaceMaps;
			if(requireSurfaceMaps)
			{
				Singleton<TerrainManager>.instance.GetSurfaceMapping(data.m_position, out data.m_dataTexture0, out data.m_dataTexture1, out data.m_dataVector3);
			}
			instanceIndex = (uint)data.m_nextInstance;
		}

		private void RefreshJunctionData(NetNode netnode, ushort nodeID, int segmentIndex, NetInfo info, ushort nodeSegmentID, ushort nodeSegment2, ref uint instanceIndex, ref RenderManager.Instance data)
		{
			data.m_position = netnode.m_position;// all copied from internal methods
			data.m_rotation = Quaternion.identity;//this sets Quaternion parameters to read only
			data.m_initialized = true;

			float vScale = 0.05f;
			Vector3 StartPosition1a = Vector3.zero;
			Vector3 StartPosition1b = Vector3.zero;
			Vector3 end_cornerPosition1c = Vector3.zero;
			Vector3 end_cornerPosition1d = Vector3.zero;
			Vector3 end_cornerDirection1a = Vector3.zero;
			Vector3 end_cornerDirection1b = Vector3.zero;
			Vector3 end_cornerDirection1c = Vector3.zero;
			Vector3 end_cornerDirection1d = Vector3.zero;
			Vector3 middlePosition1a;
			Vector3 middlePosition2a;
			Vector3 middlePosition1b;
			Vector3 middlePosition2b;
			bool smoothStart_On = true;
			bool smoothEnd_On = true;
			bool heightOffset_On = true;
			bool leftSide_On = true;
			bool smooth1a;
			bool start = Singleton<NetManager>.instance.m_segments.m_buffer[(int)nodeSegmentID].m_startNode == nodeID;

			Singleton<NetManager>.instance.m_segments.m_buffer[(int)nodeSegmentID].CalculateCorner(nodeSegmentID, heightOffset_On, start, !leftSide_On, out StartPosition1a, out end_cornerDirection1a, out smooth1a);
			Singleton<NetManager>.instance.m_segments.m_buffer[(int)nodeSegmentID].CalculateCorner(nodeSegmentID, heightOffset_On, start, leftSide_On, out StartPosition1b, out end_cornerDirection1b, out smooth1a);

			bool start2 = Singleton<NetManager>.instance.m_segments.m_buffer[(int)nodeSegment2].m_startNode == nodeID;

			Singleton<NetManager>.instance.m_segments.m_buffer[(int)nodeSegment2].CalculateCorner(nodeSegment2, heightOffset_On, start2, leftSide_On, out end_cornerPosition1c, out end_cornerDirection1c, out smooth1a);
			Singleton<NetManager>.instance.m_segments.m_buffer[(int)nodeSegment2].CalculateCorner(nodeSegment2, heightOffset_On, start2, !leftSide_On, out end_cornerPosition1d, out end_cornerDirection1d, out smooth1a);
			NetSegment.CalculateMiddlePoints(StartPosition1a, -end_cornerDirection1a, end_cornerPosition1c, -end_cornerDirection1c, smoothStart_On, smoothEnd_On, out middlePosition1a, out middlePosition2a);
			NetSegment.CalculateMiddlePoints(StartPosition1b, -end_cornerDirection1b, end_cornerPosition1d, -end_cornerDirection1d, smoothStart_On, smoothEnd_On, out middlePosition1b, out middlePosition2b);
			data.m_dataMatrix0 = NetSegment.CalculateControlMatrix(StartPosition1a, middlePosition1a, middlePosition2a, end_cornerPosition1c, StartPosition1b, middlePosition1b, middlePosition2b, end_cornerPosition1d, netnode.m_position, vScale);
			data.m_extraData.m_dataMatrix2 = NetSegment.CalculateControlMatrix(StartPosition1b, middlePosition1b, middlePosition2b, end_cornerPosition1d, StartPosition1a, middlePosition1a, middlePosition2a, end_cornerPosition1c, netnode.m_position, vScale);
			data.m_dataVector0 = new Vector4(0.5f / info.m_halfWidth, 1f / info.m_segmentLength, 1f, 1f);
			data.m_dataVector3 = RenderManager.GetColorLocation(86016u + (uint)nodeID);
			data.m_dataInt0 = (8 | segmentIndex);
			data.m_dataColor0 = info.m_color;
			data.m_dataColor0.a = 0f;
			bool requireSurfaceMaps = info.m_requireSurfaceMaps;
			if(requireSurfaceMaps)//if given node or segment has this set to true
			{
				Singleton<TerrainManager>.instance.GetSurfaceMapping(data.m_position, out data.m_dataTexture0, out data.m_dataTexture1, out data.m_dataVector1);
			}
			instanceIndex = (uint)data.m_nextInstance;
		}

		private int CalculateRendererCount(NetNode netnode, NetInfo info)
		{
			bool junctionFlag1a = (netnode.m_flags & NetNode.Flags.Junction) == NetNode.Flags.None;
			int result;
			if(junctionFlag1a)//=true
			{
				result = 1;
			}
			else
			{
				int nodeCounter = 0;
				bool requireSegmentRenderers = info.m_requireSegmentRenderers;
				if(requireSegmentRenderers)
				{
					nodeCounter += netnode.CountSegments();
				}
				bool requireDirectRenderers = info.m_requireDirectRenderers;
				if(requireDirectRenderers)
				{
					nodeCounter += (int)netnode.m_connectCount;
				}
				result = nodeCounter;
			}
			return result;
		}

		public void RenderInstanceNode(RenderManager.CameraInfo cameraInfo, ushort nodeID, int layerMask)
		{
			NetManager instance = Singleton<NetManager>.instance;
			NetNode netNode = instance.m_nodes.m_buffer[(int)nodeID];
			bool flag = netNode.m_flags == NetNode.Flags.None;
			if(!flag)
			{
				NetInfo info = netNode.Info;
				bool flag2 = !cameraInfo.Intersect(netNode.m_bounds);
				if(!flag2)
				{
					bool flag3 = netNode.m_problems != Notification.Problem.None && (layerMask & 1 << Singleton<NotificationManager>.instance.m_notificationLayer) != 0;
					if(flag3)
					{
						Vector3 position = netNode.m_position;
						position.y += Mathf.Max(5f, info.m_maxHeight);
						Notification.RenderInstance(cameraInfo, netNode.m_problems, position, 1f);
						//RenderInstance(cameraInfo, problems, position, scale);
					}
					bool flag4 = (layerMask & info.m_netLayers) == 0 || (netNode.m_flags & (NetNode.Flags.End | NetNode.Flags.Bend | NetNode.Flags.Junction)) == NetNode.Flags.None;
					if(!flag4)
					{
						bool flag5 = (netNode.m_flags & NetNode.Flags.Bend) > NetNode.Flags.None;
						if(flag5)
						{
							bool flag6 = info.m_segments == null || info.m_segments.Length == 0;
							if(flag6)
							{
								return;
							}
						}
						else
						{
							bool flag7 = info.m_nodes == null || info.m_nodes.Length == 0;
							if(flag7)
							{
								return;
							}
						}
						uint count = (uint)this.CalculateRendererCount(netNode, info);
						RenderManager instance2 = Singleton<RenderManager>.instance;
						uint instanceIndex1a;
						bool flag8 = !instance2.RequireInstance(86016u + (uint)nodeID, count, out instanceIndex1a);
						if(!flag8)
						{
							int iter_1a = 0;
							while(instanceIndex1a != 65535u)
							{
								this.RenderInstanceNode(cameraInfo, nodeID, info, iter_1a, netNode.m_flags, ref instanceIndex1a, ref instance2.m_instances[(int)instanceIndex1a]);
								//RenderInstanceNode(cameraInfo, nodeID, info, iter, flags, instanceIndex, data)
								iter_1a += 1;
								bool maxCount = iter_1a > 36;
								if(maxCount)
								{
									CODebugBase<LogChannel>.Error(LogChannel.Core, "Invalid list detected!\n" + Environment.StackTrace);
									//Error(T ll,string msg)
									break;
								}
							}
						}
					}
				}
			}
		}

		public void RenderInstanceNode(RenderManager.CameraInfo cameraInfo, ushort nodeID, NetInfo info, int iter, NetNode.Flags flags, ref uint instanceIndex, ref RenderManager.Instance data)
		{
			NetManager instance = Singleton<NetManager>.instance;
			NetNode netnode = instance.m_nodes.m_buffer[(int)nodeID];
			if(data.m_dirty)
			{
				data.m_dirty = false;
				if(iter == 0)
				{
					if((flags & NetNode.Flags.Junction) > NetNode.Flags.None)
					{
						this.RefreshJunctionData(netnode, nodeID, info, instanceIndex);
					}
					else if((flags & NetNode.Flags.Bend) > NetNode.Flags.None)
					{
						this.RefreshBendData(netnode, nodeID, info, instanceIndex, ref data);
					}
					else if((flags & NetNode.Flags.End) > NetNode.Flags.None)
					{
						this.RefreshEndData(netnode, nodeID, info, instanceIndex, ref data);
					}
				}
			}
			if(data.m_initialized)
			{
				if((flags & NetNode.Flags.Junction) > NetNode.Flags.None)
				{
					if((data.m_dataInt0 & 8) != 0)
					{
						int segment1 = netnode.GetSegment(data.m_dataInt0 & 7);
						int segment2 = netnode.GetSegment(data.m_dataInt0 >> 4);
						if(segment1 != 0 && segment2 > 0)
						{
							NetManager instance2 = Singleton<NetManager>.instance;
							info = instance2.m_segments.m_buffer[(int)segment1].Info;
							NetInfo info2 = instance2.m_segments.m_buffer[(int)segment2].Info;
							int i = 0;
							while(i < info.m_nodes.Length)
							{
								NetInfo.Node node = info.m_nodes[i];
								int num;
								if(node.CheckFlags(flags) && node.m_directConnect && (node.m_connectGroup == NetInfo.ConnectGroup.None || (node.m_connectGroup & info2.m_connectGroup & NetInfo.ConnectGroup.AllGroups) > NetInfo.ConnectGroup.None))
								{
									Vector4 dataVector = data.m_dataVector3;
									Vector4 dataVector2 = data.m_dataVector0;
									if(node.m_requireWindSpeed)
									{
										dataVector.w = data.m_dataFloat0;
									}
									if((node.m_connectGroup & NetInfo.ConnectGroup.Oneway) > NetInfo.ConnectGroup.None)
									{
										bool flag10 = instance2.m_segments.m_buffer[(int)segment1].m_startNode == nodeID == ((instance2.m_segments.m_buffer[(int)segment1].m_flags & NetSegment.Flags.Invert) == NetSegment.Flags.None);
										if(info2.m_hasBackwardVehicleLanes != info2.m_hasForwardVehicleLanes)
										{
											bool flag12 = instance2.m_segments.m_buffer[segment2].m_startNode == nodeID == ((instance2.m_segments.m_buffer[(int)segment2].m_flags & NetSegment.Flags.Invert) == NetSegment.Flags.None);
											bool flag13 = flag10 == flag12;
											if(flag13)
											{
												i += 1;
												continue;
											}
										}
										if(flag10)
										{
											if((node.m_connectGroup & NetInfo.ConnectGroup.OnewayStart) == NetInfo.ConnectGroup.None)
											{
												i += 1;
												continue;
											}
										}
										else
										{
											if(!((node.m_connectGroup & NetInfo.ConnectGroup.OnewayEnd) != NetInfo.ConnectGroup.None))
											{
												i += 1;
												continue;
											}
											dataVector2.x = -dataVector2.x;
											dataVector2.y = -dataVector2.y;
										}
									}
									instance2.m_materialBlock.Clear();
									instance2.m_materialBlock.AddMatrix(instance2.ID_LeftMatrix, data.m_dataMatrix0);
									instance2.m_materialBlock.AddMatrix(instance2.ID_RightMatrix, data.m_extraData.m_dataMatrix2);
									instance2.m_materialBlock.AddVector(instance2.ID_MeshScale, dataVector2);
									instance2.m_materialBlock.AddVector(instance2.ID_ObjectIndex, dataVector);
									instance2.m_materialBlock.AddColor(instance2.ID_Color, data.m_dataColor0);
									if(node.m_requireSurfaceMaps && data.m_dataTexture1 != null)
									{
										instance2.m_materialBlock.AddTexture(instance2.ID_SurfaceTexA, data.m_dataTexture0);
										instance2.m_materialBlock.AddTexture(instance2.ID_SurfaceTexB, data.m_dataTexture1);
										instance2.m_materialBlock.AddVector(instance2.ID_SurfaceMapping, data.m_dataVector1);
									}
									NetManager var_30_3FE_cp_0_cp_0 = instance2;
									num = var_30_3FE_cp_0_cp_0.m_drawCallData.m_defaultCalls + 1;
									var_30_3FE_cp_0_cp_0.m_drawCallData.m_defaultCalls = num;
									Graphics.DrawMesh(node.m_nodeMesh, data.m_position, data.m_rotation, node.m_nodeMaterial, node.m_layer, null, 0, instance2.m_materialBlock);
								}
								i += 1;
								continue;
							}
						}
					}
					else
					{
						ushort segment = netnode.GetSegment(data.m_dataInt0 & 7);
						if(segment > 0)
						{
							NetManager instance2 = Singleton<NetManager>.instance;
							info = instance2.m_segments.m_buffer[(int)segment].Info;
							int num;
							for(int j = 0; j < info.m_nodes.Length; j = num)
							{
								NetInfo.Node node2 = info.m_nodes[j];
								if(node2.CheckFlags(flags) && !node2.m_directConnect)
								{
									Vector4 dataVector3 = data.m_extraData.m_dataVector4;
									if(node2.m_requireWindSpeed)
									{
										dataVector3.w = data.m_dataFloat0;
									}
									instance2.m_materialBlock.Clear();
									instance2.m_materialBlock.AddMatrix(instance2.ID_LeftMatrix, data.m_dataMatrix0);
									instance2.m_materialBlock.AddMatrix(instance2.ID_RightMatrix, data.m_extraData.m_dataMatrix2);
									instance2.m_materialBlock.AddMatrix(instance2.ID_LeftMatrixB, data.m_extraData.m_dataMatrix3);
									instance2.m_materialBlock.AddMatrix(instance2.ID_RightMatrixB, data.m_dataMatrix1);
									instance2.m_materialBlock.AddVector(instance2.ID_MeshScale, data.m_dataVector0);
									instance2.m_materialBlock.AddVector(instance2.ID_CenterPos, data.m_dataVector1);
									instance2.m_materialBlock.AddVector(instance2.ID_SideScale, data.m_dataVector2);
									instance2.m_materialBlock.AddVector(instance2.ID_ObjectIndex, dataVector3);
									instance2.m_materialBlock.AddColor(instance2.ID_Color, data.m_dataColor0);
									if(node2.m_requireSurfaceMaps && data.m_dataTexture1 != null)
									{
										instance2.m_materialBlock.AddTexture(instance2.ID_SurfaceTexA, data.m_dataTexture0);
										instance2.m_materialBlock.AddTexture(instance2.ID_SurfaceTexB, data.m_dataTexture1);
										instance2.m_materialBlock.AddVector(instance2.ID_SurfaceMapping, data.m_dataVector3);
									}
									NetManager var_30_68B_cp_0_cp_0 = instance2;
									num = var_30_68B_cp_0_cp_0.m_drawCallData.m_defaultCalls + 1;
									var_30_68B_cp_0_cp_0.m_drawCallData.m_defaultCalls = num;
									Graphics.DrawMesh(node2.m_nodeMesh, data.m_position, data.m_rotation, node2.m_nodeMaterial, node2.m_layer, null, 0, instance2.m_materialBlock);
								}
								num = j + 1;
							}
						}
					}
				}
				else if((flags & NetNode.Flags.End) > NetNode.Flags.None)
				{
					NetManager instance2 = Singleton<NetManager>.instance;
					int num;
					for(int k = 0; k < info.m_nodes.Length; k = num)
					{
						NetInfo.Node node2 = info.m_nodes[k];
						if(node2.CheckFlags(flags) && !node2.m_directConnect)
						{
							Vector4 dataVector4 = data.m_extraData.m_dataVector4;
							if(node2.m_requireWindSpeed)
							{
								dataVector4.w = data.m_dataFloat0;
							}
							instance2.m_materialBlock.Clear();
							instance2.m_materialBlock.AddMatrix(instance2.ID_LeftMatrix, data.m_dataMatrix0);
							instance2.m_materialBlock.AddMatrix(instance2.ID_RightMatrix, data.m_extraData.m_dataMatrix2);
							instance2.m_materialBlock.AddMatrix(instance2.ID_LeftMatrixB, data.m_extraData.m_dataMatrix3);
							instance2.m_materialBlock.AddMatrix(instance2.ID_RightMatrixB, data.m_dataMatrix1);
							instance2.m_materialBlock.AddVector(instance2.ID_MeshScale, data.m_dataVector0);
							instance2.m_materialBlock.AddVector(instance2.ID_CenterPos, data.m_dataVector1);
							instance2.m_materialBlock.AddVector(instance2.ID_SideScale, data.m_dataVector2);
							instance2.m_materialBlock.AddVector(instance2.ID_ObjectIndex, dataVector4);
							instance2.m_materialBlock.AddColor(instance2.ID_Color, data.m_dataColor0);
							if(node2.m_requireSurfaceMaps && data.m_dataTexture1 != null)
							{
								instance2.m_materialBlock.AddTexture(instance2.ID_SurfaceTexA, data.m_dataTexture0);
								instance2.m_materialBlock.AddTexture(instance2.ID_SurfaceTexB, data.m_dataTexture1);
								instance2.m_materialBlock.AddVector(instance2.ID_SurfaceMapping, data.m_dataVector3);
							}
							NetManager var_30_8EF_cp_0_cp_0 = instance2;
							num = var_30_8EF_cp_0_cp_0.m_drawCallData.m_defaultCalls + 1;
							var_30_8EF_cp_0_cp_0.m_drawCallData.m_defaultCalls = num;
							Graphics.DrawMesh(node2.m_nodeMesh, data.m_position, data.m_rotation, node2.m_nodeMaterial, node2.m_layer, null, 0, instance2.m_materialBlock);
						}
						num = k + 1;
					}
				}
				else if((flags & NetNode.Flags.Bend) > NetNode.Flags.None)
				{
					NetManager instance2 = Singleton<NetManager>.instance;
					int num;
					for(int l = 0; l < info.m_segments.Length; l = num)
					{
						NetInfo.Segment segment2 = info.m_segments[l];
						bool flag26;
						if(segment2.CheckFlags(NetSegment.Flags.None, out flag26) && !segment2.m_disableBendNodes)
						{
							Vector4 dataVector5 = data.m_dataVector3;
							if(segment2.m_requireWindSpeed)
							{
								dataVector5.w = data.m_dataFloat0;
							}
							instance2.m_materialBlock.Clear();
							instance2.m_materialBlock.AddMatrix(instance2.ID_LeftMatrix, data.m_dataMatrix0);
							instance2.m_materialBlock.AddMatrix(instance2.ID_RightMatrix, data.m_extraData.m_dataMatrix2);
							instance2.m_materialBlock.AddVector(instance2.ID_MeshScale, data.m_dataVector0);
							instance2.m_materialBlock.AddVector(instance2.ID_ObjectIndex, dataVector5);
							instance2.m_materialBlock.AddColor(instance2.ID_Color, data.m_dataColor0);
							if(segment2.m_requireSurfaceMaps && data.m_dataTexture1 != null)
							{
								instance2.m_materialBlock.AddTexture(instance2.ID_SurfaceTexA, data.m_dataTexture0);
								instance2.m_materialBlock.AddTexture(instance2.ID_SurfaceTexB, data.m_dataTexture1);
								instance2.m_materialBlock.AddVector(instance2.ID_SurfaceMapping, data.m_dataVector1);
							}
							NetManager var_30_ADC_cp_0_cp_0 = instance2;
							num = var_30_ADC_cp_0_cp_0.m_drawCallData.m_defaultCalls + 1;
							var_30_ADC_cp_0_cp_0.m_drawCallData.m_defaultCalls = num;
							Graphics.DrawMesh(segment2.m_segmentMesh, data.m_position, data.m_rotation, segment2.m_segmentMaterial, segment2.m_layer, null, 0, instance2.m_materialBlock);
						}
						num = l + 1;
					}
					int m = 0;
					while(m < info.m_nodes.Length)
					{
						NetInfo.Node node2 = info.m_nodes[m];
						if(node2.CheckFlags(flags) && node2.m_directConnect && (node2.m_connectGroup == NetInfo.ConnectGroup.None || (node2.m_connectGroup & info.m_connectGroup & NetInfo.ConnectGroup.AllGroups) > NetInfo.ConnectGroup.None))
						{
							Vector4 dataVector6 = data.m_dataVector3;
							Vector4 dataVector7 = data.m_dataVector0;
							if(node2.m_requireWindSpeed)
							{
								dataVector6.w = data.m_dataFloat0;
							}
							if((node2.m_connectGroup & NetInfo.ConnectGroup.Oneway) > NetInfo.ConnectGroup.None)
							{
								ushort segment5 = netnode.GetSegment(data.m_dataInt0 & 7);
								ushort segment6 = netnode.GetSegment(data.m_dataInt0 >> 4);
								bool flag30 = instance2.m_segments.m_buffer[(int)segment5].m_startNode == nodeID == ((instance2.m_segments.m_buffer[(int)segment5].m_flags & NetSegment.Flags.Invert) == NetSegment.Flags.None);
								bool flag31 = instance2.m_segments.m_buffer[(int)segment6].m_startNode == nodeID == ((instance2.m_segments.m_buffer[(int)segment6].m_flags & NetSegment.Flags.Invert) == NetSegment.Flags.None);
								bool flag32 = flag30 != flag31;
								if(!flag32)
								{
									m += 1;
									continue;
								}
								if(flag30)
								{
									if((node2.m_connectGroup & NetInfo.ConnectGroup.OnewayStart) == NetInfo.ConnectGroup.None)
									{
										m += 1;
										continue;
									}
								}
								else
								{
									if(!((node2.m_connectGroup & NetInfo.ConnectGroup.OnewayEnd) > NetInfo.ConnectGroup.None))
									{
										m += 1;
										continue;
									}

									dataVector7.x = -dataVector7.x;
									dataVector7.y = -dataVector7.y;
								}
							}
							instance2.m_materialBlock.Clear();
							instance2.m_materialBlock.AddMatrix(instance2.ID_LeftMatrix, data.m_dataMatrix0);
							instance2.m_materialBlock.AddMatrix(instance2.ID_RightMatrix, data.m_extraData.m_dataMatrix2);
							instance2.m_materialBlock.AddVector(instance2.ID_MeshScale, dataVector7);
							instance2.m_materialBlock.AddVector(instance2.ID_ObjectIndex, dataVector6);
							instance2.m_materialBlock.AddColor(instance2.ID_Color, data.m_dataColor0);
							if(node2.m_requireSurfaceMaps && data.m_dataTexture1 != null)
							{
								instance2.m_materialBlock.AddTexture(instance2.ID_SurfaceTexA, data.m_dataTexture0);
								instance2.m_materialBlock.AddTexture(instance2.ID_SurfaceTexB, data.m_dataTexture1);
								instance2.m_materialBlock.AddVector(instance2.ID_SurfaceMapping, data.m_dataVector1);
							}
							NetManager var_30_E04_cp_0_cp_0 = instance2;
							num = var_30_E04_cp_0_cp_0.m_drawCallData.m_defaultCalls + 1;
							var_30_E04_cp_0_cp_0.m_drawCallData.m_defaultCalls = num;
							Graphics.DrawMesh(node2.m_nodeMesh, data.m_position, data.m_rotation, node2.m_nodeMaterial, node2.m_layer, null, 0, instance2.m_materialBlock);
						}
						m += 1;
						continue;
					}
				}
			}
			instanceIndex = (uint)data.m_nextInstance;
		}

		private void RefreshEndData(NetNode netnode, ushort nodeID, NetInfo info, uint instanceIndex, ref RenderManager.Instance data)
		{
			MethodInfo method = this.GetMethod("RefreshEndData", 4u);
			object[] array = new object[]
            {
                nodeID,
                info,
                instanceIndex,
                data
            };
			method.Invoke(netnode, array);
			data = (RenderManager.Instance)array[3];
		}

		public void RenderInstanceSegment(RenderManager.CameraInfo cameraInfo, ushort segmentID, int layerMask)
		{
			NetManager instance = Singleton<NetManager>.instance;
			NetSegment netSegment = instance.m_segments.m_buffer[(int)segmentID];
			bool flag = netSegment.m_flags == NetSegment.Flags.None;
			if(!flag)
			{
				NetInfo info = netSegment.Info;
				bool flag2 = !cameraInfo.Intersect(netSegment.m_bounds);
				//Intersect(Bounds bounds);
				if(!flag2)
				{
					bool flag3 = netSegment.m_problems != Notification.Problem.None && (layerMask & 1 << Singleton<NotificationManager>.instance.m_notificationLayer) != 0;
					if(flag3)
					{
						Vector3 middlePosition = netSegment.m_middlePosition;
						middlePosition.y += Mathf.Max(5f, info.m_maxHeight);
						Notification.RenderInstance(cameraInfo, netSegment.m_problems, middlePosition, 1f);
						//RenderInstance(RenderManager.CameraInfo cameraInfo,Notification.Problem problems,Vector3 position,float scale);
					}
					bool flag4 = (layerMask & info.m_netLayers) == 0;
					if(!flag4)
					{
						RenderManager instance2 = Singleton<RenderManager>.instance;
						uint num;
						bool flag5 = !instance2.RequireInstance((uint)(49152 + segmentID), 1u, out num);
						//RequireInstance(uint holder,uint count,out uint instanceIndex);
						if(!flag5)
						{
							this.RenderInstanceSegmentNew(cameraInfo, segmentID, layerMask, info, ref instance2.m_instances[(int)num]);
							//RenderInstanceSegmentNew(RenderManager.CameraInfo cameraInfo, ushort segmentID, int layerMask, NetInfo info, ref RenderManager.Instance data)
						}
					}
				}
			}
		}

		private void RenderInstanceSegmentNew(RenderManager.CameraInfo cameraInfo, ushort segmentID, int layerMask, NetInfo info, ref RenderManager.Instance data)
		{
			NetManager instance = Singleton<NetManager>.instance;
			bool hwyInvertTrue = false;

			if((instance.m_segments.m_buffer[(int)segmentID].m_flags & NetSegment.Flags.Invert) != NetSegment.Flags.None && info.name.Contains("Highway") && !info.name.ToLower().Contains("tunnel") && !info.name.ToLower().Contains("slope") && !info.name.ToLower().Contains("tram"))
			{
				hwyInvertTrue = true;
				ushort endNode = instance.m_segments.m_buffer[(int)segmentID].m_endNode;
				instance.m_segments.m_buffer[(int)segmentID].m_endNode = instance.m_segments.m_buffer[(int)segmentID].m_startNode;//end=start
				instance.m_segments.m_buffer[(int)segmentID].m_startNode = endNode;//												start=end
				instance.m_segments.m_buffer[(int)segmentID].m_flags = (instance.m_segments.m_buffer[(int)segmentID].m_flags & ~NetSegment.Flags.Invert);//with flags toggle invert
				Vector3 endDirection = instance.m_segments.m_buffer[(int)segmentID].m_endDirection;
				instance.m_segments.m_buffer[(int)segmentID].m_endDirection = instance.m_segments.m_buffer[(int)segmentID].m_startDirection;//change end to start
				instance.m_segments.m_buffer[(int)segmentID].m_startDirection = endDirection;//												  change start to end
			}

			if(data.m_dirty)
			{
				data.m_dirty = false;
				Vector3 position = instance.m_nodes.m_buffer[(int)instance.m_segments.m_buffer[(int)segmentID].m_startNode].m_position;
				Vector3 position2 = instance.m_nodes.m_buffer[(int)instance.m_segments.m_buffer[(int)segmentID].m_endNode].m_position;
				data.m_position = (position + position2) * 0.5f;
				data.m_rotation = Quaternion.identity;
				data.m_dataColor0 = info.m_color;
				data.m_dataColor0.a = 0f;
				data.m_dataFloat0 = Singleton<WeatherManager>.instance.GetWindSpeed(data.m_position);
				//GetWindSpeed(Vector3 pos);
				data.m_dataVector0 = new Vector4(0.5f / info.m_halfWidth, 1f / info.m_segmentLength, 1f, 1f);
				Vector4 colorLocation = RenderManager.GetColorLocation((uint)(49152 + segmentID));
				//GetColorLocation(uint instanceHolder);
				Vector4 vector = colorLocation;
				//BlendJunction(ushort nodeID);
				if(NetNode.BlendJunction(instance.m_segments.m_buffer[(int)segmentID].m_startNode))
				{
					colorLocation = RenderManager.GetColorLocation(86016u + (uint)instance.m_segments.m_buffer[(int)segmentID].m_startNode);
					//GetColorLocation(uint instanceHolder);
				}
				//BlendJunction(ushort nodeID);
				if(NetNode.BlendJunction(instance.m_segments.m_buffer[(int)segmentID].m_endNode))
				{
					vector = RenderManager.GetColorLocation(86016u + (uint)instance.m_segments.m_buffer[(int)segmentID].m_endNode);
					//GetColorLocation(uint instanceHolder);
				}
				data.m_dataVector3 = new Vector4(colorLocation.x, colorLocation.y, vector.x, vector.y);
				if(info.m_segments == null || info.m_segments.Length == 0)
				{
					bool hasLanes = info.m_lanes != null;
					if(hasLanes)
					{
						bool isSurfaceNetwork = (
												(hwyInvertTrue && !info.name.Contains("Highway")) ||
												(!hwyInvertTrue && info.name.Contains("Highway") && !info.name.ToLower().Contains("tunnel") && !info.name.ToLower().Contains("slope") && !info.name.ToLower().Contains("tram")));
						bool invert;
						if(isSurfaceNetwork)
						{
							invert = true;
							NetNode.Flags netFlags;
							Color color;
							//GetNodeState(nodeID, nodeData, segmentID, segmentData, flags, color);
							instance.m_nodes.m_buffer[(int)instance.m_segments.m_buffer[(int)segmentID].m_endNode].Info.m_netAI.GetNodeState(
								instance.m_segments.m_buffer[(int)segmentID].m_endNode/*nodeID*/,
								ref instance.m_nodes.m_buffer[(int)instance.m_segments.m_buffer[(int)segmentID].m_endNode]/*nodeData*/,
								segmentID, ref instance.m_segments.m_buffer[(int)segmentID]/*segmentData*/, out netFlags, out color);

							NetNode.Flags netFlags2;//quick shortcut/uniqe ID for flags
							Color color2;//quick shortcut/uniqe ID for color

							instance.m_nodes.m_buffer[(int)instance.m_segments.m_buffer[(int)segmentID].m_startNode].Info.m_netAI.GetNodeState(
								instance.m_segments.m_buffer[(int)segmentID].m_startNode/*nodeID*/,
								ref instance.m_nodes.m_buffer[(int)instance.m_segments.m_buffer[(int)segmentID].m_startNode]/*nodeData*/,
								segmentID, ref instance.m_segments.m_buffer[(int)segmentID]/*segmentData*/, out netFlags2, out color2);
						}//31560[Created,Junction,Untouchable,OnGround,Transition]
						//19561[Created,Junction,OnGround,Transition,TrafficLights]///36470[Basic Road Tram]		10024[Basic Road]		23898[Medium Road Tram]
						else
						{
							bool segHasFlags = (instance.m_segments.m_buffer[(int)segmentID].m_flags & NetSegment.Flags.Invert) > NetSegment.Flags.None;//enum for .None==0
							if(segHasFlags)
							{
								invert = true;
								NetNode.Flags NetNodeFlags;
								Color color;
								instance.m_nodes.m_buffer[(int)instance.m_segments.m_buffer[(int)segmentID].m_endNode].Info.m_netAI.GetNodeState(instance.m_segments.m_buffer[(int)segmentID].m_endNode, ref instance.m_nodes.m_buffer[(int)instance.m_segments.m_buffer[(int)segmentID].m_endNode], segmentID, ref instance.m_segments.m_buffer[(int)segmentID], out NetNodeFlags, out color);

								NetNode.Flags NetNodeFlags2;
								Color color2;
								instance.m_nodes.m_buffer[(int)instance.m_segments.m_buffer[(int)segmentID].m_startNode].Info.m_netAI.GetNodeState(instance.m_segments.m_buffer[(int)segmentID].m_startNode, ref instance.m_nodes.m_buffer[(int)instance.m_segments.m_buffer[(int)segmentID].m_startNode], segmentID, ref instance.m_segments.m_buffer[(int)segmentID], out NetNodeFlags2, out color2);
								//GetNodeState(ushort nodeID,ref NetNode nodeData,ushort segmentID,ref NetSegment segmentData,out NetNode.Flags flags,out Color color);
							}
							else
							{
								invert = false;
								NetNode.Flags flags;
								Color color;
								instance.m_nodes.m_buffer[(int)instance.m_segments.m_buffer[(int)segmentID].m_startNode].Info.m_netAI.GetNodeState(instance.m_segments.m_buffer[(int)segmentID].m_startNode, ref instance.m_nodes.m_buffer[(int)instance.m_segments.m_buffer[(int)segmentID].m_startNode], segmentID, ref instance.m_segments.m_buffer[(int)segmentID], out flags, out color);

								NetNode.Flags flags2;
								Color color2;
								instance.m_nodes.m_buffer[(int)instance.m_segments.m_buffer[(int)segmentID].m_endNode].Info.m_netAI.GetNodeState(instance.m_segments.m_buffer[(int)segmentID].m_endNode, ref instance.m_nodes.m_buffer[(int)instance.m_segments.m_buffer[(int)segmentID].m_endNode], segmentID, ref instance.m_segments.m_buffer[(int)segmentID], out flags2, out color2);
								//GetNodeState(ushort nodeID,ref NetNode nodeData,ushort segmentID,ref NetSegment segmentData,out NetNode.Flags flags,out Color color);			
							}
						}
						float startAngle = (float)instance.m_segments.m_buffer[(int)segmentID].m_cornerAngleStart * 0.02454369f;
						float endAngle = (float)instance.m_segments.m_buffer[(int)segmentID].m_cornerAngleEnd * 0.02454369f;
						int PropIndex = 0;
						uint netLaneID = instance.m_segments.m_buffer[(int)segmentID].m_lanes;
						int NetLane_info = 0;
						while(NetLane_info < info.m_lanes.Length && netLaneID > 0u)
						{
							instance.m_lanes.m_buffer[(int)netLaneID].RefreshInstance(netLaneID, info.m_lanes[NetLane_info], startAngle, endAngle, invert, ref data, ref PropIndex);
							//RefreshInstance(laneID,laneInfo,startAngle,endAngle,invert,data,propIndex);
							netLaneID = instance.m_lanes.m_buffer[(int)netLaneID].m_nextLane;
							//int num4 = NetLane_info + 1;
							NetLane_info += 1;
						}
					}
				}
				else
				{
					float vScale = info.m_netAI.GetVScale();
					bool smoothStart;
					bool smoothEnd;//
					bool heightOffsetOn = true;
					bool isStart = true;
					bool isLeftSide = true;
					Vector3 corner_startDirection;
					Vector3 startDir2;

					Vector3 endDir;
					Vector3 endDir2;

					Vector3 corner_startPosition;
					Vector3 vector3;
					Vector3 vector4;
					Vector3 vector5;
					Vector3 vector6;
					Vector3 vector7;
					Vector3 vector8;
					Vector3 vector9;

					instance.m_segments.m_buffer[(int)segmentID].CalculateCorner(segmentID, heightOffsetOn, isStart, isLeftSide, out corner_startPosition, out corner_startDirection, out smoothStart);
					instance.m_segments.m_buffer[(int)segmentID].CalculateCorner(segmentID, heightOffsetOn, !isStart, isLeftSide, out vector3, out endDir, out smoothEnd);
					instance.m_segments.m_buffer[(int)segmentID].CalculateCorner(segmentID, heightOffsetOn, isStart, !isLeftSide, out vector4, out startDir2, out smoothStart);
					instance.m_segments.m_buffer[(int)segmentID].CalculateCorner(segmentID, heightOffsetOn, !isStart, !isLeftSide, out vector5, out endDir2, out smoothEnd);
					//CalculateCorner( segmentID, heightOffset, start,bool leftSide, cornerPos, cornerDirection, smooth);
					NetSegment.CalculateMiddlePoints(corner_startPosition, corner_startDirection, vector5, endDir2, smoothStart, smoothEnd, out vector6, out vector7);
					//CalculateMiddlePoints(Vector3 startPos,Vector3 startDir,Vector3 endPos,Vector3 endDir,bool smoothStart,bool smoothEnd,out Vector3 middlePos1,out Vector3 middlePos2);
					NetSegment.CalculateMiddlePoints(vector4, startDir2, vector3, endDir, smoothStart, smoothEnd, out vector8, out vector9);
					data.m_dataMatrix0 = NetSegment.CalculateControlMatrix(corner_startPosition, vector6, vector7, vector5, vector4, vector8, vector9, vector3, data.m_position, vScale);
					data.m_dataMatrix1 = NetSegment.CalculateControlMatrix(vector4, vector8, vector9, vector3, corner_startPosition, vector6, vector7, vector5, data.m_position, vScale);
					//CalculateControlMatrix(Vector3 startPos,Vector3 middlePos1,Vector3 middlePos2,Vector3 endPos,Vector3 startPosB,Vector3 middlePosB1,Vector3 middlePosB2,Vector3 endPosB,Vector3 transform,float vScale);
				}
				if(info.m_requireSurfaceMaps)
				{
					Singleton<TerrainManager>.instance.GetSurfaceMapping(data.m_position, out data.m_dataTexture0, out data.m_dataTexture1, out data.m_dataVector1);
					//GetSurfaceMapping(Vector3 worldPos,out Texture _SurfaceTexA,out Texture _SurfaceTexB,out Vector4 _SurfaceMapping);
				}
			}

			if(info.m_segments != null)
			{
				for(int i = 0; i < info.m_segments.Length; i++)
				{
					NetInfo.Segment segment = info.m_segments[i];
					bool turnAround;
					bool flag10;
					//CheckFlags(NetSegment.Flags flags,out bool turnAround);
					if(segment.CheckFlags(instance.m_segments.m_buffer[(int)segmentID].m_flags, out turnAround))
					{
						Vector4 dataVector = data.m_dataVector3;
						Vector4 dataVector2 = data.m_dataVector0;
						if(segment.m_requireWindSpeed)
						{
							dataVector.w = data.m_dataFloat0;
						}
						if(turnAround)
						{
							dataVector2.x = -dataVector2.x;
							dataVector2.y = -dataVector2.y;
						}
						instance.m_materialBlock.Clear();
						instance.m_materialBlock.AddMatrix(instance.ID_LeftMatrix, data.m_dataMatrix0);
						instance.m_materialBlock.AddMatrix(instance.ID_RightMatrix, data.m_dataMatrix1);
						//AddMatrix(int nameID,Matrix4x4 value)
						instance.m_materialBlock.AddVector(instance.ID_MeshScale, dataVector2);
						instance.m_materialBlock.AddVector(instance.ID_ObjectIndex, dataVector);
						//AddVector(int nameID,Vector4 value)
						instance.m_materialBlock.AddColor(instance.ID_Color, data.m_dataColor0);
						//AddColor(int nameID,Color value);
						if(segment.m_requireSurfaceMaps && data.m_dataTexture0 != null)
						{
							instance.m_materialBlock.AddTexture(instance.ID_SurfaceTexA, data.m_dataTexture0);
							instance.m_materialBlock.AddTexture(instance.ID_SurfaceTexB, data.m_dataTexture1);
							//AddTexture(int nameID,Texture value);
							instance.m_materialBlock.AddVector(instance.ID_SurfaceMapping, data.m_dataVector1);
							//AddVector(int nameID,Vector4 value)
						}
						NetManager var_66_AE1_cp_0_cp_0 = instance;
						num4 = var_66_AE1_cp_0_cp_0.m_drawCallData.m_defaultCalls + 1;
						var_66_AE1_cp_0_cp_0.m_drawCallData.m_defaultCalls = num4;
						Graphics.DrawMesh(segment.m_segmentMesh, data.m_position, data.m_rotation, segment.m_segmentMaterial, segment.m_layer, null, 0, instance.m_materialBlock);
						//DrawMesh(Mesh mesh,Vector3 position,Quaternion rotation,Material material,int layer,Camera camera,int submeshIndex,MaterialPropertyBlock properties);
					}
				}
			}//
			//
			if(hwyInvertTrue)
			{
				ushort endNode2 = instance.m_segments.m_buffer[(int)segmentID].m_endNode;
				instance.m_segments.m_buffer[(int)segmentID].m_endNode = instance.m_segments.m_buffer[(int)segmentID].m_startNode;
				instance.m_segments.m_buffer[(int)segmentID].m_startNode = endNode2;
				NetSegment[] SegID = instance.m_segments.m_buffer;
				SegID[(int)segmentID].m_flags = (SegID[(int)segmentID].m_flags | NetSegment.Flags.Invert);
				Vector3 endDirection2 = instance.m_segments.m_buffer[(int)segmentID].m_endDirection;
				instance.m_segments.m_buffer[(int)segmentID].m_endDirection = instance.m_segments.m_buffer[(int)segmentID].m_startDirection;
				instance.m_segments.m_buffer[(int)segmentID].m_startDirection = endDirection2;
			}

			if(!(info.m_lanes == null || ((layerMask & info.m_treeLayers) == 0 && !cameraInfo.CheckRenderDistance(data.m_position, info.m_maxPropDistance + 128f))))
			{
				bool invert2;
				NetNode.Flags startFlags;
				Color startColor;
				NetNode.Flags endFlags;
				Color endColor;
				if((instance.m_segments.m_buffer[(int)segmentID].m_flags & NetSegment.Flags.Invert) > NetSegment.Flags.None)
				{
					invert2 = true;
					instance.m_nodes.m_buffer[(int)instance.m_segments.m_buffer[(int)segmentID].m_endNode].Info.m_netAI.GetNodeState(
						instance.m_segments.m_buffer[(int)segmentID].m_endNode, ref instance.m_nodes.m_buffer[(int)instance.m_segments.m_buffer[(int)segmentID].m_endNode], segmentID, ref instance.m_segments.m_buffer[(int)segmentID], out startFlags, out startColor);
					//	GetNodeState(nodeID,									nodeData,																					segmentID, segmentData,										 flags,			 color);                    
					instance.m_nodes.m_buffer[(int)instance.m_segments.m_buffer[(int)segmentID].m_startNode].Info.m_netAI.GetNodeState(
						instance.m_segments.m_buffer[(int)segmentID].m_startNode, ref instance.m_nodes.m_buffer[(int)instance.m_segments.m_buffer[(int)segmentID].m_startNode], segmentID, ref instance.m_segments.m_buffer[(int)segmentID], out endFlags, out endColor);
					//	GetNodeState(nodeID,									  nodeData,																						segmentID, segmentData,										 flags,		   color);
				}
				else
				{
					invert2 = false;
					instance.m_nodes.m_buffer[(int)instance.m_segments.m_buffer[(int)segmentID].m_startNode].Info.m_netAI.GetNodeState(
						instance.m_segments.m_buffer[(int)segmentID].m_startNode, ref instance.m_nodes.m_buffer[(int)instance.m_segments.m_buffer[(int)segmentID].m_startNode], segmentID, ref instance.m_segments.m_buffer[(int)segmentID], out startFlags, out startColor);
					//	GetNodeState(nodeID,									  nodeData,																						segmentID, segmentData,										 flags,			 color);
					instance.m_nodes.m_buffer[(int)instance.m_segments.m_buffer[(int)segmentID].m_endNode].Info.m_netAI.GetNodeState(
						instance.m_segments.m_buffer[(int)segmentID].m_endNode, ref instance.m_nodes.m_buffer[(int)instance.m_segments.m_buffer[(int)segmentID].m_endNode], segmentID, ref instance.m_segments.m_buffer[(int)segmentID], out endFlags, out endColor);
					//	GetNodeState(nodeID,									nodeData,																				    segmentID, segmentData,										 flags,		   color);
				}
				float startAngle2 = (float)instance.m_segments.m_buffer[(int)segmentID].m_cornerAngleStart * 0.02454369f;
				float endAngle2 = (float)instance.m_segments.m_buffer[(int)segmentID].m_cornerAngleEnd * 0.02454369f;

				Vector4 objectIndex = new Vector4(data.m_dataVector3.x, data.m_dataVector3.y, 1f, data.m_dataFloat0);
				Vector4 objectIndex2 = new Vector4(data.m_dataVector3.z, data.m_dataVector3.w, 1f, data.m_dataFloat0);

				InfoManager.InfoMode currentMode = Singleton<InfoManager>.instance.CurrentMode;

				if(currentMode != InfoManager.InfoMode.None && !info.m_netAI.ColorizeProps(currentMode))
				{
					objectIndex.z = 0f;
					objectIndex2.z = 0f;
				}
				int propIndex_1a = (info.m_segments == null || info.m_segments.Length == 0) ? 0 : -1;
				uint laneID_1a = instance.m_segments.m_buffer[(int)segmentID].m_lanes;
				int laneInfo_1a = 0;
				while(laneInfo_1a < info.m_lanes.Length && laneID_1a > 0u)
				{
					//RenderInstance(cameraInfo,segmentID,laneID,laneInfo,startFlags,endFlags,startColor,endColor,startAngle,endAngle,invert,layerMask,objectIndex1,objectIndex2,data,propIndex);
					instance.m_lanes.m_buffer[(int)laneID_1a].RenderInstance(cameraInfo, segmentID, laneID_1a, info.m_lanes[laneInfo_1a], startFlags, endFlags, startColor, endColor, startAngle2, endAngle2, invert2, layerMask, objectIndex, objectIndex2, ref data, ref propIndex_1a);
					laneID_1a = instance.m_lanes.m_buffer[(int)laneID_1a].m_nextLane;
					//int num4=laneInfo_1a+1;
					laneInfo_1a += 1;
				}
			}
		}
	}
}