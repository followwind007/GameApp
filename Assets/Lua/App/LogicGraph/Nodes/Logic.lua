
---@class LogicGraph.Logic
local Logic = DefineClass()

---@input in schedule
---@output out schedule
---@param callback fun(param1:number)
---@param startValue number
---@param endValue number
---@param step number
function Logic.For(callback, startValue, endValue, step)
    for i = startValue, endValue, step do
        callback(i)
    end
end

---@input in schedule
---@output out schedule
---@param callback fun
function Logic.Do(callback)
    callback()
end

return Logic