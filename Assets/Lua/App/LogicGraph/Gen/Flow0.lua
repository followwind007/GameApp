local LogicGraph_Logic = require('App.LogicGraph.Nodes.Logic')
local LogicGraph_Util_Log = require('App.LogicGraph.Nodes.Util.Log')

local Flow0 = DefineClass(BaseLogicGraph)

Flow0.graphId = 'Flow0'
Flow0.timers = {}

function Flow0:Start()
	GraphDebug.Enter("LogicGraph.Logic.For_1")
	LogicGraph_Logic.For(function(param1) self:Callback_1_1(param1) end, self.LogicGraph_Logic_For_1_startValue, self.LogicGraph_Logic_For_1_endValue, self.LogicGraph_Logic_For_1_step)
end

function Flow0:Callback_1_1(param1)
	GraphDebug.Enter("LogicGraph.Callback.Callback_1_1")
	GraphDebug.Enter("LogicGraph.Util.Log.Print_1")
	LogicGraph_Util_Log.Print(param1)
end

return Flow0