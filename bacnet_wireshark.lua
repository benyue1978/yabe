-- define a new protocol
local my_bacnet_detector = Proto("my_bacnet_detector", "My BACnet Detector (Heuristic)")

print("[Lua] my_bacnet_detector script loaded OK.")

-- dissector function (heuristic)
function my_bacnet_detector.dissector(buffer, pinfo, tree)
    print("[Lua] Heuristic dissector called. Packet length:", buffer:len())

    -- if the packet is too small, skip
    if buffer:len() < 4 then
        print("[Lua] Buffer too small, skipping.")
        return false
    end

    -- check the BVLC type
    local first_byte = buffer(0,1):uint()
    print(string.format("[Lua] First byte: 0x%02X", first_byte))

    if first_byte ~= 0x81 then
        print("[Lua] Not a BACnet BVLC packet, skipping.")
        return false
    end

    print("[Lua] Detected BACnet packet!")

    -- modify the Packet List protocol column
    pinfo.cols.protocol = "MyBACnet"

    -- add to the Packet Details view
    local subtree = tree:add(my_bacnet_detector, buffer(), "My BACnet Heuristic Detector")
    subtree:add(buffer(0,1), "BVLC Type: " .. string.format("0x%02X", first_byte))

    -- try to call the built-in BACnet dissector
    local bacnet_proto = Dissector.get("bacnet")
    if bacnet_proto then
        print("[Lua] Found built-in BACnet dissector, calling it...")
        bacnet_proto:call(buffer, pinfo, tree)
    else
        print("[Lua] ERROR: Built-in BACnet dissector not found!")
    end

    return true
end

-- register as heuristic UDP dissector
my_bacnet_detector:register_heuristic("udp", my_bacnet_detector.dissector)
print("[Lua] Registered my_bacnet_detector as heuristic dissector for UDP.")