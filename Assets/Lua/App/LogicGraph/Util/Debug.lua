
---@class LogicGraph.Debug
local Debug = {}

function Debug.Enter(id)
    if UnityEngine.Application.isEditor then
        EventDispatcher.Instance:Dispatch("GRAPH_NODE_ENTER", id)
    end
end

function Debug.Exit(id)
    if UnityEngine.Application.isEditor then
        EventDispatcher.Instance:Dispatch("GRAPH_NODE_EXIT", id)
    end
end

function Debug.Play(inst)
    if UnityEngine.Application.isEditor then
        EventDispatcher.Instance:Dispatch("GRAPH_PLAY", inst)
    end
end

return Debug