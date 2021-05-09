
---@field public text UnityEngine.UI.Text
---@class Test
local Test = DefineClass()

function Test:Ctor()
    print("in Ctor")
end

function Test:Awake()
    print("in Awake")
end

function Test:Update()
    self.text.text = UnityEngine.Time.time
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
