
---@field public vec2 UnityEngine.Vector2
---@class BehaviourTree.TestAction2 : BehaviourTree.Task
local TestAction2 = DefineClass(BehaviourTree.Task)

function TestAction2:OnAwake()
    print("TestAction2 OnAwake()", self.vec2.x, self.vec2.y)
end

function TestAction2:OnStart()
    print("TestAction2 OnStart()")
end

function TestAction2:OnUpdate()
    local ab = self.binder.Owner:GetActionBinder("ab_test1")
    if ab then print(ab.Vals:GetData("str")) end
    return BehaviourTree.TS.Success
end

function TestAction2:OnEnd()
    print("TestAction2 OnEnd()")
end

return TestAction2
