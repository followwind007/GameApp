//this source code was auto-generated by tolua#, do not modify it
using System;
using LuaInterface;

public class UnityEngine_TextureWrap
{
	public static void Register(LuaState L)
	{
		L.BeginClass(typeof(UnityEngine.Texture), typeof(UnityEngine.Object));
		L.RegFunction("SetGlobalAnisotropicFilteringLimits", new LuaCSFunction(SetGlobalAnisotropicFilteringLimits));
		L.RegFunction("GetNativeTexturePtr", new LuaCSFunction(GetNativeTexturePtr));
		L.RegFunction("IncrementUpdateCount", new LuaCSFunction(IncrementUpdateCount));
		L.RegFunction("SetStreamingTextureMaterialDebugProperties", new LuaCSFunction(SetStreamingTextureMaterialDebugProperties));
		L.RegFunction("__eq", new LuaCSFunction(op_Equality));
		L.RegFunction("__tostring", new LuaCSFunction(ToLua.op_ToString));
		L.RegVar("GenerateAllMips", new LuaCSFunction(get_GenerateAllMips), null);
		L.RegVar("masterTextureLimit", new LuaCSFunction(get_masterTextureLimit), new LuaCSFunction(set_masterTextureLimit));
		L.RegVar("mipmapCount", new LuaCSFunction(get_mipmapCount), null);
		L.RegVar("anisotropicFiltering", new LuaCSFunction(get_anisotropicFiltering), new LuaCSFunction(set_anisotropicFiltering));
		L.RegVar("graphicsFormat", new LuaCSFunction(get_graphicsFormat), null);
		L.RegVar("width", new LuaCSFunction(get_width), new LuaCSFunction(set_width));
		L.RegVar("height", new LuaCSFunction(get_height), new LuaCSFunction(set_height));
		L.RegVar("dimension", new LuaCSFunction(get_dimension), new LuaCSFunction(set_dimension));
		L.RegVar("isReadable", new LuaCSFunction(get_isReadable), null);
		L.RegVar("wrapMode", new LuaCSFunction(get_wrapMode), new LuaCSFunction(set_wrapMode));
		L.RegVar("wrapModeU", new LuaCSFunction(get_wrapModeU), new LuaCSFunction(set_wrapModeU));
		L.RegVar("wrapModeV", new LuaCSFunction(get_wrapModeV), new LuaCSFunction(set_wrapModeV));
		L.RegVar("wrapModeW", new LuaCSFunction(get_wrapModeW), new LuaCSFunction(set_wrapModeW));
		L.RegVar("filterMode", new LuaCSFunction(get_filterMode), new LuaCSFunction(set_filterMode));
		L.RegVar("anisoLevel", new LuaCSFunction(get_anisoLevel), new LuaCSFunction(set_anisoLevel));
		L.RegVar("mipMapBias", new LuaCSFunction(get_mipMapBias), new LuaCSFunction(set_mipMapBias));
		L.RegVar("texelSize", new LuaCSFunction(get_texelSize), null);
		L.RegVar("updateCount", new LuaCSFunction(get_updateCount), null);
		L.RegVar("totalTextureMemory", new LuaCSFunction(get_totalTextureMemory), null);
		L.RegVar("desiredTextureMemory", new LuaCSFunction(get_desiredTextureMemory), null);
		L.RegVar("targetTextureMemory", new LuaCSFunction(get_targetTextureMemory), null);
		L.RegVar("currentTextureMemory", new LuaCSFunction(get_currentTextureMemory), null);
		L.RegVar("nonStreamingTextureMemory", new LuaCSFunction(get_nonStreamingTextureMemory), null);
		L.RegVar("streamingMipmapUploadCount", new LuaCSFunction(get_streamingMipmapUploadCount), null);
		L.RegVar("streamingRendererCount", new LuaCSFunction(get_streamingRendererCount), null);
		L.RegVar("streamingTextureCount", new LuaCSFunction(get_streamingTextureCount), null);
		L.RegVar("nonStreamingTextureCount", new LuaCSFunction(get_nonStreamingTextureCount), null);
		L.RegVar("streamingTexturePendingLoadCount", new LuaCSFunction(get_streamingTexturePendingLoadCount), null);
		L.RegVar("streamingTextureLoadingCount", new LuaCSFunction(get_streamingTextureLoadingCount), null);
		L.RegVar("streamingTextureForceLoadAll", new LuaCSFunction(get_streamingTextureForceLoadAll), new LuaCSFunction(set_streamingTextureForceLoadAll));
		L.RegVar("streamingTextureDiscardUnusedMips", new LuaCSFunction(get_streamingTextureDiscardUnusedMips), new LuaCSFunction(set_streamingTextureDiscardUnusedMips));
		L.RegVar("allowThreadedTextureCreation", new LuaCSFunction(get_allowThreadedTextureCreation), new LuaCSFunction(set_allowThreadedTextureCreation));
		L.EndClass();
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetGlobalAnisotropicFilteringLimits(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			int arg0 = (int)LuaDLL.luaL_checkinteger(L, 1);
			int arg1 = (int)LuaDLL.luaL_checkinteger(L, 2);
			UnityEngine.Texture.SetGlobalAnisotropicFilteringLimits(arg0, arg1);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetNativeTexturePtr(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			UnityEngine.Texture obj = (UnityEngine.Texture)ToLua.CheckObject<UnityEngine.Texture>(L, 1);
			System.IntPtr o = obj.GetNativeTexturePtr();
			LuaDLL.lua_pushlightuserdata(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int IncrementUpdateCount(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			UnityEngine.Texture obj = (UnityEngine.Texture)ToLua.CheckObject<UnityEngine.Texture>(L, 1);
			obj.IncrementUpdateCount();
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetStreamingTextureMaterialDebugProperties(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 0);
			UnityEngine.Texture.SetStreamingTextureMaterialDebugProperties();
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int op_Equality(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			UnityEngine.Object arg0 = (UnityEngine.Object)ToLua.ToObject(L, 1);
			UnityEngine.Object arg1 = (UnityEngine.Object)ToLua.ToObject(L, 2);
			bool o = arg0 == arg1;
			LuaDLL.lua_pushboolean(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_GenerateAllMips(IntPtr L)
	{
		try
		{
			LuaDLL.lua_pushinteger(L, UnityEngine.Texture.GenerateAllMips);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_masterTextureLimit(IntPtr L)
	{
		try
		{
			LuaDLL.lua_pushinteger(L, UnityEngine.Texture.masterTextureLimit);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_mipmapCount(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			UnityEngine.Texture obj = (UnityEngine.Texture)o;
			int ret = obj.mipmapCount;
			LuaDLL.lua_pushinteger(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index mipmapCount on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_anisotropicFiltering(IntPtr L)
	{
		try
		{
			ToLua.Push(L, UnityEngine.Texture.anisotropicFiltering);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_graphicsFormat(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			UnityEngine.Texture obj = (UnityEngine.Texture)o;
			UnityEngine.Experimental.Rendering.GraphicsFormat ret = obj.graphicsFormat;
			ToLua.Push(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index graphicsFormat on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_width(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			UnityEngine.Texture obj = (UnityEngine.Texture)o;
			int ret = obj.width;
			LuaDLL.lua_pushinteger(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index width on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_height(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			UnityEngine.Texture obj = (UnityEngine.Texture)o;
			int ret = obj.height;
			LuaDLL.lua_pushinteger(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index height on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_dimension(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			UnityEngine.Texture obj = (UnityEngine.Texture)o;
			UnityEngine.Rendering.TextureDimension ret = obj.dimension;
			ToLua.Push(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index dimension on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_isReadable(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			UnityEngine.Texture obj = (UnityEngine.Texture)o;
			bool ret = obj.isReadable;
			LuaDLL.lua_pushboolean(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index isReadable on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_wrapMode(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			UnityEngine.Texture obj = (UnityEngine.Texture)o;
			UnityEngine.TextureWrapMode ret = obj.wrapMode;
			ToLua.Push(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index wrapMode on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_wrapModeU(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			UnityEngine.Texture obj = (UnityEngine.Texture)o;
			UnityEngine.TextureWrapMode ret = obj.wrapModeU;
			ToLua.Push(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index wrapModeU on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_wrapModeV(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			UnityEngine.Texture obj = (UnityEngine.Texture)o;
			UnityEngine.TextureWrapMode ret = obj.wrapModeV;
			ToLua.Push(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index wrapModeV on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_wrapModeW(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			UnityEngine.Texture obj = (UnityEngine.Texture)o;
			UnityEngine.TextureWrapMode ret = obj.wrapModeW;
			ToLua.Push(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index wrapModeW on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_filterMode(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			UnityEngine.Texture obj = (UnityEngine.Texture)o;
			UnityEngine.FilterMode ret = obj.filterMode;
			ToLua.Push(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index filterMode on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_anisoLevel(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			UnityEngine.Texture obj = (UnityEngine.Texture)o;
			int ret = obj.anisoLevel;
			LuaDLL.lua_pushinteger(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index anisoLevel on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_mipMapBias(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			UnityEngine.Texture obj = (UnityEngine.Texture)o;
			float ret = obj.mipMapBias;
			LuaDLL.lua_pushnumber(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index mipMapBias on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_texelSize(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			UnityEngine.Texture obj = (UnityEngine.Texture)o;
			UnityEngine.Vector2 ret = obj.texelSize;
			ToLua.Push(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index texelSize on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_updateCount(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			UnityEngine.Texture obj = (UnityEngine.Texture)o;
			uint ret = obj.updateCount;
			LuaDLL.lua_pushinteger(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index updateCount on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_totalTextureMemory(IntPtr L)
	{
		try
		{
			LuaDLL.tolua_pushuint64(L, UnityEngine.Texture.totalTextureMemory);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_desiredTextureMemory(IntPtr L)
	{
		try
		{
			LuaDLL.tolua_pushuint64(L, UnityEngine.Texture.desiredTextureMemory);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_targetTextureMemory(IntPtr L)
	{
		try
		{
			LuaDLL.tolua_pushuint64(L, UnityEngine.Texture.targetTextureMemory);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_currentTextureMemory(IntPtr L)
	{
		try
		{
			LuaDLL.tolua_pushuint64(L, UnityEngine.Texture.currentTextureMemory);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_nonStreamingTextureMemory(IntPtr L)
	{
		try
		{
			LuaDLL.tolua_pushuint64(L, UnityEngine.Texture.nonStreamingTextureMemory);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_streamingMipmapUploadCount(IntPtr L)
	{
		try
		{
			LuaDLL.tolua_pushuint64(L, UnityEngine.Texture.streamingMipmapUploadCount);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_streamingRendererCount(IntPtr L)
	{
		try
		{
			LuaDLL.tolua_pushuint64(L, UnityEngine.Texture.streamingRendererCount);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_streamingTextureCount(IntPtr L)
	{
		try
		{
			LuaDLL.tolua_pushuint64(L, UnityEngine.Texture.streamingTextureCount);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_nonStreamingTextureCount(IntPtr L)
	{
		try
		{
			LuaDLL.tolua_pushuint64(L, UnityEngine.Texture.nonStreamingTextureCount);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_streamingTexturePendingLoadCount(IntPtr L)
	{
		try
		{
			LuaDLL.tolua_pushuint64(L, UnityEngine.Texture.streamingTexturePendingLoadCount);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_streamingTextureLoadingCount(IntPtr L)
	{
		try
		{
			LuaDLL.tolua_pushuint64(L, UnityEngine.Texture.streamingTextureLoadingCount);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_streamingTextureForceLoadAll(IntPtr L)
	{
		try
		{
			LuaDLL.lua_pushboolean(L, UnityEngine.Texture.streamingTextureForceLoadAll);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_streamingTextureDiscardUnusedMips(IntPtr L)
	{
		try
		{
			LuaDLL.lua_pushboolean(L, UnityEngine.Texture.streamingTextureDiscardUnusedMips);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_allowThreadedTextureCreation(IntPtr L)
	{
		try
		{
			LuaDLL.lua_pushboolean(L, UnityEngine.Texture.allowThreadedTextureCreation);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_masterTextureLimit(IntPtr L)
	{
		try
		{
			int arg0 = (int)LuaDLL.luaL_checkinteger(L, 2);
			UnityEngine.Texture.masterTextureLimit = arg0;
			UnityEngine.Texture.masterTextureLimit = arg0;
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_anisotropicFiltering(IntPtr L)
	{
		try
		{
			UnityEngine.AnisotropicFiltering arg0 = (UnityEngine.AnisotropicFiltering)ToLua.CheckObject(L, 2, TypeTraits<UnityEngine.AnisotropicFiltering>.type);
			UnityEngine.Texture.anisotropicFiltering = arg0;
			UnityEngine.Texture.anisotropicFiltering = arg0;
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_width(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			UnityEngine.Texture obj = (UnityEngine.Texture)o;
			int arg0 = (int)LuaDLL.luaL_checkinteger(L, 2);
			obj.width = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index width on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_height(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			UnityEngine.Texture obj = (UnityEngine.Texture)o;
			int arg0 = (int)LuaDLL.luaL_checkinteger(L, 2);
			obj.height = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index height on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_dimension(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			UnityEngine.Texture obj = (UnityEngine.Texture)o;
			UnityEngine.Rendering.TextureDimension arg0 = (UnityEngine.Rendering.TextureDimension)ToLua.CheckObject(L, 2, TypeTraits<UnityEngine.Rendering.TextureDimension>.type);
			obj.dimension = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index dimension on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_wrapMode(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			UnityEngine.Texture obj = (UnityEngine.Texture)o;
			UnityEngine.TextureWrapMode arg0 = (UnityEngine.TextureWrapMode)ToLua.CheckObject(L, 2, TypeTraits<UnityEngine.TextureWrapMode>.type);
			obj.wrapMode = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index wrapMode on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_wrapModeU(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			UnityEngine.Texture obj = (UnityEngine.Texture)o;
			UnityEngine.TextureWrapMode arg0 = (UnityEngine.TextureWrapMode)ToLua.CheckObject(L, 2, TypeTraits<UnityEngine.TextureWrapMode>.type);
			obj.wrapModeU = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index wrapModeU on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_wrapModeV(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			UnityEngine.Texture obj = (UnityEngine.Texture)o;
			UnityEngine.TextureWrapMode arg0 = (UnityEngine.TextureWrapMode)ToLua.CheckObject(L, 2, TypeTraits<UnityEngine.TextureWrapMode>.type);
			obj.wrapModeV = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index wrapModeV on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_wrapModeW(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			UnityEngine.Texture obj = (UnityEngine.Texture)o;
			UnityEngine.TextureWrapMode arg0 = (UnityEngine.TextureWrapMode)ToLua.CheckObject(L, 2, TypeTraits<UnityEngine.TextureWrapMode>.type);
			obj.wrapModeW = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index wrapModeW on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_filterMode(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			UnityEngine.Texture obj = (UnityEngine.Texture)o;
			UnityEngine.FilterMode arg0 = (UnityEngine.FilterMode)ToLua.CheckObject(L, 2, TypeTraits<UnityEngine.FilterMode>.type);
			obj.filterMode = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index filterMode on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_anisoLevel(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			UnityEngine.Texture obj = (UnityEngine.Texture)o;
			int arg0 = (int)LuaDLL.luaL_checkinteger(L, 2);
			obj.anisoLevel = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index anisoLevel on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_mipMapBias(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			UnityEngine.Texture obj = (UnityEngine.Texture)o;
			float arg0 = (float)LuaDLL.luaL_checknumber(L, 2);
			obj.mipMapBias = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index mipMapBias on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_streamingTextureForceLoadAll(IntPtr L)
	{
		try
		{
			bool arg0 = LuaDLL.luaL_checkboolean(L, 2);
			UnityEngine.Texture.streamingTextureForceLoadAll = arg0;
			UnityEngine.Texture.streamingTextureForceLoadAll = arg0;
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_streamingTextureDiscardUnusedMips(IntPtr L)
	{
		try
		{
			bool arg0 = LuaDLL.luaL_checkboolean(L, 2);
			UnityEngine.Texture.streamingTextureDiscardUnusedMips = arg0;
			UnityEngine.Texture.streamingTextureDiscardUnusedMips = arg0;
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_allowThreadedTextureCreation(IntPtr L)
	{
		try
		{
			bool arg0 = LuaDLL.luaL_checkboolean(L, 2);
			UnityEngine.Texture.allowThreadedTextureCreation = arg0;
			UnityEngine.Texture.allowThreadedTextureCreation = arg0;
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}
}

