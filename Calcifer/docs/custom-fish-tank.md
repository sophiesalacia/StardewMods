<!-- can merge this into README later as desired -->
## Custom Fish Tank

Feature to create custom fish tank furniture.

It's not strictly required to use this feature, if you just want a normal fish tank the `fishtank` type is sufficient. However with this feature you can:
- Change tank `capacity` and bounds (`xPos`, `yPos`, `width`, `height`).
- Make a `painting` into a fish tank to have wall mounted fish tanks.

Besides `fishtank`, only `painting` is allowed to become a fish tank through this feature. **WARNING**: if the tank is 1x1 and the player places it on a table, the fish inside will be lost! This is a vanilla problem with dressers as well, so please just don't allow this to happen.

To use this, add a special context tag `calcifer_fishtank_c(capacity)x(xPos)y(yPos)w(width)h(height)` to your furniture. All numbers are integers, no decimals allowed. The bounds are in pixel, and it is relative to the top left tile of the bounding box.

When used with [connected textures](./connected-textures.md), the tank bounds merge horizontally and you effectively end up with a big combined tank, though the actual fish inventory is still per 

_Note:_ Because context tags are saved with the furniture until a save is reloaded, you will need to spawn a new Fish Tank after `patch reload`.

## Examples


```json
{
    "Action": "EditData",
    "Target": "Data/Furniture",
    "Entries": {
        // regular tank
        "{{ModId}}_fishtank_horizontal": "{{ModId}}_fishtank_horizontal/fishtank/2 4/2 1/1/1000/-1/{{ModId}}_fishtank_horizontal/0/{{ModId}}\\fishtank_horizontal/false/calcifer_fishtank_c4x4y-128w128h164",
        // wall tank
        "{{ModId}}_fishtank_horizontal_wall": "{{ModId}}_fishtank_horizontal_wall/painting/2 4/2 3/1/1000/-1/{{ModId}}_fishtank_horizontal_wall/0/{{ModId}}\\fishtank_horizontal/false/calcifer_fishtank_c4x0y0w128h164",
        // thin wall tank
        "{{ModId}}_fishtankthin_horizontal_wall": "{{ModId}}_fishtankthin_horizontal_wall/painting/1 3/1 3/1/1000/-1/{{ModId}}_fishtankthin_horizontal_wall/0/{{ModId}}\\fishtankthin_horizontal/false/calcifer_fishtank_c2x0y40w64h120"
    }
},
```

The exact pattern is [^calcifer_fishtank_c(\d+)x(-?\d+)y(-?\d+)w(\d+)h(\d+)$](https://regex101.com/r/4QBUTI/1).

See [example pack here](./examples/CustomFishTankExamples).
