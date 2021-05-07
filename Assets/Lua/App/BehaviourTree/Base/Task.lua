---@field binder BehaviorDesigner.Runtime.Tasks.ActionBinder
---@class BehaviourTree.Task
local Task = DefineClass()

function Task:OnAwake() end

function Task:OnStart() end

---@return BehaviourTree.TaskStatus
function Task:OnUpdate()
    return self.Status.Inactive
end

---@param flag boolean
function Task:OnPause(flag) end

function Task:OnBehaviorComplete() end

function Task:OnEnd() end

function Task:OnBehaviorRestart() end

function Task:OnReset() end

return Task