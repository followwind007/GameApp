local Init = require("Init")
--主入口函数。从这里开始lua逻辑
function Main()					
	print("logic start")
	Init:Start()
end

--场景切换通知
function OnLevelWasLoaded(_)
	collectgarbage("collect")
	Time.timeSinceLevelLoad = 0
end

function OnApplicationQuit()
end

Main()