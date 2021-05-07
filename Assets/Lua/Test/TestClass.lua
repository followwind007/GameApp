
---@field public test integer |1|2|3
---@field public v4 UnityEngine.Rect
---@field public str string
---@field public color UnityEngine.Color
---@class TestClass
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

---@param p1 number
---@param p2 UnityEngine.Vector3
---@return string
function Test:Func(p1, p2)
	
end

return Test