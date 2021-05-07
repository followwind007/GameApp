
---@param val @value to be dumped
---@param desc @optional: identity of the value
---@param nesting @optional: nest indent, default to be 3
---@param hideFlag @optional: default print
function dump(val, desc, nesting, hideFlag)
    if type(nesting) ~= "number" then nesting = 3 end
    local result = {}

    local function dump_value_(v)
        if type(v) == "string" then
            v = "\"" .. v .. "\""
        end
        return tostring(v)
    end

    local function dump_(value, description, indent, nest, keylen)
        description = description or type(value)
        local spc = ""
        if type(keylen) == "number" then
            spc = string.rep(" ", keylen - string.len(dump_value_(description)))
        end
        if type(value) ~= "table" then
            result[#result + 1] = string.format("%s%s%s = %s", indent, dump_value_(description), spc, dump_value_(value))
        else
            if nest > nesting then
                result[#result + 1] = string.format("%s%s = *MAX NESTING*", indent, dump_value_(description))
            else
                result[#result + 1] = string.format("%s%s = {", indent, dump_value_(description))
                local indent2 = indent.."    "
                local _keys = {}
                keylen = keylen == nil and 0 or keylen
                local values = {}
                for k, v in pairs(value) do
                    _keys[#_keys + 1] = k
                    local vk = dump_value_(k)
                    local vkl = string.len(vk)
                    if vkl > keylen then keylen = vkl end
                    values[k] = v
                end
                table.sort(_keys, function(a, b)
                    if type(a) == "number" and type(b) == "number" then
                        return a < b
                    else
                        return tostring(a) < tostring(b)
                    end
                end)
                for _, k in ipairs(_keys) do
                    dump_(values[k], k, indent2, nest + 1, keylen)
                end
                result[#result +1] = string.format("%s}", indent)
            end
        end
    end

    dump_(val, desc, " ", 1)
    
    local res = ""
    for _, line in ipairs(result) do
        res = res..line.."\n"
    end
    
    if res ~= nil and not hideFlag then
        local traceback = string.split(debug.traceback("", 2), "\n")
        local from = "dump from: "..traceback[3]..(desc or "")
        print(from.."\n"..res)
    end
    return res
end

---@vararg string
function printstack(...)
    local args = {...}
    local msg = ""
    for _, v in pairs(args) do
        local s = tostring(v)
        if s then msg = msg..s.."\t" end
    end
    local stack = debug.traceback("", 2)
    
    local res = dump(stack, "stack", 3, true)
    
    print(msg.."\n"..res) 
end