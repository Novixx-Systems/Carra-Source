while true do
    local pos = player.getPosition()
    local x
    local y
    local didX = false
    for word in string.gmatch(pos, '([^,]+)') do
        if didX then
            y = word
        else
            x = word
            didX = true
        end
    end
    x = tonumber(x) or 0
    y = tonumber(y) or 0
    -- If player is in water (y = 0 to 5, x = 0 to 10)
    if player.isColliding(1) then
        -- Set the player's speed to 1
        player.setSpeed(1)
        player.showMessage("You are in water!")
    end
end