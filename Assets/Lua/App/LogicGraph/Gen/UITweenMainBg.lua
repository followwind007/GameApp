local Custom_Dispatcher_Dispatch = require("LogicGraph.Nodes.Custom.Dispatcher.Dispatch")
local Util_AddTimer = require("LogicGraph.Nodes.Util.AddTimer")
local Custom_Tween_TweenAnchorPos = require("LogicGraph.Nodes.Custom.Tween.TweenAnchorPos")
local Custom_Tween_TweenScale = require("LogicGraph.Nodes.Custom.Tween.TweenScale")
local Custom_Tween_TweenRotate = require("LogicGraph.Nodes.Custom.Tween.TweenRotate")

local UITweenMainBg = DefineClass(BaseLogicGraph)

UITweenMainBg.graphId = "UITweenMainBg"
UITweenMainBg.timers = {}

function UITweenMainBg:Play()
	GraphDebug.Enter("Custom_Tween_TweenScale_2")
	Custom_Tween_TweenScale:Do(self.scaler, self.Custom_Tween_TweenScale_2_destination, self.Custom_Tween_TweenScale_2_duration, self.scaleEaseType)
	GraphDebug.Enter("Util_AddTimer_2")
	self.timers.AddTimer_2 = Util_AddTimer:Do(function() self:Callback_0_2() end, self.Util_AddTimer_2_delay, self.Util_AddTimer_2_count)
	GraphDebug.Enter("Util_AddTimer_1")
	self.timers.AddTimer_1 = Util_AddTimer:Do(function() self:Callback_0_1() end, self.Util_AddTimer_1_delay, self.Util_AddTimer_1_count)
	GraphDebug.Enter("Util_AddTimer_3")
	self.timers.AddTimer_3 = Util_AddTimer:Do(function() self:Callback_0_3() end, self.totalTime, self.Util_AddTimer_3_count)
end

function UITweenMainBg:Callback_0_2()
	GraphDebug.Enter("Graph_Callback_0_2")
	GraphDebug.Enter("Custom_Tween_TweenAnchorPos_1")
	Custom_Tween_TweenAnchorPos:Do(self.containerMap, self.destination, self.Custom_Tween_TweenAnchorPos_1_duration, self.moveEaseType)
end

function UITweenMainBg:Callback_0_1()
	GraphDebug.Enter("Graph_Callback_0_1")
	GraphDebug.Enter("Custom_Tween_TweenScale_1")
	Custom_Tween_TweenScale:Do(self.scaler, self.Custom_Tween_TweenScale_1_destination, self.zoomOutTime, self.scaleEaseType)
end

function UITweenMainBg:Callback_0_4()
	GraphDebug.Enter("Graph_Callback_0_4")
	GraphDebug.Enter("Custom_Tween_TweenRotate_1")
	Custom_Tween_TweenRotate:Do(self.containerMap, self.Custom_Tween_TweenRotate_1_destination, self.Custom_Tween_TweenRotate_1_duration)
end

function UITweenMainBg:Callback_0_5()
	GraphDebug.Enter("Graph_Callback_0_5")
	GraphDebug.Enter("Custom_Tween_TweenRotate_2")
	Custom_Tween_TweenRotate:Do(self.containerMap, self.Custom_Tween_TweenRotate_2_destination, self.Custom_Tween_TweenRotate_2_duration)
end

function UITweenMainBg:Callback_0_3()
	GraphDebug.Enter("Graph_Callback_0_3")
	GraphDebug.Enter("Custom_Dispatcher_Dispatch_1")
	Custom_Dispatcher_Dispatch:Do(self.Custom_Dispatcher_Dispatch_1_event, 1)
end

return UITweenMainBg