-- 定义一个新的协议（注意不要冲突）
local my_bacnet_detector = Proto("my_bacnet_detector", "My BACnet Detector (Heuristic)")

print("[Lua] my_bacnet_detector script loaded OK.")

-- dissector 函数（启发式）
function my_bacnet_detector.dissector(buffer, pinfo, tree)
    print("[Lua] Heuristic dissector called. Packet length:", buffer:len())

    -- 如果包太小，跳过
    if buffer:len() < 4 then
        print("[Lua] Buffer too small, skipping.")
        return false
    end

    -- 检查BVLC类型
    local first_byte = buffer(0,1):uint()
    print(string.format("[Lua] First byte: 0x%02X", first_byte))

    if first_byte ~= 0x81 then
        print("[Lua] Not a BACnet BVLC packet, skipping.")
        return false
    end

    print("[Lua] Detected BACnet packet!")

    -- 修改Packet List协议列
    pinfo.cols.protocol = "MyBACnet"

    -- 添加到Packet Details视图
    local subtree = tree:add(my_bacnet_detector, buffer(), "My BACnet Heuristic Detector")
    subtree:add(buffer(0,1), "BVLC Type: " .. string.format("0x%02X", first_byte))

    -- 尝试调用内置BACnet解析器
    local bacnet_proto = Dissector.get("bacnet")
    if bacnet_proto then
        print("[Lua] Found built-in BACnet dissector, calling it...")
        bacnet_proto:call(buffer, pinfo, tree)
    else
        print("[Lua] ERROR: Built-in BACnet dissector not found!")
    end

    return true
end

-- 注册为UDP启发式解析器
my_bacnet_detector:register_heuristic("udp", my_bacnet_detector.dissector)
print("[Lua] Registered my_bacnet_detector as heuristic dissector for UDP.")