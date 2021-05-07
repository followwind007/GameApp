local Math_Multiply = require("LogicGraph.Nodes.Math.Multiply")
local Util_Print = require("LogicGraph.Nodes.Util.Print")
local Util_PrintStack = require("LogicGraph.Nodes.Util.PrintStack")
local Custom_Test = require("LogicGraph.Nodes.Custom.Test")

local Flow = DefineClass(BaseLogicGraph)

Flow.graphId = "Flow"

function Flow:Start()
	local Math_Multiply_1_return = Math_Multiply:Do(2, self.f1)
	Util_Print:Do(Math_Multiply_1_return)
	Util_PrintStack:Do("gg")
	Custom_Test:Do(Math_Multiply_1_return, Math_Multiply_1_return, self.Custom_Test_1_c, self.Custom_Test_1_d, self.Custom_Test_1_e, self.Custom_Test_1_f, self.Custom_Test_1_g, self.Custom_Test_1_h, self.Custom_Test_1_i, self.Custom_Test_1_j)
	self:Dispose()
end

return Flow