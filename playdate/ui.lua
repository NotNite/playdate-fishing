local gfx = playdate.graphics

local options = {
	"Cast",
	"Patience",
	"Patience II",
	"Prize Catch",
	"Identical Cast",
	"Surface Slap",
	"Thaliak's Favor",
	"Hi-Cordial",
	"Mooch",
	"Mooch II",
}
local option = 1

gfx.setColor(gfx.kColorWhite)
gfx.fillRect(0, 0, 400, 240)
gfx.setBackgroundColor(gfx.kColorWhite)

---@class fishing.ui
fishing.ui = {}

function fishing.ui.draw()
	gfx.clear()
	gfx.drawText(options[option], 16, 16)
end

function fishing.ui.up()
	option = option - 1

	if option < 1 then
		option = #options
	end
end

function fishing.ui.down()
	option = option + 1

	if option > #options then
		option = 1
	end
end

function fishing.ui.select()
	log("option", option)
end
