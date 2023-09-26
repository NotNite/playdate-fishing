---@meta

---@class playdate.graphics.image
local playdate_graphics_image = {}

---@class playdate.graphics.sprite
local playdate_graphics_sprite = {
  ---@param self playdate.graphics.sprite
  ---@param x number
  ---@param y number
  moveTo = function(self, x, y) end,

  ---@param self playdate.graphics.sprite
  add = function(self) end,

  ---@type number
  x = 0,
  ---@type number
  y = 0,
}

---@class playdate.graphics.animator
local playdate_graphics_animator = {
  ---@param self playdate.graphics.animator
  ---@return boolean
  ended = function(self) end,

  ---@param self playdate.graphics.animator
  ---@return number
  currentValue = function(self) end,

  ---@param self playdate.graphics.animator
  ---@param time number
  ---@return number
  valueAtTime = function(self, time) end,
}

---@class playdate.graphics.font
local playdate_graphics_font = {}

playdate = {
  graphics = {
    kColorWhite = 1,

    image = {
      ---@param path string
      ---@return playdate.graphics.image
      new = function(path) end,
    },

    sprite = {
      ---@param image playdate.graphics.image
      ---@return playdate.graphics.sprite
      new = function(image) end,
    },

    animator = {
      ---@param duration number
      ---@param start_value number
      ---@param end_value number
      ---@return playdate.graphics.animator
      new = function(duration, start_value, end_value, easing_function) end,
    },

    font = {
      ---@param path string
      ---@return playdate.graphics.font
      new = function(path) end,
    },
  },
}
