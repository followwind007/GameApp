---@field Inactive number
---@field Failure number
---@field Success number
---@field Running number
---@class BehaviourTree.TaskStatus
local Status = DefineClass()

local _vals = {
    Inactive = 0,
    Failure = 1,
    Success = 2,
    Running = 3,
}

function Status.__index(_, k)
    return _vals[k]
end

function Status.__newindex(_, _, _) end

return Status