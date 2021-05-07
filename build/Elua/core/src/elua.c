
#include <string.h>
#include <math.h>

#include "elua.h"
#include "elib.h"

int ubox = -1;

LUA_API void elua_openlibs(lua_State *L) {
	ubox = pushubox(L);
}