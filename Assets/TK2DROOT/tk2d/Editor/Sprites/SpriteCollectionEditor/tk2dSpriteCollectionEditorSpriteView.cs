using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace tk2dEditor.SpriteCollectionEditor
{
	public class SpriteView
	{
		public SpriteCollectionProxy SpriteCollection { get { return host.SpriteCollection; } }
		TextureEditor textureEditor;
		
		int[] extraPadAmountValues;
		string[] extraPadAmountLabels;
		
		IEditorHost host;
		public SpriteView(IEditorHost host)
		{
			this.host = host;
			
			int MAX_PAD_AMOUNT = 17;
			extraPadAmountValues = new int[MAX_PAD_AMOUNT];
			extraPadAmountLabels = new string[MAX_PAD_AMOUNT];
			for (int i = 0; i < MAX_PAD_AMOUNT; ++i)
			{
				extraPadAmountValues[i] = i;
				extraPadAmountLabels[i] = (i==0)?"None":(i.ToString());
			}
			
			textureEditor = new TextureEditor();
		}
		
		void DrawSpriteEditorMultiView(List<SpriteCollectionEditorEntry> entries)
		{
			var param = SpriteCollection.textureParams[entries[0].index];
			EditorGUILayout.BeginHorizontal();
			
			// texture
			textureEditor.DrawEmptyTextureView();
			
			EditorGUILayout.BeginVertical(tk2dEditorSkin.SC_InspectorBG, GUILayout.MaxWidth(260), GUILayout.ExpandHeight(true));
			if (SpriteCollection.premultipliedAlpha)
			{
				param.additive = EditorGUILayout.Toggle("Additive", param.additive);
			}
			EditorGUILayout.EndVertical();
		}
		
		delegate bool SpriteCollectionEntryComparerDelegate(tk2dSpriteCollectionDefinition a, tk2dSpriteCollectionDefinition b);
		delegate void SpriteCollectionEntryAssignerDelegate(tk2dSpriteCollectionDefinition a, tk2dSpriteCollectionDefinition b);
		void HandleMultiSelection(List<SpriteCollectionEditorEntry> entries, SpriteCollectionEntryComparerDelegate comparer, SpriteCollectionEntryAssignerDelegate assigner)
		{
			if (entries.Count <= 1) return;
			var activeSelection = SpriteCollection.textureParams[entries[entries.Count - 1].index];
			
			bool needButton = false;
			foreach (var entry in entries)
			{
				var sel = SpriteCollection.textureParams[entry.index];
				if (sel != activeSelection && !comparer(activeSelection, sel))
				{
					needButton = true;
					break;
				}
			}
			if (needButton) 
			{ 
				GUILayout.BeginHorizontal(); GUILayout.FlexibleSpace();
				if (GUILayout.Button("Apply", EditorStyles.miniButton))
				{
					foreach (var entry in entries)
					{
						var sel = SpriteCollection.textureParams[entry.index];
						if (sel != activeSelection) assigner(activeSelection, sel);
					}
				}
				GUILayout.EndHorizontal();
			}
		}
		
		// Only call this when both a AND b have poly colliders and all other comparisons 
		// are successful prior to calling this, its a waste of time otherwise
		bool ComparePolyCollider(tk2dSpriteCollectionDefinition a, tk2dSpriteCollectionDefinition b)
		{
			if (a.polyColliderIslands.Length != b.polyColliderIslands.Length)
				return false;
			for (int i = 0; i < a.polyColliderIslands.Length; ++i)
				if (!a.polyColliderIslands[i].CompareTo(b.polyColliderIslands[i])) return false;
			return true;
		}
		
		void CopyPolyCollider(tk2dSpriteCollectionDefinition src, tk2dSpriteCollectionDefinition dest)
		{
			dest.polyColliderIslands = new tk2dSpriteColliderIsland[src.polyColliderIslands.Length];
			for (int i = 0; i < dest.polyColliderIslands.Length; ++i)
			{
				dest.polyColliderIslands[i] = new tk2dSpriteColliderIsland();
				dest.polyColliderIslands[i].CopyFrom(src.polyColliderIslands[i]);
			}
		}
	
		public void DrawSpriteEditorInspector(List<SpriteCollectionEditorEntry> entries, bool allowDelete, bool editingSpriteSheet)
		{
			var entry = entries[entries.Count - 1];
			var param = SpriteCollection.textureParams[entry.index];
			var spriteTexture = param.extractRegion?host.GetTextureForSprite(entry.index):SpriteCollection.textureRefs[entry.index];

			// Inspector
			EditorGUILayout.BeginVertical();

			// Header
			EditorGUILayout.BeginVertical(tk2dEditorSkin.SC_InspectorHeaderBG, GUILayout.MaxWidth(host.InspectorWidth), GUILayout.ExpandHeight(true));
			if (entries.Count > 1)
				EditorGUILayout.TextField("Name", param.name);
			else
			{
				string name = EditorGUILayout.TextField("Name", param.name);
				if (name != param.name)
				{
					param.name = name;
					entries[entries.Count - 1].name = name;
					host.OnSpriteCollectionSortChanged();
				}
			}
			GUILayout.BeginHorizontal();
			bool doDelete = false;
			bool doSelect = false;
			bool doSelectSpriteSheet = false;
			if (entries.Count == 1)
			{
				if (param.extractRegion)
					EditorGUILayout.ObjectField("Texture", spriteTexture, typeof(Texture2D), false);
				else
					SpriteCollection.textureRefs[entry.index] = EditorGUILayout.ObjectField("Texture", spriteTexture, typeof(Texture2D), false) as Texture2D;
				GUILayout.FlexibleSpace();
				GUILayout.BeginVertical();
				if (editingSpriteSheet && GUILayout.Button("Edit...", EditorStyles.miniButton)) doSelect = true;
				if (!editingSpriteSheet && param.hasSpriteSheetId && GUILayout.Button("Source", EditorStyles.miniButton)) doSelectSpriteSheet = true;
				if (allowDelete && GUILayout.Button("Delete", EditorStyles.miniButton)) doDelete = true;
				GUILayout.EndVertical();
			}
			else
			{
				string countLabel = (entries.Count > 1)?entries.Count.ToString() + " sprites selected":"";
				GUILayout.Label(countLabel);
				GUILayout.FlexibleSpace();
				if (editingSpriteSheet && GUILayout.Button("Edit...", EditorStyles.miniButton)) doSelect = true;
				if (!editingSpriteSheet && param.hasSpriteSheetId)
				{
					int id = param.spriteSheetId;
					foreach (var v in entries)
					{
						var p = SpriteCollection.textureParams[v.index];
						if (!p.hasSpriteSheetId ||
							p.spriteSheetId != id) 
						{ 
							id = -1; 
							break; 
						}
					}
					if (id != -1 && GUILayout.Button("Source", EditorStyles.miniButton)) doSelectSpriteSheet = true;
				}
				if (allowDelete && GUILayout.Button("Delete", EditorStyles.miniButton)) doDelete = true;
			}
			GUILayout.EndHorizontal();
			EditorGUILayout.EndVertical();
			
			// Body
			EditorGUILayout.BeginVertical(tk2dEditorSkin.SC_InspectorBG, GUILayout.MaxWidth(host.InspectorWidth), GUILayout.ExpandHeight(true));
			
			if (SpriteCollection.premultipliedAlpha)
			{
				param.additive = EditorGUILayout.Toggle("Additive", param.additive);
				HandleMultiSelection(entries, (a,b) => a.additive == b.additive, (a,b) => b.additive = a.additive);
			}
			// fixup
			if (param.scale == Vector3.zero)
				param.scale = Vector3.one;
			param.scale = EditorGUILayout.Vector3Field("Scale", param.scale);
			HandleMultiSelection(entries, (a,b) => a.scale == b.scale, (a,b) => b.scale = a.scale);
			
			// Anchor
			param.anchor = (tk2dSpriteCollectionDefinition.Anchor)EditorGUILayout.EnumPopup("Anchor", param.anchor);
			if (param.anchor == tk2dSpriteCollectionDefinition.Anchor.Custom)
			{
				EditorGUI.indentLevel++;
				EditorGUILayout.BeginHorizontal();
				param.anchorX = EditorGUILayout.FloatField("X", param.anchorX);
				bool roundAnchorX = GUILayout.Button("R", EditorStyles.miniButton, GUILayout.MaxWidth(24));
				EditorGUILayout.EndHorizontal();
	
				EditorGUILayout.BeginHorizontal();
				param.anchorY = EditorGUILayout.FloatField("Y", param.anchorY);
				bool roundAnchorY = GUILayout.Button("R", EditorStyles.miniButton, GUILayout.MaxWidth(24));
				EditorGUILayout.EndHorizontal();
				
				if (roundAnchorX) param.anchorX = Mathf.Round(param.anchorX);
				if (roundAnchorY) param.anchorY = Mathf.Round(param.anchorY);
				EditorGUI.indentLevel--;
				EditorGUILayout.Separator();
				
				HandleMultiSelection(entries, 
					(a,b) => (a.anchor == b.anchor && a.anchorX == b.anchorX && a.anchorY == b.anchorY),
					delegate(tk2dSpriteCollectionDefinition a, tk2dSpriteCollectionDefinition b) {
						b.anchor = a.anchor;
						b.anchorX = a.anchorX;
						b.anchorY = a.anchorY;
					});				
			}
			else
			{
				HandleMultiSelection(entries, (a,b) => a.anchor == b.anchor, (a,b) => b.anchor = a.anchor);
			}
	
			param.colliderType = (tk2dSpriteCollectionDefinition.ColliderType)EditorGUILayout.EnumPopup("Collider Type", param.colliderType);
			if (param.colliderType == tk2dSpriteCollectionDefinition.ColliderType.BoxCustom)
			{
				EditorGUI.indentLevel++;
				param.boxColliderMin = EditorGUILayout.Vector2Field("Min", param.boxColliderMin);
				param.boxColliderMax = EditorGUILayout.Vector2Field("Max", param.boxColliderMax);
				EditorGUI.indentLevel--;
				EditorGUILayout.Separator();

				HandleMultiSelection(entries, 
					(a,b) => (a.colliderType == b.colliderType && a.boxColliderMin == b.boxColliderMin && a.boxColliderMax == b.boxColliderMax),
					delegate(tk2dSpriteCollectionDefinition a, tk2dSpriteCollectionDefinition b) {
						b.colliderType = a.colliderType;
						b.boxColliderMin = a.boxColliderMin;
						b.boxColliderMax = a.boxColliderMax;
					});				
			}
			else if (param.colliderType == tk2dSpriteCollectionDefinition.ColliderType.Polygon)
			{
				EditorGUI.indentLevel++;
				param.polyColliderCap = (tk2dSpriteCollectionDefinition.PolygonColliderCap)EditorGUILayout.EnumPopup("Collider Cap", param.polyColliderCap);
				param.colliderConvex = EditorGUILayout.Toggle("Convex", param.colliderConvex);
				param.colliderSmoothSphereCollisions = EditorGUILayout.Toggle(new GUIContent("SmoothSphereCollisions", "Smooth Sphere Collisions"), param.colliderSmoothSphereCollisions);
				EditorGUI.indentLevel--;
				EditorGUILayout.Separator();

				HandleMultiSelection(entries, 
					(a,b) => (a.colliderType == b.colliderType && a.polyColliderCap == b.polyColliderCap 
							&& a.colliderConvex == b.colliderConvex && a.colliderSmoothSphereCollisions == b.colliderSmoothSphereCollisions
							&& ComparePolyCollider(a, b)),
					delegate(tk2dSpriteCollectionDefinition a, tk2dSpriteCollectionDefinition b) {
						b.colliderType = a.colliderType;
						b.polyColliderCap = a.polyColliderCap;
						b.colliderConvex = a.colliderConvex;
						b.colliderSmoothSphereCollisions = a.colliderSmoothSphereCollisions;
						CopyPolyCollider(a, b);
					});				
			}
			else
			{
				HandleMultiSelection(entries, (a,b) => a.colliderType == b.colliderType, (a,b) => b.colliderType = a.colliderType);				
			}
			
			
			// Dicing
			if (!SpriteCollection.allowMultipleAtlases)
			{
				param.dice = EditorGUILayout.Toggle("Dice", param.dice);
				if (param.dice)
				{
					EditorGUI.indentLevel++;
					param.diceUnitX = EditorGUILayout.IntField("X", param.diceUnitX);
					param.diceUnitY = EditorGUILayout.IntField("Y", param.diceUnitY);
					EditorGUI.indentLevel--;
					EditorGUILayout.Separator();

					HandleMultiSelection(entries, 
						(a,b) => a.dice == b.dice && a.diceUnitX == b.diceUnitX && a.diceUnitY == b.diceUnitY, 
						delegate(tk2dSpriteCollectionDefinition a, tk2dSpriteCollectionDefinition b) {
							b.dice = a.dice;
							b.diceUnitX = a.diceUnitX;
							b.diceUnitY = a.diceUnitY;
					});
				}
				else
				{
					HandleMultiSelection(entries, (a,b) => a.dice == b.dice, (a,b) => b.dice = a.dice);
				}
			}
			
			// Pad amount
			param.pad = (tk2dSpriteCollectionDefinition.Pad)EditorGUILayout.EnumPopup("Pad method", param.pad);
			HandleMultiSelection(entries, (a,b) => a.pad == b.pad, (a,b) => b.pad = a.pad);
			
			// Extra padding
			param.extraPadding = EditorGUILayout.IntPopup("Extra Padding", param.extraPadding, extraPadAmountLabels, extraPadAmountValues);
			HandleMultiSelection(entries, (a,b) => a.extraPadding == b.extraPadding, (a,b) => b.extraPadding = a.extraPadding);
			GUILayout.FlexibleSpace();
			
			// Draw additional inspector
			textureEditor.DrawTextureInspector(param, spriteTexture);
			
			EditorGUILayout.EndVertical();
			EditorGUILayout.EndVertical(); // inspector
			
			// Defer delete to avoid messing about anything else
			if (doDelete)
			{
				foreach (var e in entries)
				{
					SpriteCollection.textureRefs[e.index] = null;
					SpriteCollection.textureParams[e.index] = new tk2dSpriteCollectionDefinition();
				}
				SpriteCollection.Trim();
				if (editingSpriteSheet)
					host.OnSpriteCollectionChanged(true);
				else
					host.OnSpriteCollectionChanged(false);
			}
			
			if (doSelect)
			{
				List<int> spriteIdList = new List<int>();
				foreach (var e in entries)
					spriteIdList.Add(e.index);
				host.SelectSpritesFromList(spriteIdList.ToArray());
			}
			
			if (doSelectSpriteSheet)
			{
				List<int> spriteIdList = new List<int>();
				foreach (var e in entries)
					spriteIdList.Add(e.index);
				host.SelectSpritesInSpriteSheet(param.spriteSheetId, spriteIdList.ToArray());
			}
		}
		
		public void DrawSpriteSheetView(List<SpriteCollectionEditorEntry> entries)
		{
			if (entries.Count > 1)
			{
				GUILayout.Label("Multi editing sprite sheets not supported");
				return;
			}
			
			//spriteSheetSelection = DrawSpriteSheetView(spriteSheetSelection);
		}

		void DrawSpriteEditorView(List<SpriteCollectionEditorEntry> entries)
		{
			if (entries.Count == 0)
				return;
			var entry = entries[entries.Count - 1];
			var param = SpriteCollection.textureParams[entry.index];
			var spriteTexture = param.extractRegion?host.GetTextureForSprite(entry.index):SpriteCollection.textureRefs[entry.index];
			EditorGUILayout.BeginHorizontal();
	
			// Cache texture or draw it
			textureEditor.DrawTextureView(param, spriteTexture);
			DrawSpriteEditorInspector(entries, true, false);
		
			EditorGUILayout.EndHorizontal();
		}
		
		public void Draw(List<SpriteCollectionEditorEntry> entries)
		{
			EditorGUIUtility.LookLikeControls(100.0f, 100.0f);
			
			GUILayout.BeginVertical(GUILayout.ExpandWidth(false));
			if (entries == null || entries.Count == 0)
			{
				GUILayout.Label("");
			}
			else
			{
				var entryType = entries[0].type;
				switch (entryType)
				{
				case SpriteCollectionEditorEntry.Type.Sprite: DrawSpriteEditorView(entries); break;
				case SpriteCollectionEditorEntry.Type.SpriteSheet: DrawSpriteSheetView(entries); break;
				}
			}
			GUILayout.EndVertical();
		}
	}
}