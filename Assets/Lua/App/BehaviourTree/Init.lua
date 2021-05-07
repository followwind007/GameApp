
require "App.BehaviourTree.BTAdapter"

BehaviourTree = declare("BehaviourTree", {})
BehaviourTree = {}

---@type BehaviourTree.TaskStatus
BehaviourTree.TS = require("App.BehaviourTree.Base.TaskStatus")()

BehaviourTree.Task = require("App.BehaviourTree.Base.Task")