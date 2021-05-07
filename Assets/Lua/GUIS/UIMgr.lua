local UIViewItem = require("guis.UIViewItem")

---@field public camera UnityEngine.Camera
---@field public canvas UnityEngine.Canvas
---@field public contentStack UnityEngine.RectTransform
---@field public contentModal UnityEngine.RectTransform
---@field public contentHided UnityEngine.RectTransform
---@field public stateOpen string
---@field public stateClose string
---@field public modalRaycaster @UnityEngine.UI.GraphicRaycaster
---@field public stackRaycaster @UnityEngine.UI.GraphicRaycaster
---@class UIMgr
local UIMgr = DefineClass()

UIMgr.isDebug = UnityEngine.Debug.isDebugBuild

local ViewStatus = UIViewItem.ViewStatus

function UIMgr:Open(uid, callback)
    uid = self:GetUId(uid)
    if self.isDebug then print("UIMgr Open", uid) end
    local view = self:GetView(uid)
    view.activeCallback = callback
    
    if UIConfig:IsStack(uid) then
        if self.stack:tail() ~= uid then
            self.stack:erase(uid)
            self.stack:push(uid)
        end
        self.isDirty = true
        return
    end
    view:SetStatus(ViewStatus.Active)
end

function UIMgr:Close(uid, destroyFlag)
    self:InternalClose(uid, {directCloseFlag = false, destroyFlag = destroyFlag})
end

---@param uid string
function UIMgr:CloseNoValidate(uid)
    self:InternalClose(uid, {directCloseFlag = true})
end

function UIMgr:GetActiveStackView()
    return self.curTop
end

---@param uid string
function UIMgr:GetViewTable(uid)
    return self:GetView(self:GetUId(uid)).tbl
end

function UIMgr:IsViewActive(uid)
    return self:GetView(self:GetUId(uid)):IsActive()
end

function UIMgr:ClearStack()
    self.backupStack = self.stack:clone()
    for _, v in ilist(self.stack) do
        local view = self:GetView(v)
        view:Destroy()
    end
    self.curTop = nil
    self.stack:clear()
end

function UIMgr:ResumeStack()
    self.stack = self.backupStack:clone()
    if self.stack.length == 0 then
        self.stack:push(Views.MainPanel.id)
    end
    self.isDirty = true
end

-----------------------------private funcs-------------------------------

function UIMgr:Awake()
    declare("UIMgr", self)
    self.stack = list:new()
    self.backupStack = list:new()

    self.isDirty = false
    self.curTop = nil

    ---@type table<string, UIViewItem>
    self.views = {}

    self.holding = false
    UIHelper.UIDefinePostDeal()
end

function UIMgr:Start()
    Object.DontDestroyOnLoad(self.gameObject)
    Event.AddListener(EventNames.ON_KEY_DOWN, self.OnKeydown, self)
end

function UIMgr:Update()
    if not self.isDirty or self.holding then return end
    self.isDirty = false
    StartCoroutine(function() self:SyncStack() end)
end

function UIMgr:SyncStack()
    local topUid = self.stack:tail()
    if self.curTop == topUid then return end
    
    print("UIMgr Sync", self.curTop, "=>", topUid)
    self.holding = true
    local top = self:GetView(topUid)
    --create
    if top and not top.view then top:SetStatus(ViewStatus.Create) end
    
    local useTween = self.curTop and topUid
    --hide prev top
    local cur = self:GetView(self.curTop)
    if self.curTop and self.curTop ~= top then
        cur:SetStatus(ViewStatus.InActive)
        if useTween then
            --wait tween exit
            if cur.stateLength > 0 then WaitForSeconds(cur.stateLength) end
            dispatcher:dispatch(EventNames.UI_TWEEN, {from = self.curTop, to = topUid})
        else
            if cur.stateLength > 0 then WaitForSeconds(cur.stateLength) end
        end
    end
    --show current top
    if topUid then top:SetStatus(ViewStatus.Active) end
    
    self.curTop = topUid
    self.holding = false
end

---@return UIViewItem
function UIMgr:GetView(uid)
    if uid == nil then return end
    if self.views[uid] == nil then
        self.views[uid] = UIViewItem(uid)
    end
    return self.views[uid]
end

function UIMgr:SetInteractable(val)
    --self.modalRaycaster.enabled = val
    self.stackRaycaster.enabled = val
end

function UIMgr:GetUId(uid)
    return type(uid) == "table" and uid.id or uid
end

function UIMgr:OnKeydown()
    local isEscapeDown = LuaHelper.IsKeyDown(KeyCode.Escape)
    if isEscapeDown then LHLoginManager.Exit() end
end

function UIMgr:InternalClose(uid, config)
    uid = self:GetUId(uid)
    if self.isDebug then print("UIMgr Close", uid) end
    local view = self:GetView(uid)

    table.merge(view, config)
    
    if UIConfig:IsStack(uid) then
        self.stack:erase(uid)
        self.isDirty = true
        return
    end
    view:SetStatus(ViewStatus.InActive)
end

return UIMgr