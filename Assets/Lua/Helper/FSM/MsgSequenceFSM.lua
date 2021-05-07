
---@field fsm FSM
---@class MsgSequenceFSM
local MsgSequenceFSM = DefineClass()

---@param func function @function will be called at start
---@return MsgSequenceFSM
function MsgSequenceFSM.New(func)
    return MsgSequenceFSM(func)
end

function MsgSequenceFSM:Ctor(func)
    self.stateIdCount = 0
    self.fsm = FSM.New()
    
    self.stateStart = FSMState.New(self:GetStateId(), func)
    self.fsm:AddState(self.stateStart.id, self.stateStart)
    
    self.statePre = self.stateStart
end

---@param func function @function will be called when message received
---@param msgName string @message name
---@param msgContent string @message content
---@param isTrigger boolean @value set as a trigger, will not persist
function MsgSequenceFSM:Add(func, msgName, msgContent, isTrigger)
    local stateId = self:GetStateId()
    local state = FSMState.New(stateId, func)
    
    self.fsm:AddState(stateId, state)
    self.fsm:AddValue(msgName, nil, true, isTrigger)
    
    local condition = FSMCondition.New(FSMCondition.Type.Equal, msgName, msgContent)
    if self.statePre then
        self.fsm:AddLink(self.statePre.id, stateId, {condition})
    end
    self.statePre = state
end

function MsgSequenceFSM:Start()
    if self.stateStart then
        self.fsm:PlayState(self.stateStart.id)
    end
end

function MsgSequenceFSM:GetStateId()
    self.stateIdCount = self.stateIdCount + 1
    return tostring(self.stateIdCount)
end

return MsgSequenceFSM

--[[
local function MsgSeqTest()
    local mf = MsgSequenceFSM.New(function() print("seq start s1") end)
    mf:Add(function() print("in s2") end, "smsg", "m1-2")
    mf:Add(function() print("is s3") end, "smsg", "m2-3")
    mf.fsm:ToString(true)
    mf:Start()
    mf.fsm:ToString(true)
    dispatcher:dispatch("smsg", "m1-2")
    mf.fsm:ToString(true)
    dispatcher:dispatch("smsg", "m2-3")
    mf.fsm:ToString(true)
end

MsgSeqTest()
]]