function input(j)
	local data = json.decode(j)
	---@cast data table

	for k, v in pairs(data) do
		print(k, v)
	end
end

function log(type, msg)
	print("[playdate-fishing][" .. type .. "] " .. tostring(msg))
end
