---@alias integer number

local Init = {}

function Init:Start()
	print("init start")
	self:PrepareEnv()
	self:Init()
	print("init end")
end

function Init:PrepareEnv()
	print("global property counts:", #_G)
end

function Init:Init()
	require "Common.Init"
	require "GUIS.Init"
	require "Test.Init"
	require "App.Adapter"
	require "Helper.Init"
	require "LogicGraph.Init"
	require "BehaviourTree.Init"
end

return Init