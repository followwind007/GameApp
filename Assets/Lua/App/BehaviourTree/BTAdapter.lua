--Adapter for behaviour tree

BTAdapter = declare("BTAdapter", {})

local MethodType = {
    OnAwake = 0,
    OnStart = 1,
    OnUpdate = 2,
    OnPause = 3,
    OnBehaviorComplete = 4,
    OnEnd = 5,
    OnBehaviorRestart = 6,
    OnReset = 7,
}

local this = BTAdapter
this.objs = {}

function BTAdapter.Call(obj, method, ...)
    ---@type BehaviourTree.Task
    local tbl = Adapter.GetLuaTable(obj)
    if tbl == nil then return end

    if method == MethodType.OnUpdate then
        return tbl:OnUpdate()
    elseif method == MethodType.OnAwake then
        tbl:OnAwake()
    elseif method == MethodType.OnStart then
        tbl:OnStart()
    elseif method == MethodType.OnPause then
        tbl:OnPause(...)
    elseif method == MethodType.OnBehaviorComplete then
        tbl:OnBehaviorComplete()
        Adapter.objs[obj:GetInstanceID()] = nil
    elseif method == MethodType.OnEnd then
        tbl:OnEnd()
    elseif method == MethodType.OnBehaviorRestart then
        tbl:OnBehaviorRestart()
    elseif method == MethodType.OnReset then
        tbl:OnReset()
    end
end

return BTAdapter