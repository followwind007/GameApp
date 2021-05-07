local Custom_Test = require("LogicGraph.Nodes.Custom.Test")

local Undo = DefineClass(BaseLogicGraph)

Undo.graphId = "Undo"
Undo.timers = {}

function Undo:Start()
	GraphDebug.Enter("Custom_Test_1")
	Custom_Test:Do(self.Custom_Test_1_a, self.Custom_Test_1_b, self.Custom_Test_1_c, self.Custom_Test_1_d, self.Custom_Test_1_e, self.Custom_Test_1_f, self.Custom_Test_1_g, self.Custom_Test_1_h, self.Custom_Test_1_i, self.Custom_Test_1_j)
end

return Undo