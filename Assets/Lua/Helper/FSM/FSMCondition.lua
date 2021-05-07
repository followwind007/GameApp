---@type number
---@class FSMCondition
local FSMCondition = DefineClass()

FSMCondition.Type = {
    Greater = 1,
    Less = 2,
    Equal = 3,
    NotEqual = 4,
}

---@param type number @condition tye, FSMCondition.Type
---@param name string @name of condition param
---@param value @default value of condition
---@return FSMCondition
function FSMCondition.New(type, name, value)
    return FSMCondition(type, name, value)
end

function FSMCondition:Ctor(type, name, value)
    self.type = type
    self.name = name
    self.value = value
end

---@param val @value to be compared with current condition
---@return boolean
function FSMCondition:IsValid(val)
    if self.type == self.Type.Greater then
        return val > self.value
    elseif self.type == self.Type.Less then
        return val < self.value
    elseif self.type == self.Type.Equal then
        return self:CheckEqual(self.value, val)
    elseif self.type == self.Type.NotEqual then
        return not self:CheckEqual(self.value, val)
    end
end

function FSMCondition:CheckEqual(a, b)
    if type(a) == "table" and type(b) == "table" then
        if table.valueCount(a) ~= table.valueCount(b) then
            return false
        end
        local isEqual = true
        for i, v in pairs(b) do
            if a[i] == nil or a[i] ~= v then
                isEqual = false
                break
            end
        end
        return isEqual
    end
    return a == b
end

function FSMCondition:ToString(printFlag)
    local res = string.format("type:%s name:%s value:%s\n", self.type, self.name, self.value)
    if printFlag then print(res) end
    return res
end

return FSMCondition