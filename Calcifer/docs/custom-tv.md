<!-- can merge this into README later as desired -->
## Custom TV

Feature to create custom TV furniture.

To make any furniture into a TV, add a special context tag `calcifer_tv_x(offsetX)y(offsetY)s(scale)` to your furniture. You can use decimal numbers if needed, and the XY offset is from the top left point of the furniture's bounding box. While there is no code restriction on what can become a TV, you will not be able to interact with a `rug` even if it has been made into a TV.

You cannot change the aspect ratio of the TV, they are still 3:2 with base resolution of 42x28.

Furniture context tags are space separated, so you can still add context tag for catalogue too.

_Note:_ Because context tags are saved with the furniture until a save is reloaded, you will need to spawn a new TV after `patch reload`.

## Examples

```
{
    "Action": "EditData",
    "Target": "Data/Furniture",
    "Entries": {
        // floor TV
        "{{ModId}}_tv_horizontal": "{{ModId}}_tv_horizontal/decor/3 4/3 1/1/2500/2/{{ModId}}_tv_horizontal/0/{{ModId}}\\tv_horizontal/true/calcifer_tv_x12y-148s4",
        // wall TV
        "{{ModId}}_tv_horizontal_wall": "{{ModId}}_tv_horizontal_wall/painting/3 4/3 3/1/2500/2/{{ModId}}_tv_horizontal_wall/0/{{ModId}}\\tv_horizontal/true/calcifer_tv_x12y-20s4",
    }
},
```

The exact pattern is [^calcifer_tv_x(-?\d+(?:\.\d+)?)y(-?\d+(?:\.\d+)?)s(-?\d+(?:\.\d+)?)$](https://regex101.com/r/9QZhP5/1).

See [example pack here](./examples/CustomTVExamples).