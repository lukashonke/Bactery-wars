// Copyright (c) 2015, Lukas Honke
// ========================
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.scripts.Base
{
	public class CustomGraphics
	{
		public static Sprite LoadSprite(string path, int width, int height, int ppi=100)
		{
			Texture2D tex = LoadTexture(path, width, height);

			if (tex != null)
			{
				Sprite sprite = Sprite.Create(tex,
				new Rect(0, 0, tex.width, tex.height),
				new Vector2(0.5f, 0.5f),
				ppi);

				return sprite;
			}
			return null;
		}

		public static Texture2D LoadTexture(string path, int width, int height)
		{
			try
			{
				byte[] data = File.ReadAllBytes("extern_graphics/" + path + ".png");
				Texture2D texture = new Texture2D(width, height, TextureFormat.DXT1, false);
				texture.LoadImage(data);
				texture.name = Path.GetFileNameWithoutExtension(path);

				return texture;
			}
			catch (Exception e)
			{
				Debug.LogError("tex not found: " + e.StackTrace);
				return null;
			}
		}
	}
}
