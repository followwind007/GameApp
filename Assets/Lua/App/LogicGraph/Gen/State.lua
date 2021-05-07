local Util_Print = require("LogicGraph.Nodes.Util.Print")
local Util_PrintStack = require("LogicGraph.Nodes.Util.PrintStack")
local Math_Add = require("LogicGraph.Nodes.Math.Add")
local Math_Multiply = require("LogicGraph.Nodes.Math.Multiply")
local Math_Minus = require("LogicGraph.Nodes.Math.Minus")
local Custom_SubGraph_Instantiate = require("LogicGraph.Nodes.Custom.SubGraph.Instantiate")
local Custom_SubGraph_SetProperty = require("LogicGraph.Nodes.Custom.SubGraph.SetProperty")
local Custom_SubGraph_Start = require("LogicGraph.Nodes.Custom.SubGraph.Start")

local State = DefineClass(BaseLogicGraph)

State.graphId = "State"
State.timers = {}

function State:Init()
	local fsm = FSM.New()
	self.fsm = fsm

	local State_1 = FSMState.New("State_1", function() self:State_1_Enter() end, function() self:State_1_Exit() end)
	fsm:AddState("State_1", State_1)

	local State_2 = FSMState.New("State_2", function() self:State_2_Enter(); self:Exit() end, nil)
	fsm:AddState("State_2", State_2)

	local State_Transition_1 = FSMCondition.New(FSMCondition.Type.Equal, "M_1", 3.5)
	fsm:AddLink("State_1", "State_2", {State_Transition_1})

	fsm:AddValue("M_1", self.M_1, true, true)
end

function State:Entry()
	GraphDebug.Enter("State_Entry_1")
	self.fsm:PlayState("State_1")
end

function State:State_1_Exit()
	GraphDebug.Enter("Util_Print_2")
	Util_Print:Do(222)
end

function State:State_1_Enter()
	GraphDebug.Enter("State_State_1")
	GraphDebug.Enter("Util_Print_1")
	Util_Print:Do("this is not good")
	GraphDebug.Enter("Util_PrintStack_1")
	Util_PrintStack:Do("this is not good")
	GraphDebug.Enter("Custom_SubGraph_Instantiate_1")
	local Custom_SubGraph_Instantiate_1_return, Custom_SubGraph_Instantiate_1_return_1 = Custom_SubGraph_Instantiate:Do(self.Custom_SubGraph_Instantiate_1_asset)
	GraphDebug.Enter("Custom_SubGraph_SetProperty_1")
	Custom_SubGraph_SetProperty:Do(Custom_SubGraph_Instantiate_1_return_1, self.Custom_SubGraph_SetProperty_1_name, 233)
	GraphDebug.Enter("Custom_SubGraph_SetProperty_2")
	Custom_SubGraph_SetProperty:Do(Custom_SubGraph_Instantiate_1_return_1, self.Custom_SubGraph_SetProperty_2_name, 333)
	GraphDebug.Enter("Custom_SubGraph_Start_1")
	Custom_SubGraph_Start:Do(Custom_SubGraph_Instantiate_1_return, Custom_SubGraph_Instantiate_1_return_1)
end

function State:State_2_Enter()
	GraphDebug.Enter("State_State_2")
	GraphDebug.Enter("Util_Print_4")
	Util_Print:Do(345)
end

function State:Exit()
	GraphDebug.Enter("State_Exit_1")
	GraphDebug.Enter("Math_Multiply_1")
	local Math_Multiply_1_return = Math_Multiply:Do(5, 3)
	GraphDebug.Enter("Math_Minus_1")
	local Math_Minus_1_return = Math_Minus:Do(3, self.M_1)
	GraphDebug.Enter("Math_Add_1")
	local Math_Add_1_return = Math_Add:Do(Math_Multiply_1_return, Math_Minus_1_return)
	GraphDebug.Enter("Util_Print_5")
	Util_Print:Do(Math_Add_1_return)
	self.fsm:Dispose()
	self:Dispose()
end

return State