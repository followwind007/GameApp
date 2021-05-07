local LogicGraph_Property = require('App.LogicGraph.Nodes.Property')

local Flow1 = DefineClass(BaseLogicGraph)

Flow1.graphId = 'Flow1'
Flow1.timers = {}

function Flow1:Start()
	GraphDebug.Enter("UnityEngine_GameObject_New_1")
	local UnityEngine_GameObject_New_1_return_0 = UnityEngine.GameObject.New(self.UnityEngine_GameObject_New_1_name)
	GraphDebug.Enter("LogicGraph_Property_Set_1")
	LogicGraph_Property.Set(UnityEngine_GameObject_New_1_return_0, self.LogicGraph_Property_Set_1_field, "TestName")
	GraphDebug.Enter("UnityEngine_GameObject_SetActive_1")
	UnityEngine.GameObject.SetActive(UnityEngine_GameObject_New_1_return_0, self.UnityEngine_GameObject_SetActive_1_value)
	GraphDebug.Enter("UnityEngine_GameObject_GetComponent_1")
	local UnityEngine_GameObject_GetComponent_1_return_0 = UnityEngine.GameObject.GetComponent(UnityEngine_GameObject_New_1_return_0, self.UnityEngine_GameObject_GetComponent_1_type)
	GraphDebug.Enter("UnityEngine_Vector3_New_2")
	local UnityEngine_Vector3_New_2_return_0 = UnityEngine.Vector3.New(self.UnityEngine_Vector3_New_2_x, self.UnityEngine_Vector3_New_2_y, self.UnityEngine_Vector3_New_2_z)
	GraphDebug.Enter("LogicGraph_Property_Set_2")
	LogicGraph_Property.Set(UnityEngine_GameObject_GetComponent_1_return_0, self.LogicGraph_Property_Set_2_field, UnityEngine_Vector3_New_2_return_0)
end

return Flow1