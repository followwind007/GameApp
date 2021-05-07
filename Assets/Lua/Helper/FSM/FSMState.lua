
---@field nextId string
---@field conditions FSMCondition[]
---@class FSMConditions
local FSMConditions = DefineClass()
function FSMConditions:Ctor(nextId, conditions)
    self.nextId = nextId
    self.conditions = conditions
end

---@field id string
---@field func function
---@field nexts FSMConditions[]
---@class FSMState
local FSMState = DefineClass()

---@param id string @id of state
---@param enterFunc function @function will be called when enter state
---@param exitFunc function @function will be called when exit state
---@return FSMState
function FSMState.New(id, enterFunc, exitFunc)
    return FSMState(id, enterFunc, exitFunc)
end

function FSMState:Ctor(id, enterFunc, exitFunc)
    self.id = id
    self.enterFunc = enterFunc
    self.exitFunc = exitFunc
    self.nexts = {}
end

function FSMState:OnStateEnter()
    if self.enterFunc then
        self.enterFunc()
    end
end

function FSMState:OnStateExit()
    if self.exitFunc then 
        self.exitFunc() 
    end
end

---@param vals table @values of FSM param
---@return string @next state id
function FSMState:Next(vals)
    for _, cl in ipairs(self.nexts) do
        local valid = true
        for _, condition in ipairs(cl.conditions) do
            if not condition:IsValid(vals[condition.name]) then
                valid = false
                break
            end
        end
        if valid then return cl.nextId end
    end
end

---@param nextId string
---@param conditions FSMCondition[]
function FSMState:AddNext(nextId, conditions)
    table.insert(self.nexts, FSMConditions(nextId, conditions))
end

function FSMState:ToString(printFlag)
    local res = ""
    res = res..string.format("id: %s, func: %s\n", self.id, tostring(self.enterFunc))
    for _, v in ipairs(self.nexts) do
        res = res.."--conditions:\n"
        for _, c in ipairs(v.conditions) do
            res = res..c:ToString(false)
        end
    end
    res = res.."\n"
    if printFlag then print(res) end
    return res
end

return FSMState