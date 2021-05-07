---@type GameApp.Util.UIUtils
local UIUtils = GameApp.Util.UIUtils

---@class UIHelper
local UIHelper = DefineClass()

function UIHelper.LoadRoot(cb)
    RES:LoadObject(UIConst.RootPrefab, function(res)
        ---@type UnityEngine.GameObject
        local uiroot = GameObject.Instantiate(res.m_oAssetObject)
        uiroot.transform.position = Vector3(10000, 10000, 10000)
        if cb then cb() end
    end, nil)
end

---@param uid string
---@param callback fun(go:UnityEngine.GameObject)
function UIHelper.LoadView(uid, callback)
    local config = UIConfig:GetConfig(uid)
    local path = string.format(UIConst.PrefabPathFormat, config.path)
    UIHelper.LoadObject(path, function(prefab)
        local go = UnityEngine.Object.Instantiate(prefab)
        callback(go)
    end)
end

---@param go UnityEngine.GameObject
function UIHelper.UIViewPostDeal(go)
    if go == nil then return end
    --go:GetOrAddComponent(typeof(UnityEngine.UI.UIEventCleaner))
end

---@param module string
---@param name string
---@param rawImage UnityEngine.UI.RawImage
---@param useNativeSize boolean
function UIHelper.SetRawImage(module, name, rawImage, useNativeSize)
    if rawImage == nil then
        printstack("nil RawImage")
        return
    end
    local path = UIConst.GetIconPath(module, name)
    UIHelper.SetRawImageAtPath(path, rawImage, useNativeSize)
end

---@param path string
---@param rawImage UnityEngine.UI.RawImage
---@param useNativeSize boolean
function UIHelper.SetRawImageAtPath(path, rawImage, useNativeSize)
    UIHelper.LoadObject(path, function(obj)
        if IsNil(rawImage) then return end
        rawImage.texture = obj
        if useNativeSize then
            rawImage:SetNativeSize()
        end
    end)
end

---@param path string
---@param callback fun(go:UnityEngine.GameObject)
function UIHelper.LoadObject(path, callback)
    RES:LoadObject(path, function(res)
        callback(res.m_oAssetObject)
    end, nil)
end

---@param path string
---@param callback fun(go:UnityEngine.GameObject)
function UIHelper.SyncLoadAsset(path, callback)
    RES:SyncLoadAsset(path, nil,function(res)
        callback(res.m_oAssetObject)
    end, nil)
end

---@param animator UnityEngine.Animator
---@param clipName string @optional, id nil return current state length
---@return number
function UIHelper.GetAnimatorClipLength(animator, clipName)
    if IsNil(animator) then return 0 end
    local clips = animator.runtimeAnimatorController.animationClips
    local len = clips.Length
    for i = 0, len - 1 do
        local clip = clips[i]
        local _, id = string.find(clip.name, clipName)
        if id ~= nil and id > 0 then
            return clip.length
        end
    end
    return 0
end

---@param animator UnityEngine.Animator
function UIHelper.GetCurrentAnimatorStateLength(animator, stateName)
    if IsNil(animator) then return 0 end
    ---@type UnityEngine.AnimatorStateInfo
    local state = animator:GetCurrentAnimatorStateInfo(0)
    if IsNil(state) then return 0 end
    
    if stateName == nil or state:IsName(stateName) then
        local clipInfos = animator:GetCurrentAnimatorClipInfo(0)
        if clipInfos.Length < 1 then
            return 0
        end
        local start = 0
        if clipInfos[start].clip.empty then
            return 0
        end
        return state.length
    end
    return 0
end

---@param go UnityEngine.GameObject
---@param config UIConfig.ViewConfig
function UIHelper.SetCanvasOrder(go, config)
    if go == nil or config.order == nil then return end
    UIUtils.SetCanvas(go, UIMgr.camera, config.order, config.distance)
end

function  UIHelper.SetParticleOrder(go, layerName)
    if go == nil then return end
    local canvas = go:GetComponentInParent(typeof(UnityEngine.Canvas))
    if IsNil(canvas) then return end
    local baseorder = canvas.sortingOrder
    if math.fmod( baseorder, 100) ~= 0 then
        logError("界面的order必须为100的整数倍！！！！！！！！！！！！！！！！！！！！！！！")
    end
    local renders = go:GetComponentsInChildren(typeof(UnityEngine.Renderer), true)
    for i = 0, renders.Length - 1 do
        local order = renders[i].sortingOrder
        if order > 99 then -- 100为界，防止越界穿界面
            order = math.fmod( order, 100)
        end
        if layerName then renders[i].sortingLayerName = layerName end
        renders[i].sortingOrder = order + baseorder
    end
end

function UIHelper.ReloadUI(fullClear)
    for i, v in pairs(UIMgr.views) do
        GameObject.Destroy(v.view)
        UIMgr.views[i] = nil
    end
    local last = UIMgr.stack:pop()
    UIMgr.stack:clear()
    if not fullClear then
        UIMgr:Open(last)
    end
end

function UIHelper.Dispose()
    TipController:Dispose()
    UIHelper.ReloadUI(true)
end

function UIHelper.UIDefinePostDeal()
    --generate plane distance
    ---@type UIConfig.ViewConfig[]
    local dl = {}
    for i, v in pairs(Views) do
        if UIConfig:IsModal(i) then
            table.insert(dl, v)    
        end
    end
    table.sort(dl, function(a, b)
        if a.order and b.order then
            return a.order < b.order
        end
        return true
    end)
    
    local farDist = 100
    local areaGap = 1.5
    local cur = 1
    
    local prev = {order = -1, distance = farDist}
    for _, v in ipairs(dl) do
        if prev.order ~= v.order then
            prev = v
            cur = cur / areaGap
            v.distance = farDist * Mathf.Pow(cur, 0.5)
        else
            v.distance = prev.distance
        end
    end
end

return UIHelper