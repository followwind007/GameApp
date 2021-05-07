
---@class LogicGraph.Property
local Property = DefineClass()

---@param target Elua
---@param field string
---@return Elua
function Property.Get(target, field)
    return target[field]
end

---@param target Elua
---@param field string
---@param value Elua
function Property.Set(target, field, value)
    target[field] = value
end

---@param name string
---@return Elua
function Property.GetGlobal(name)
    local global = _G
    return global[name]
end

return Property