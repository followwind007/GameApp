﻿//this source code was auto-generated by tolua#, do not modify it
using System;
using LuaInterface;

public class GameApp_Util_UtilsWrap
{
	public static void Register(LuaState L)
	{
		L.BeginStaticLibs("Utils");
		L.RegFunction("IsSubClassOf", new LuaCSFunction(IsSubClassOf));
		L.RegFunction("GetTimeStamp", new LuaCSFunction(GetTimeStamp));
		L.RegFunction("StartWatch", new LuaCSFunction(StartWatch));
		L.RegFunction("StopWatch", new LuaCSFunction(StopWatch));
		L.RegFunction("ReadWatch", new LuaCSFunction(ReadWatch));
		L.RegFunction("ReadResetWatch", new LuaCSFunction(ReadResetWatch));
		L.EndStaticLibs();
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int IsSubClassOf(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			object arg0 = ToLua.ToVarObject(L, 1);
			System.Type arg1 = ToLua.CheckMonoType(L, 2);
			bool o = GameApp.Util.Utils.IsSubClassOf(arg0, arg1);
			LuaDLL.lua_pushboolean(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetTimeStamp(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 0);
			int o = GameApp.Util.Utils.GetTimeStamp();
			LuaDLL.lua_pushinteger(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int StartWatch(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			string arg0 = ToLua.CheckString(L, 1);
			GameApp.Util.Utils.StartWatch(arg0);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int StopWatch(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			string arg0 = ToLua.CheckString(L, 1);
			GameApp.Util.Utils.StopWatch(arg0);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int ReadWatch(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			string arg0 = ToLua.CheckString(L, 1);
			int o = GameApp.Util.Utils.ReadWatch(arg0);
			LuaDLL.lua_pushinteger(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int ReadResetWatch(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			string arg0 = ToLua.CheckString(L, 1);
			int o = GameApp.Util.Utils.ReadResetWatch(arg0);
			LuaDLL.lua_pushinteger(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}
}

