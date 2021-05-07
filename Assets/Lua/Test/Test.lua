
---@field public test GameApp.DataBinder.ValueWrap
---@field public v4 GameApp.DataBinder.ValueWrap
---@field public str GameApp.DataBinder.ValueWrap
---@field public color GameApp.DataBinder.ValueWrap
---@class Test
local Test = DefineClass()

function Test:Ctor()
    print("in Ctor")
end

function Test:Awake()
    print("in Awake")
end

function Test:OnDestroy()
    print("in Destroy")
end

function Test:OnApplicationPause(value)
    print("Pause", value)
end

function Test:OnApplicationFocus(value)
    print("Focus", value)
end

return Test
