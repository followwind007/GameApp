
---@class UIConfig
local UIConfig = DefineClass()

---@class UIConfig.ViewType
UIConfig.ViewType = {
    Stack = 1,
    Modal = 2,
}

UIConfig.ReferenceResolution = Vector2(1280, 720)

---@field id string
---@field type UIConfig.ViewType
---@field order number
---@field path string
---@field isResident boolean
---@field distance number
---@class UIConfig.ViewConfig
UIConfig.ViewConfig = DefineClass()

---@param uid string
---@return UIConfig.ViewConfig
function UIConfig:GetConfig(uid)
    local config = Views[uid]
    if config == nil then
        config = {}
        print("nil config for", uid)
    end
    return config
end

function UIConfig:IsStack(uid)
    return self:GetConfig(uid).type == UIConfig.ViewType.Stack
end

function UIConfig:IsModal(uid)
    return self:GetConfig(uid).type == UIConfig.ViewType.Modal
end

function UIConfig:IsResident(uid)
    return self:GetConfig(uid).isResident and true or false
end

--[[
function UIConfig:Parse()
    local res = ""
    local format = "%s = {id=\"%s\",type=%s,path=\"%s\"},\n"
    for i, v in pairs(PIDS) do
        local path = v.mPath
        path = string.gsub(path,"Assets/Art/UI/Prefabs/", "")
        path = string.gsub(path,".prefab","")
        local t = "T.Stack"
        res = res..string.format(format, i, i, t, path)
    end
    print(res)
end
]]

return UIConfig