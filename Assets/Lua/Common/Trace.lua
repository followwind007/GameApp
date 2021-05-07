local debug = debug
local pairs = pairs
local string = string

local trace = {}

trace.started = false

local focuses = {
    func = {},
    file = {},
}

local function hook(hooktype)
    --hooktype : "call", "return"
    local info = debug.getinfo(2, 'nS')
    for i, v in pairs(focuses.file) do
        if info.short_src and string.find(info.short_src, i) then
            if v then v(info, hooktype) end
        end
    end
    for i, v in pairs(focuses.func) do
        if info.name and info.name == i then
            if v then v(info, hooktype) end
        end
    end
end

local function count(tbl)
    local c = 0
    for _, v in pairs(tbl) do
        if v ~= nil then c = c + 1 end
    end
    return c
end

function trace.start(focus, func, focusType)
    if focusType == nil then focusType = "func" end
    focuses[focusType][focus] = func
    if not trace.started then
        debug.sethook(hook, "cr")
    end
end

function trace.finish(focus, focusType)
    if focusType == nil then focusType = "func" end
    
    focuses[focusType][focus] = nil
    
    if count(focuses.func) < 1 and count(focuses.file) < 1 then
        debug.sethook()
        trace.started = false
    end
end

return trace