﻿//this source code was auto-generated by tolua#, do not modify it
using System;
using GameApp.DataBinder;
using LuaInterface;

public class GameApp_DataBinder_StateBinderWrap
{
	public static void Register(LuaState L)
	{
		L.BeginClass(typeof(GameApp.DataBinder.StateBinder), typeof(UnityEngine.StateMachineBehaviour));
		L.RegFunction("OnStateMachineEnter", new LuaCSFunction(OnStateMachineEnter));
		L.RegFunction("OnStateMachineExit", new LuaCSFunction(OnStateMachineExit));
		L.RegFunction("OnStateEnter", new LuaCSFunction(OnStateEnter));
		L.RegFunction("OnStateMove", new LuaCSFunction(OnStateMove));
		L.RegFunction("OnStateExit", new LuaCSFunction(OnStateExit));
		L.RegFunction("OnStateIK", new LuaCSFunction(OnStateIK));
		L.RegFunction("RegisterMethod", new LuaCSFunction(RegisterMethod));
		L.RegFunction("Init", new LuaCSFunction(Init));
		L.RegFunction("New", new LuaCSFunction(_CreateGameApp_DataBinder_StateBinder));
		L.RegFunction("__eq", new LuaCSFunction(op_Equality));
		L.RegFunction("__tostring", new LuaCSFunction(ToLua.op_ToString));
		L.RegVar("luaPath", new LuaCSFunction(get_luaPath), new LuaCSFunction(set_luaPath));
		L.RegVar("interfaceLuaPaths", new LuaCSFunction(get_interfaceLuaPaths), new LuaCSFunction(set_interfaceLuaPaths));
		L.RegVar("values", new LuaCSFunction(get_values), new LuaCSFunction(set_values));
		L.RegVar("LuaPath", new LuaCSFunction(get_LuaPath), null);
		L.RegVar("Interfaces", new LuaCSFunction(get_Interfaces), null);
		L.RegVar("Methods", new LuaCSFunction(get_Methods), new LuaCSFunction(set_Methods));
		L.RegVar("InitDone", new LuaCSFunction(get_InitDone), new LuaCSFunction(set_InitDone));
		L.RegVar("Vals", new LuaCSFunction(get_Vals), new LuaCSFunction(set_Vals));
		L.EndClass();
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int _CreateGameApp_DataBinder_StateBinder(IntPtr L)
	{
		try
		{
			int count = LuaDLL.lua_gettop(L);

			if (count == 0)
			{
				GameApp.DataBinder.StateBinder obj = new GameApp.DataBinder.StateBinder();
				ToLua.Push(L, obj);
				return 1;
			}
			else
			{
				return LuaDLL.luaL_throw(L, "invalid arguments to ctor method: GameApp.DataBinder.StateBinder.New");
			}
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int OnStateMachineEnter(IntPtr L)
	{
		try
		{
			int count = LuaDLL.lua_gettop(L);

			if (count == 3)
			{
				GameApp.DataBinder.StateBinder obj = (GameApp.DataBinder.StateBinder)ToLua.CheckObject<GameApp.DataBinder.StateBinder>(L, 1);
				UnityEngine.Animator arg0 = (UnityEngine.Animator)ToLua.CheckObject<UnityEngine.Animator>(L, 2);
				int arg1 = (int)LuaDLL.luaL_checkinteger(L, 3);
				obj.OnStateMachineEnter(arg0, arg1);
				return 0;
			}
			else if (count == 4)
			{
				GameApp.DataBinder.StateBinder obj = (GameApp.DataBinder.StateBinder)ToLua.CheckObject<GameApp.DataBinder.StateBinder>(L, 1);
				UnityEngine.Animator arg0 = (UnityEngine.Animator)ToLua.CheckObject<UnityEngine.Animator>(L, 2);
				int arg1 = (int)LuaDLL.luaL_checkinteger(L, 3);
				UnityEngine.Animations.AnimatorControllerPlayable arg2 = StackTraits<UnityEngine.Animations.AnimatorControllerPlayable>.Check(L, 4);
				obj.OnStateMachineEnter(arg0, arg1, arg2);
				return 0;
			}
			else
			{
				return LuaDLL.luaL_throw(L, "invalid arguments to method: GameApp.DataBinder.StateBinder.OnStateMachineEnter");
			}
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int OnStateMachineExit(IntPtr L)
	{
		try
		{
			int count = LuaDLL.lua_gettop(L);

			if (count == 3)
			{
				GameApp.DataBinder.StateBinder obj = (GameApp.DataBinder.StateBinder)ToLua.CheckObject<GameApp.DataBinder.StateBinder>(L, 1);
				UnityEngine.Animator arg0 = (UnityEngine.Animator)ToLua.CheckObject<UnityEngine.Animator>(L, 2);
				int arg1 = (int)LuaDLL.luaL_checkinteger(L, 3);
				obj.OnStateMachineExit(arg0, arg1);
				return 0;
			}
			else if (count == 4)
			{
				GameApp.DataBinder.StateBinder obj = (GameApp.DataBinder.StateBinder)ToLua.CheckObject<GameApp.DataBinder.StateBinder>(L, 1);
				UnityEngine.Animator arg0 = (UnityEngine.Animator)ToLua.CheckObject<UnityEngine.Animator>(L, 2);
				int arg1 = (int)LuaDLL.luaL_checkinteger(L, 3);
				UnityEngine.Animations.AnimatorControllerPlayable arg2 = StackTraits<UnityEngine.Animations.AnimatorControllerPlayable>.Check(L, 4);
				obj.OnStateMachineExit(arg0, arg1, arg2);
				return 0;
			}
			else
			{
				return LuaDLL.luaL_throw(L, "invalid arguments to method: GameApp.DataBinder.StateBinder.OnStateMachineExit");
			}
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int OnStateEnter(IntPtr L)
	{
		try
		{
			int count = LuaDLL.lua_gettop(L);

			if (count == 4)
			{
				GameApp.DataBinder.StateBinder obj = (GameApp.DataBinder.StateBinder)ToLua.CheckObject<GameApp.DataBinder.StateBinder>(L, 1);
				UnityEngine.Animator arg0 = (UnityEngine.Animator)ToLua.CheckObject<UnityEngine.Animator>(L, 2);
				UnityEngine.AnimatorStateInfo arg1 = StackTraits<UnityEngine.AnimatorStateInfo>.Check(L, 3);
				int arg2 = (int)LuaDLL.luaL_checkinteger(L, 4);
				obj.OnStateEnter(arg0, arg1, arg2);
				return 0;
			}
			else if (count == 5)
			{
				GameApp.DataBinder.StateBinder obj = (GameApp.DataBinder.StateBinder)ToLua.CheckObject<GameApp.DataBinder.StateBinder>(L, 1);
				UnityEngine.Animator arg0 = (UnityEngine.Animator)ToLua.CheckObject<UnityEngine.Animator>(L, 2);
				UnityEngine.AnimatorStateInfo arg1 = StackTraits<UnityEngine.AnimatorStateInfo>.Check(L, 3);
				int arg2 = (int)LuaDLL.luaL_checkinteger(L, 4);
				UnityEngine.Animations.AnimatorControllerPlayable arg3 = StackTraits<UnityEngine.Animations.AnimatorControllerPlayable>.Check(L, 5);
				obj.OnStateEnter(arg0, arg1, arg2, arg3);
				return 0;
			}
			else
			{
				return LuaDLL.luaL_throw(L, "invalid arguments to method: GameApp.DataBinder.StateBinder.OnStateEnter");
			}
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int OnStateMove(IntPtr L)
	{
		try
		{
			int count = LuaDLL.lua_gettop(L);

			if (count == 4)
			{
				GameApp.DataBinder.StateBinder obj = (GameApp.DataBinder.StateBinder)ToLua.CheckObject<GameApp.DataBinder.StateBinder>(L, 1);
				UnityEngine.Animator arg0 = (UnityEngine.Animator)ToLua.CheckObject<UnityEngine.Animator>(L, 2);
				UnityEngine.AnimatorStateInfo arg1 = StackTraits<UnityEngine.AnimatorStateInfo>.Check(L, 3);
				int arg2 = (int)LuaDLL.luaL_checkinteger(L, 4);
				obj.OnStateMove(arg0, arg1, arg2);
				return 0;
			}
			else if (count == 5)
			{
				GameApp.DataBinder.StateBinder obj = (GameApp.DataBinder.StateBinder)ToLua.CheckObject<GameApp.DataBinder.StateBinder>(L, 1);
				UnityEngine.Animator arg0 = (UnityEngine.Animator)ToLua.CheckObject<UnityEngine.Animator>(L, 2);
				UnityEngine.AnimatorStateInfo arg1 = StackTraits<UnityEngine.AnimatorStateInfo>.Check(L, 3);
				int arg2 = (int)LuaDLL.luaL_checkinteger(L, 4);
				UnityEngine.Animations.AnimatorControllerPlayable arg3 = StackTraits<UnityEngine.Animations.AnimatorControllerPlayable>.Check(L, 5);
				obj.OnStateMove(arg0, arg1, arg2, arg3);
				return 0;
			}
			else
			{
				return LuaDLL.luaL_throw(L, "invalid arguments to method: GameApp.DataBinder.StateBinder.OnStateMove");
			}
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int OnStateExit(IntPtr L)
	{
		try
		{
			int count = LuaDLL.lua_gettop(L);

			if (count == 4)
			{
				GameApp.DataBinder.StateBinder obj = (GameApp.DataBinder.StateBinder)ToLua.CheckObject<GameApp.DataBinder.StateBinder>(L, 1);
				UnityEngine.Animator arg0 = (UnityEngine.Animator)ToLua.CheckObject<UnityEngine.Animator>(L, 2);
				UnityEngine.AnimatorStateInfo arg1 = StackTraits<UnityEngine.AnimatorStateInfo>.Check(L, 3);
				int arg2 = (int)LuaDLL.luaL_checkinteger(L, 4);
				obj.OnStateExit(arg0, arg1, arg2);
				return 0;
			}
			else if (count == 5)
			{
				GameApp.DataBinder.StateBinder obj = (GameApp.DataBinder.StateBinder)ToLua.CheckObject<GameApp.DataBinder.StateBinder>(L, 1);
				UnityEngine.Animator arg0 = (UnityEngine.Animator)ToLua.CheckObject<UnityEngine.Animator>(L, 2);
				UnityEngine.AnimatorStateInfo arg1 = StackTraits<UnityEngine.AnimatorStateInfo>.Check(L, 3);
				int arg2 = (int)LuaDLL.luaL_checkinteger(L, 4);
				UnityEngine.Animations.AnimatorControllerPlayable arg3 = StackTraits<UnityEngine.Animations.AnimatorControllerPlayable>.Check(L, 5);
				obj.OnStateExit(arg0, arg1, arg2, arg3);
				return 0;
			}
			else
			{
				return LuaDLL.luaL_throw(L, "invalid arguments to method: GameApp.DataBinder.StateBinder.OnStateExit");
			}
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int OnStateIK(IntPtr L)
	{
		try
		{
			int count = LuaDLL.lua_gettop(L);

			if (count == 4)
			{
				GameApp.DataBinder.StateBinder obj = (GameApp.DataBinder.StateBinder)ToLua.CheckObject<GameApp.DataBinder.StateBinder>(L, 1);
				UnityEngine.Animator arg0 = (UnityEngine.Animator)ToLua.CheckObject<UnityEngine.Animator>(L, 2);
				UnityEngine.AnimatorStateInfo arg1 = StackTraits<UnityEngine.AnimatorStateInfo>.Check(L, 3);
				int arg2 = (int)LuaDLL.luaL_checkinteger(L, 4);
				obj.OnStateIK(arg0, arg1, arg2);
				return 0;
			}
			else if (count == 5)
			{
				GameApp.DataBinder.StateBinder obj = (GameApp.DataBinder.StateBinder)ToLua.CheckObject<GameApp.DataBinder.StateBinder>(L, 1);
				UnityEngine.Animator arg0 = (UnityEngine.Animator)ToLua.CheckObject<UnityEngine.Animator>(L, 2);
				UnityEngine.AnimatorStateInfo arg1 = StackTraits<UnityEngine.AnimatorStateInfo>.Check(L, 3);
				int arg2 = (int)LuaDLL.luaL_checkinteger(L, 4);
				UnityEngine.Animations.AnimatorControllerPlayable arg3 = StackTraits<UnityEngine.Animations.AnimatorControllerPlayable>.Check(L, 5);
				obj.OnStateIK(arg0, arg1, arg2, arg3);
				return 0;
			}
			else
			{
				return LuaDLL.luaL_throw(L, "invalid arguments to method: GameApp.DataBinder.StateBinder.OnStateIK");
			}
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int RegisterMethod(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			GameApp.DataBinder.StateBinder obj = (GameApp.DataBinder.StateBinder)ToLua.CheckObject<GameApp.DataBinder.StateBinder>(L, 1);
			string arg0 = ToLua.CheckString(L, 2);
			obj.RegisterMethod(arg0);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Init(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			GameApp.DataBinder.StateBinder obj = (GameApp.DataBinder.StateBinder)ToLua.CheckObject<GameApp.DataBinder.StateBinder>(L, 1);
			obj.Init();
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
	static int get_luaPath(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			GameApp.DataBinder.StateBinder obj = (GameApp.DataBinder.StateBinder)o;
			string ret = obj.luaPath;
			LuaDLL.lua_pushstring(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index luaPath on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_interfaceLuaPaths(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			GameApp.DataBinder.StateBinder obj = (GameApp.DataBinder.StateBinder)o;
			System.Collections.Generic.List<GameApp.DataBinder.LuaPath> ret = obj.interfaceLuaPaths;
			ToLua.PushSealed(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index interfaceLuaPaths on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_values(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			GameApp.DataBinder.StateBinder obj = (GameApp.DataBinder.StateBinder)o;
			GameApp.DataBinder.BindableValues ret = obj.values;
			ToLua.PushObject(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index values on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_LuaPath(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			GameApp.DataBinder.StateBinder obj = (GameApp.DataBinder.StateBinder)o;
			string ret = obj.LuaPath;
			LuaDLL.lua_pushstring(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index LuaPath on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_Interfaces(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			GameApp.DataBinder.StateBinder obj = (GameApp.DataBinder.StateBinder)o;
			System.Collections.Generic.List<GameApp.DataBinder.LuaPath> ret = obj.Interfaces;
			ToLua.PushSealed(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index Interfaces on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_Methods(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			GameApp.DataBinder.StateBinder obj = (GameApp.DataBinder.StateBinder)o;
			System.Collections.Generic.HashSet<string> ret = obj.Methods;
			ToLua.PushObject(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index Methods on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_InitDone(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			GameApp.DataBinder.StateBinder obj = (GameApp.DataBinder.StateBinder)o;
			bool ret = obj.InitDone;
			LuaDLL.lua_pushboolean(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index InitDone on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_Vals(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			GameApp.DataBinder.StateBinder obj = (GameApp.DataBinder.StateBinder)o;
			GameApp.DataBinder.BindableValues ret = obj.Vals;
			ToLua.PushObject(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index Vals on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_luaPath(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			GameApp.DataBinder.StateBinder obj = (GameApp.DataBinder.StateBinder)o;
			string arg0 = ToLua.CheckString(L, 2);
			obj.luaPath = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index luaPath on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_interfaceLuaPaths(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			GameApp.DataBinder.StateBinder obj = (GameApp.DataBinder.StateBinder)o;
			System.Collections.Generic.List<GameApp.DataBinder.LuaPath> arg0 = (System.Collections.Generic.List<GameApp.DataBinder.LuaPath>)ToLua.CheckObject(L, 2, TypeTraits<System.Collections.Generic.List<GameApp.DataBinder.LuaPath>>.type);
			obj.interfaceLuaPaths = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index interfaceLuaPaths on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_values(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			GameApp.DataBinder.StateBinder obj = (GameApp.DataBinder.StateBinder)o;
			GameApp.DataBinder.BindableValues arg0 = (GameApp.DataBinder.BindableValues)ToLua.CheckObject<GameApp.DataBinder.BindableValues>(L, 2);
			obj.values = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index values on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_Methods(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			GameApp.DataBinder.StateBinder obj = (GameApp.DataBinder.StateBinder)o;
			System.Collections.Generic.HashSet<string> arg0 = (System.Collections.Generic.HashSet<string>)ToLua.CheckObject<System.Collections.Generic.HashSet<string>>(L, 2);
			obj.Methods = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index Methods on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_InitDone(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			GameApp.DataBinder.StateBinder obj = (GameApp.DataBinder.StateBinder)o;
			bool arg0 = LuaDLL.luaL_checkboolean(L, 2);
			obj.InitDone = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index InitDone on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_Vals(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			GameApp.DataBinder.StateBinder obj = (GameApp.DataBinder.StateBinder)o;
			GameApp.DataBinder.BindableValues arg0 = (GameApp.DataBinder.BindableValues)ToLua.CheckObject<GameApp.DataBinder.BindableValues>(L, 2);
			obj.Vals = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index Vals on a nil value");
		}
	}
}
