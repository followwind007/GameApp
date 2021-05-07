
---@field public camera UnityEngine.Camera
---@field public canvas UnityEngine.Canvas
---@field public contentStack UnityEngine.RectTransform
---@field public contentModal UnityEngine.RectTransform
---@field public contentHided UnityEngine.RectTransform
---@field public stateOpen string
---@field public stateClose string
---@field public modalRaycaster UnityEngine.UI.GraphicRaycaster
---@field public stackRaycaster UnityEngine.UI.GraphicRaycaster
---@class UIMgr_Seq
local UIMgr = DefineClass()

UIMgr.isDebug = true

---@param uid string
---@param callback fun(view: table) @called when view be shown
function UIMgr:Open(uid, callback)
    uid = self:GetUId(uid)
    if self:IsViewActive(uid) then 
        local tbl = self:GetViewTable(uid)
        if tbl and callback then callback(tbl) end
        if tbl and tbl.OnShow then tbl:OnShow() end
        return
    end
    if UIConfig:IsStack(uid) then
        if self.holding then
            self.commands:push({method = "Open", uid = uid, param1 = callback})
            return
        end
        self.holding = true
        if self.isDebug then print("UIMgr open", uid) end
        if self:IsViewActive(self.stack:tail()) then
            self:InternalHide(self.stack:tail(), function()
                self.holding = true
                self:InternalGetView(uid, function(_, createFlag) self:InternalOpen(uid, callback, createFlag) end)
            end)
            return
        end
    end

    self:InternalGetView(uid, function(_, createFlag) self:InternalOpen(uid, callback, createFlag) end)
end

function UIMgr:Close(uid, destroyFlag)
    uid = self:GetUId(uid)
    local view = self.views[uid]
    if view == nil then return end
    
    if UIConfig:IsStack(uid) then
        if self.holding then
            self.commands:push({method = "Close", uid = uid, param1 = destroyFlag})
            return
        end
        self.holding = true
    end
    if self.isDebug then printstack("UIMgr close", uid) end
    self:InternalHide(uid, function()
        if destroyFlag or not UIConfig:IsResident(uid)then
            Object.Destroy(view)
            self.views[uid] = nil
        end
        if UIConfig:IsStack(uid) then
            if self.stack:tail() ~= uid then
                self.stack:erase(uid)
                return
            end
            self.stack:pop()
            if self.stack.length > 0 and not self:ExistOpenCommand() then
                self.holding = true
                self:InternalShow(self.stack:tail())
            end
        end
    end)
end

function UIMgr:CloseNoValidate(uid)
    uid = self:GetUId(uid)
    local view = self.views[uid]
    if view == nil then return end
    local tbl = self:GetViewTable(uid)
    if tbl.OnBeforeHide then tbl:OnBeforeHide() end
    if not UIConfig:IsResident(uid) then
        Object.Destroy(view)
        self.views[uid] = nil
    else
        view.transform:SetParent(self.contentHided)
    end
    self.stack:erase(uid)
end

function UIMgr:GetViewTable(uid, useUnActive)
    uid = self:GetUId(uid)
    local view = self.views[uid]
    if view and (view.activeSelf or not view.activeSelf and useUnActive) then
        local tbl = Adapter.GetLuaTable(view)
        if tbl then return tbl end
        local lb = view:GetComponent("LuaBehaviour")
        if lb then return lb.luaTable end
    end
    printstack("try to get nil view")
end

function UIMgr:IsViewActive(uid)
    uid = self:GetUId(uid)
    local view = self.views[uid]
    if view and (view.transform.parent == self.contentModal or view.transform.parent == self.contentStack) then
        return true
    end
    return false
end

-----------------------private func----------------------
function UIMgr:Awake()
    declare("UIMgr", self)
    self.stack = list:new()
    ---@type table<string, UnityEngine.GameObject>
    self.views = {}

    self.loadingList = list:new()

    self.holding = false
    self.commands = list:new()
end

function UIMgr:Start()
    Object.DontDestroyOnLoad(self.gameObject)
    Timer.New(function() self:InternalSchedule() end, 0.1, -1):Start()
end

function UIMgr:InternalGetView(uid, onGet)
    uid = self:GetUId(uid)
    if self.views[uid] then
        onGet(self.views[uid])
        return
    end
    if self.loadingList:find(uid) then
        printstack("aready in loading", uid)
        return
    end
    self.loadingList:push(uid)

    UIHelper.LoadView(uid, function(go)
        self.loadingList:erase(uid)
        self.views[uid] = go
        UIHelper.UIViewPostDeal(go)
        local tbl = self:GetViewTable(uid)
        if tbl and tbl.Init then tbl:Init() end
        onGet(go, true)
    end)
end

function UIMgr:ExistOpenCommand()
    local exist = false
    self.commands:foreach(function(v)
        if v.method == "Open" then exist = true end
    end)
    return exist
end

function UIMgr:InternalSchedule()
    if not self.holding and self.commands.length > 0 then
        local cmd = self.commands:shift()
        self[cmd.method](self, cmd.uid, cmd.param1)
    end
end

function UIMgr:InternalOpen(uid, callback, createFlag)
    local view = self.views[uid]
    view:SetActive(true)

    self:InternalShow(uid)
    local config = UIConfig:GetConfig(uid)
    if config.type == UIConfig.ViewType.Modal then
        UIHelper.SetCanvasOrder(view, config.order)
        UIHelper.SetParticleOrder(view)
    elseif config.type == UIConfig.ViewType.Stack then
        self.stack:erase(uid)
        self.stack:push(uid)
    end

    self.contentModal.gameObject:SetActive(config.hideOther and false or true)
    local tbl = self:GetViewTable(uid)
    if tbl then
        if callback then callback(tbl) end
        if not createFlag and tbl.OnShow then tbl:OnShow() end
    end
end

function UIMgr:InternalShow(uid, onShow)
    local config = UIConfig:GetConfig(uid)
    local view = self.views[uid]
    if config.type == UIConfig.ViewType.Modal then
        view.transform:SetParentFull(self.contentModal)
    elseif config.type == UIConfig.ViewType.Stack then
        view.transform:SetParentFull(self.contentStack)
    end
    self:InternalPlayState(uid, self.stateOpen, function()
        if onShow then onShow() end
    end)
end

function UIMgr:InternalHide(uid, onHide)
    self:InternalPlayState(uid, self.stateClose, function()
        local tbl = self:GetViewTable(uid)
        if tbl and tbl.OnBeforeHide then tbl:OnBeforeHide() end
        if self.views[uid] then
            self.views[uid].transform:SetParent(self.contentHided)
        end
        if onHide then onHide() end
    end)
end

function UIMgr:InternalPlayState(uid, state, onFinish)
    local view = self.views[uid]
    ---@type UnityEngine.Animator
    local animator = view:GetComponent("Animator")
    if not animator then
        if UIConfig:IsStack(uid) then self.holding = false end
        if onFinish then onFinish(uid) end
        return
    end
    self:SetInteractable(false)
    local animators = view:GetComponentsInChildren(typeof(UnityEngine.Animator))
    for i = 0, animators.Length - 1 do
        animators[i]:Play(state)
    end
    FrameTimer.New(function()
        local dura = UIHelper.GetCurrentAnimatorStateLength(animator, state)
        Timer.New(function()
            self:SetInteractable(true)
            if onFinish then onFinish(uid) end
            if UIConfig:IsStack(uid) then self.holding = false end
        end, dura, 1):Start()
    end, 1, 1):Start()
end

function UIMgr:SetInteractable(val)
    self.modalRaycaster.enabled = val
    self.stackRaycaster.enabled = val
end

function UIMgr:GetUId(uid)
    return type(uid) == "table" and uid.id or uid
end

return UIMgr
