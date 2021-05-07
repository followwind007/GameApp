---@field active FSMState
---@field states table<string, FSMState>
---@field vals table
---@class FSM
local FSM = DefineClass()

---@return FSM
function FSM.New()
    return FSM()
end

function FSM:Ctor()
    self.active = nil
    self.states = {}
    self.vals = {}

    self.listenedEvts = {}
    self.valueChangeCallback = {}
end

---@param id string
---@param param @param can be type of function or FSMState
function FSM:AddState(id, param)
    if type(param) == "function" then
        self.states[id] = FSMState(id, param)
    else
        self.states[id] = param
    end
end

---@param from string
---@param to string
---@param conditions FSMCondition[]
function FSM:AddLink(from, to, conditions)
    local fState = self.states[from]
    if fState then
        fState:AddNext(to, conditions)
    end
end

---@summary add the FSM constion value 
---@param name string @condition value name
---@param value @any type
---@param isListen boolean @optional: will listen the value change
---@param isTrigger boolean @optional: is value will be set as trigger
function FSM:AddValue(name, value, isListen, isTrigger)
    self.vals[name] = value
    if isListen and not self.listenedEvts[name] and dispatcher then
        local listenFunc
        if isTrigger then
            listenFunc = function(newVal) self:SetTrigger(name, newVal) end
        else
            listenFunc = function(newVal) self:SetValue(name, newVal) end
        end
        dispatcher:add(name, listenFunc)
        self.listenedEvts[name] = listenFunc
    end
end

---@param name string @condition value name
---@param callback function @is called when set value
function FSM:SetValueChangeCallback(name, callback)
    self.valueChangeCallback[name] = callback
end

---@param id string @state name
---@param enterCallback function @optional: will called after state enter
function FSM:PlayState(id, enterCallback)
    if id == nil or self.states[id] == nil then
        logWarn(string.format("can not find target state: %s", id))
        return
    end
    
    if self.active then self.active:OnStateExit() end
    
    self.active = self.states[id]
    if self.active == nil then return end
    self.active:OnStateEnter()
    
    if enterCallback then enterCallback() end
    
    self:Schedule()
end

function FSM:SetTrigger(id, val)
    local oldVal = self.vals[id]
    self.vals[id] = val
    if self.valueChangeCallback[id] then self.valueChangeCallback[id]() end
    
    local next = self.active:Next(self.vals)
    if next then
        self:PlayState(next, function() self:InternalResetValue(id, oldVal) end)
    else
        self:InternalResetValue(id, oldVal)
    end
end

function FSM:SetValue(id, val)
    self.vals[id] = val
    if self.valueChangeCallback[id] then self.valueChangeCallback[id]() end
    self:Schedule()
end

function FSM:Schedule()
    local next = self.active:Next(self.vals)
    if next then
        self:PlayState(next)
    end
end

function FSM:Dispose()
    for name, func in pairs(self.listenedEvts) do
        dispatcher:remove(name, func)
    end
end

function FSM:InternalResetValue(name, val)
    self.vals[name] = val
end

function FSM:ToString(printFlag)
    local res = "\n"
    res = res..string.format("-active: %s\n", self.active and self.active.id or "nil")
    
    res = res.."\n-vals:\n"
    for i, v in pairs(self.vals) do
        res = res..string.format("k:%s, v:%s\n", i, tostring(v))
    end
    
    res = res.."\n-states:\n"
    for _, v in pairs(self.states) do
        res = res..v:ToString(false)
    end
    
    if printFlag then print(res) end
    return res
end

return FSM

--[[
function FSMTest()
    local fsm = FSM.New()
    fsm:AddState("s1", function() print("in s1 state") end)
    fsm:AddState("s2", function() print("in s2 state") end)
    fsm:AddState("s3", function() print("in s3 state") end)
    
    fsm:AddValue("_msg", "", true)
    
    local c12 = FSMCondition.New(FSMCondition.Type.Equal, "_msg", "m1-2")
    fsm:AddLink("s1", "s2", {c12})
    
    local c23 = FSMCondition.New(FSMCondition.Type.Equal, "_msg", "m2-3")
    fsm:AddLink("s2", "s3", {c23})

    fsm:PlayState("s1")
    
    fsm:ToString(true)
    dispatcher:dispatch("_msg", "m1-2")
    fsm:ToString(true)
end
]] 