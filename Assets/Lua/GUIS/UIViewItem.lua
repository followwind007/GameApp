---@field uid string
---@field config UIConfig.ViewConfig
---@field view UnityEngine.GameObject
---@field status string
---@field stateLength number
---@field tbl table
---@field activeCallback function<table>
---@field directCloseFlag boolean
---@field destroyFlag boolean
---@class UIViewItem
local UIViewItem = DefineClass()

UIViewItem.ViewStatus = {
    InActive = "InActive",
    Active = "Active",
    Create = "Create",
}

function UIViewItem:Ctor(uid)
    self.uid = uid
    self.config = Views[uid]
    self.status = self.ViewStatus.InActive
    
    self.stateLength = 0
    
    self.isLoading = false
    self.createFlag = false
    self.directCloseFlag = false
    self.destroyFlag = false
    self.activeCallback = nil
end

function UIViewItem:SetStatus(status)
    self.status = status
    if (status == self.ViewStatus.Create or status == self.ViewStatus.Active) and self.view == nil then
        self:Create()
        return
    end
    self:SyncStatus()
end

function UIViewItem:Create()
    if self.isLoading then return end
    self.isLoading = true
    UIHelper.LoadView(self.uid, function(go)
        self.view = go
        self.tbl = Adapter.GetLuaTable(go)
        if self.tbl == nil then
            local lb = go:GetComponent("LuaBehaviour")
            if lb then self.tbl = lb.luaTable end
        end
        self.isLoading = false

        if self.tbl and self.tbl.Init then self.tbl:Init() end
        self:SyncStatus()
    end)
end

function UIViewItem:SyncStatus()
    self.stateLength = 0
    if self.status == self.ViewStatus.InActive then
        self:Close()
    elseif self.status == self.ViewStatus.Active then
        self:Open()
    elseif self.status == self.ViewStatus.Create then
        self:Hide()
    end
end

function UIViewItem:Open()
    self.view:SetActive(true)
    if self.activeCallback then self.activeCallback(self.tbl) end
    if self.config.type == UIConfig.ViewType.Modal then
        UIHelper.SetCanvasOrder(self.view, self.config)
        UIHelper.SetParticleOrder(self.view)
        self.view.transform:SetParentFull(UIMgr.contentModal)
    elseif self.config.type == UIConfig.ViewType.Stack then
        self.view.transform:SetParentFull(UIMgr.contentStack)
    end
    if not self.createFlag then
        self.createFlag = true
    else
        if self.tbl and self.tbl.OnShow then self.tbl:OnShow() end
    end
end

function UIViewItem:Close()
    if self.view == nil then return end
    local callback = function()
        if self.tbl and self.tbl.OnBeforeHide then self.tbl:OnBeforeHide() end
        if self:IsDestroy() then
            self:Destroy()
        else
            self:Hide()
        end
    end
    if self.directCloseFlag then
        callback()
    else
        -- stack 转场不需要播放ui_close动画
        if UIConfig:IsStack(self.uid) then
            callback()
        else
            self:PlayState("close", callback)
        end
    end
end

function UIViewItem:Hide()
    if not self.view then return end
    self.view.transform:SetParent(UIMgr.contentHided)
end

function UIViewItem:Destroy()
    Object.Destroy(self.view)
    self.createFlag = false
    self.stateLength = 0
    self.view = nil
end

function UIViewItem:PlayState(state, onFinish)
    ---@type UnityEngine.Animator
    local animator = self.view:GetComponent("Animator")
    
    local dura = 0
    if not IsNil(animator) then
        local animators = self.view:GetComponentsInChildren(typeof(UnityEngine.Animator))
        for i = 0, animators.Length - 1 do
            animators[i]:Play(state)
        end
        dura = UIHelper.GetAnimatorClipLength(animator, state)
    end
    self.stateLength = dura
    
    if dura > 0 then
        UIMgr:SetInteractable(false)
        Timer.New(function()
            UIMgr:SetInteractable(true)
            if onFinish then onFinish() end
        end, dura, 1):Start()
    else
        if onFinish then onFinish() end
    end
end

function UIViewItem:IsActive()
    if not IsNil(self.view) and self.view.activeInHierarchy then return true end
    return false
end

function UIViewItem:IsDestroy()
    if not self.destroyFlag and (self.config.isResident or self:InStack()) then return false end
    return true
end

function UIViewItem:InStack()
    return UIMgr.stack:find(self.uid) and true or false
end

return UIViewItem