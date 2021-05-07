local BaseLogicGraph = DefineClass()

BaseLogicGraph.timers = {}

function BaseLogicGraph:GetType()
    return "LogicGraph"
end

function BaseLogicGraph:Dispose()
    for _, v in pairs(self.timers) do
        v:Stop()
    end
    LogicGraphMgr:Dispose(self)
end

return BaseLogicGraph