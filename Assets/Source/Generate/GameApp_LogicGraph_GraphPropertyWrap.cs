﻿//this source code was auto-generated by tolua#, do not modify it
using System;
using LuaInterface;

public class GameApp_LogicGraph_GraphPropertyWrap
{
	public static void Register(LuaState L)
	{
		L.BeginClass(typeof(GameApp.LogicGraph.GraphProperty), typeof(System.Object));
		L.RegFunction("New", new LuaCSFunction(_CreateGameApp_LogicGraph_GraphProperty));
		L.RegFunction("__tostring", new LuaCSFunction(ToLua.op_ToString));
		L.RegVar("name", new LuaCSFunction(get_name), new LuaCSFunction(set_name));
		L.RegVar("type", new LuaCSFunction(get_type), new LuaCSFunction(set_type));
		L.RegVar("isListen", new LuaCSFunction(get_isListen), new LuaCSFunction(set_isListen));
		L.RegVar("isTrigger", new LuaCSFunction(get_isTrigger), new LuaCSFunction(set_isTrigger));
		L.RegVar("scope", new LuaCSFunction(get_scope), new LuaCSFunction(set_scope));
		L.EndClass();
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int _CreateGameApp_LogicGraph_GraphProperty(IntPtr L)
	{
		try
		{
			int count = LuaDLL.lua_gettop(L);

			if (count == 0)
			{
				GameApp.LogicGraph.GraphProperty obj = new GameApp.LogicGraph.GraphProperty();
				ToLua.PushObject(L, obj);
				return 1;
			}
			else
			{
				return LuaDLL.luaL_throw(L, "invalid arguments to ctor method: GameApp.LogicGraph.GraphProperty.New");
			}
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_name(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			GameApp.LogicGraph.GraphProperty obj = (GameApp.LogicGraph.GraphProperty)o;
			string ret = obj.name;
			LuaDLL.lua_pushstring(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index name on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_type(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			GameApp.LogicGraph.GraphProperty obj = (GameApp.LogicGraph.GraphProperty)o;
			string ret = obj.type;
			LuaDLL.lua_pushstring(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index type on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_isListen(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			GameApp.LogicGraph.GraphProperty obj = (GameApp.LogicGraph.GraphProperty)o;
			bool ret = obj.isListen;
			LuaDLL.lua_pushboolean(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index isListen on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_isTrigger(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			GameApp.LogicGraph.GraphProperty obj = (GameApp.LogicGraph.GraphProperty)o;
			bool ret = obj.isTrigger;
			LuaDLL.lua_pushboolean(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index isTrigger on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_scope(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			GameApp.LogicGraph.GraphProperty obj = (GameApp.LogicGraph.GraphProperty)o;
			GameApp.LogicGraph.GraphProperty.Scope ret = obj.scope;
			ToLua.Push(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index scope on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_name(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			GameApp.LogicGraph.GraphProperty obj = (GameApp.LogicGraph.GraphProperty)o;
			string arg0 = ToLua.CheckString(L, 2);
			obj.name = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index name on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_type(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			GameApp.LogicGraph.GraphProperty obj = (GameApp.LogicGraph.GraphProperty)o;
			string arg0 = ToLua.CheckString(L, 2);
			obj.type = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index type on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_isListen(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			GameApp.LogicGraph.GraphProperty obj = (GameApp.LogicGraph.GraphProperty)o;
			bool arg0 = LuaDLL.luaL_checkboolean(L, 2);
			obj.isListen = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index isListen on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_isTrigger(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			GameApp.LogicGraph.GraphProperty obj = (GameApp.LogicGraph.GraphProperty)o;
			bool arg0 = LuaDLL.luaL_checkboolean(L, 2);
			obj.isTrigger = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index isTrigger on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_scope(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			GameApp.LogicGraph.GraphProperty obj = (GameApp.LogicGraph.GraphProperty)o;
			GameApp.LogicGraph.GraphProperty.Scope arg0 = (GameApp.LogicGraph.GraphProperty.Scope)ToLua.CheckObject(L, 2, TypeTraits<GameApp.LogicGraph.GraphProperty.Scope>.type);
			obj.scope = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index scope on a nil value");
		}
	}
}

