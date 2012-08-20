using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace tk2dEditor.SpriteCollectionEditor
{
	public class SettingsView	
	{
		public bool show = false;
		Vector2 settingsScrollbar = Vector2.zero;
		int[] padAmountValues = null;
		string[] padAmountLabels = null;
		bool displayAtlasFoldout = true;
		
		IEditorHost host;
		public SettingsView(IEditorHost host)
		{
			this.host = host;
		}
		
		SpriteCollectionProxy SpriteCollection { get { return host.SpriteCollection; } }
		
		public void Draw()
		{
			if (SpriteCollection == null)
				return;
			
			// initialize internal stuff
			if (padAmountValues == null || padAmountValues.Length == 0)
			{
				int MAX_PAD_AMOUNT = 18;
				padAmountValues = new int[MAX_PAD_AMOUNT];
				padAmountLabels = new string[MAX_PAD_AMOUNT];
				for (int i = 0; i < MAX_PAD_AMOUNT; ++i)
				{
					padAmountValues[i] = -1 + i;
					padAmountLabels[i] = (i==0)?"Default":((i-1).ToString());
				}
			}
	
			GUILayout.BeginHorizontal();
			
			GUILayout.BeginVertical(tk2dEditorSkin.SC_BodyBackground, GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
			GUILayout.EndVertical();
			
			
			int inspectorWidth = host.InspectorWidth;
			EditorGUIUtility.LookLikeControls(130.0f, 100.0f);
			
			settingsScrollbar = GUILayout.BeginScrollView(settingsScrollbar, GUILayout.ExpandHeight(true), GUILayout.Width(inspectorWidth));
	
			GUILayout.BeginVertical(tk2dEditorSkin.SC_InspectorHeaderBG, GUILayout.ExpandWidth(true));
			GUILayout.Label("Settings", EditorStyles.largeLabel);
			SpriteCollection.spriteCollection = EditorGUILayout.ObjectField("Data object", SpriteCollection.spriteCollection, typeof(tk2dSpriteCollectionData), false) as tk2dSpriteCollectionData;
			GUILayout.EndVertical();
			
			GUILayout.BeginVertical(tk2dEditorSkin.SC_InspectorBG, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
			SpriteCollection.premultipliedAlpha = EditorGUILayout.Toggle("Premultiplied Alpha", SpriteCollection.premultipliedAlpha);
			SpriteCollection.pixelPerfectPointSampled = EditorGUILayout.Toggle("Point Sampled", SpriteCollection.pixelPerfectPointSampled);
			SpriteCollection.physicsDepth = EditorGUILayout.FloatField("Collider depth", SpriteCollection.physicsDepth);
			SpriteCollection.disableTrimming = EditorGUILayout.Toggle("Disable Trimming", SpriteCollection.disableTrimming);
			SpriteCollection.normalGenerationMode = (tk2dSpriteCollection.NormalGenerationMode)EditorGUILayout.EnumPopup("Normal Generation", SpriteCollection.normalGenerationMode);
			SpriteCollection.padAmount = EditorGUILayout.IntPopup("Pad Amount", SpriteCollection.padAmount, padAmountLabels, padAmountValues);
	
			SpriteCollection.useTk2dCamera = EditorGUILayout.Toggle("Use tk2dCamera", SpriteCollection.useTk2dCamera);
			if (!SpriteCollection.useTk2dCamera)
			{
				EditorGUI.indentLevel = EditorGUI.indentLevel + 1;
				SpriteCollection.targetHeight = EditorGUILayout.IntField("Target Height", SpriteCollection.targetHeight);
				SpriteCollection.targetOrthoSize = EditorGUILayout.FloatField("Target Ortho Size", SpriteCollection.targetOrthoSize);
				EditorGUI.indentLevel = EditorGUI.indentLevel - 1;
			}
			
			displayAtlasFoldout = EditorGUILayout.Foldout(displayAtlasFoldout, "Atlas");
			if (displayAtlasFoldout)
			{
				EditorGUI.indentLevel++;
				
				int[] allowedAtlasSizes = { 128, 256, 512, 1024, 2048, 4096 };
				string[] allowedAtlasSizesString = new string[allowedAtlasSizes.Length];
				for (int i = 0; i < allowedAtlasSizes.Length; ++i)
					allowedAtlasSizesString[i] = allowedAtlasSizes[i].ToString();
				
				SpriteCollection.maxTextureSize = EditorGUILayout.IntPopup("Max Size", SpriteCollection.maxTextureSize, allowedAtlasSizesString, allowedAtlasSizes);
				SpriteCollection.allowMultipleAtlases = EditorGUILayout.Toggle("Multiple Atlases", SpriteCollection.allowMultipleAtlases);
				if (SpriteCollection.allowMultipleAtlases)
				{
					tk2dGuiUtility.InfoBox("Sprite collections with multiple atlas spanning enabled cannot be used with the Static Sprite" +
						" Batcher and the TileMap Editor.", tk2dGuiUtility.WarningLevel.Info);
				}
				SpriteCollection.textureCompression = (tk2dSpriteCollection.TextureCompression)EditorGUILayout.EnumPopup("Compression", SpriteCollection.textureCompression);
				SpriteCollection.forceSquareAtlas = EditorGUILayout.Toggle("Force Square", SpriteCollection.forceSquareAtlas);
				
				if (SpriteCollection.allowMultipleAtlases)
				{
					EditorGUILayout.LabelField("Num Atlases", SpriteCollection.atlasTextures.Length.ToString());
				}
				else
				{
					EditorGUILayout.LabelField("Atlas Width", SpriteCollection.atlasWidth.ToString());
					EditorGUILayout.LabelField("Atlas Height", SpriteCollection.atlasHeight.ToString());
					EditorGUILayout.LabelField("Atlas Wastage", SpriteCollection.atlasWastage.ToString("0.00") + "%");
				}
				
				EditorGUI.indentLevel--;
			}
			
			GUILayout.EndVertical();
			GUILayout.EndScrollView();
			
			GUILayout.EndHorizontal();
		}		
	}
}
