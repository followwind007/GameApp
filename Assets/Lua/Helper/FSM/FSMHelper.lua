---
--- Generated by EmmyLua(https://github.com/EmmyLua)
--- Created by hzcaolei01.
--- DateTime: 2019/3/7 15:07
---

local FSMHelper = DefineClass()

function FSMHelper:GetTimerCondition(seconds)
    if self.m_timerId == nil then
        self.m_timerId = 0
    end

    self.m_timerId = self.m_timerId + 1

    local timerCondition = {
        id = self.m_timerId,
        start = function()
            Timer.DelayCall(function()
                dispatcher:dispatch(EventNames.TIMER_CONDITION, self.m_timerId)
            end, seconds)
        end
    }

    return timerCondition
end

return FSMHelper