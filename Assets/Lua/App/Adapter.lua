local IsSubClassOf = GameApp.Util.Utils.IsSubClassOf

Adapter = {}

local this = Adapter
this.objs = {}

function Adapter.Init(obj, path)
    path = this.GetUniPath(path)
    local Cls = require(path)
    if not Cls.__isClass then
        error("try to get none class object is not supported!")
        return
    end

    local tbl = Cls()
    tbl.__binder = obj
    tbl.args = {}
    if IsSubClassOf(obj, typeof(UnityEngine.Component)) then
        tbl.gameObject = obj.gameObject
        tbl.transform = tbl.gameObject.transform
        tbl.args = {gameObject = tbl.gameObject, transform = tbl.transform}
    end
    this.RegisterMethods(obj, tbl)

    this.objs[obj:GetInstanceID()] = tbl
    this.LoadData(tbl)
end

local RegisterFuncs = {"OnApplicationPause", "OnApplicationFocus", "Update", "LateUpdate"}
---@summary functions need to be checked before call lua
function Adapter.RegisterMethods(obj, tbl)
    for _, func in ipairs(RegisterFuncs) do
        if tbl[func] then obj:RegisterMethod(func) end
    end
end

function Adapter.LoadData(tbl)
    if tbl == nil or tbl.__binder == nil then
        print("load data fail")
        return
    end

    ---@type GameApp.DataBinder.BindableValues
    local binds = tbl.__binder.Vals
    local iter = binds:GetDataEnumerator()
    while iter:MoveNext() do
        local k = iter.Current.Key
        local v = iter.Current.Value
        tbl[k] = v
        tbl.args[k] = v
    end
end

function Adapter.LoadAllData()
    for _, tbl in pairs(this.objs) do
        this.LoadData(tbl)
    end
end

function Adapter.Call(obj, method, ...)
    local tbl = this.objs[obj:GetInstanceID()]
    if tbl == nil then return end
    local func = tbl[method]

    if func then func(tbl, ...) end

    if method == "OnDestroy" then
        this.objs[obj:GetInstanceID()] = nil
    end
end

---@param obj UnityEngine.Object
function Adapter.GetLuaTable(obj)
    if not IsNil(obj) and obj:GetType() == typeof(UnityEngine.GameObject) then
        obj = obj:GetComponent("DataBinder")
    end
    if obj == nil then return end

    local id = obj:GetInstanceID()
    if this.objs[id] == nil then
        obj:Init()
    end
    return this.objs[id]
end

function Adapter.Reload(path)
    path = this.GetUniPath(path)
    package.preload[path] = nil
    package.loaded[path] = nil
    local Cls = require(path)

    this.ReloadContent(path, Cls)
end

function Adapter.ReloadContent(path, content)
    path = this.GetUniPath(path)
    content.__path = path

    package.preload[path] = content
    package.loaded[path] = content

    for _, tbl in pairs(this.objs) do
        if tbl.__path and tbl.__path == path then
            setmetatable(tbl, content)
            tbl.__class = content
        end
    end
end

function Adapter.ReloadAll()
    local modules = {}
    for _, tbl in pairs(this.objs) do
        if modules[tbl.__path] == nil then
            modules[tbl.__path] = 1
        else
            modules[tbl.__path] = modules[tbl.__path] + 1
        end
    end
    for module, count in pairs(modules) do
        if count > 0 then
            this.Reload(module)
        end
    end
end

function Adapter.GetUniPath(path)
    return string.gsub(path, "/", ".")
end

function Adapter.Profile()
    dump(this.objs, "objs:", 2)
end

return Adapter