using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GenericManagerInspector : Editor {
	public string newName = "";
	public bool mConfirmDelete = false;
	public int mIndex = 0;
	public float defaultLabelWidth = 80f;
	public ItemManager itemManager { get { return ItemManager.itemManager; } }
	public QuestManager questManager { get { return QuestManager.questManager; } }
	string[] _allItems;
	public string[] allItems { 
		get {
			if(_allItems == null) {
				_allItems =  new string[itemManager.items.Count];
				foreach(BaseItem i in itemManager.items)
					allItems[itemManager.items.IndexOf(i)] = i.itemName;
			}
			return _allItems;
		}
	}

	string[] _allQuests;
	public string[] allQuests { 
		get {
			if(_allQuests == null) {
				_allQuests =  new string[questManager.quests.Count];
				foreach(BaseQuest i in questManager.quests)
					allQuests[questManager.quests.IndexOf(i)] = i.itemName;
			}
			return _allQuests;
		}
	}

	public void RebuildAllItems() { _allItems = null;}
	
	public Rect DrawSprite (Texture2D tex, Rect sprite, Material mat) { return DrawSprite(tex, sprite, mat, true, 0); }
	
	public Rect DrawSprite (Texture2D tex, Rect sprite, Material mat, bool addPadding) { return DrawSprite(tex, sprite, mat, addPadding, 0); }
	
	public Rect DrawSprite (Texture2D tex, Rect sprite, Material mat, bool addPadding, int maxSize) {
		float paddingX = addPadding ? 4f / tex.width : 0f;
		float paddingY = addPadding ? 4f / tex.height : 0f;
		float ratio = (sprite.height + paddingY) / (sprite.width + paddingX);
		ratio *= (float)tex.height / tex.width;
		Color c = GUI.color;
		Rect rect = NGUIEditorTools.DrawBackground(tex, ratio);
		GUI.color = c;
		
		if (maxSize > 0) {
			float dim = maxSize / Mathf.Max(rect.width, rect.height);
			rect.width *= dim;
			rect.height *= dim;
		}
		
		// We only want to draw into this rectangle
		if (Event.current.type == EventType.Repaint) {
			if (mat == null)
				GUI.DrawTextureWithTexCoords(rect, tex, sprite);
			else
				UnityEditor.EditorGUI.DrawPreviewTexture(sprite, tex, mat);
			rect = new Rect(sprite.x + rect.x, sprite.y + rect.y, sprite.width, sprite.height);
		}
		return rect;
	}
	
	public void SpriteSelection (BaseThing item) {
		string iconName = "";
		float iconSize = 64f;
		bool drawIcon = false;
		float extraSpace = 0f;
		if (item.iconAtlas != null) {
			BetterList<string> sprites = item.iconAtlas.GetListOfSprites();
			sprites.Insert(0, "<None>");
			int spriteIndex = 0;
			if(item.iconName == "")
				item.iconName = sprites[0];
			string spriteName = item.iconName;
			
			// We need to find the sprite in order to have it selected
			if (!string.IsNullOrEmpty(spriteName)) {
				for (int i = 1; i < sprites.size; ++i) {
					if (spriteName.Equals(sprites[i], System.StringComparison.OrdinalIgnoreCase)) {
						spriteIndex = i;
						break;
					}
				}
			}
			
			// Draw the sprite selection popup
			string[] colorList = ColorDefines.iconColors.Keys.ToArray();
			item.iconTintIndex = EditorGUILayout.Popup("Tint", item.iconTintIndex, colorList);
			spriteIndex = EditorGUILayout.Popup("Icon", spriteIndex, sprites.ToArray());
			UISpriteData sprite = (spriteIndex > 0) ? item.iconAtlas.GetSprite(sprites[spriteIndex]) : null;
			
			if (sprite != null) {
				iconName = sprite.name;
				Material mat = item.iconAtlas.spriteMaterial;
				if (mat != null) {
					Texture2D tex = mat.mainTexture as Texture2D;
					
					if (tex != null) {
						drawIcon = true;

						Color col = ColorDefines.HexStringToColor(ColorDefines.iconColors[colorList[item.iconTintIndex]]);
						GUI.color = col;
						Rect rect = new Rect(sprite.x, sprite.y, sprite.width, sprite.height);
						rect = NGUIMath.ConvertToTexCoords(rect, tex.width, tex.height);
						GUILayout.Space(4f);
						GUILayout.BeginHorizontal(); {
							GUILayout.Space(Screen.width - iconSize);
							DrawSprite(tex, rect, null, false);
						}
						GUILayout.EndHorizontal();
						extraSpace = iconSize * (float)sprite.height / sprite.width;
					}
				}
			}
		}
		
		// Calculate the extra spacing necessary for the icon to show up properly and not overlap anything
		if (drawIcon) {
			extraSpace = Mathf.Max(0f, extraSpace - 0f);
			GUILayout.Space(extraSpace);
		}
		
		if(!iconName.Equals(item.iconName))
			item.iconName = iconName;
	}

	public void NavigationSection(int count) {
		// Navigation section
		NGUIEditorTools.DrawSeparator();
		GUILayout.BeginHorizontal();
		if (mIndex == 0) GUI.color = Color.grey;
		if (GUILayout.Button("<<")) { mConfirmDelete = false; --mIndex; }
		GUI.color = Color.white;
		mIndex = EditorGUILayout.IntField(mIndex + 1, GUILayout.Width(40f)) - 1;
		GUILayout.Label("/ " + count, GUILayout.Width(40f));
		if (mIndex + 1 == count) GUI.color = Color.grey;
		if (GUILayout.Button(">>")) { mConfirmDelete = false; ++mIndex; }
		GUI.color = Color.white;
		GUILayout.EndHorizontal();
		NGUIEditorTools.DrawSeparator();
	}
}

