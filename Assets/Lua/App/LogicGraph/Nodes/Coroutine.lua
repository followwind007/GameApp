local coroutine = coroutine

---@class LogicGraph.Coroutine
local Coroutine = {}

---@param seconds number
function Coroutine.Wait(seconds)
    coroutine.wait(seconds)
end

---@param name string
function Coroutine.WaitEvent_0(name)
    local co = coroutine.running()
    local f = function()
        coroutine.resume(co)
    end
    dispatcher:add(name, f)
    coroutine.yield()
    dispatcher:remove(name, f)
end


---@param name string
---@param param1 Elua
function Coroutine.WaitEvent_1(name, param1)
    local co = coroutine.running()
    local f = function(p1)
        if table.contains(param1, p1) then 
            coroutine.resume(co) 
        end
    end
    dispatcher:add(name, f)
    coroutine.yield()
    dispatcher:remove(name, f)
end

---@param name string
---@param param1 Elua
---@param param2 Elua
function Coroutine.WaitEvent_2(name, param1, param2)
    local co = coroutine.running()
    local f = function(p1, p2)
        if table.contains(param1, p1) and table.contains(param2, p2) then 
            coroutine.resume(co)
        end
    end
    dispatcher:add(name, f)
    coroutine.yield()
    dispatcher:remove(name, f)
end

---@param frames integer
function Coroutine.Step(frames)
    coroutine.step(frames)
end

function Coroutine.Yield()
    coroutine.yield()
end

---@param co Elua
function Coroutine.Resume(co)
    coroutine.resume(co)
end

return Coroutine