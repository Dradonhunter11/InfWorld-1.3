using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameInput;
using Terraria.Localization;
using Terraria.ModLoader;
using static Terraria.Main;

namespace InfWorld.Patching.Detours
{
    static partial class Detours
    {
        private static void OnClampScreenPositionToWorld(On.Terraria.Main.orig_ClampScreenPositionToWorld orig)
        {
            return;
        }

		/*private static void OnDrawMap(On.Terraria.Main.orig_DrawMap orig, Terraria.Main self)
		{
			string text = "";
			if (!mapEnabled || !mapReady)
				return;

			float num = 0f;
			float num2 = 0f;
			float num3 = num;
			float num4 = num2;
			float num5 = 2f;
			byte b = byte.MaxValue;
			_ = maxTilesX / textureMaxWidth;
			int num6 = maxTilesY / textureMaxHeight;
			float num7 = Lighting.offScreenTiles;
			float num8 = Lighting.offScreenTiles;
			float num9 = maxTilesX - Lighting.offScreenTiles - 1;
			float num10 = maxTilesY - Lighting.offScreenTiles - 42;
			float num11 = 0f;
			float num12 = 0f;
			num7 = 10f;
			num8 = 10f;
			num9 = maxTilesX - 10;
			num10 = maxTilesY - 10;
			for (int i = 0; i < self.mapTarget.GetLength(0); i++)
			{
				for (int j = 0; j < self.mapTarget.GetLength(1); j++)
				{
					if (self.mapTarget[i, j] != null)
					{
						if (self.mapTarget[i, j].IsContentLost && !mapWasContentLost[i, j])
						{
							mapWasContentLost[i, j] = true;
							refreshMap = true;
							clearMap = true;
						}
						else if (!self.mapTarget[i, j].IsContentLost && mapWasContentLost[i, j])
						{
							mapWasContentLost[i, j] = false;
						}
					}
				}
			}

			num = 200f;
			num2 = 300f;
			float num13 = 0f;
			float num14 = 0f;
			float num15 = num9 - 1f;
			float num16 = num10 - 1f;
			num5 = (mapFullscreen ? mapFullscreenScale : ((mapStyle != 1) ? mapOverlayScale : mapMinimapScale));
			bool flag = false;
			Matrix transformMatrix = UIScaleMatrix;
			if (mapStyle != 1)
				transformMatrix = Matrix.Identity;

			if (mapFullscreen)
				transformMatrix = Matrix.Identity;

			if (!mapFullscreen && num5 > 1f)
			{
				spriteBatch.End();
				spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, transformMatrix);
				flag = true;
			}

			if (mapFullscreen)
			{
                
				int x = (int)(num + mapFullscreenScale * 10);
				int y = (int)(num2 + mapFullscreenScale * 10);
				int width = (int)((maxTilesX - 40) * mapFullscreenScale);
				int height = (int)((maxTilesY - 40) * mapFullscreenScale);
				var destinationRectangle = new Microsoft.Xna.Framework.Rectangle(x, y, width, height);
				spriteBatch.Draw(mapTexture, destinationRectangle, new Microsoft.Xna.Framework.Rectangle(40, 4, 848, 240), Microsoft.Xna.Framework.Color.White);
				int edgeWidth = (int)(40 * mapFullscreenScale * 5);
				int edgeHeight = (int)(4 * mapFullscreenScale * 5);
				destinationRectangle = new Microsoft.Xna.Framework.Rectangle(x - edgeWidth, y - edgeHeight, edgeWidth, height + 2 * edgeHeight);
				spriteBatch.Draw(mapTexture, destinationRectangle, new Microsoft.Xna.Framework.Rectangle(0, 0, 40, 248), Microsoft.Xna.Framework.Color.White);
				destinationRectangle = new Microsoft.Xna.Framework.Rectangle(x + width, y - edgeHeight, edgeWidth, height + 2 * edgeHeight);
				spriteBatch.Draw(mapTexture, destinationRectangle, new Microsoft.Xna.Framework.Rectangle(888, 0, 40, 248), Microsoft.Xna.Framework.Color.White);
				if (num5 < 1f)
				{
					spriteBatch.End();
					spriteBatch.Begin();
					flag = false;
				}
			}
			else if (mapStyle == 1)
			{
				miniMapWidth = 240;
				miniMapHeight = 240;
				miniMapX = screenWidth - miniMapWidth - 52;
				miniMapY = 90;
				_ = (float)miniMapHeight / (float)maxTilesY;
				if ((double)mapMinimapScale < 0.2)
					mapMinimapScale = 0.2f;

				if (mapMinimapScale > 3f)
					mapMinimapScale = 3f;

				if ((double)mapMinimapAlpha < 0.01)
					mapMinimapAlpha = 0.01f;

				if (mapMinimapAlpha > 1f)
					mapMinimapAlpha = 1f;

				num5 = mapMinimapScale;
				b = (byte)(255f * mapMinimapAlpha);
				num = miniMapX;
				num2 = miniMapY;
				num3 = num;
				num4 = num2;
				float num28 = (screenPosition.X + (float)(PlayerInput.RealScreenWidth / 2)) / 16f;
				float num29 = (screenPosition.Y + (float)(PlayerInput.RealScreenHeight / 2)) / 16f;
				num11 = (0f - (num28 - (float)(int)((screenPosition.X + (float)(PlayerInput.RealScreenWidth / 2)) / 16f))) * num5;
				num12 = (0f - (num29 - (float)(int)((screenPosition.Y + (float)(PlayerInput.RealScreenHeight / 2)) / 16f))) * num5;
				num15 = (float)miniMapWidth / num5;
				num16 = (float)miniMapHeight / num5;
				num13 = (float)(int)num28 - num15 / 2f;
				num14 = (float)(int)num29 - num16 / 2f;
				_ = (float)maxTilesY + num14;
				float x = num3 - 6f;
				float y = num4 - 6f;
				spriteBatch.Draw(miniMapFrame2Texture, new Vector2(x, y), new Microsoft.Xna.Framework.Rectangle(0, 0, miniMapFrame2Texture.Width, miniMapFrame2Texture.Height), new Microsoft.Xna.Framework.Color(b, b, b, b), 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
			}
			else if (mapStyle == 2)
			{
				float num30 = (float)screenWidth / (float)maxTilesX;
				if (mapOverlayScale < num30)
					mapOverlayScale = num30;

				if (mapOverlayScale > 16f)
					mapOverlayScale = 16f;

				if ((double)mapOverlayAlpha < 0.01)
					mapOverlayAlpha = 0.01f;

				if (mapOverlayAlpha > 1f)
					mapOverlayAlpha = 1f;

				num5 = mapOverlayScale;
				b = (byte)(255f * mapOverlayAlpha);
				_ = maxTilesX;
				_ = maxTilesY;
				float num31 = (screenPosition.X + (float)(screenWidth / 2)) / 16f;
				float num32 = (screenPosition.Y + (float)(screenHeight / 2)) / 16f;
				num31 *= num5;
				float num33 = num32 * num5;
				num = 0f - num31 + (float)(screenWidth / 2);
				num2 = 0f - num33 + (float)(screenHeight / 2);
				num += num7 * num5;
				num2 += num8 * num5;
			}

			if (mapStyle == 1 && !mapFullscreen)
			{
				if (num13 < num7)
					num -= (num13 - num7) * num5;

				if (num14 < num8)
					num2 -= (num14 - num8) * num5;
			}

			num15 = num13 + num15;
			num16 = num14 + num16;
			if (num13 > num7)
				num7 = num13;

			if (num14 > num8)
				num8 = num14;

			if (num15 < num9)
				num9 = num15;

			if (num16 < num10)
				num10 = num16;

			float num34 = (float)textureMaxWidth * num5;
			float num35 = (float)textureMaxHeight * num5;
			float num36 = num;
			float num37 = 0f;
			for (int k = 0; k <= mapTargetX - 1; k++)
			{
				if (!((float)((k + 1) * textureMaxWidth) > num7) || !((float)(k * textureMaxWidth) < num7 + num9))
					continue;

				for (int l = 0; l <= num6; l++)
				{
					if ((float)((l + 1) * textureMaxHeight) > num8 && (float)(l * textureMaxHeight) < num8 + num10)
					{
						float num38 = num + (float)(int)((float)k * num34);
						float num39 = num2 + (float)(int)((float)l * num35);
						float num40 = k * textureMaxWidth;
						float num41 = l * textureMaxHeight;
						float num42 = 0f;
						float num43 = 0f;
						if (num40 < num7)
						{
							num42 = num7 - num40;
							num38 = num;
						}
						else
						{
							num38 -= num7 * num5;
						}

						if (num41 < num8)
						{
							num43 = num8 - num41;
							num39 = num2;
						}
						else
						{
							num39 -= num8 * num5;
						}

						num38 = num36;
						float num44 = textureMaxWidth;
						float num45 = textureMaxHeight;
						float num46 = (k + 1) * textureMaxWidth;
						float num47 = (l + 1) * textureMaxHeight;
						if (num46 >= num9)
							num44 -= num46 - num9;

						if (num47 >= num10)
							num45 -= num47 - num10;

						num38 += num11;
						num39 += num12;
						if (num44 > num42)
							spriteBatch.Draw(self.mapTarget[k, l], new Vector2(num38, num39), new Microsoft.Xna.Framework.Rectangle((int)num42, (int)num43, (int)num44 - (int)num42, (int)num45 - (int)num43), new Microsoft.Xna.Framework.Color(b, b, b, b), 0f, default(Vector2), num5, SpriteEffects.None, 0f);

						num37 = (float)((int)num44 - (int)num42) * num5;
					}

					if (l == num6)
						num36 += num37;
				}
			}

			if (flag)
			{
				spriteBatch.End();
				spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, transformMatrix);
			}

			if (!mapFullscreen)
			{
				if (mapStyle == 2)
				{
					float num48 = (num5 * 0.2f * 2f + 1f) / 3f;
					if (num48 > 1f)
						num48 = 1f;

					num48 *= UIScale;
					for (int m = 0; m < 200; m++)
					{
						if (npc[m].active && npc[m].townNPC)
						{
							int num49 = NPC.TypeToHeadIndex(npc[m].type);
							if (num49 > 0)
							{
								SpriteEffects effects = SpriteEffects.None;
								if (npc[m].direction > 0)
									effects = SpriteEffects.FlipHorizontally;

								float num50 = (npc[m].position.X + (float)(npc[m].width / 2)) / 16f * num5;
								float num51 = (npc[m].position.Y + (float)(npc[m].height / 2)) / 16f * num5;
								num50 += num;
								num51 += num2;
								num50 -= 10f * num5;
								num51 -= 10f * num5;
								spriteBatch.Draw(npcHeadTexture[num49], new Vector2(num50, num51), new Microsoft.Xna.Framework.Rectangle(0, 0, npcHeadTexture[num49].Width, npcHeadTexture[num49].Height), new Microsoft.Xna.Framework.Color(b, b, b, b), 0f, new Vector2(npcHeadTexture[num49].Width / 2, npcHeadTexture[num49].Height / 2), num48, effects, 0f);
							}
						}

						if (!npc[m].active || npc[m].GetBossHeadTextureIndex() == -1)
							continue;

						float bossHeadRotation = npc[m].GetBossHeadRotation();
						SpriteEffects bossHeadSpriteEffects = npc[m].GetBossHeadSpriteEffects();
						Vector2 vector = npc[m].Center + new Vector2(0f, npc[m].gfxOffY);
						if (npc[m].type == 134)
						{
							Vector2 center = npc[m].Center;
							int num52 = 1;
							int num53 = (int)npc[m].ai[0];
							while (num52 < 15 && npc[num53].active && npc[num53].type >= 134 && npc[num53].type <= 136)
							{
								num52++;
								center += npc[num53].Center;
								num53 = (int)npc[num53].ai[0];
							}

							center /= (float)num52;
							vector = center;
						}

						int bossHeadTextureIndex = npc[m].GetBossHeadTextureIndex();
						float num54 = vector.X / 16f * num5;
						float num55 = vector.Y / 16f * num5;
						num54 += num;
						num55 += num2;
						num54 -= 10f * num5;
						num55 -= 10f * num5;
						spriteBatch.Draw(npcHeadBossTexture[bossHeadTextureIndex], new Vector2(num54, num55), null, new Microsoft.Xna.Framework.Color(b, b, b, b), bossHeadRotation, npcHeadBossTexture[bossHeadTextureIndex].Size() / 2f, num48, bossHeadSpriteEffects, 0f);
					}

					spriteBatch.End();
					spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, transformMatrix);
					for (int n = 0; n < 255; n++)
					{
						if (player[n].active && !player[n].dead && n != myPlayer && ((!player[myPlayer].hostile && !player[n].hostile) || (player[myPlayer].team == player[n].team && player[n].team != 0) || n == myPlayer))
						{
							float num56 = (player[n].position.X + (float)(player[n].width / 2)) / 16f * num5;
							float num57 = player[n].position.Y / 16f * num5;
							num56 += num;
							num57 += num2;
							num56 -= 6f;
							num57 -= 2f;
							num57 -= 2f - num5 / 5f * 2f;
							num56 -= 10f * num5;
							num57 -= 10f * num5;
							DrawPlayerHead(player[n], num56, num57, (float)(int)b / 255f, num48);
						}
					}

					spriteBatch.End();
					spriteBatch.Begin();
				}

				if (mapStyle == 1)
				{
					float num58 = num3 - 6f;
					float num59 = num4 - 6f;
					float num60 = (num5 * 0.25f * 2f + 1f) / 3f;
					if (num60 > 1f)
						num60 = 1f;

					for (int num61 = 0; num61 < 200; num61++)
					{
						if (npc[num61].active && npc[num61].townNPC)
						{
							int num62 = NPC.TypeToHeadIndex(npc[num61].type);
							if (num62 > 0)
							{
								SpriteEffects effects2 = SpriteEffects.None;
								if (npc[num61].direction > 0)
									effects2 = SpriteEffects.FlipHorizontally;

								float num63 = ((npc[num61].position.X + (float)(npc[num61].width / 2)) / 16f - num13) * num5;
								float num64 = ((npc[num61].position.Y + npc[num61].gfxOffY + (float)(npc[num61].height / 2)) / 16f - num14) * num5;
								num63 += num3;
								num64 += num4;
								num64 -= 2f * num5 / 5f;
								if (num63 > (float)(miniMapX + 12) && num63 < (float)(miniMapX + miniMapWidth - 16) && num64 > (float)(miniMapY + 10) && num64 < (float)(miniMapY + miniMapHeight - 14))
								{
									spriteBatch.Draw(npcHeadTexture[num62], new Vector2(num63 + num11, num64 + num12), new Microsoft.Xna.Framework.Rectangle(0, 0, npcHeadTexture[num62].Width, npcHeadTexture[num62].Height), new Microsoft.Xna.Framework.Color(b, b, b, b), 0f, new Vector2(npcHeadTexture[num62].Width / 2, npcHeadTexture[num62].Height / 2), num60, effects2, 0f);
									float num65 = num63 - (float)(npcHeadTexture[num62].Width / 2) * num60;
									float num66 = num64 - (float)(npcHeadTexture[num62].Height / 2) * num60;
									float num67 = num65 + (float)npcHeadTexture[num62].Width * num60;
									float num68 = num66 + (float)npcHeadTexture[num62].Height * num60;
									if ((float)mouseX >= num65 && (float)mouseX <= num67 && (float)mouseY >= num66 && (float)mouseY <= num68)
										text = npc[num61].FullName;
								}
							}
						}

						if (!npc[num61].active || npc[num61].GetBossHeadTextureIndex() == -1)
							continue;

						float bossHeadRotation2 = npc[num61].GetBossHeadRotation();
						SpriteEffects bossHeadSpriteEffects2 = npc[num61].GetBossHeadSpriteEffects();
						Vector2 vector2 = npc[num61].Center + new Vector2(0f, npc[num61].gfxOffY);
						if (npc[num61].type == 134)
						{
							Vector2 center2 = npc[num61].Center;
							int num69 = 1;
							int num70 = (int)npc[num61].ai[0];
							while (num69 < 15 && npc[num70].active && npc[num70].type >= 134 && npc[num70].type <= 136)
							{
								num69++;
								center2 += npc[num70].Center;
								num70 = (int)npc[num70].ai[0];
							}

							center2 /= (float)num69;
							vector2 = center2;
						}

						int bossHeadTextureIndex2 = npc[num61].GetBossHeadTextureIndex();
						float num71 = (vector2.X / 16f - num13) * num5;
						float num72 = (vector2.Y / 16f - num14) * num5;
						num71 += num3;
						num72 += num4;
						num72 -= 2f * num5 / 5f;
						if (num71 > (float)(miniMapX + 12) && num71 < (float)(miniMapX + miniMapWidth - 16) && num72 > (float)(miniMapY + 10) && num72 < (float)(miniMapY + miniMapHeight - 14))
						{
							spriteBatch.Draw(npcHeadBossTexture[bossHeadTextureIndex2], new Vector2(num71 + num11, num72 + num12), null, new Microsoft.Xna.Framework.Color(b, b, b, b), bossHeadRotation2, npcHeadBossTexture[bossHeadTextureIndex2].Size() / 2f, num60, bossHeadSpriteEffects2, 0f);
							float num73 = num71 - (float)(npcHeadBossTexture[bossHeadTextureIndex2].Width / 2) * num60;
							float num74 = num72 - (float)(npcHeadBossTexture[bossHeadTextureIndex2].Height / 2) * num60;
							float num75 = num73 + (float)npcHeadBossTexture[bossHeadTextureIndex2].Width * num60;
							float num76 = num74 + (float)npcHeadBossTexture[bossHeadTextureIndex2].Height * num60;
							if ((float)mouseX >= num73 && (float)mouseX <= num75 && (float)mouseY >= num74 && (float)mouseY <= num76)
								text = npc[num61].GivenOrTypeName;
						}
					}

					spriteBatch.End();
					spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, UIScaleMatrix);
					for (int num77 = 0; num77 < 255; num77++)
					{
						if (!player[num77].active || ((player[myPlayer].hostile || player[num77].hostile) && (player[myPlayer].team != player[num77].team || player[num77].team == 0) && num77 != myPlayer))
							continue;

						float num78 = ((player[num77].position.X + (float)(player[num77].width / 2)) / 16f - num13) * num5;
						float num79 = ((player[num77].position.Y + player[num77].gfxOffY + (float)(player[num77].height / 2)) / 16f - num14) * num5;
						num78 += num3;
						num79 += num4;
						num78 -= 6f;
						num79 -= 6f;
						num79 -= 2f - num5 / 5f * 2f;
						num78 += num11;
						num79 += num12;
						if (screenPosition.X != leftWorld + 640f + 16f && screenPosition.X + (float)screenWidth != rightWorld - 640f - 32f && screenPosition.Y != topWorld + 640f + 16f && !(screenPosition.Y + (float)screenHeight > bottomWorld - 640f - 32f) && num77 == myPlayer && zoomX == 0f && zoomY == 0f)
						{
							num78 = num3 + (float)(miniMapWidth / 2);
							num79 = num4 + (float)(miniMapHeight / 2);
							num79 -= 3f;
							num78 -= 4f;
						}

						if (!player[num77].dead && num78 > (float)(miniMapX + 6) && num78 < (float)(miniMapX + miniMapWidth - 16) && num79 > (float)(miniMapY + 6) && num79 < (float)(miniMapY + miniMapHeight - 14))
						{
							DrawPlayerHead(player[num77], num78, num79, (float)(int)b / 255f, num60);
							if (num77 != myPlayer)
							{
								float num80 = num78 + 4f - 14f * num60;
								float num81 = num79 + 2f - 14f * num60;
								float num82 = num80 + 28f * num60;
								float num83 = num81 + 28f * num60;
								if ((float)mouseX >= num80 && (float)mouseX <= num82 && (float)mouseY >= num81 && (float)mouseY <= num83)
									text = player[num77].name;
							}
						}

						if (!player[num77].showLastDeath)
							continue;

						num78 = (player[num77].lastDeathPostion.X / 16f - num13) * num5;
						num79 = (player[num77].lastDeathPostion.Y / 16f - num14) * num5;
						num78 += num3;
						num79 += num4;
						num79 -= 2f - num5 / 5f * 2f;
						num78 += num11;
						num79 += num12;
						if (num78 > (float)(miniMapX + 8) && num78 < (float)(miniMapX + miniMapWidth - 18) && num79 > (float)(miniMapY + 8) && num79 < (float)(miniMapY + miniMapHeight - 16))
						{
							spriteBatch.Draw(mapDeathTexture, new Vector2(num78, num79), new Microsoft.Xna.Framework.Rectangle(0, 0, mapDeathTexture.Width, mapDeathTexture.Height), Microsoft.Xna.Framework.Color.White, 0f, new Vector2((float)mapDeathTexture.Width * 0.5f, (float)mapDeathTexture.Height * 0.5f), num60, SpriteEffects.None, 0f);
							float num84 = num78 + 4f - 14f * num60;
							float num85 = num79 + 2f - 14f * num60;
							num84 -= 4f;
							num85 -= 4f;
							float num86 = num84 + 28f * num60;
							float num87 = num85 + 28f * num60;
							if ((float)mouseX >= num84 && (float)mouseX <= num86 && (float)mouseY >= num85 && (float)mouseY <= num87)
							{
								TimeSpan timeSpan = DateTime.Now - player[num77].lastDeathTime;
								text = Language.GetTextValue("Game.PlayerDeathTime", player[num77].name, Lang.LocalizedDuration(timeSpan, abbreviated: false, showAllAvailableUnits: false));
							}
						}
					}

					spriteBatch.End();
					spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, UIScaleMatrix);
					spriteBatch.Draw(miniMapFrameTexture, new Vector2(num58, num59), new Microsoft.Xna.Framework.Rectangle(0, 0, miniMapFrameTexture.Width, miniMapFrameTexture.Height), Microsoft.Xna.Framework.Color.White, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
					for (int num88 = 0; num88 < 3; num88++)
					{
						float num89 = num58 + 148f + (float)(num88 * 26);
						float num90 = num59 + 234f;
						if (!((float)mouseX > num89) || !((float)mouseX < num89 + 22f) || !((float)mouseY > num90) || !((float)mouseY < num90 + 22f))
							continue;

						spriteBatch.Draw(miniMapButtonTexture[num88], new Vector2(num89, num90), new Microsoft.Xna.Framework.Rectangle(0, 0, miniMapButtonTexture[num88].Width, miniMapButtonTexture[num88].Height), Microsoft.Xna.Framework.Color.White, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
						if (PlayerInput.IgnoreMouseInterface)
							continue;

						player[myPlayer].mouseInterface = true;
						if (mouseLeft)
						{
							if (mouseLeftRelease)
								PlaySound(12);

							switch (num88)
							{
								case 0:
									mapMinimapScale = 1.25f;
									break;
								case 1:
									mapMinimapScale *= 0.975f;
									break;
								case 2:
									mapMinimapScale *= 1.025f;
									break;
							}
						}
					}
				}
			}

			if (mapFullscreen)
			{
				int num91 = (int)((0f - num + (float)mouseX) / num5 + num7);
				int num92 = (int)((0f - num2 + (float)mouseY) / num5 + num8);
				bool flag2 = false;
				if ((float)num91 < num7)
					flag2 = true;

				if ((float)num91 >= num9)
					flag2 = true;

				if ((float)num92 < num8)
					flag2 = true;

				if ((float)num92 >= num10)
					flag2 = true;

				if (!flag2 && Map[num91, num92].Light > 40)
				{
					int type = Map[num91, num92].Type;
					int num93 = MapHelper.tileLookup[21];
					int num94 = MapHelper.tileLookup[441];
					int num95 = MapHelper.tileOptionCounts[21];
					int num96 = MapHelper.tileLookup[467];
					int num97 = MapHelper.tileLookup[468];
					int num98 = MapHelper.tileOptionCounts[467];
					int num99 = MapHelper.tileLookup[88];
					int num100 = MapHelper.tileOptionCounts[88];
					LocalizedText[] chestType = Lang.chestType;
					LocalizedText[] chestType2 = Lang.chestType2;
					if (type >= num93 && type < num93 + num95)
					{
						Tile tile = Main.tile[num91, num92];
						if (tile != null)
						{
							int num101 = num91;
							int num102 = num92;
							if (tile.frameX % 36 != 0)
								num101--;

							if (tile.frameY % 36 != 0)
								num102--;

							text = DrawMap_FindChestName(chestType, tile, num101, num102);
						}
					}
					else if (type >= num96 && type < num96 + num98)
					{
						Tile tile2 = Main.tile[num91, num92];
						if (tile2 != null)
						{
							int num103 = num91;
							int num104 = num92;
							if (tile2.frameX % 36 != 0)
								num103--;

							if (tile2.frameY % 36 != 0)
								num104--;

							text = DrawMap_FindChestName(chestType2, tile2, num103, num104);
						}
					}
					else if (type >= num94 && type < num94 + num95)
					{
						Tile tile3 = Main.tile[num91, num92];
						if (tile3 != null)
						{
							int num105 = num91;
							int num106 = num92;
							if (tile3.frameX % 36 != 0)
								num105--;

							if (tile3.frameY % 36 != 0)
								num106--;

							text = chestType[tile3.frameX / 36].Value;
						}
					}
					else if (type >= num97 && type < num97 + num98)
					{
						Tile tile4 = Main.tile[num91, num92];
						if (tile4 != null)
						{
							int num107 = num91;
							int num108 = num92;
							if (tile4.frameX % 36 != 0)
								num107--;

							if (tile4.frameY % 36 != 0)
								num108--;

							text = chestType2[tile4.frameX / 36].Value;
						}
					}
					else if (type >= num99 && type < num99 + num100)
					{
						//patch file: num91, num92
						Tile tile5 = Main.tile[num91, num92];
						if (tile5 != null)
						{
							int num109 = num92;
							int x2 = num91 - tile5.frameX % 54 / 18;
							if (tile5.frameY % 36 != 0)
								num109--;

							int num110 = Chest.FindChest(x2, num109);
							text = ((num110 < 0) ? Lang.dresserType[0].Value : ((!(chest[num110].name != "")) ? Lang.dresserType[tile5.frameX / 54].Value : (Lang.dresserType[tile5.frameX / 54].Value + ": " + chest[num110].name)));
						}
					}
					else
					{
						text = Lang.GetMapObjectName(type);
						text = Lang._mapLegendCache.FromTile(Map[num91, num92], num91, num92);
					}
				}

				float num111 = (num5 * 0.25f * 2f + 1f) / 3f;
				if (num111 > 1f)
					num111 = 1f;

				num111 = 1f;
				num111 = UIScale;
				for (int num112 = 0; num112 < 200; num112++)
				{
					if (npc[num112].active && npc[num112].townNPC)
					{
						int num113 = NPC.TypeToHeadIndex(npc[num112].type);
						if (num113 > 0)
						{
							SpriteEffects effects3 = SpriteEffects.None;
							if (npc[num112].direction > 0)
								effects3 = SpriteEffects.FlipHorizontally;

							float num114 = (npc[num112].position.X + (float)(npc[num112].width / 2)) / 16f * num5;
							float num115 = (npc[num112].position.Y + npc[num112].gfxOffY + (float)(npc[num112].height / 2)) / 16f * num5;
							num114 += num;
							num115 += num2;
							num114 -= 10f * num5;
							num115 -= 10f * num5;
							spriteBatch.Draw(npcHeadTexture[num113], new Vector2(num114, num115), new Microsoft.Xna.Framework.Rectangle(0, 0, npcHeadTexture[num113].Width, npcHeadTexture[num113].Height), new Microsoft.Xna.Framework.Color(b, b, b, b), 0f, new Vector2(npcHeadTexture[num113].Width / 2, npcHeadTexture[num113].Height / 2), num111, effects3, 0f);
							float num116 = num114 - (float)(npcHeadTexture[num113].Width / 2) * num111;
							float num117 = num115 - (float)(npcHeadTexture[num113].Height / 2) * num111;
							float num118 = num116 + (float)npcHeadTexture[num113].Width * num111;
							float num119 = num117 + (float)npcHeadTexture[num113].Height * num111;
							if ((float)mouseX >= num116 && (float)mouseX <= num118 && (float)mouseY >= num117 && (float)mouseY <= num119)
								text = npc[num112].FullName;
						}
					}

					if (!npc[num112].active || npc[num112].GetBossHeadTextureIndex() == -1)
						continue;

					float bossHeadRotation3 = npc[num112].GetBossHeadRotation();
					SpriteEffects bossHeadSpriteEffects3 = npc[num112].GetBossHeadSpriteEffects();
					Vector2 vector3 = npc[num112].Center + new Vector2(0f, npc[num112].gfxOffY);
					if (npc[num112].type == 134)
					{
						Vector2 center3 = npc[num112].Center;
						int num120 = 1;
						int num121 = (int)npc[num112].ai[0];
						while (num120 < 15 && npc[num121].active && npc[num121].type >= 134 && npc[num121].type <= 136)
						{
							num120++;
							center3 += npc[num121].Center;
							num121 = (int)npc[num121].ai[0];
						}

						center3 /= (float)num120;
						vector3 = center3;
					}

					int bossHeadTextureIndex3 = npc[num112].GetBossHeadTextureIndex();
					float num122 = vector3.X / 16f * num5;
					float num123 = vector3.Y / 16f * num5;
					num122 += num;
					num123 += num2;
					num122 -= 10f * num5;
					num123 -= 10f * num5;
					spriteBatch.Draw(npcHeadBossTexture[bossHeadTextureIndex3], new Vector2(num122, num123), null, new Microsoft.Xna.Framework.Color(b, b, b, b), bossHeadRotation3, npcHeadBossTexture[bossHeadTextureIndex3].Size() / 2f, num111, bossHeadSpriteEffects3, 0f);
					float num124 = num122 - (float)(npcHeadBossTexture[bossHeadTextureIndex3].Width / 2) * num111;
					float num125 = num123 - (float)(npcHeadBossTexture[bossHeadTextureIndex3].Height / 2) * num111;
					float num126 = num124 + (float)npcHeadBossTexture[bossHeadTextureIndex3].Width * num111;
					float num127 = num125 + (float)npcHeadBossTexture[bossHeadTextureIndex3].Height * num111;
					if ((float)mouseX >= num124 && (float)mouseX <= num126 && (float)mouseY >= num125 && (float)mouseY <= num127)
						text = npc[num112].GivenOrTypeName;
				}

				bool flag3 = false;
				spriteBatch.End();
				spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
				for (int num128 = 0; num128 < 255; num128++)
				{
					if (player[num128].active && ((!player[myPlayer].hostile && !player[num128].hostile) || (player[myPlayer].team == player[num128].team && player[num128].team != 0) || num128 == myPlayer) && player[num128].showLastDeath)
					{
						float num129 = (player[num128].lastDeathPostion.X / 16f - num13) * num5;
						float num130 = (player[num128].lastDeathPostion.Y / 16f - num14) * num5;
						num129 += num;
						num130 += num2;
						num130 -= 2f - num5 / 5f * 2f;
						num129 -= 10f * num5;
						num130 -= 10f * num5;
						spriteBatch.Draw(mapDeathTexture, new Vector2(num129, num130), new Microsoft.Xna.Framework.Rectangle(0, 0, mapDeathTexture.Width, mapDeathTexture.Height), Microsoft.Xna.Framework.Color.White, 0f, new Vector2((float)mapDeathTexture.Width * 0.5f, (float)mapDeathTexture.Height * 0.5f), num111, SpriteEffects.None, 0f);
						float num131 = num129 + 4f - 14f * num111;
						float num132 = num130 + 2f - 14f * num111;
						float num133 = num131 + 28f * num111;
						float num134 = num132 + 28f * num111;
						if ((float)mouseX >= num131 && (float)mouseX <= num133 && (float)mouseY >= num132 && (float)mouseY <= num134)
						{
							TimeSpan timeSpan2 = DateTime.Now - player[num128].lastDeathTime;
							text = Language.GetTextValue("Game.PlayerDeathTime", player[num128].name, Lang.LocalizedDuration(timeSpan2, abbreviated: false, showAllAvailableUnits: false));
						}
					}
				}

				for (int num135 = 0; num135 < 255; num135++)
				{
					if (!player[num135].active || ((player[myPlayer].hostile || player[num135].hostile) && (player[myPlayer].team != player[num135].team || player[num135].team == 0) && num135 != myPlayer))
						continue;

					float num136 = ((player[num135].position.X + (float)(player[num135].width / 2)) / 16f - num13) * num5;
					float num137 = ((player[num135].position.Y + player[num135].gfxOffY + (float)(player[num135].height / 2)) / 16f - num14) * num5;
					num136 += num;
					num137 += num2;
					num136 -= 6f;
					num137 -= 2f;
					num137 -= 2f - num5 / 5f * 2f;
					num136 -= 10f * num5;
					num137 -= 10f * num5;
					float num138 = num136 + 4f - 14f * num111;
					float num139 = num137 + 2f - 14f * num111;
					float num140 = num138 + 28f * num111;
					float num141 = num139 + 28f * num111;
					if (player[num135].dead)
						continue;

					DrawPlayerHead(player[num135], num136, num137, (float)(int)b / 255f, num111);
					if (!((float)mouseX >= num138) || !((float)mouseX <= num140) || !((float)mouseY >= num139) || !((float)mouseY <= num141))
						continue;

					text = player[num135].name;
					if (num135 != myPlayer && player[myPlayer].team > 0 && player[myPlayer].team == player[num135].team && netMode == 1 && player[myPlayer].HasUnityPotion())
					{
						flag3 = true;
						if (!unityMouseOver)
							PlaySound(12);

						unityMouseOver = true;
						DrawPlayerHead(player[num135], num136, num137, 2f, num111 + 0.5f);
						text = Language.GetTextValue("Game.TeleportTo", player[num135].name);
						if (mouseLeft && mouseLeftRelease)
						{
							mouseLeftRelease = false;
							mapFullscreen = false;
							player[myPlayer].UnityTeleport(player[num135].position);
							player[myPlayer].TakeUnityPotion();
						}
					}
				}

				if (!flag3 && unityMouseOver)
					unityMouseOver = false;

				spriteBatch.End();
				spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, UIScaleMatrix);
				PlayerInput.SetZoom_UI();
				int num142 = 10;
				int num143 = screenHeight - 40;
				if (showFrameRate)
					num143 -= 15;

				int num144 = 0;
				int num145 = 130;
				if (mouseX >= num142 && mouseX <= num142 + 32 && mouseY >= num143 && mouseY <= num143 + 30)
				{
					num145 = 255;
					num144 += 4;
					player[myPlayer].mouseInterface = true;
					if (mouseLeft && mouseLeftRelease)
					{
						PlaySound(10);
						mapFullscreen = false;
					}
				}

				spriteBatch.Draw(mapIconTexture[num144], new Vector2(num142, num143), new Microsoft.Xna.Framework.Rectangle(0, 0, mapIconTexture[num144].Width, mapIconTexture[num144].Height), new Microsoft.Xna.Framework.Color(num145, num145, num145, num145), 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
				ModHooks.PostDrawFullscreenMap(ref text);
				DrawCursor(DrawThickCursor());
			}

			if (text != "")
				MouseText(text, 0, 0);

			spriteBatch.End();
			spriteBatch.Begin();
			PlayerInput.SetZoom_Unscaled();
			TimeLogger.DetailedDrawTime(9);
		}
		*/
	}
}
