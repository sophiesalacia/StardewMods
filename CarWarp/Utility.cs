using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarWarp
{
    static class Utility
    {
        public static Texture2D GetAlphaMaskedTexture(Texture2D original, Texture2D mask)
        {
            if (original.Width != mask.Width || original.Height != mask.Height)
            {
                throw new ArgumentException("Invalid mask size for input texture.");
            }

            Color[] outputData = new Color[original.Width * original.Height];
            original.GetData(outputData);

            Color[] originalData = new Color[original.Width * original.Height];
            original.GetData(originalData);

            Color[] maskData = new Color[original.Width * original.Height];
            mask.GetData(maskData);

            for (int x = 0; x < original.Width; x++)
            {
                for (int y = 0; y < original.Height; y++)
                {
                    // convert x and y to 1-dimensional index
                    int index = x + y * mask.Width;

                    // keep RGB of original and set A of output to A of mask
                    outputData[index].A = maskData[index].A;
                }
            }

			// construct new Texture2D and assign it the output data
			Texture2D output = new Texture2D(Game1.graphics.GraphicsDevice, original.Height, original.Width);
            output.SetData(outputData);

            return output;
        }

    }
}
