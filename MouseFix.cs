using UnityEngine;
using Harmony12;
using System;
using System.Reflection;
using UnityModManagerNet;

namespace MouseFix
{
    internal static class Main
	{
        private static bool Load(UnityModManager.ModEntry modEntry)
		{
			Main.mod = modEntry;
			modEntry.OnToggle = new Func<UnityModManager.ModEntry, bool, bool>(Main.OnToggle);
			HarmonyInstance harmonyInstance = HarmonyInstance.Create(modEntry.Info.Id);
			harmonyInstance.PatchAll(Assembly.GetExecutingAssembly());
			return true;
		}

		private static bool OnToggle(UnityModManager.ModEntry modEntry, bool value)
		{
			Main.enabled = value;
			modEntry.Logger.Log("Starting MouseFix");
			return true;
		}
		public static bool enabled;

		public static UnityModManager.ModEntry mod;
	}
	public class MouseFix {

		[HarmonyPatch(typeof(IntroPlayer))]
		[HarmonyPatch("Awake")]
		internal class BackgroundExecution
		{
			private static void Prefix()
			{
				Application.runInBackground = true;
			}
		}

		[HarmonyPatch(typeof(Cursor3D))]
		[HarmonyPatch("NormalCursorLock")]
		[HarmonyPatch(new[] { typeof(bool) })]
		internal class MouseLock
		{
			public static bool Prefix(bool isLocked)
			{
				var gameScript = GameScript.Get();
				if (GameMode.Get().CompareWithCurrentMode(gameMode.UI) ||
					gameScript != null && gameScript.CurrentSceneType == SceneType.Menu ||
					gameScript.CurrentSceneType == SceneType.Showroom ||
					gameScript.CurrentSceneType == SceneType.Auction)
				{
					Screen.lockCursor = false;
					if (isLocked)
					{
						Cursor.visible = false;
						return false;
					}

					Cursor.lockState = 0;
					return false;
				}

				return true;
			}
		}

		[HarmonyPatch(typeof(Cursor3D))]
		[HarmonyPatch("MouseMove")]
		internal class MouseMovement
		{
			private static bool Prefix(ref Vector2 ___screenPos, ref RectTransform ___Cursor)
			{
				var gameScript = GameScript.Get();
				if (GameMode.Get().CompareWithCurrentMode(gameMode.UI) ||
					gameScript != null && gameScript.CurrentSceneType == SceneType.Menu ||
					gameScript.CurrentSceneType == SceneType.Showroom ||
					gameScript.CurrentSceneType == SceneType.Auction)
				{
					___screenPos.x = Input.mousePosition.x;
					___screenPos.y = Input.mousePosition.y;
					___Cursor.transform.position = ___screenPos;
					return false;
				}

				return true;
			}
		}
	}
}