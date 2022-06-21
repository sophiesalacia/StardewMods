using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using System;
using System.Collections.Generic;

namespace CarWarp
{
	class CarWarp // : IAssetLoader
	{
		private static Dictionary<string, WarpLocationModel> WarpLocations;
		//private static string carTexturePath = "sophie\\CarWarp\\CurrentCarTexture";
		//private static string windshieldTexturePath = "sophie\\CarWarp\\WindshieldTexture";

		private Building car;
		private Vector2 carDriversSeat;
		//private Texture2D carTexture;

		public CarWarp(Building car)
		{
			this.car = car;

			float driversSeatOffset = GetDriversSeatOffset();
			// get drivers seat of car into Activate function
			carDriversSeat = new Vector2(
					x: (car.tileX.Value + driversSeatOffset) * 64f,
					y: (car.tileY.Value + 4.75f) * 64f
				);

			//carTexture = paintedTexture;

			//carOverlayTexture = new Texture2D()
		}

		public CarWarp()
		{
			car = null;
			carDriversSeat = Game1.player.Position;
		}

		/// <summary>
		/// Creates the car warp question dialogue.
		/// </summary>
		public void Activate()
		{
			// invalidate and reload warp locations asset so that it is up-to-date
			Globals.ContentHelper.InvalidateCache(Globals.WarpLocationsContentPath);
			WarpLocations = Globals.ContentHelper.Load<Dictionary<string, WarpLocationModel>>(Globals.WarpLocationsContentPath, StardewModdingAPI.ContentSource.GameContent);

			// build list of destination responses
			List<Response> responses = new List<Response>();
			foreach ((string key, WarpLocationModel warpLoc) in WarpLocations)
			{
				// validate destination before adding response
				if (IsValidLocation(warpLoc.Location))
				{
					// add response keyed to the warp location model key
					responses.Add(new Response(key, warpLoc.DisplayName));
				}
			}

			// add cancel option
			responses.Add(new Response("Cancel", Game1.content.LoadString("Strings\\Locations:MineCart_Destination_Cancel")));

			// create question dialogue
			Game1.currentLocation.createQuestionDialogue("", responses.ToArray(), AnswerCarWarp);
		}

		/// <summary>
		/// Handles the response to the car warp question dialogue.
		/// </summary>
		private void AnswerCarWarp(Farmer who, string answer)
		{
			// back out if selected answer is Cancel
			if (answer == "Cancel")
				return;

			// use key to get warp location details from WarpLocations
			WarpLocationModel selectedWarp = WarpLocations[answer];

			StartCarWarp(selectedWarp);

		}

		private void StartCarWarp(WarpLocationModel warp)
		{
			// put player in car
			Game1.player.Position = carDriversSeat;
			Game1.player.faceDirection(2);

			// freeze player in place
			Game1.player.freezePause = 3000;

			// player door open sound
			Game1.currentLocation.playSound("doorClose");

			// slightly randomize engine pitch
			int enginePitch = Game1.random.Next(1000, 2000);

			// play engine starting up sound after 1 second
			DelayedAction.playSoundAfterDelay("busDriveOff", 1000, Game1.currentLocation, enginePitch);

			// 5% chance to "honk" horn
			if (Game1.random.Next(99) < 5)
			{
				DelayedAction.playSoundAfterDelay("Duck", 3000, Game1.currentLocation, enginePitch * 8);
				DelayedAction.playSoundAfterDelay("Duck", 3250, Game1.currentLocation, enginePitch * 8);
			}

			// initiate warp after roughly 3 seconds
			// the lower the engine pitch, the longer the sound is drawn out, so we delay the warp a little to compensate
			// otherwise the sound lingers unpleasantly on the new map
			DelayedAction.functionAfterDelay(
					delegate
						{
							LocationRequest locationRequest = Game1.getLocationRequest(warp.Location);
							Game1.warpFarmer(locationRequest, warp.X, warp.Y, 2);
						},
					3300 - (enginePitch / 10)
				);
		}

		/// <summary>
		/// Checks to make sure provided location name is a valid location in the game.
		/// </summary>
		private static bool IsValidLocation(string loc)
		{
			return Game1.getLocationFromName(loc) is not null;
		}

		private float GetDriversSeatOffset()
		{
			// 1f - left side
			// or 2f - right side

			switch (Globals.Config.Configuration)
			{
				case "Right":
					return 2f;

				case "Left":
					return 1f;

				case "None":
					return GetClosestSide();

				case "Empty":
					return GetClosestSide();

				default:
					return 2f;
			}
		}

		private float GetClosestSide()
		{
			// do not adjust position if there is no car
			if (car is null)
				return 0f;

			float playerX = Game1.player.Position.X;
			float carX = car.tileX.Value * 64f;

			return playerX < carX ? 1f : 2f;
		}
	}
}
