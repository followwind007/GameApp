local LogicGraph_UIMgrUtil = require('app.LogicGraph.Custom.UIMgrUtil')
local GUI_View_DialogueView = require('app.guis.View.DialogueView')
local LogicGraph_Coroutine = require('app.LogicGraph.Nodes.Coroutine')

local TestFlow = DefineClass(BaseLogicGraph)

function TestFlow:Ctor()
	self.graphId = 'TestFlow'
	self.timers = {}

	self.uid = "DialogueView"
	self.view = nil
	self.GUI_View_DialogueView_ShowDialogue_1_idRange = Vector2(10001,10004)
	self.GUI_View_DialogueView_ShowBg_1_id = "common_bg.jpg"
	self.LogicGraph_Coroutine_WaitEvent_0_1_name = "UI_DIALOGUE_VIEW_FINISH"
	self.uid = "DialogueView"
	self.view = nil
	self.GUI_View_DialogueView_ChangeBg_1_id = "ladder_bg.jpg"
	self.LogicGraph_Coroutine_Wait_1_seconds = 3
end

function TestFlow:Start()
	GraphDebug.Enter("LogicGraph.UIMgrUtil.Open_1")
	LogicGraph_UIMgrUtil.Open(self.uid, function(param1) coroutine.start(self.Callback_1_1, nil, self, param1) end)
end

function TestFlow:Callback_1_1(param1)
	GraphDebug.Enter("LogicGraph.Callback.Callback_1_1")
	self.view = param1
	GraphDebug.Enter("GUI.View.DialogueView.ShowDialogue_1")
	GUI_View_DialogueView.ShowDialogue(self.view, self.GUI_View_DialogueView_ShowDialogue_1_idRange)
	GraphDebug.Enter("GUI.View.DialogueView.ShowBg_1")
	GUI_View_DialogueView.ShowBg(self.view, self.GUI_View_DialogueView_ShowBg_1_id)
	GraphDebug.Enter("LogicGraph.Coroutine.WaitEvent_0_1")
	LogicGraph_Coroutine.WaitEvent_0(self.LogicGraph_Coroutine_WaitEvent_0_1_name)
	GraphDebug.Enter("GUI.View.DialogueView.ChangeBg_1")
	GUI_View_DialogueView.ChangeBg(self.view, self.GUI_View_DialogueView_ChangeBg_1_id)
	GraphDebug.Enter("LogicGraph.Coroutine.Wait_1")
	LogicGraph_Coroutine.Wait(self.LogicGraph_Coroutine_Wait_1_seconds)
	GraphDebug.Enter("LogicGraph.UIMgrUtil.Close_1")
	LogicGraph_UIMgrUtil.Close(self.uid)
end

return TestFlow