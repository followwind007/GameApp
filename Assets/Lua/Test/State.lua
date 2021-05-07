
---@field public str1 string
---@class State
local State = DefineClass()

function State:OnStateEnter()
    print("----------enter state---------")
end

return State
