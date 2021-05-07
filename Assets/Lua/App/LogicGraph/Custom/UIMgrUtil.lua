---@class LogicGraph.UIMgrUtil
local UIMgrUtil = DefineClass()


---@param uid string
---@param callback transition
function UIMgrUtil.Open(uid, callback)
    UIMgr:Open(uid, callback)
end

---@param uid string
---@return Elua
function UIMgrUtil.GetViewTable(uid)
    return UIMgr:GetViewTable(uid)
end

---@param uid string
function UIMgrUtil.Close(uid)
    UIMgr:Close(uid)
end

return UIMgrUtil