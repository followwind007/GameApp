
Object = UnityEngine.Object

--允许使用的全局变量列表
local declaredNames = {
    
}

function declare (name, initval)
    rawset(_G, name, initval)
    declaredNames[name] = true
    return rawget(_G, name)
end

function declareList(tbl)
    if type(tbl) ~= "table" then return end
    for _, v in pairs(tbl) do
        declaredNames[v] = true
    end
end

setmetatable(_G, {
    __newindex = function (t, n, v)
        rawset(t, n, v)
        if not declaredNames[n] then
            if type(v) == "table" and rawget(v, "__isClass") then
                return
            end
            local info = debug.getinfo(2) or {}
            local notice = "define in declaredNames@app/common/define or use declare(name, initval)"
            local msg = string.format("attempt to write undeclared global %s(%s) %s: %s\n%s", n, type(v), info.source, info.currentline, notice)
            print(msg)
        end
    end,
    __index = function (_, n)
        if not declaredNames[n] then
            --UnityEngine.Debug.LogWarning("attempt to read undeclared var: "..n)
        end
    end,
})