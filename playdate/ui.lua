local gfx = playdate.graphics

gfx.setColor(gfx.kColorWhite)
gfx.fillRect(0, 0, 400, 240)
gfx.setBackgroundColor(gfx.kColorWhite)

function fishing.ui()
	gfx.clear()
	gfx.drawText("meow", 0, 0)
end
