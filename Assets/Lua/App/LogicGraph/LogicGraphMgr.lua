---@type GameApp.DataBinder.BehaviourBinder
local DataBinder = GameApp.DataBinder.BehaviourBinder

---@class LogicGraphMgr
local LogicGraphMgr = {}
local this = LogicGraphMgr

local GRAPH_PATH_FORMAT = "Assets/Art/Graph/%s.LogicGraph.asset"

function LogicGraphMgr:Play(id, onLoad)
    local path = string.format(GRAPH_PATH_FORMAT, id)
    ResHelper.LoadAsset(path, function(res)
        local asset = res.m_oAssetObject
        ResHelper.UnloadAssets(path)
        local lgObj = Object.Instantiate(asset)
        local inst = this.InstantiateGraph(lgObj)
        if onLoad then onLoad(lgObj, inst) end
        this.StartGraph(lgObj)
    end, nil)
end

function LogicGraphMgr:PlayRes(id)
    local asset = UnityEngine.Resources.Load(id..".LogicGraph")
    local lgObj = Object.Instantiate(asset)
    this.PlayGraph(lgObj)
end

function LogicGraphMgr:Stop(lgObj)
    local tbl = Adapter.objs[lgObj:GetInstanceID()]
    if tbl.OnDestroy then tbl:OnDestroy() end
    Adapter.objs[lgObj:GetInstanceID()] = nil
end

function LogicGraphMgr:Dispose(tbl)
    for i, v in pairs(Adapter.objs) do
        if v == tbl then
            Adapter.objs[i] = nil
            tbl = nil
            break
        end
    end
end

---------------------------------------------------------
function LogicGraphMgr.PlayGraph(lgObj)
    this.InstantiateGraph(lgObj)
    this.StartGraph(lgObj)
end

function LogicGraphMgr.StartGraph(lgObj)
    local inst = Adapter.GetLuaTable(lgObj)
    GraphDebug.Play(inst)
    if inst.Start then
        --Flow Graph
        inst:Start() 
    else
        --State Graph
        if inst.Init then inst:Init() end
        if inst.Entry then inst:Entry() end
    end
end

function LogicGraphMgr.PlayGraphWithId(id)
    local Cls = require("LogicGraph.Gen."..id)
    if Cls then 
        local tbl = Cls()
        coroutine.start(function() tbl:Start() end)
    end
end

function LogicGraphMgr.InstantiateGraph(lgObj)
    local requirePath = DataBinder.GetLuaPath(lgObj.exportPath)
    local Cls = require(requirePath)
    local inst = Cls()
    inst.binds = lgObj.binds

    Adapter.objs[lgObj:GetInstanceID()] = inst

    Adapter.LoadData(inst)
    return inst
end

function LogicGraphMgr.GetAllGraph()
    local graphs = {}
    for _, v in pairs(Adapter.objs) do
        if v.GetType and v:GetType() == "LogicGraph" then
            print(v.graphId)
            table.insert(graphs, v)
        end
    end
    return graphs
end

return LogicGraphMgr