
---@field public str string |"'test1'"|"'test2'"
---@field public str2 string
---@field public color UnityEngine.Color
---@class BehaviourTree.TestAction : BehaviourTree.Task
local TestAction = DefineClass(BehaviourTree.Task)

function TestAction:OnAwake()
    print("TestAction OnAwake()", self.str)
    Timer.New(function() self.binder.Owner:SendEventEx("test", "abc") end, 2, 1):Start()
end

function TestAction:OnStart()
    print("TestAction OnStart()")
end

function TestAction:OnUpdate()
    return BehaviourTree.TS.Success
end

function TestAction:OnEnd()
    print("TestAction OnEnd()")
end

return TestAction
