
local function GetBaseList(obj, tbl)
    if obj then
        tbl[#tbl + 1] = obj
    end

    if obj.__bases then
        for _, base in ipairs(obj.__bases) do
            GetBaseList(base, tbl)
        end
    end
end

local function GetClass(...)
    local Cls = {}
    local args = {...}

    Cls.__path = string.gsub(debug.getinfo(3).short_src, "/", ".")

    Cls.Ctor = function() end
    Cls.ctor = Cls.Ctor
    Cls.__isClass = true

    Cls.__bases = Cls.__bases and {}
    for i = 1, #args do
        local base = args[i]
        local baseType = type(base)
        if baseType == "function" then
            assert(Cls.__create == nil,
                    string.format("Class() - create class with more than one creating function"));
            Cls.__create = base
        elseif baseType == "table" then
            Cls.__bases = Cls.__bases or {}
            Cls.__bases[#Cls.__bases + 1] = base
            if not Cls._base then
                Cls._base, Cls.base = base, base
            end
        else
            error(string.format("Class() - create class with invalid base type"), 0)
        end
    end
    Cls.__index = Cls

    local _M = {}
    if Cls.__bases and #Cls.__bases == 1 then
        _M.__index = Cls._base
    elseif Cls.__bases and #Cls.__bases > 1 then
        _M.__index = function(_, key)
            for i = 1, #Cls.__bases do
                local base = Cls.__bases[i]
                if base[key] then return base[key] end
            end
        end
    end

    setmetatable(Cls, _M)
    return Cls
end

function Class(...)
    local Cls = GetClass(...)
    local _M = getmetatable(Cls)

    _M.__call = function (_, ...)
        local inst = Cls.__create and Cls.__create(...) or {}
        inst.__class = Cls
        setmetatable(inst, Cls)

        local baseList = {}
        GetBaseList(Cls, baseList)

        for i = #baseList, 1, -1 do
            local ctor = rawget(baseList[i], "ctor")
            local Ctor = rawget(baseList[i], "Ctor")
            if ctor then ctor(inst, ...) end
            if Ctor then Ctor(inst, ...) end
        end
        return inst
    end

    return Cls
end

--不调用基类构造函数
function DefineClass(...)
    local Cls = GetClass(...)
    local _M = getmetatable(Cls)

    _M.__call = function (_, ...)
        local inst = Cls.__create and Cls.__create(...) or {}
        setmetatable(inst, Cls)
        inst.__class = Cls
        local ctor = rawget(Cls, "ctor")
        local Ctor = rawget(Cls, "Ctor")
        if ctor then ctor(inst, ...) end
        if Ctor then Ctor(inst, ...) end

        return inst
    end

    return Cls
end