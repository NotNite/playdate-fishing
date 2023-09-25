---@param change number
---@param accelChange number
function playdate.cranked(change, accelChange)
	log("crankMove", change .. " " .. accelChange)
end

function playdate.crankDocked()
	log("crankDock", true)
end

function playdate.crankUndocked()
	log("crankDock", false)
end

function playdate.upButtonDown()
	fishing.ui.up()
end

function playdate.downButtonDown()
	fishing.ui.down()
end

function playdate.AButtonDown()
	fishing.ui.select()
end

function playdate.update()
	fishing.ui.draw()
end
