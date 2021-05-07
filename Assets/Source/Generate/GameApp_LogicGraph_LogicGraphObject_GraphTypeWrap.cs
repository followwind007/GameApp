﻿//this source code was auto-generated by tolua#, do not modify it
using System;
using LuaInterface;

public class GameApp_LogicGraph_LogicGraphObject_GraphTypeWrap
{
	public static void Register(LuaState L)
	{
		L.BeginEnum(typeof(GameApp.LogicGraph.LogicGraphObject.GraphType));
		L.RegVar("Flow", new LuaCSFunction(get_Flow), null);
		L.RegVar("State", new LuaCSFunction(get_State), null);
		L.RegFunction("IntToEnum", new LuaCSFunction(IntToEnum));
		L.EndEnum();
		TypeTraits<GameApp.LogicGraph.LogicGraphObject.GraphType>.Check = CheckType;
		StackTraits<GameApp.LogicGraph.LogicGraphObject.GraphType>.Push = Push;
	}

	static void Push(IntPtr L, GameApp.LogicGraph.LogicGraphObject.GraphType arg)
	{
		ToLua.Push(L, arg);
	}

	static Type TypeOf_GameApp_LogicGraph_LogicGraphObject_GraphType = typeof(GameApp.LogicGraph.LogicGraphObject.GraphType);

	static bool CheckType(IntPtr L, int pos)
	{
		return TypeChecker.CheckEnumType(TypeOf_GameApp_LogicGraph_LogicGraphObject_GraphType, L, pos);
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_Flow(IntPtr L)
	{
		ToLua.Push(L, GameApp.LogicGraph.LogicGraphObject.GraphType.Flow);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_State(IntPtr L)
	{
		ToLua.Push(L, GameApp.LogicGraph.LogicGraphObject.GraphType.State);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int IntToEnum(IntPtr L)
	{
		int arg0 = (int)LuaDLL.lua_tointeger(L, 1);
		GameApp.LogicGraph.LogicGraphObject.GraphType o = (GameApp.LogicGraph.LogicGraphObject.GraphType)arg0;
		ToLua.Push(L, o);
		return 1;
	}
}

