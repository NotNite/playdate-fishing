local gfx = playdate.graphics
local font = gfx.font.new("assets/Asheville-Mono-Light-24-px")
gfx.setColor(gfx.kColorWhite)
gfx.fillRect(0, 0, 400, 240)
gfx.setBackgroundColor(gfx.kColorWhite)

local options = {
  "Cast",
  "Hook",
  "Quit",
  "Double Hook",
  "Triple Hook",
  "Thaliak's Favor",
  "Hi-Cordial",
  "Patience",
  "Patience II",
  "Prize Catch",
  "Identical Cast",
  "Surface Slap",
  "Mooch",
  "Mooch II",
}
local option = 1

local sprite_size = 80
local sprite_padding = 32
local sprite_start_x = (playdate.display.getWidth() / 2)
local sprite_start_y = (playdate.display.getHeight() / 2)
local sprite_move = sprite_size + sprite_padding

---@type playdate.graphics.sprite[]
local sprites = {}

---@type table<playdate.graphics.sprite, playdate.graphics.animator>
local running_animators = {}

for i, option in pairs(options) do
  local path = "assets/actions/" .. option
  local image = playdate.graphics.image.new(path)
  local sprite = playdate.graphics.sprite.new(image)

  local x = sprite_start_x + (sprite_move * (i - 1))
  local y = sprite_start_y

  sprite:moveTo(x, y)
  sprite:add()
  table.insert(sprites, sprite)
end

---@class fishing.ui
fishing.ui = {}

function fishing.ui.draw()
  for sprite, animator in pairs(running_animators) do
    if animator:ended() then
      running_animators[sprite] = nil
    else
      sprite:moveTo(animator:currentValue(), sprite.y)
    end
  end

  gfx.clear()
  gfx.sprite.update()

  gfx.setFont(font)
  gfx.drawTextAligned(
    options[option],
    playdate.display.getWidth() / 2,
    playdate.display.getHeight() / 8,
    kTextAlignment.center
  )
end

---@param delta number
local function shift(delta)
  local duration = 500

  for _, sprite in pairs(sprites) do
    local target = sprite.x + delta
    if running_animators[sprite] then
      local end_time = running_animators[sprite]:valueAtTime(duration)
      target = end_time + delta
      running_animators[sprite] = nil
    end

    local animator = playdate.graphics.animator.new(
      duration,
      sprite.x,
      target,
      playdate.easingFunctions.outQuad
    )

    running_animators[sprite] = animator
  end
end

function fishing.ui.left()
  option = option - 1

  if option < 1 then
    option = 1
    return
  end

  log("hover", option)
  shift(sprite_move)
end

function fishing.ui.right()
  option = option + 1

  if option > #options then
    option = #options
    return
  end

  log("hover", option)
  shift(-sprite_move)
end

function fishing.ui.select()
  log("option", option)
end
