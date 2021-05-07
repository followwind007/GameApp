
function TestParam(a, b, c)
    print(a, b, c)
    dump(debug.getinfo(1))
end

--TestParam(1, 2, 3)