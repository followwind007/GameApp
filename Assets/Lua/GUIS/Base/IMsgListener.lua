---@class IMsgListener
local IMsgListener = DefineClass()

---@type table<string, string>
IMsgListener.messages = {}

function IMsgListener:Register()
    if type(self.messages) ~= "table" then return end
    for msgName, func in pairs(self.messages) do
        dispatcher:add(msgName, func, self)
    end
end

function IMsgListener:Deregister()
    if type(self.messages) ~= "table" then return end
    for msgName, func in pairs(self.messages) do
        dispatcher:remove(msgName, func, self)
    end
end

return IMsgListener