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
		public bool hookEnabled=false;
		private Dictionary<MethodInfo,RedirectCallsState> redirects=new Dictionary<MethodInfo,RedirectCallsState>();//create new dictionary
		//public static Material invertedBridgeMat;
		private int CalculateRendererCount(NetInfo info)
		{
			if((this.m_flags&NetNode.Flags.Junction)!=NetNode.Flags.None)
			{
				int num=(int)this.m_connectCount;
				if(info.m_requireSegmentRenderers)
				{
					num+=this.CountSegments();
				}
				return num;
			}
			return 1;
		}
		// NetNode
		private void RefreshBendData(ushort nodeID,NetInfo info,uint instanceIndex,ref RenderManager.Instance data)
		{
			data.m_position=this.m_position;
			data.m_rotation=Quaternion.identity;
			data.m_initialized=true;
			float vScale=info.m_netAI.GetVScale();
			Vector3 zero=Vector3.zero;
			Vector3 zero2=Vector3.zero;
			Vector3 zero3=Vector3.zero;
			Vector3 zero4=Vector3.zero;
			Vector3 zero5=Vector3.zero;
			Vector3 zero6=Vector3.zero;
			Vector3 zero7=Vector3.zero;
			Vector3 zero8=Vector3.zero;
			int num=0;
			int num2=0;
			bool flag=false;
			int num3=0;
			for(int i=0;i<8;i++)
			{
				ushort segment=this.GetSegment(i);
				if(segment!=0)
				{
					NetSegment netSegment=Singleton<NetManager>.instance.m_segments.m_buffer[(int)segment];
					bool flag2=++num3==1;
					bool flag3=netSegment.m_startNode==nodeID;
					if((!flag2&&!flag)||(flag2&&!flag3))
					{
						bool flag4;
						netSegment.CalculateCorner(segment,true,flag3,false,out zero,out zero5,out flag4);
						netSegment.CalculateCorner(segment,true,flag3,true,out zero2,out zero6,out flag4);
						flag=true;
						num=i;
					}
					else
					{
						bool flag4;
						netSegment.CalculateCorner(segment,true,flag3,true,out zero3,out zero7,out flag4);
						netSegment.CalculateCorner(segment,true,flag3,false,out zero4,out zero8,out flag4);
						num2=i;
					}
				}
			}
			Vector3 vector;
			Vector3 vector2;
			NetSegment.CalculateMiddlePoints(zero,-zero5,zero3,-zero7,true,true,out vector,out vector2);
			Vector3 vector3;
			Vector3 vector4;
			NetSegment.CalculateMiddlePoints(zero2,-zero6,zero4,-zero8,true,true,out vector3,out vector4);
			data.m_dataMatrix0=NetSegment.CalculateControlMatrix(zero,vector,vector2,zero3,zero2,vector3,vector4,zero4,this.m_position,vScale);
			data.m_extraData.m_dataMatrix2=NetSegment.CalculateControlMatrix(zero2,vector3,vector4,zero4,zero,vector,vector2,zero3,this.m_position,vScale);
			data.m_dataVector0=new Vector4(0.5f/info.m_halfWidth,1f/info.m_segmentLength,1f,1f);
			Vector4 colorLocation=RenderManager.GetColorLocation(86016u+(uint)nodeID);
			data.m_dataVector3=new Vector4(colorLocation.x,colorLocation.y,colorLocation.x,colorLocation.y);
			data.m_dataColor0=info.m_color;
			data.m_dataColor0.a=0f;
			data.m_dataFloat0=Singleton<WeatherManager>.instance.GetWindSpeed(data.m_position);
			data.m_dataInt0=(num|num2<<4);
			if(info.m_requireSurfaceMaps)
			{
				Singleton<TerrainManager>.instance.GetSurfaceMapping(data.m_position,out data.m_dataTexture0,out data.m_dataTexture1,out data.m_dataVector1);
			}
		}
		// NetNode
		private void RefreshJunctionData(ushort nodeID,NetInfo info,uint instanceIndex)
		{
			NetManager instance=Singleton<NetManager>.instance;
			Vector3 vector=this.m_position;
			for(int i=0;i<8;i++)
			{
				ushort segment=this.GetSegment(i);
				if(segment!=0)
				{
					NetInfo info2=instance.m_segments.m_buffer[(int)segment].Info;
					ItemClass connectionClass=info2.GetConnectionClass();
					Vector3 a=(nodeID!=instance.m_segments.m_buffer[(int)segment].m_startNode)?instance.m_segments.m_buffer[(int)segment].m_endDirection:instance.m_segments.m_buffer[(int)segment].m_startDirection;
					float num=-1f;
					for(int j=0;j<8;j++)
					{
						ushort segment2=this.GetSegment(j);
						if(segment2!=0&&segment2!=segment)
						{
							NetInfo info3=instance.m_segments.m_buffer[(int)segment2].Info;
							ItemClass connectionClass2=info3.GetConnectionClass();
							if(connectionClass.m_service==connectionClass2.m_service)
							{
								Vector3 vector2=(nodeID!=instance.m_segments.m_buffer[(int)segment2].m_startNode)?instance.m_segments.m_buffer[(int)segment2].m_endDirection:instance.m_segments.m_buffer[(int)segment2].m_startDirection;
								float num2=a.x*vector2.x+a.z*vector2.z;
								num=Mathf.Max(num,num2);
								bool flag=info2.m_requireDirectRenderers&&(info2.m_nodeConnectGroups==NetInfo.ConnectGroup.None||(info2.m_nodeConnectGroups&info3.m_connectGroup)!=NetInfo.ConnectGroup.None);
								bool flag2=info3.m_requireDirectRenderers&&(info3.m_nodeConnectGroups==NetInfo.ConnectGroup.None||(info3.m_nodeConnectGroups&info2.m_connectGroup)!=NetInfo.ConnectGroup.None);
								if(j>i&&(flag||flag2))
								{
									float num3=0.01f-Mathf.Min(info2.m_maxTurnAngleCos,info3.m_maxTurnAngleCos);
									if(num2<num3&&instanceIndex!=65535u)
									{
										float num4;
										if(flag)
										{
											num4=info2.m_netAI.GetNodeInfoPriority(segment,ref instance.m_segments.m_buffer[(int)segment]);
										}
										else
										{
											num4=-1E+08f;
										}
										float num5;
										if(flag2)
										{
											num5=info3.m_netAI.GetNodeInfoPriority(segment2,ref instance.m_segments.m_buffer[(int)segment2]);
										}
										else
										{
											num5=-1E+08f;
										}
										if(num4>=num5)
										{
											this.RefreshJunctionData(nodeID,i,j,info2,info3,segment,segment2,ref instanceIndex,ref Singleton<RenderManager>.instance.m_instances[(int)((UIntPtr)instanceIndex)]);
										}
										else
										{
											this.RefreshJunctionData(nodeID,j,i,info3,info2,segment2,segment,ref instanceIndex,ref Singleton<RenderManager>.instance.m_instances[(int)((UIntPtr)instanceIndex)]);
										}
									}
								}
							}
						}
					}
					vector+=a*(2f+num*2f);
				}
			}
			vector.y=this.m_position.y+(float)this.m_heightOffset*0.015625f;
			if(info.m_requireSegmentRenderers)
			{
				for(int k=0;k<8;k++)
				{
					ushort segment3=this.GetSegment(k);
					if(segment3!=0&&instanceIndex!=65535u)
					{
						this.RefreshJunctionData(nodeID,k,segment3,vector,ref instanceIndex,ref Singleton<RenderManager>.instance.m_instances[(int)((UIntPtr)instanceIndex)]);
					}
				}
			}
		}
		// NetNode
		private void RefreshJunctionData(NetNode netnode,ushort nodeID,int segmentIndex,int segmentIndex2,NetInfo info,NetInfo info2,ushort nodeSegment,ushort nodeSegment2,ref uint instanceIndex,ref RenderManager.Instance data)
		{
			data.m_position=netnode.m_position;
			data.m_rotation=Quaternion.identity;
			data.m_initialized=true;
			float vScale=info.m_netAI.GetVScale();
			Vector3 zero=Vector3.zero;
			Vector3 zero2=Vector3.zero;
			Vector3 vector=Vector3.zero;
			Vector3 vector2=Vector3.zero;
			Vector3 zero3=Vector3.zero;
			Vector3 zero4=Vector3.zero;
			Vector3 zero5=Vector3.zero;
			Vector3 zero6=Vector3.zero;
			bool start=Singleton<NetManager>.instance.m_segments.m_buffer[(int)nodeSegment].m_startNode==nodeID;
			bool flag;
			Singleton<NetManager>.instance.m_segments.m_buffer[(int)nodeSegment].CalculateCorner(nodeSegment,true,start,false,out zero,out zero3,out flag);
			Singleton<NetManager>.instance.m_segments.m_buffer[(int)nodeSegment].CalculateCorner(nodeSegment,true,start,true,out zero2,out zero4,out flag);
			start=(Singleton<NetManager>.instance.m_segments.m_buffer[(int)nodeSegment2].m_startNode==nodeID);
			Singleton<NetManager>.instance.m_segments.m_buffer[(int)nodeSegment2].CalculateCorner(nodeSegment2,true,start,true,out vector,out zero5,out flag);
			Singleton<NetManager>.instance.m_segments.m_buffer[(int)nodeSegment2].CalculateCorner(nodeSegment2,true,start,false,out vector2,out zero6,out flag);
			Vector3 b=(vector2-vector)*(info.m_halfWidth/info2.m_halfWidth*0.5f-0.5f);
			vector-=b;
			vector2+=b;
			Vector3 vector3;
			Vector3 vector4;
			NetSegment.CalculateMiddlePoints(zero,-zero3,vector,-zero5,true,true,out vector3,out vector4);
			Vector3 vector5;
			Vector3 vector6;
			NetSegment.CalculateMiddlePoints(zero2,-zero4,vector2,-zero6,true,true,out vector5,out vector6);
			data.m_dataMatrix0=NetSegment.CalculateControlMatrix(zero,vector3,vector4,vector,zero2,vector5,vector6,vector2,netnode.m_position,vScale);
			data.m_extraData.m_dataMatrix2=NetSegment.CalculateControlMatrix(zero2,vector5,vector6,vector2,zero,vector3,vector4,vector,netnode.m_position,vScale);
			data.m_dataVector0=new Vector4(0.5f/info.m_halfWidth,1f/info.m_segmentLength,1f,1f);
			Vector4 colorLocation;
			Vector4 vector7;
			if(NetNode.BlendJunction(nodeID))
			{
				colorLocation=RenderManager.GetColorLocation(86016u+(uint)nodeID);
				vector7=colorLocation;
			}
			else
			{
				colorLocation=RenderManager.GetColorLocation((uint)(49152+nodeSegment));
				vector7=RenderManager.GetColorLocation((uint)(49152+nodeSegment2));
			}
			data.m_dataVector3=new Vector4(colorLocation.x,colorLocation.y,vector7.x,vector7.y);
			data.m_dataInt0=(8|segmentIndex|segmentIndex2<<4);
			data.m_dataColor0=info.m_color;
			data.m_dataColor0.a=0f;
			data.m_dataFloat0=Singleton<WeatherManager>.instance.GetWindSpeed(data.m_position);
			if(info.m_requireSurfaceMaps)
			{
				Singleton<TerrainManager>.instance.GetSurfaceMapping(data.m_position,out data.m_dataTexture0,out data.m_dataTexture1,out data.m_dataVector1);
			}
			instanceIndex=(uint)data.m_nextInstance;
		}
		// NetNode
		private void RefreshJunctionData(NetNode netnode,ushort nodeID,int segmentIndex,ushort nodeSegment,Vector3 centerPos,ref uint instanceIndex,ref RenderManager.Instance data)
		{
			NetManager instance=Singleton<NetManager>.instance;
			data.m_position=netnode.m_position;
			data.m_rotation=Quaternion.identity;
			data.m_initialized=true;
			Vector3 zero=Vector3.zero;
			Vector3 zero2=Vector3.zero;
			Vector3 zero3=Vector3.zero;
			Vector3 zero4=Vector3.zero;
			Vector3 vector=Vector3.zero;
			Vector3 vector2=Vector3.zero;
			Vector3 a=Vector3.zero;
			Vector3 a2=Vector3.zero;
			Vector3 zero5=Vector3.zero;
			Vector3 zero6=Vector3.zero;
			Vector3 zero7=Vector3.zero;
			Vector3 zero8=Vector3.zero;
			NetSegment netSegment=instance.m_segments.m_buffer[(int)nodeSegment];
			NetInfo info=netSegment.Info;
			float vScale=info.m_netAI.GetVScale();
			ItemClass connectionClass=info.GetConnectionClass();
			Vector3 vector3=(nodeID!=netSegment.m_startNode)?netSegment.m_endDirection:netSegment.m_startDirection;
			float num=-4f;
			float num2=-4f;
			ushort num3=0;
			ushort num4=0;
			for(int i=0;i<8;i++)
			{
				ushort segment=netnode.GetSegment(i);
				if(segment!=0&&segment!=nodeSegment)
				{
					NetInfo info2=instance.m_segments.m_buffer[(int)segment].Info;
					ItemClass connectionClass2=info2.GetConnectionClass();
					if(connectionClass.m_service==connectionClass2.m_service)
					{
						NetSegment netSegment2=instance.m_segments.m_buffer[(int)segment];
						Vector3 vector4=(nodeID!=netSegment2.m_startNode)?netSegment2.m_endDirection:netSegment2.m_startDirection;
						float num5=vector3.x*vector4.x+vector3.z*vector4.z;
						if(vector4.z*vector3.x-vector4.x*vector3.z<0f)
						{
							if(num5>num)
							{
								num=num5;
								num3=segment;
							}
							num5=-2f-num5;
							if(num5>num2)
							{
								num2=num5;
								num4=segment;
							}
						}
						else
						{
							if(num5>num2)
							{
								num2=num5;
								num4=segment;
							}
							num5=-2f-num5;
							if(num5>num)
							{
								num=num5;
								num3=segment;
							}
						}
					}
				}
			}
			bool start=netSegment.m_startNode==nodeID;
			bool flag;
			netSegment.CalculateCorner(nodeSegment,true,start,false,out zero,out zero3,out flag);
			netSegment.CalculateCorner(nodeSegment,true,start,true,out zero2,out zero4,out flag);
			if(num3!=0&&num4!=0)
			{
				float num6=info.m_pavementWidth/info.m_halfWidth*0.5f;
				float y=1f;
				if(num3!=0)
				{
					NetSegment netSegment3=instance.m_segments.m_buffer[(int)num3];
					NetInfo info3=netSegment3.Info;
					start=(netSegment3.m_startNode==nodeID);
					netSegment3.CalculateCorner(num3,true,start,true,out vector,out a,out flag);
					netSegment3.CalculateCorner(num3,true,start,false,out vector2,out a2,out flag);
					float num7=info3.m_pavementWidth/info3.m_halfWidth*0.5f;
					num6=(num6+num7)*0.5f;
					y=2f*info.m_halfWidth/(info.m_halfWidth+info3.m_halfWidth);
				}
				float num8=info.m_pavementWidth/info.m_halfWidth*0.5f;
				float w=1f;
				if(num4!=0)
				{
					NetSegment netSegment4=instance.m_segments.m_buffer[(int)num4];
					NetInfo info4=netSegment4.Info;
					start=(netSegment4.m_startNode==nodeID);
					netSegment4.CalculateCorner(num4,true,start,true,out zero5,out zero7,out flag);
					netSegment4.CalculateCorner(num4,true,start,false,out zero6,out zero8,out flag);
					float num9=info4.m_pavementWidth/info4.m_halfWidth*0.5f;
					num8=(num8+num9)*0.5f;
					w=2f*info.m_halfWidth/(info.m_halfWidth+info4.m_halfWidth);
				}
				Vector3 vector5;
				Vector3 vector6;
				NetSegment.CalculateMiddlePoints(zero,-zero3,vector,-a,true,true,out vector5,out vector6);
				Vector3 vector7;
				Vector3 vector8;
				NetSegment.CalculateMiddlePoints(zero2,-zero4,vector2,-a2,true,true,out vector7,out vector8);
				Vector3 vector9;
				Vector3 vector10;
				NetSegment.CalculateMiddlePoints(zero,-zero3,zero5,-zero7,true,true,out vector9,out vector10);
				Vector3 vector11;
				Vector3 vector12;
				NetSegment.CalculateMiddlePoints(zero2,-zero4,zero6,-zero8,true,true,out vector11,out vector12);
				data.m_dataMatrix0=NetSegment.CalculateControlMatrix(zero,vector5,vector6,vector,zero,vector5,vector6,vector,netnode.m_position,vScale);
				data.m_extraData.m_dataMatrix2=NetSegment.CalculateControlMatrix(zero2,vector7,vector8,vector2,zero2,vector7,vector8,vector2,netnode.m_position,vScale);
				data.m_extraData.m_dataMatrix3=NetSegment.CalculateControlMatrix(zero,vector9,vector10,zero5,zero,vector9,vector10,zero5,netnode.m_position,vScale);
				data.m_dataMatrix1=NetSegment.CalculateControlMatrix(zero2,vector11,vector12,zero6,zero2,vector11,vector12,zero6,netnode.m_position,vScale);
				data.m_dataVector0=new Vector4(0.5f/info.m_halfWidth,1f/info.m_segmentLength,0.5f-info.m_pavementWidth/info.m_halfWidth*0.5f,info.m_pavementWidth/info.m_halfWidth*0.5f);
				data.m_dataVector1=centerPos-data.m_position;
				data.m_dataVector1.w=(data.m_dataMatrix0.m33+data.m_extraData.m_dataMatrix2.m33+data.m_extraData.m_dataMatrix3.m33+data.m_dataMatrix1.m33)*0.25f;
				data.m_dataVector2=new Vector4(num6,y,num8,w);
			}
			else
			{
				centerPos.x=(zero.x+zero2.x)*0.5f;
				centerPos.z=(zero.z+zero2.z)*0.5f;
				vector=zero2;
				vector2=zero;
				a=zero4;
				a2=zero3;
				float d=Mathf.Min(info.m_halfWidth*1.33333337f,16f);
				Vector3 vector13=zero-zero3*d;
				Vector3 vector14=vector-a*d;
				Vector3 vector15=zero2-zero4*d;
				Vector3 vector16=vector2-a2*d;
				Vector3 vector17=zero+zero3*d;
				Vector3 vector18=vector+a*d;
				Vector3 vector19=zero2+zero4*d;
				Vector3 vector20=vector2+a2*d;
				data.m_dataMatrix0=NetSegment.CalculateControlMatrix(zero,vector13,vector14,vector,zero,vector13,vector14,vector,netnode.m_position,vScale);
				data.m_extraData.m_dataMatrix2=NetSegment.CalculateControlMatrix(zero2,vector19,vector20,vector2,zero2,vector19,vector20,vector2,netnode.m_position,vScale);
				data.m_extraData.m_dataMatrix3=NetSegment.CalculateControlMatrix(zero,vector17,vector18,vector,zero,vector17,vector18,vector,netnode.m_position,vScale);
				data.m_dataMatrix1=NetSegment.CalculateControlMatrix(zero2,vector15,vector16,vector2,zero2,vector15,vector16,vector2,netnode.m_position,vScale);
				data.m_dataMatrix0.SetRow(3,data.m_dataMatrix0.GetRow(3)+new Vector4(0.2f,0.2f,0.2f,0.2f));
				data.m_extraData.m_dataMatrix2.SetRow(3,data.m_extraData.m_dataMatrix2.GetRow(3)+new Vector4(0.2f,0.2f,0.2f,0.2f));
				data.m_extraData.m_dataMatrix3.SetRow(3,data.m_extraData.m_dataMatrix3.GetRow(3)+new Vector4(0.2f,0.2f,0.2f,0.2f));
				data.m_dataMatrix1.SetRow(3,data.m_dataMatrix1.GetRow(3)+new Vector4(0.2f,0.2f,0.2f,0.2f));
				data.m_dataVector0=new Vector4(0.5f/info.m_halfWidth,1f/info.m_segmentLength,0.5f-info.m_pavementWidth/info.m_halfWidth*0.5f,info.m_pavementWidth/info.m_halfWidth*0.5f);
				data.m_dataVector1=centerPos-data.m_position;
				data.m_dataVector1.w=(data.m_dataMatrix0.m33+data.m_extraData.m_dataMatrix2.m33+data.m_extraData.m_dataMatrix3.m33+data.m_dataMatrix1.m33)*0.25f;
				data.m_dataVector2=new Vector4(info.m_pavementWidth/info.m_halfWidth*0.5f,1f,info.m_pavementWidth/info.m_halfWidth*0.5f,1f);
			}
			Vector4 colorLocation;
			Vector4 vector21;
			if(NetNode.BlendJunction(nodeID))
			{
				colorLocation=RenderManager.GetColorLocation(86016u+(uint)nodeID);
				vector21=colorLocation;
			}
			else
			{
				colorLocation=RenderManager.GetColorLocation((uint)(49152+nodeSegment));
				vector21=RenderManager.GetColorLocation(86016u+(uint)nodeID);
			}
			data.m_extraData.m_dataVector4=new Vector4(colorLocation.x,colorLocation.y,vector21.x,vector21.y);
			data.m_dataInt0=segmentIndex;
			data.m_dataColor0=info.m_color;
			data.m_dataColor0.a=0f;
			data.m_dataFloat0=Singleton<WeatherManager>.instance.GetWindSpeed(data.m_position);
			if(info.m_requireSurfaceMaps)
			{
				Singleton<TerrainManager>.instance.GetSurfaceMapping(data.m_position,out data.m_dataTexture0,out data.m_dataTexture1,out data.m_dataVector3);
			}
			instanceIndex=(uint)data.m_nextInstance;
		}
		// NetNode
		public void RenderInstance(RenderManager.CameraInfo cameraInfo,ushort nodeID,int layerMask)
		{
			if(this.m_flags==NetNode.Flags.None)
			{
				return;
			}
			NetInfo info=this.Info;
			if(!cameraInfo.Intersect(this.m_bounds))
			{
				return;
			}
			if(this.m_problems!=Notification.Problem.None&&(layerMask&1<<Singleton<NotificationManager>.instance.m_notificationLayer)!=0)
			{
				Vector3 position=this.m_position;
				position.y+=Mathf.Max(5f,info.m_maxHeight);
				Notification.RenderInstance(cameraInfo,this.m_problems,position,1f);
			}
			if((layerMask&info.m_netLayers)==0)
			{
				return;
			}
			if((this.m_flags&(NetNode.Flags.End|NetNode.Flags.Bend|NetNode.Flags.Junction))==NetNode.Flags.None)
			{
				return;
			}
			if((this.m_flags&NetNode.Flags.Bend)!=NetNode.Flags.None)
			{
				if(info.m_segments==null||info.m_segments.Length==0)
				{
					return;
				}
			}
			else if(info.m_nodes==null||info.m_nodes.Length==0)
			{
				return;
			}
			uint count=(uint)this.CalculateRendererCount(info);
			RenderManager instance=Singleton<RenderManager>.instance;
			uint num;
			if(instance.RequireInstance(86016u+(uint)nodeID,count,out num))
			{
				int num2=0;
				while(num!=65535u)
				{
					this.RenderInstance(cameraInfo,nodeID,info,num2,this.m_flags,ref num,ref instance.m_instances[(int)((UIntPtr)num)]);
					if(++num2>36)
					{
						CODebugBase<LogChannel>.Error(LogChannel.Core,"Invalid list detected!\n"+Environment.StackTrace);
						break;
					}
				}
			}
		}
		// NetNode
		private void RenderInstance(RenderManager.CameraInfo cameraInfo,ushort nodeID,NetInfo info,int iter,NetNode.Flags flags,ref uint instanceIndex,ref RenderManager.Instance data)
		{
			if(data.m_dirty)
			{
				data.m_dirty=false;
				if(iter==0)
				{
					if((flags&NetNode.Flags.Junction)!=NetNode.Flags.None)
					{
						this.RefreshJunctionData(nodeID,info,instanceIndex);
					}
					else if((flags&NetNode.Flags.Bend)!=NetNode.Flags.None)
					{
						this.RefreshBendData(nodeID,info,instanceIndex,ref data);
					}
					else if((flags&NetNode.Flags.End)!=NetNode.Flags.None)
					{
						this.RefreshEndData(nodeID,info,instanceIndex,ref data);
					}
				}
			}
			if(data.m_initialized)
			{
				if((flags&NetNode.Flags.Junction)!=NetNode.Flags.None)
				{
					if((data.m_dataInt0&8)!=0)
					{
						ushort segment=this.GetSegment(data.m_dataInt0&7);
						ushort segment2=this.GetSegment(data.m_dataInt0>>4);
						if(segment!=0&&segment2!=0)
						{
							NetManager instance=Singleton<NetManager>.instance;
							info=instance.m_segments.m_buffer[(int)segment].Info;
							NetInfo info2=instance.m_segments.m_buffer[(int)segment2].Info;
							for(int i=0;i<info.m_nodes.Length;i++)
							{
								NetInfo.Node node=info.m_nodes[i];
								if(node.CheckFlags(flags)&&node.m_directConnect&&(node.m_connectGroup==NetInfo.ConnectGroup.None||(node.m_connectGroup&info2.m_connectGroup&NetInfo.ConnectGroup.AllGroups)!=NetInfo.ConnectGroup.None))
								{
									Vector4 dataVector=data.m_dataVector3;
									Vector4 dataVector2=data.m_dataVector0;
									if(node.m_requireWindSpeed)
									{
										dataVector.w=data.m_dataFloat0;
									}
									if((node.m_connectGroup&NetInfo.ConnectGroup.Oneway)!=NetInfo.ConnectGroup.None)
									{
										bool flag=instance.m_segments.m_buffer[(int)segment].m_startNode==nodeID==((instance.m_segments.m_buffer[(int)segment].m_flags&NetSegment.Flags.Invert)==NetSegment.Flags.None);
										if(info2.m_hasBackwardVehicleLanes!=info2.m_hasForwardVehicleLanes)
										{
											bool flag2=instance.m_segments.m_buffer[(int)segment2].m_startNode==nodeID==((instance.m_segments.m_buffer[(int)segment2].m_flags&NetSegment.Flags.Invert)==NetSegment.Flags.None);
											if(flag==flag2)
											{
												goto IL_51C;
											}
										}
										if(flag)
										{
											if((node.m_connectGroup&NetInfo.ConnectGroup.OnewayStart)==NetInfo.ConnectGroup.None)
											{
												goto IL_51C;
											}
										}
										else
										{
											if((node.m_connectGroup&NetInfo.ConnectGroup.OnewayEnd)==NetInfo.ConnectGroup.None)
											{
												goto IL_51C;
											}
											dataVector2.x=-dataVector2.x;
											dataVector2.y=-dataVector2.y;
										}
									}
									if(cameraInfo.CheckRenderDistance(data.m_position,node.m_lodRenderDistance))
									{
										instance.m_materialBlock.Clear();
										instance.m_materialBlock.AddMatrix(instance.ID_LeftMatrix,data.m_dataMatrix0);
										instance.m_materialBlock.AddMatrix(instance.ID_RightMatrix,data.m_extraData.m_dataMatrix2);
										instance.m_materialBlock.AddVector(instance.ID_MeshScale,dataVector2);
										instance.m_materialBlock.AddVector(instance.ID_ObjectIndex,dataVector);
										instance.m_materialBlock.AddColor(instance.ID_Color,data.m_dataColor0);
										if(node.m_requireSurfaceMaps&&data.m_dataTexture1!=null)
										{
											instance.m_materialBlock.AddTexture(instance.ID_SurfaceTexA,data.m_dataTexture0);
											instance.m_materialBlock.AddTexture(instance.ID_SurfaceTexB,data.m_dataTexture1);
											instance.m_materialBlock.AddVector(instance.ID_SurfaceMapping,data.m_dataVector1);
										}
										NetManager expr_36F_cp_0=instance;
										expr_36F_cp_0.m_drawCallData.m_defaultCalls=expr_36F_cp_0.m_drawCallData.m_defaultCalls+1;
										Graphics.DrawMesh(node.m_nodeMesh,data.m_position,data.m_rotation,node.m_nodeMaterial,node.m_layer,null,0,instance.m_materialBlock);
									}
									else
									{
										NetInfo.LodValue combinedLod=node.m_combinedLod;
										if(combinedLod!=null)
										{
											if(node.m_requireSurfaceMaps&&data.m_dataTexture0!=combinedLod.m_surfaceTexA)
											{
												if(combinedLod.m_lodCount!=0)
												{
													NetSegment.RenderLod(cameraInfo,combinedLod);
												}
												combinedLod.m_surfaceTexA=data.m_dataTexture0;
												combinedLod.m_surfaceTexB=data.m_dataTexture1;
												combinedLod.m_surfaceMapping=data.m_dataVector1;
											}
											combinedLod.m_leftMatrices[combinedLod.m_lodCount]=data.m_dataMatrix0;
											combinedLod.m_rightMatrices[combinedLod.m_lodCount]=data.m_extraData.m_dataMatrix2;
											combinedLod.m_meshScales[combinedLod.m_lodCount]=dataVector2;
											combinedLod.m_objectIndices[combinedLod.m_lodCount]=dataVector;
											combinedLod.m_meshLocations[combinedLod.m_lodCount]=data.m_position;
											combinedLod.m_lodMin=Vector3.Min(combinedLod.m_lodMin,data.m_position);
											combinedLod.m_lodMax=Vector3.Max(combinedLod.m_lodMax,data.m_position);
											if(++combinedLod.m_lodCount==combinedLod.m_leftMatrices.Length)
											{
												NetSegment.RenderLod(cameraInfo,combinedLod);
											}
										}
									}
								}
							IL_51C: ;
							}
						}
					}
					else
					{
						ushort segment3=this.GetSegment(data.m_dataInt0&7);
						if(segment3!=0)
						{
							NetManager instance2=Singleton<NetManager>.instance;
							info=instance2.m_segments.m_buffer[(int)segment3].Info;
							for(int j=0;j<info.m_nodes.Length;j++)
							{
								NetInfo.Node node2=info.m_nodes[j];
								if(node2.CheckFlags(flags)&&!node2.m_directConnect)
								{
									Vector4 dataVector3=data.m_extraData.m_dataVector4;
									if(node2.m_requireWindSpeed)
									{
										dataVector3.w=data.m_dataFloat0;
									}
									if(cameraInfo.CheckRenderDistance(data.m_position,node2.m_lodRenderDistance))
									{
										instance2.m_materialBlock.Clear();
										instance2.m_materialBlock.AddMatrix(instance2.ID_LeftMatrix,data.m_dataMatrix0);
										instance2.m_materialBlock.AddMatrix(instance2.ID_RightMatrix,data.m_extraData.m_dataMatrix2);
										instance2.m_materialBlock.AddMatrix(instance2.ID_LeftMatrixB,data.m_extraData.m_dataMatrix3);
										instance2.m_materialBlock.AddMatrix(instance2.ID_RightMatrixB,data.m_dataMatrix1);
										instance2.m_materialBlock.AddVector(instance2.ID_MeshScale,data.m_dataVector0);
										instance2.m_materialBlock.AddVector(instance2.ID_CenterPos,data.m_dataVector1);
										instance2.m_materialBlock.AddVector(instance2.ID_SideScale,data.m_dataVector2);
										instance2.m_materialBlock.AddVector(instance2.ID_ObjectIndex,dataVector3);
										instance2.m_materialBlock.AddColor(instance2.ID_Color,data.m_dataColor0);
										if(node2.m_requireSurfaceMaps&&data.m_dataTexture1!=null)
										{
											instance2.m_materialBlock.AddTexture(instance2.ID_SurfaceTexA,data.m_dataTexture0);
											instance2.m_materialBlock.AddTexture(instance2.ID_SurfaceTexB,data.m_dataTexture1);
											instance2.m_materialBlock.AddVector(instance2.ID_SurfaceMapping,data.m_dataVector3);
										}
										NetManager expr_74B_cp_0=instance2;
										expr_74B_cp_0.m_drawCallData.m_defaultCalls=expr_74B_cp_0.m_drawCallData.m_defaultCalls+1;
										Graphics.DrawMesh(node2.m_nodeMesh,data.m_position,data.m_rotation,node2.m_nodeMaterial,node2.m_layer,null,0,instance2.m_materialBlock);
									}
									else
									{
										NetInfo.LodValue combinedLod2=node2.m_combinedLod;
										if(combinedLod2!=null)
										{
											if(node2.m_requireSurfaceMaps&&data.m_dataTexture0!=combinedLod2.m_surfaceTexA)
											{
												if(combinedLod2.m_lodCount!=0)
												{
													NetNode.RenderLod(cameraInfo,combinedLod2);
												}
												combinedLod2.m_surfaceTexA=data.m_dataTexture0;
												combinedLod2.m_surfaceTexB=data.m_dataTexture1;
												combinedLod2.m_surfaceMapping=data.m_dataVector3;
											}
											combinedLod2.m_leftMatrices[combinedLod2.m_lodCount]=data.m_dataMatrix0;
											combinedLod2.m_leftMatricesB[combinedLod2.m_lodCount]=data.m_extraData.m_dataMatrix3;
											combinedLod2.m_rightMatrices[combinedLod2.m_lodCount]=data.m_extraData.m_dataMatrix2;
											combinedLod2.m_rightMatricesB[combinedLod2.m_lodCount]=data.m_dataMatrix1;
											combinedLod2.m_meshScales[combinedLod2.m_lodCount]=data.m_dataVector0;
											combinedLod2.m_centerPositions[combinedLod2.m_lodCount]=data.m_dataVector1;
											combinedLod2.m_sideScales[combinedLod2.m_lodCount]=data.m_dataVector2;
											combinedLod2.m_objectIndices[combinedLod2.m_lodCount]=dataVector3;
											combinedLod2.m_meshLocations[combinedLod2.m_lodCount]=data.m_position;
											combinedLod2.m_lodMin=Vector3.Min(combinedLod2.m_lodMin,data.m_position);
											combinedLod2.m_lodMax=Vector3.Max(combinedLod2.m_lodMax,data.m_position);
											if(++combinedLod2.m_lodCount==combinedLod2.m_leftMatrices.Length)
											{
												NetNode.RenderLod(cameraInfo,combinedLod2);
											}
										}
									}
								}
							}
						}
					}
				}
				else if((flags&NetNode.Flags.End)!=NetNode.Flags.None)
				{
					NetManager instance3=Singleton<NetManager>.instance;
					for(int k=0;k<info.m_nodes.Length;k++)
					{
						NetInfo.Node node3=info.m_nodes[k];
						if(node3.CheckFlags(flags)&&!node3.m_directConnect)
						{
							Vector4 dataVector4=data.m_extraData.m_dataVector4;
							if(node3.m_requireWindSpeed)
							{
								dataVector4.w=data.m_dataFloat0;
							}
							if(cameraInfo.CheckRenderDistance(data.m_position,node3.m_lodRenderDistance))
							{
								instance3.m_materialBlock.Clear();
								instance3.m_materialBlock.AddMatrix(instance3.ID_LeftMatrix,data.m_dataMatrix0);
								instance3.m_materialBlock.AddMatrix(instance3.ID_RightMatrix,data.m_extraData.m_dataMatrix2);
								instance3.m_materialBlock.AddMatrix(instance3.ID_LeftMatrixB,data.m_extraData.m_dataMatrix3);
								instance3.m_materialBlock.AddMatrix(instance3.ID_RightMatrixB,data.m_dataMatrix1);
								instance3.m_materialBlock.AddVector(instance3.ID_MeshScale,data.m_dataVector0);
								instance3.m_materialBlock.AddVector(instance3.ID_CenterPos,data.m_dataVector1);
								instance3.m_materialBlock.AddVector(instance3.ID_SideScale,data.m_dataVector2);
								instance3.m_materialBlock.AddVector(instance3.ID_ObjectIndex,dataVector4);
								instance3.m_materialBlock.AddColor(instance3.ID_Color,data.m_dataColor0);
								if(node3.m_requireSurfaceMaps&&data.m_dataTexture1!=null)
								{
									instance3.m_materialBlock.AddTexture(instance3.ID_SurfaceTexA,data.m_dataTexture0);
									instance3.m_materialBlock.AddTexture(instance3.ID_SurfaceTexB,data.m_dataTexture1);
									instance3.m_materialBlock.AddVector(instance3.ID_SurfaceMapping,data.m_dataVector3);
								}
								NetManager expr_B86_cp_0=instance3;
								expr_B86_cp_0.m_drawCallData.m_defaultCalls=expr_B86_cp_0.m_drawCallData.m_defaultCalls+1;
								Graphics.DrawMesh(node3.m_nodeMesh,data.m_position,data.m_rotation,node3.m_nodeMaterial,node3.m_layer,null,0,instance3.m_materialBlock);
							}
							else
							{
								NetInfo.LodValue combinedLod3=node3.m_combinedLod;
								if(combinedLod3!=null)
								{
									if(node3.m_requireSurfaceMaps&&data.m_dataTexture0!=combinedLod3.m_surfaceTexA)
									{
										if(combinedLod3.m_lodCount!=0)
										{
											NetNode.RenderLod(cameraInfo,combinedLod3);
										}
										combinedLod3.m_surfaceTexA=data.m_dataTexture0;
										combinedLod3.m_surfaceTexB=data.m_dataTexture1;
										combinedLod3.m_surfaceMapping=data.m_dataVector3;
									}
									combinedLod3.m_leftMatrices[combinedLod3.m_lodCount]=data.m_dataMatrix0;
									combinedLod3.m_leftMatricesB[combinedLod3.m_lodCount]=data.m_extraData.m_dataMatrix3;
									combinedLod3.m_rightMatrices[combinedLod3.m_lodCount]=data.m_extraData.m_dataMatrix2;
									combinedLod3.m_rightMatricesB[combinedLod3.m_lodCount]=data.m_dataMatrix1;
									combinedLod3.m_meshScales[combinedLod3.m_lodCount]=data.m_dataVector0;
									combinedLod3.m_centerPositions[combinedLod3.m_lodCount]=data.m_dataVector1;
									combinedLod3.m_sideScales[combinedLod3.m_lodCount]=data.m_dataVector2;
									combinedLod3.m_objectIndices[combinedLod3.m_lodCount]=dataVector4;
									combinedLod3.m_meshLocations[combinedLod3.m_lodCount]=data.m_position;
									combinedLod3.m_lodMin=Vector3.Min(combinedLod3.m_lodMin,data.m_position);
									combinedLod3.m_lodMax=Vector3.Max(combinedLod3.m_lodMax,data.m_position);
									if(++combinedLod3.m_lodCount==combinedLod3.m_leftMatrices.Length)
									{
										NetNode.RenderLod(cameraInfo,combinedLod3);
									}
								}
							}
						}
					}
				}
				else if((flags&NetNode.Flags.Bend)!=NetNode.Flags.None)
				{
					NetManager instance4=Singleton<NetManager>.instance;
					for(int l=0;l<info.m_segments.Length;l++)
					{
						NetInfo.Segment segment4=info.m_segments[l];
						bool flag3;
						if(segment4.CheckFlags(info.m_netAI.GetBendFlags(nodeID,ref this),out flag3)&&!segment4.m_disableBendNodes)
						{
							Vector4 dataVector5=data.m_dataVector3;
							if(segment4.m_requireWindSpeed)
							{
								dataVector5.w=data.m_dataFloat0;
							}
							if(cameraInfo.CheckRenderDistance(data.m_position,segment4.m_lodRenderDistance))
							{
								instance4.m_materialBlock.Clear();
								instance4.m_materialBlock.AddMatrix(instance4.ID_LeftMatrix,data.m_dataMatrix0);
								instance4.m_materialBlock.AddMatrix(instance4.ID_RightMatrix,data.m_extraData.m_dataMatrix2);
								instance4.m_materialBlock.AddVector(instance4.ID_MeshScale,data.m_dataVector0);
								instance4.m_materialBlock.AddVector(instance4.ID_ObjectIndex,dataVector5);
								instance4.m_materialBlock.AddColor(instance4.ID_Color,data.m_dataColor0);
								if(segment4.m_requireSurfaceMaps&&data.m_dataTexture1!=null)
								{
									instance4.m_materialBlock.AddTexture(instance4.ID_SurfaceTexA,data.m_dataTexture0);
									instance4.m_materialBlock.AddTexture(instance4.ID_SurfaceTexB,data.m_dataTexture1);
									instance4.m_materialBlock.AddVector(instance4.ID_SurfaceMapping,data.m_dataVector1);
								}
								NetManager expr_F5C_cp_0=instance4;
								expr_F5C_cp_0.m_drawCallData.m_defaultCalls=expr_F5C_cp_0.m_drawCallData.m_defaultCalls+1;
								Graphics.DrawMesh(segment4.m_segmentMesh,data.m_position,data.m_rotation,segment4.m_segmentMaterial,segment4.m_layer,null,0,instance4.m_materialBlock);
							}
							else
							{
								NetInfo.LodValue combinedLod4=segment4.m_combinedLod;
								if(combinedLod4!=null)
								{
									if(segment4.m_requireSurfaceMaps&&data.m_dataTexture0!=combinedLod4.m_surfaceTexA)
									{
										if(combinedLod4.m_lodCount!=0)
										{
											NetSegment.RenderLod(cameraInfo,combinedLod4);
										}
										combinedLod4.m_surfaceTexA=data.m_dataTexture0;
										combinedLod4.m_surfaceTexB=data.m_dataTexture1;
										combinedLod4.m_surfaceMapping=data.m_dataVector1;
									}
									combinedLod4.m_leftMatrices[combinedLod4.m_lodCount]=data.m_dataMatrix0;
									combinedLod4.m_rightMatrices[combinedLod4.m_lodCount]=data.m_extraData.m_dataMatrix2;
									combinedLod4.m_meshScales[combinedLod4.m_lodCount]=data.m_dataVector0;
									combinedLod4.m_objectIndices[combinedLod4.m_lodCount]=dataVector5;
									combinedLod4.m_meshLocations[combinedLod4.m_lodCount]=data.m_position;
									combinedLod4.m_lodMin=Vector3.Min(combinedLod4.m_lodMin,data.m_position);
									combinedLod4.m_lodMax=Vector3.Max(combinedLod4.m_lodMax,data.m_position);
									if(++combinedLod4.m_lodCount==combinedLod4.m_leftMatrices.Length)
									{
										NetSegment.RenderLod(cameraInfo,combinedLod4);
									}
								}
							}
						}
					}
					for(int m=0;m<info.m_nodes.Length;m++)
					{
						NetInfo.Node node4=info.m_nodes[m];
						if(node4.CheckFlags(flags)&&node4.m_directConnect&&(node4.m_connectGroup==NetInfo.ConnectGroup.None||(node4.m_connectGroup&info.m_connectGroup&NetInfo.ConnectGroup.AllGroups)!=NetInfo.ConnectGroup.None))
						{
							Vector4 dataVector6=data.m_dataVector3;
							Vector4 dataVector7=data.m_dataVector0;
							if(node4.m_requireWindSpeed)
							{
								dataVector6.w=data.m_dataFloat0;
							}
							if((node4.m_connectGroup&NetInfo.ConnectGroup.Oneway)!=NetInfo.ConnectGroup.None)
							{
								ushort segment5=this.GetSegment(data.m_dataInt0&7);
								ushort segment6=this.GetSegment(data.m_dataInt0>>4);
								bool flag4=instance4.m_segments.m_buffer[(int)segment5].m_startNode==nodeID==((instance4.m_segments.m_buffer[(int)segment5].m_flags&NetSegment.Flags.Invert)==NetSegment.Flags.None);
								bool flag5=instance4.m_segments.m_buffer[(int)segment6].m_startNode==nodeID==((instance4.m_segments.m_buffer[(int)segment6].m_flags&NetSegment.Flags.Invert)==NetSegment.Flags.None);
								if(flag4==flag5)
								{
									goto IL_1579;
								}
								if(flag4)
								{
									if((node4.m_connectGroup&NetInfo.ConnectGroup.OnewayStart)==NetInfo.ConnectGroup.None)
									{
										goto IL_1579;
									}
								}
								else
								{
									if((node4.m_connectGroup&NetInfo.ConnectGroup.OnewayEnd)==NetInfo.ConnectGroup.None)
									{
										goto IL_1579;
									}
									dataVector7.x=-dataVector7.x;
									dataVector7.y=-dataVector7.y;
								}
							}
							if(cameraInfo.CheckRenderDistance(data.m_position,node4.m_lodRenderDistance))
							{
								instance4.m_materialBlock.Clear();
								instance4.m_materialBlock.AddMatrix(instance4.ID_LeftMatrix,data.m_dataMatrix0);
								instance4.m_materialBlock.AddMatrix(instance4.ID_RightMatrix,data.m_extraData.m_dataMatrix2);
								instance4.m_materialBlock.AddVector(instance4.ID_MeshScale,dataVector7);
								instance4.m_materialBlock.AddVector(instance4.ID_ObjectIndex,dataVector6);
								instance4.m_materialBlock.AddColor(instance4.ID_Color,data.m_dataColor0);
								if(node4.m_requireSurfaceMaps&&data.m_dataTexture1!=null)
								{
									instance4.m_materialBlock.AddTexture(instance4.ID_SurfaceTexA,data.m_dataTexture0);
									instance4.m_materialBlock.AddTexture(instance4.ID_SurfaceTexB,data.m_dataTexture1);
									instance4.m_materialBlock.AddVector(instance4.ID_SurfaceMapping,data.m_dataVector1);
								}
								NetManager expr_13CB_cp_0=instance4;
								expr_13CB_cp_0.m_drawCallData.m_defaultCalls=expr_13CB_cp_0.m_drawCallData.m_defaultCalls+1;
								Graphics.DrawMesh(node4.m_nodeMesh,data.m_position,data.m_rotation,node4.m_nodeMaterial,node4.m_layer,null,0,instance4.m_materialBlock);
							}
							else
							{
								NetInfo.LodValue combinedLod5=node4.m_combinedLod;
								if(combinedLod5!=null)
								{
									if(node4.m_requireSurfaceMaps&&data.m_dataTexture0!=combinedLod5.m_surfaceTexA)
									{
										if(combinedLod5.m_lodCount!=0)
										{
											NetSegment.RenderLod(cameraInfo,combinedLod5);
										}
										combinedLod5.m_surfaceTexA=data.m_dataTexture0;
										combinedLod5.m_surfaceTexB=data.m_dataTexture1;
										combinedLod5.m_surfaceMapping=data.m_dataVector1;
									}
									combinedLod5.m_leftMatrices[combinedLod5.m_lodCount]=data.m_dataMatrix0;
									combinedLod5.m_rightMatrices[combinedLod5.m_lodCount]=data.m_extraData.m_dataMatrix2;
									combinedLod5.m_meshScales[combinedLod5.m_lodCount]=dataVector7;
									combinedLod5.m_objectIndices[combinedLod5.m_lodCount]=dataVector6;
									combinedLod5.m_meshLocations[combinedLod5.m_lodCount]=data.m_position;
									combinedLod5.m_lodMin=Vector3.Min(combinedLod5.m_lodMin,data.m_position);
									combinedLod5.m_lodMax=Vector3.Max(combinedLod5.m_lodMax,data.m_position);
									if(++combinedLod5.m_lodCount==combinedLod5.m_leftMatrices.Length)
									{
										NetSegment.RenderLod(cameraInfo,combinedLod5);
									}
								}
							}
						}
					IL_1579: ;
					}
				}
			}
			instanceIndex=(uint)data.m_nextInstance;
		}
		// NetSegment
		public void RenderInstance(RenderManager.CameraInfo cameraInfo,ushort segmentID,int layerMask)
		{
			if(this.m_flags==NetSegment.Flags.None)
			{
				return;
			}
			NetInfo info=this.Info;
			if(!cameraInfo.Intersect(this.m_bounds))
			{
				return;
			}
			if(this.m_problems!=Notification.Problem.None&&(layerMask&1<<Singleton<NotificationManager>.instance.m_notificationLayer)!=0)
			{
				Vector3 middlePosition=this.m_middlePosition;
				middlePosition.y+=Mathf.Max(5f,info.m_maxHeight);
				Notification.RenderInstance(cameraInfo,this.m_problems,middlePosition,1f);
			}
			if((layerMask&info.m_netLayers)==0)
			{
				return;
			}
			RenderManager instance=Singleton<RenderManager>.instance;
			uint num;
			if(instance.RequireInstance((uint)(49152+segmentID),1u,out num))
			{
				this.RenderInstance(cameraInfo,segmentID,layerMask,info,ref instance.m_instances[(int)((UIntPtr)num)]);
			}
		}
		// NetSegment
		private void RenderInstance(RenderManager.CameraInfo cameraInfo,ushort segmentID,int layerMask,NetInfo info,ref RenderManager.Instance data)
		{
			NetManager instance=Singleton<NetManager>.instance;
			if(data.m_dirty)
			{
				data.m_dirty=false;
				Vector3 position=instance.m_nodes.m_buffer[(int)this.m_startNode].m_position;
				Vector3 position2=instance.m_nodes.m_buffer[(int)this.m_endNode].m_position;
				data.m_position=(position+position2)*0.5f;
				data.m_rotation=Quaternion.identity;
				data.m_dataColor0=info.m_color;
				data.m_dataColor0.a=0f;
				data.m_dataFloat0=Singleton<WeatherManager>.instance.GetWindSpeed(data.m_position);
				data.m_dataVector0=new Vector4(0.5f/info.m_halfWidth,1f/info.m_segmentLength,1f,1f);
				Vector4 colorLocation=RenderManager.GetColorLocation((uint)(49152+segmentID));
				Vector4 vector=colorLocation;
				if(NetNode.BlendJunction(this.m_startNode))
				{
					colorLocation=RenderManager.GetColorLocation(86016u+(uint)this.m_startNode);
				}
				if(NetNode.BlendJunction(this.m_endNode))
				{
					vector=RenderManager.GetColorLocation(86016u+(uint)this.m_endNode);
				}
				data.m_dataVector3=new Vector4(colorLocation.x,colorLocation.y,vector.x,vector.y);
				if(info.m_segments==null||info.m_segments.Length==0)
				{
					if(info.m_lanes!=null)
					{
						bool invert;
						if((this.m_flags&NetSegment.Flags.Invert)!=NetSegment.Flags.None)
						{
							invert=true;
							NetInfo info2=instance.m_nodes.m_buffer[(int)this.m_endNode].Info;
							NetNode.Flags flags;
							Color color;
							info2.m_netAI.GetNodeState(this.m_endNode,ref instance.m_nodes.m_buffer[(int)this.m_endNode],segmentID,ref this,out flags,out color);
							NetInfo info3=instance.m_nodes.m_buffer[(int)this.m_startNode].Info;
							NetNode.Flags flags2;
							Color color2;
							info3.m_netAI.GetNodeState(this.m_startNode,ref instance.m_nodes.m_buffer[(int)this.m_startNode],segmentID,ref this,out flags2,out color2);
						}
						else
						{
							invert=false;
							NetInfo info4=instance.m_nodes.m_buffer[(int)this.m_startNode].Info;
							NetNode.Flags flags;
							Color color;
							info4.m_netAI.GetNodeState(this.m_startNode,ref instance.m_nodes.m_buffer[(int)this.m_startNode],segmentID,ref this,out flags,out color);
							NetInfo info5=instance.m_nodes.m_buffer[(int)this.m_endNode].Info;
							NetNode.Flags flags2;
							Color color2;
							info5.m_netAI.GetNodeState(this.m_endNode,ref instance.m_nodes.m_buffer[(int)this.m_endNode],segmentID,ref this,out flags2,out color2);
						}
						float startAngle=(float)this.m_cornerAngleStart*0.0245436933f;
						float endAngle=(float)this.m_cornerAngleEnd*0.0245436933f;
						int num=0;
						uint num2=this.m_lanes;
						int num3=0;
						while(num3<info.m_lanes.Length&&num2!=0u)
						{
							instance.m_lanes.m_buffer[(int)((UIntPtr)num2)].RefreshInstance(num2,info.m_lanes[num3],startAngle,endAngle,invert,ref data,ref num);
							num2=instance.m_lanes.m_buffer[(int)((UIntPtr)num2)].m_nextLane;
							num3++;
						}
					}
				}
				else
				{
					float vScale=info.m_netAI.GetVScale();
					Vector3 vector2;
					Vector3 startDir;
					bool smoothStart;
					this.CalculateCorner(segmentID,true,true,true,out vector2,out startDir,out smoothStart);
					Vector3 vector3;
					Vector3 endDir;
					bool smoothEnd;
					this.CalculateCorner(segmentID,true,false,true,out vector3,out endDir,out smoothEnd);
					Vector3 vector4;
					Vector3 startDir2;
					this.CalculateCorner(segmentID,true,true,false,out vector4,out startDir2,out smoothStart);
					Vector3 vector5;
					Vector3 endDir2;
					this.CalculateCorner(segmentID,true,false,false,out vector5,out endDir2,out smoothEnd);
					Vector3 vector6;
					Vector3 vector7;
					NetSegment.CalculateMiddlePoints(vector2,startDir,vector5,endDir2,smoothStart,smoothEnd,out vector6,out vector7);
					Vector3 vector8;
					Vector3 vector9;
					NetSegment.CalculateMiddlePoints(vector4,startDir2,vector3,endDir,smoothStart,smoothEnd,out vector8,out vector9);
					data.m_dataMatrix0=NetSegment.CalculateControlMatrix(vector2,vector6,vector7,vector5,vector4,vector8,vector9,vector3,data.m_position,vScale);
					data.m_dataMatrix1=NetSegment.CalculateControlMatrix(vector4,vector8,vector9,vector3,vector2,vector6,vector7,vector5,data.m_position,vScale);
				}
				if(info.m_requireSurfaceMaps)
				{
					Singleton<TerrainManager>.instance.GetSurfaceMapping(data.m_position,out data.m_dataTexture0,out data.m_dataTexture1,out data.m_dataVector1);
				}
			}
			if(info.m_segments!=null)
			{
				for(int i=0;i<info.m_segments.Length;i++)
				{
					NetInfo.Segment segment=info.m_segments[i];
					bool flag;
					if(segment.CheckFlags(this.m_flags,out flag))
					{
						Vector4 dataVector=data.m_dataVector3;
						Vector4 dataVector2=data.m_dataVector0;
						if(segment.m_requireWindSpeed)
						{
							dataVector.w=data.m_dataFloat0;
						}
						if(flag)
						{
							dataVector2.x=-dataVector2.x;
							dataVector2.y=-dataVector2.y;
						}
						if(cameraInfo.CheckRenderDistance(data.m_position,segment.m_lodRenderDistance))
						{
							instance.m_materialBlock.Clear();
							instance.m_materialBlock.AddMatrix(instance.ID_LeftMatrix,data.m_dataMatrix0);
							instance.m_materialBlock.AddMatrix(instance.ID_RightMatrix,data.m_dataMatrix1);
							instance.m_materialBlock.AddVector(instance.ID_MeshScale,dataVector2);
							instance.m_materialBlock.AddVector(instance.ID_ObjectIndex,dataVector);
							instance.m_materialBlock.AddColor(instance.ID_Color,data.m_dataColor0);
							if(segment.m_requireSurfaceMaps&&data.m_dataTexture0!=null)
							{
								instance.m_materialBlock.AddTexture(instance.ID_SurfaceTexA,data.m_dataTexture0);
								instance.m_materialBlock.AddTexture(instance.ID_SurfaceTexB,data.m_dataTexture1);
								instance.m_materialBlock.AddVector(instance.ID_SurfaceMapping,data.m_dataVector1);
							}
							NetManager expr_5D7_cp_0=instance;
							expr_5D7_cp_0.m_drawCallData.m_defaultCalls=expr_5D7_cp_0.m_drawCallData.m_defaultCalls+1;
							Graphics.DrawMesh(segment.m_segmentMesh,data.m_position,data.m_rotation,segment.m_segmentMaterial,segment.m_layer,null,0,instance.m_materialBlock);
						}
						else
						{
							NetInfo.LodValue combinedLod=segment.m_combinedLod;
							if(combinedLod!=null)
							{
								if(segment.m_requireSurfaceMaps&&data.m_dataTexture0!=combinedLod.m_surfaceTexA)
								{
									if(combinedLod.m_lodCount!=0)
									{
										NetSegment.RenderLod(cameraInfo,combinedLod);
									}
									combinedLod.m_surfaceTexA=data.m_dataTexture0;
									combinedLod.m_surfaceTexB=data.m_dataTexture1;
									combinedLod.m_surfaceMapping=data.m_dataVector1;
								}
								combinedLod.m_leftMatrices[combinedLod.m_lodCount]=data.m_dataMatrix0;
								combinedLod.m_rightMatrices[combinedLod.m_lodCount]=data.m_dataMatrix1;
								combinedLod.m_meshScales[combinedLod.m_lodCount]=dataVector2;
								combinedLod.m_objectIndices[combinedLod.m_lodCount]=dataVector;
								combinedLod.m_meshLocations[combinedLod.m_lodCount]=data.m_position;
								combinedLod.m_lodMin=Vector3.Min(combinedLod.m_lodMin,data.m_position);
								combinedLod.m_lodMax=Vector3.Max(combinedLod.m_lodMax,data.m_position);
								if(++combinedLod.m_lodCount==combinedLod.m_leftMatrices.Length)
								{
									NetSegment.RenderLod(cameraInfo,combinedLod);
								}
							}
						}
					}
				}
			}
			if(info.m_lanes!=null&&((layerMask&info.m_treeLayers)!=0||cameraInfo.CheckRenderDistance(data.m_position,info.m_maxPropDistance+128f)))
			{
				bool invert2;
				NetNode.Flags startFlags;
				Color startColor;
				NetNode.Flags endFlags;
				Color endColor;
				if((this.m_flags&NetSegment.Flags.Invert)!=NetSegment.Flags.None)
				{
					invert2=true;
					NetInfo info6=instance.m_nodes.m_buffer[(int)this.m_endNode].Info;
					info6.m_netAI.GetNodeState(this.m_endNode,ref instance.m_nodes.m_buffer[(int)this.m_endNode],segmentID,ref this,out startFlags,out startColor);
					NetInfo info7=instance.m_nodes.m_buffer[(int)this.m_startNode].Info;
					info7.m_netAI.GetNodeState(this.m_startNode,ref instance.m_nodes.m_buffer[(int)this.m_startNode],segmentID,ref this,out endFlags,out endColor);
				}
				else
				{
					invert2=false;
					NetInfo info8=instance.m_nodes.m_buffer[(int)this.m_startNode].Info;
					info8.m_netAI.GetNodeState(this.m_startNode,ref instance.m_nodes.m_buffer[(int)this.m_startNode],segmentID,ref this,out startFlags,out startColor);
					NetInfo info9=instance.m_nodes.m_buffer[(int)this.m_endNode].Info;
					info9.m_netAI.GetNodeState(this.m_endNode,ref instance.m_nodes.m_buffer[(int)this.m_endNode],segmentID,ref this,out endFlags,out endColor);
				}
				float startAngle2=(float)this.m_cornerAngleStart*0.0245436933f;
				float endAngle2=(float)this.m_cornerAngleEnd*0.0245436933f;
				Vector4 objectIndex=new Vector4(data.m_dataVector3.x,data.m_dataVector3.y,1f,data.m_dataFloat0);
				Vector4 objectIndex2=new Vector4(data.m_dataVector3.z,data.m_dataVector3.w,1f,data.m_dataFloat0);
				InfoManager.InfoMode currentMode=Singleton<InfoManager>.instance.CurrentMode;
				if(currentMode!=InfoManager.InfoMode.None&&!info.m_netAI.ColorizeProps(currentMode))
				{
					objectIndex.z=0f;
					objectIndex2.z=0f;
				}
				int num4=(info.m_segments!=null&&info.m_segments.Length!=0)?-1:0;
				uint num5=this.m_lanes;
				int num6=0;
				while(num6<info.m_lanes.Length&&num5!=0u)
				{
					instance.m_lanes.m_buffer[(int)((UIntPtr)num5)].RenderInstance(cameraInfo,segmentID,num5,info.m_lanes[num6],startFlags,endFlags,startColor,endColor,startAngle2,endAngle2,invert2,layerMask,objectIndex,objectIndex2,ref data,ref num4);
					num5=instance.m_lanes.m_buffer[(int)((UIntPtr)num5)].m_nextLane;
					num6++;
				}
			}
		}

	}//854*365//1249
}
//RefreshInstance(uint laneID,NetInfo.Lane laneInfo,float startAngle,float endAngle,bool invert,ref RenderManager.Instance data,ref int propIndex);
//GetColorLocation(uint instanceHolder);
//GetNodeState(ushort nodeID,ref NetNode nodeData,ushort segmentID,ref NetSegment segmentData,out NetNode.Flags flags,out Color color);
//GetSegment(int index);
//AddMatrix(int nameID,Matrix4x4 value)
//AddColor(int nameID,Color value);
//AddTexture(int nameID,Texture value);
//AddVector(int nameID,Vector4 value)
//DrawMesh(Mesh mesh,Vector3 position,Quaternion rotation,Material material,int layer,Camera camera,int submeshIndex,MaterialPropertyBlock properties);
//CheckFlags(NetSegment.Flags flags,out bool turnAround);
//GetSurfaceMapping(Vector3 worldPos,out Texture _SurfaceTexA,out Texture _SurfaceTexB,out Vector4 _SurfaceMapping);
//CalculateMiddlePoints(startPos,startDir,endPos,endDir,smoothStart,smoothEnd,middlePos1.middlePos2);
//CalculateControlMatrix(startPos, middlePos1, middlePos2, endPos, startPosB, middlePosB1, middlePosB2, endPosB, transform, vScale);
//GetSurfaceMapping(worldPos       ,out _SurfaceTexA,       out _SurfaceTexB,       out _SurfaceMapping);


/*
 * using ColossalFramework;
 * using System;
 * using System.Collections.Generic;
 * using System.Linq;
 * using System.Reflection;
 * using System.Runtime.CompilerServices;
 * using UnityEngine;
 * Hook4: MonoBehaviour
 * public bool hookEnabled=false;
 * private Dictionary<MethodInfo,RedirectCallsState> redirects=new Dictionary<MethodInfo,RedirectCallsState>();//create new dictionary
 * private int CalculateRendererCount(NetInfo info)
 * private void RefreshBendData(ushort nodeID,NetInfo info,uint instanceIndex,ref RenderManager.Instance data)
 * private void RefreshJunctionData(ushort nodeID,NetInfo info,uint instanceIndex)
 * private void RefreshJunctionData(NetNode netnode,ushort nodeID,int segmentIndex,int segmentIndex2,NetInfo info,NetInfo info2,ushort nodeSegment,ushort nodeSegment2,ref uint instanceIndex,ref RenderManager.Instance data)
 * private void RefreshJunctionData(NetNode netnode,ushort nodeID,int segmentIndex,ushort nodeSegment,Vector3 centerPos,ref uint instanceIndex,ref RenderManager.Instance data)
 * public void RenderInstance(RenderManager.CameraInfo cameraInfo,ushort nodeID,int layerMask)
 * private void RenderInstance(RenderManager.CameraInfo cameraInfo,ushort nodeID,NetInfo info,int iter,NetNode.Flags flags,ref uint instanceIndex,ref RenderManager.Instance data)
 * public void RenderInstance(RenderManager.CameraInfo cameraInfo,ushort segmentID,int layerMask)
 * private void RenderInstance(RenderManager.CameraInfo cameraInfo,ushort segmentID,int layerMask,NetInfo info,ref RenderManager.Instance data)
 * 
*/