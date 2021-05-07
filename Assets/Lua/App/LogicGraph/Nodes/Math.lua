
---@class LogicGraph.Math
local Math = DefineClass()

---@param a Elua
---@param b Elua
---@return Elua
function Math.Add(a, b)
    return a + b
end

---@param a Elua
---@param b Elua
---@return Elua
function Math.Minus(a, b)
    return a - b
end

return Math