
local ilist = ilist

---@class Event.Dispatcher
local EventDispatcher = DefineClass()

---@field func function
---@field from table
---@field priority number
---@class Event.Dispatcher.Item
local Item = DefineClass()

function Item.New(func, from, priority)
    return Item(func, from, priority)
end

function Item:Ctor(func, from, priority)
    self.func = func
    self.from = from
    self.priority = priority
end

---@param a Event.Dispatcher.Item
---@param b Event.Dispatcher.Item
function Item.__eq(a, b)
    return a.func == b.func and a.from == b.from
end

local DefaultPriority = 65535

function EventDispatcher:Ctor()
    self.eventListener = {}
    self.locked = nil
    self.used = false
    self.commands = list:new()

    self.onDispatch = {}
end

---@summary 添加监听
---@param eventName string @事件名, 通常为字符串
---@param func function @事件回调函数
---@param from table @如果回调函数是table中的方法, 并且需要使用self, 需提供self参数
---@param priority number @事件处理优先级, 请不要大于默认上限65535
function EventDispatcher:add(eventName, func, from, priority)
    if eventName == nil then
        print("param [1] eventName is nil")
        return
    end
    if self.locked == eventName then
        self.commands:push({method = "add", eventName = eventName, func = func, from = from, priority = priority})
        return
    end
    if type(func) ~= "function" then
        print("param [2] should be function for eventName:", eventName)
        return
    end
    if priority and priority > DefaultPriority then
        print("priority should not bigger than 65535", eventName)
    end

    ---@type Event.Dispatcher.Item
    local lt = Item.New(func, from, priority or DefaultPriority)

    local listeners = self:getListener(eventName, true)

    if listeners:find(lt) then
        print(eventName, "listener already exsit, don't add repeatedly!")
        return
    end

    if lt.priority == DefaultPriority then
        listeners:push(lt)
    else
        local iter = listeners:back()
        for i, v in ilist(listeners) do
            if lt.priority <= v.priority then
                iter = i._prev
                break
            end
        end
        listeners:insert(lt, iter)
    end
end

---@summary 移除监听
---@param eventName string @事件名, 通常为字符串
---@param func function @事件回调函数
---@param from table @如果回调函数是table中的方法, 并且需要使用self, 需提供self参数
function EventDispatcher:remove(eventName, func, from)
    if self.locked == eventName then
        self.commands:push({method = "remove", eventName = eventName, func = func, from = from})
        return
    end
    if type(func) ~= "function" then
        print("can't remove "..eventName..", need function (and) source param, use removeAllListener() instead!")
        return false
    end
    local listeners = self:getListener(eventName)
    local isExist, iter = self:checkExistListener(listeners, func, from)
    if isExist then
        listeners:remove(iter)
        if listeners.length == 0 then
            self.eventListener[eventName] = nil
        end
        return true
    end
    return false
end

function EventDispatcher:scheduleCommands()
    if self.commands.length < 1 then return end
    for _, v in ilist(self.commands) do
        self[v.method](self, v.eventName, v.func, v.from, v.priority)
    end
    self.commands:clear()
end

function EventDispatcher:removeAll(eventName)
    print("dispatcher.RemoveAll is deprecated, please use remove or IMsgListener")
    local listeners = self:getListener(eventName)
    if listeners then
        self.eventListener[eventName] = nil
    end
end

function EventDispatcher:addList(eventList, from)
    if type(eventList) ~= "table" then return end
    for eventName, func in pairs(eventList) do
        self:add(eventName, func, from)
    end
end

function EventDispatcher:removeList(eventList, from)
    if type(eventList) ~= "table" then return end
    for eventName, func in pairs(eventList) do
        self:remove(eventName, func, from)
    end
end

---@param eventName string @事件名
function EventDispatcher:dispatch(eventName, ...)
    if eventName == nil then return end
    for _, v in pairs(self.onDispatch) do
        v(eventName, ...)
    end
    local listeners = self:getListener(eventName)
    if listeners == nil then return end
    self.locked = eventName
    self.used = false
    for _, lt in ilist(listeners) do
        if self.used then break end
        if lt and lt.func and lt.from then
            lt.func(lt.from, ...)
        elseif lt and lt.func then
            lt.func(...)
        end
    end
    self.used = false
    self.locked = nil
    self:scheduleCommands()
end

---@summary 消费当前消息, 不会再向后传递
function EventDispatcher:use()
    self.used = true
end

function EventDispatcher:checkExistListener(listeners, func, from)
    if listeners == nil then return false, nil end
    for i, lt in ilist(listeners) do
        if lt ~= nil and lt.func == func and lt.from == from then
            return true, i
        end
    end
    return false, nil
end

---@return list
function EventDispatcher:getListener(eventName, createFlag)
    local listeners = self.eventListener[eventName]
    if listeners == nil then
        listeners = list.new()
        if createFlag == true then
            self.eventListener[eventName] = listeners
        end
    end
    return listeners
end

function EventDispatcher:profile(eventName)
    if eventName then
        self:profileSingle(eventName)
        return
    end
    for name, _ in pairs(self.eventListener) do
        self:profileSingle(name)
    end
end

function EventDispatcher:profileSingle(eventName)
    local listeners = self:getListener(eventName)
    if listeners == nil then
        print("No event:", eventName)
        return
    end
    local res = eventName .. "\t" .. listeners.length .. "\n"
    for _, value in ilist(listeners) do
        if value == nil then return end
        if type(value.from) == "table" and type(value.from.__class) == "table" then
            for funcName, func in pairs(value.from.__class) do
                if func == value.func then
                    res = res .. value.from.__path .. ":" .. funcName .. "\t" .. value.priority .. "\n"
                    break;
                end
            end
        else
            res = res .. tostring(value.func) .. "\t" .. value.priority .. "\n"
        end
    end
    print(res)
end

---@type Event.Dispatcher
dispatcher = declare("dispatcher", EventDispatcher())