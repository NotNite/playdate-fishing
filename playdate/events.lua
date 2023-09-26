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

function playdate.leftButtonDown()
	fishing.ui.left()
end

function playdate.rightButtonDown()
	fishing.ui.right()
end

function playdate.AButtonDown()
	fishing.ui.select()
end

function playdate.update()
	fishing.ui.draw()
end
