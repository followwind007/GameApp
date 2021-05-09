local LogicGraph_Property = require('app.LogicGraph.Nodes.Property')

local Flow0 = DefineClass(BaseLogicGraph)

function Flow0:Ctor()
	self.graphId = 'Flow0'
	self.timers = {}
	self.coroutines = {}

end

function Flow0:Start()
	GraphDebug.Enter("UnityEngine.GameObject.Find_1")
	local UnityEngine_GameObject_Find_1_return_1 = UnityEngine.GameObject.Find(self.UnityEngine_GameObject_Find_1_name)
	GraphDebug.Enter("LogicGraph.Property.Get_1")
	local LogicGraph_Property_Get_1_return_1 = LogicGraph_Property.Get(UnityEngine_GameObject_Find_1_return_1, self.LogicGraph_Property_Get_1_field)
	self.cubeTransform = LogicGraph_Property_Get_1_return_1
end

function Flow0:Update()
	GraphDebug.Enter("UnityEngine.Transform.Rotate_1")
	UnityEngine.Transform.Rotate(self.cubeTransform, self.UnityEngine_Transform_Rotate_1_eulers, self.UnityEngine_Transform_Rotate_1_relativeTo)
end

return Flow0