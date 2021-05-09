
---@class LogicGraph.Variable
local Variable = DefineClass()

---@param value boolean
---@return boolean
function Variable.boolean(value)

end

---@param value number
---@return number
function Variable.number(value)

end

---@param value string
---@return string
function Variable.string(value)

end

---@param value UnityEngine.Color
---@return UnityEngine.Color
function Variable.Color(value)

end

---@param value UnityEngine.Vector2
---@return UnityEngine.Vector2
function Variable.Vector2(value)

end

---@param value UnityEngine.Vector3
---@return UnityEngine.Vector3
function Variable.Vector3(value)

end

---@param value UnityEngine.Vector4
---@return UnityEngine.Vector4
function Variable.Vector4(value)

end

---@param value UnityEngine.Rect
---@return UnityEngine.Rect
function Variable.Rect(value)

end

---@param value UnityEngine.Bounds
---@return UnityEngine.Bounds
function Variable.Bounds(value)

end

---@param value UnityEngine.Object
---@return UnityEngine.Object
function Variable.Object(value)

end

---@param value Elua
---@return Elua
function Variable.Elua(value)

end

return Variable