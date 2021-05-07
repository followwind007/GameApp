#elua contains lua and some util libs
cmake_minimum_required (VERSION 2.8)

project(Elua)

set(LUA_SRC_PATH lua-5.3.5/src/)
set(CORE_SRC_PATH core/src/)
set(CJSON_SRC_PATH lua-cjson/)
set(SOCKET_SRC_PATH luasocket/)

#include
include_directories(./ ./${LUA_SRC_PATH} ./${CORE_SRC_PATH} ./${CJSON_SRC_PATH} ./${SOCKET_SRC_PATH})

#lua_src list
file(GLOB lua_src ${LUA_SRC_PATH}*.c)
list(FILTER lua_src EXCLUDE REGEX ".*lua.c|luac.c$")

#core_src list
file(GLOB core_src ${CORE_SRC_PATH}.c)

#cjson_src list
file(GLOB cjson_src ${CJSON_SRC_PATH}*.c)

#socket_src list
file(GLOB socket_src ${SOCKET_SRC_PATH}*.c)
list(FILTER socket_src EXCLUDE REGEX ".*luasocket_script.c|serial.c|unix.c|usocket.c|wsocket.c$")

if (WIN32 AND NOT CYGWIN)
    list(APPEND socket_src luasocket/wsocket.c)
else ()
    list(APPEND socket_src luasocket/usocket.c)
endif ()

#src
set(APPEND_PATH ${core_src} ${cjson_src} ${socket_src})
set(SRC_PATH ${lua_src} ${APPEND_PATH})

#elua lib
add_library(lib_elua ${SRC_PATH})
set_target_properties(lib_elua PROPERTIES OUTPUT_NAME elua)

#elua dll
add_library(dll_elua SHARED ${SRC_PATH})
target_compile_definitions (dll_elua PRIVATE _CRT_SECURE_NO_WARNINGS LUA_BUILD_AS_DLL)
set_target_properties(dll_elua PROPERTIES OUTPUT_NAME elua)

#property
macro(set_append_property src_path)
	set_property(
		SOURCE ${${src_path}}
		APPEND
		PROPERTY COMPILE_DEFINITIONS
		LUA_COMPAT_APIINTCASTS LUA_LIB _CRT_SECURE_NO_WARNINGS _WINSOCK_DEPRECATED_NO_WARNINGS)
endmacro(set_append_property)

foreach(src_path ${APPEND_PATH})
	set_append_property(src_path)
endforeach(src_path)

#link libraries for platform
#ws2_32 for win socket
if (WIN32 AND NOT CYGWIN)
    target_link_libraries(dll_elua ws2_32)
else ()
    target_link_libraries(dll_elua m)
endif ()

#elua executable
add_executable(elua ${LUA_SRC_PATH}lua.c)
target_link_libraries(elua dll_elua)