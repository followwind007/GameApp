
#include "lualib.h"
#include "lauxlib.h"
#include "elib.h"

void int pushubox(lua_State *L) {
	lua_newtable(L); //tbl = {}
	lua_newtable(L); //meta = {}
	lua_pushstring(L, "__mode");
	lua_pushstring(L, "v");
	lua_rawset(L, -3); //meta.__mode = "v"
	lua_setmetatable(L, -2); //setmetatable(tbl, meta)

	int idx = luaL_ref(L, LUA_REGISTRYINDEX);
	lua_rawseti(L, LUA_REGISTRYINDEX, idx);
	return idx;
}