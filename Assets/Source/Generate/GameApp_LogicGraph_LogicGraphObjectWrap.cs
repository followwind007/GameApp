﻿//this source code was auto-generated by tolua#, do not modify it
using System;
using LuaInterface;

public class GameApp_LogicGraph_LogicGraphObjectWrap
{
	public static void Register(LuaState L)
	{
		L.BeginClass(typeof(GameApp.LogicGraph.LogicGraphObject), typeof(UnityEngine.ScriptableObject));
		L.RegFunction("New", new LuaCSFunction(_CreateGameApp_LogicGraph_LogicGraphObject));
		L.RegFunction("__eq", new LuaCSFunction(op_Equality));
		L.RegFunction("__tostring", new LuaCSFunction(ToLua.op_ToString));
		L.RegVar("exportPath", new LuaCSFunction(get_exportPath), new LuaCSFunction(set_exportPath));
		L.RegVar("type", new LuaCSFunction(get_type), new LuaCSFunction(set_type));
		L.RegVar("useAsset", new LuaCSFunction(get_useAsset), new LuaCSFunction(set_useAsset));
		L.RegVar("lastClosePosition", new LuaCSFunction(get_lastClosePosition), new LuaCSFunction(set_lastClosePosition));
		L.RegVar("lastCloseScale", new LuaCSFunction(get_lastCloseScale), new LuaCSFunction(set_lastCloseScale));
		L.RegVar("logicGraphData", new LuaCSFunction(get_logicGraphData), new LuaCSFunction(set_logicGraphData));
		L.RegVar("binds", new LuaCSFunction(get_binds), new LuaCSFunction(set_binds));
		L.RegVar("properties", new LuaCSFunction(get_properties), new LuaCSFunction(set_properties));
		L.RegVar("IsStateGraph", new LuaCSFunction(get_IsStateGraph), null);
		L.RegVar("GraphId", new LuaCSFunction(get_GraphId), null);
		L.EndClass();
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int _CreateGameApp_LogicGraph_LogicGraphObject(IntPtr L)
	{
		try
		{
			int count = LuaDLL.lua_gettop(L);

			if (count == 0)
			{
				GameApp.LogicGraph.LogicGraphObject obj = new GameApp.LogicGraph.LogicGraphObject();
				ToLua.Push(L, obj);
				return 1;
			}
			else
			{
				return LuaDLL.luaL_throw(L, "invalid arguments to ctor method: GameApp.LogicGraph.LogicGraphObject.New");
			}
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
	static int get_exportPath(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			GameApp.LogicGraph.LogicGraphObject obj = (GameApp.LogicGraph.LogicGraphObject)o;
			string ret = obj.exportPath;
			LuaDLL.lua_pushstring(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index exportPath on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_type(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			GameApp.LogicGraph.LogicGraphObject obj = (GameApp.LogicGraph.LogicGraphObject)o;
			GameApp.LogicGraph.LogicGraphObject.GraphType ret = obj.type;
			ToLua.Push(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index type on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_useAsset(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			GameApp.LogicGraph.LogicGraphObject obj = (GameApp.LogicGraph.LogicGraphObject)o;
			bool ret = obj.useAsset;
			LuaDLL.lua_pushboolean(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index useAsset on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_lastClosePosition(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			GameApp.LogicGraph.LogicGraphObject obj = (GameApp.LogicGraph.LogicGraphObject)o;
			UnityEngine.Vector3 ret = obj.lastClosePosition;
			ToLua.Push(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index lastClosePosition on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_lastCloseScale(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			GameApp.LogicGraph.LogicGraphObject obj = (GameApp.LogicGraph.LogicGraphObject)o;
			UnityEngine.Vector3 ret = obj.lastCloseScale;
			ToLua.Push(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index lastCloseScale on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_logicGraphData(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			GameApp.LogicGraph.LogicGraphObject obj = (GameApp.LogicGraph.LogicGraphObject)o;
			GameApp.LogicGraph.LogicGraphData ret = obj.logicGraphData;
			ToLua.PushObject(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index logicGraphData on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_binds(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			GameApp.LogicGraph.LogicGraphObject obj = (GameApp.LogicGraph.LogicGraphObject)o;
			GameApp.DataBinder.BindableValues ret = obj.binds;
			ToLua.PushObject(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index binds on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_properties(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			GameApp.LogicGraph.LogicGraphObject obj = (GameApp.LogicGraph.LogicGraphObject)o;
			System.Collections.Generic.List<GameApp.LogicGraph.GraphProperty> ret = obj.properties;
			ToLua.PushSealed(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index properties on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_IsStateGraph(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			GameApp.LogicGraph.LogicGraphObject obj = (GameApp.LogicGraph.LogicGraphObject)o;
			bool ret = obj.IsStateGraph;
			LuaDLL.lua_pushboolean(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index IsStateGraph on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_GraphId(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			GameApp.LogicGraph.LogicGraphObject obj = (GameApp.LogicGraph.LogicGraphObject)o;
			string ret = obj.GraphId;
			LuaDLL.lua_pushstring(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index GraphId on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_exportPath(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			GameApp.LogicGraph.LogicGraphObject obj = (GameApp.LogicGraph.LogicGraphObject)o;
			string arg0 = ToLua.CheckString(L, 2);
			obj.exportPath = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index exportPath on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_type(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			GameApp.LogicGraph.LogicGraphObject obj = (GameApp.LogicGraph.LogicGraphObject)o;
			GameApp.LogicGraph.LogicGraphObject.GraphType arg0 = (GameApp.LogicGraph.LogicGraphObject.GraphType)ToLua.CheckObject(L, 2, TypeTraits<GameApp.LogicGraph.LogicGraphObject.GraphType>.type);
			obj.type = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index type on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_useAsset(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			GameApp.LogicGraph.LogicGraphObject obj = (GameApp.LogicGraph.LogicGraphObject)o;
			bool arg0 = LuaDLL.luaL_checkboolean(L, 2);
			obj.useAsset = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index useAsset on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_lastClosePosition(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			GameApp.LogicGraph.LogicGraphObject obj = (GameApp.LogicGraph.LogicGraphObject)o;
			UnityEngine.Vector3 arg0 = ToLua.ToVector3(L, 2);
			obj.lastClosePosition = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index lastClosePosition on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_lastCloseScale(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			GameApp.LogicGraph.LogicGraphObject obj = (GameApp.LogicGraph.LogicGraphObject)o;
			UnityEngine.Vector3 arg0 = ToLua.ToVector3(L, 2);
			obj.lastCloseScale = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index lastCloseScale on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_logicGraphData(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			GameApp.LogicGraph.LogicGraphObject obj = (GameApp.LogicGraph.LogicGraphObject)o;
			GameApp.LogicGraph.LogicGraphData arg0 = (GameApp.LogicGraph.LogicGraphData)ToLua.CheckObject<GameApp.LogicGraph.LogicGraphData>(L, 2);
			obj.logicGraphData = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index logicGraphData on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_binds(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			GameApp.LogicGraph.LogicGraphObject obj = (GameApp.LogicGraph.LogicGraphObject)o;
			GameApp.DataBinder.BindableValues arg0 = (GameApp.DataBinder.BindableValues)ToLua.CheckObject<GameApp.DataBinder.BindableValues>(L, 2);
			obj.binds = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index binds on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_properties(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			GameApp.LogicGraph.LogicGraphObject obj = (GameApp.LogicGraph.LogicGraphObject)o;
			System.Collections.Generic.List<GameApp.LogicGraph.GraphProperty> arg0 = (System.Collections.Generic.List<GameApp.LogicGraph.GraphProperty>)ToLua.CheckObject(L, 2, TypeTraits<System.Collections.Generic.List<GameApp.LogicGraph.GraphProperty>>.type);
			obj.properties = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index properties on a nil value");
		}
	}
}
