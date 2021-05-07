local Custom_UI_OpenUI = require("LogicGraph.Nodes.Custom.UI.OpenUI")
local Custom_Guide_Util_SetTimeScale = require("LogicGraph.Nodes.Custom.Guide.Util.SetTimeScale")

local Test = DefineClass(BaseLogicGraph)

Test.graphId = "Test"
Test.timers = {}

function Test:Init()
	local fsm = FSM.New()
	self.fsm = fsm

	local State_1 = FSMState.New("State_1", function() self:State_1_Enter() end, nil)
	fsm:AddState("State_1", State_1)

	local State_2 = FSMState.New("State_2", function() self:State_2_Enter(); self:Exit() end, nil)
	fsm:AddState("State_2", State_2)

	local State_Transition_1 = FSMCondition.New(FSMCondition.Type.Equal, "TOP_MASK_CLOSED", true)
	fsm:AddLink("State_1", "State_2", {State_Transition_1})

	fsm:AddValue("TOP_MASK_CLOSED", self.TOP_MASK_CLOSED, true, false)
end

function Test:Entry()
	GraphDebug.Enter("State_Entry_1")
	self.fsm:PlayState("State_1")
end

function Test:State_1_Enter()
	GraphDebug.Enter("State_State_1")
	GraphDebug.Enter("Custom_Guide_Util_SetTimeScale_1")
	Custom_Guide_Util_SetTimeScale:Do(self.Custom_Guide_Util_SetTimeScale_1_scale)
	GraphDebug.Enter("Custom_UI_OpenUI_1")
	Custom_UI_OpenUI:Do(self.Custom_UI_OpenUI_1_uid)
end

function Test:State_2_Enter()
	GraphDebug.Enter("State_State_2")
	GraphDebug.Enter("Custom_Guide_Util_SetTimeScale_2")
	Custom_Guide_Util_SetTimeScale:Do(self.Custom_Guide_Util_SetTimeScale_2_scale)
end

function Test:Exit()
	GraphDebug.Enter("State_Exit_1")
	self.fsm:Dispose()
	self:Dispose()
end

return Test