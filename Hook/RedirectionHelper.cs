using System;
using System.Reflection;

namespace RoadsUnited_Core
{
 public static class RedirectionHelper
	{
		 public static RedirectCallsState RedirectCalls(MethodInfo from,MethodInfo to)
		 {
			 IntPtr functionPointer=from.MethodHandle.GetFunctionPointer();
			 IntPtr functionPointer2=to.MethodHandle.GetFunctionPointer();
			 return RedirectionHelper.PatchJumpTo(functionPointer,functionPointer2);
		 }

		public static void RevertRedirect(MethodInfo from,RedirectCallsState state)
		{
			IntPtr functionPointer=from.MethodHandle.GetFunctionPointer();
			RedirectionHelper.RevertJumpTo(functionPointer,state);
		}

		private unsafe static RedirectCallsState PatchJumpTo(IntPtr site,IntPtr target)
		{
			RedirectCallsState result=default(RedirectCallsState);
			byte* ptr=(byte*)site.ToPointer();
			result.a=*ptr;
			result.b=ptr[1];
			result.c=ptr[10];
			result.d=ptr[11];
			result.e=ptr[12];
			result.f=(ulong)(*(long*)(ptr+2));
			*ptr=73;
			ptr[1]=187;
			*(long*)(ptr+2)=target.ToInt64();
			ptr[10]=65;
			ptr[11]=255;
			ptr[12]=227;
			return result;
		}

		private unsafe static void RevertJumpTo(IntPtr site,RedirectCallsState state)
		{
			byte* ptr=(byte*)site.ToPointer();
			*ptr=state.a;
			ptr[1]=state.b;
			*(long*)(ptr+2)=(long)state.f;
			ptr[10]=state.c;
			ptr[11]=state.d;
			ptr[12]=state.e;
		}
	}
}
