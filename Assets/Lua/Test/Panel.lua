---@class_type static

---@field public v1 number
---@field public v3 UnityEngine.Vector3 @:"use vals" |"Vector3(1, 2, 3)"|"Vector3(2, 3, 4)"
---@field public v4 UnityEngine.Rect
---@field public str string |"'onClosed'"|"'onData'"
---@field public img UnityEngine.UI.Image
---@field public text UnityEngine.UI.Text
---@field public text1 UnityEngine.UI.Text
---@field public text2 UnityEngine.UI.Text
---@field public trans UnityEngine.RectTransform
---@field public color UnityEngine.Color |"Color(1,1,1,1)"|"Color(0,0,0,0)"
---@class Panel
local Panel = DefineClass()

function Panel:Ctor()
    
end

function Panel:Awake()
    print("--------------in panel awake123")
    self.text.text = self.str
    self.text.color = self.color
end

return Panel
