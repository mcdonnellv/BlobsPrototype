using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[CustomEditor(typeof(ItemManager))]
public class ItemManagerInspector : Editor {
	ItemManager itemManager;
	bool showGenes = false;
	string newName = "";
	bool mConfirmDelete = false;
	static int mIndex = 0;
	
	
	public override void OnInspectorGUI() {
		NGUIEditorTools.SetLabelWidth(80f);
		itemManager = (ItemManager)target;
		BaseItem item = null;

		if (itemManager.items == null || itemManager.items.Count == 0)
			mIndex = 0;
		else {
			mIndex = Mathf.Clamp(mIndex, 0, itemManager.items.Count - 1);
			item = itemManager.items[mIndex];
		}

		if (mConfirmDelete) {
			// Show the confirmation dialog
			GUILayout.Label("Are you sure you want to delete '" + item.itemName + "'?");
			NGUIEditorTools.DrawSeparator();
			
			GUILayout.BeginHorizontal();{
				GUI.backgroundColor = Color.green;
				if (GUILayout.Button("Cancel")) mConfirmDelete = false;
				GUI.backgroundColor = Color.red;
				
				if (GUILayout.Button("Delete")){
					itemManager.items.RemoveAt(mIndex);
					mConfirmDelete = false;
				}
				GUI.backgroundColor = Color.white;
			}
			GUILayout.EndHorizontal();
		}
		else {

			// Database icon atlas
			UIAtlas atlas = EditorGUILayout.ObjectField("Icon Atlas", itemManager.iconAtlas, typeof(UIAtlas), false) as UIAtlas;

			if (atlas != itemManager.iconAtlas) {
				itemManager.iconAtlas = atlas;
				foreach (BaseItem i in itemManager.items) i.iconAtlas = atlas;
			}

			// "New" button
			EditorGUILayout.BeginHorizontal();{
				newName = EditorGUILayout.TextField(newName, GUILayout.Width(100f));
				GUI.backgroundColor = Color.green;
				if (GUILayout.Button("New Item") && itemManager.DoesNameExistInList(newName) == false){
					BaseItem i = new BaseItem();
					i.itemName = newName;
					if(item != null) {
						if(newName == "")
							i.itemName = item.itemName + " copy";
						i.description = item.description;
					}
					itemManager.items.Add(i);
					mIndex = itemManager.items.Count - 1;
					newName = "";
					item = i;
				}
			}
			EditorGUILayout.EndHorizontal();
			GUI.backgroundColor = Color.white;

			if (item != null) {
				NGUIEditorTools.DrawSeparator();
				
				// Navigation section
				GUILayout.BeginHorizontal();
				{
					if (mIndex == 0) GUI.color = Color.grey;
					if (GUILayout.Button("<<")) { mConfirmDelete = false; --mIndex; }
					GUI.color = Color.white;
					mIndex = EditorGUILayout.IntField(mIndex + 1, GUILayout.Width(40f)) - 1;
					GUILayout.Label("/ " + itemManager.items.Count, GUILayout.Width(40f));
					if (mIndex + 1 == itemManager.items.Count) GUI.color = Color.grey;
					if (GUILayout.Button(">>")) { mConfirmDelete = false; ++mIndex; }
					GUI.color = Color.white;
				}
				GUILayout.EndHorizontal();
				NGUIEditorTools.DrawSeparator();

				// Item name and delete item button
				GUILayout.BeginHorizontal();
				{
					string itemName = EditorGUILayout.TextField("Item Name", item.itemName);
					GUI.backgroundColor = Color.red;
					if (GUILayout.Button("Delete", GUILayout.Width(55f)))
						mConfirmDelete = true;
					GUI.backgroundColor = Color.white;
					if (!itemName.Equals(item.itemName) && itemManager.DoesNameExistInList(itemName) == false)
						item.itemName = itemName;
				}
				GUILayout.EndHorizontal();
				item.description = GUILayout.TextArea(item.description, 200, GUILayout.Height(100f));


				string iconName = "";
				float iconSize = 64f;
				bool drawIcon = false;
				float extraSpace = 0f;

				if (item.iconAtlas != null) {
					BetterList<string> sprites = item.iconAtlas.GetListOfSprites();
					sprites.Insert(0, "<None>");
					int index = 0;
					string spriteName = (item.iconName != null) ? item.iconName : sprites[0];
					
					// We need to find the sprite in order to have it selected
					if (!string.IsNullOrEmpty(spriteName)) {
						for (int i = 1; i < sprites.size; ++i) {
							if (spriteName.Equals(sprites[i], System.StringComparison.OrdinalIgnoreCase)) {
								index = i;
								break;
							}
						}
					}
					
					// Draw the sprite selection popup
					index = EditorGUILayout.Popup("Icon", index, sprites.ToArray());
					UISpriteData sprite = (index > 0) ? item.iconAtlas.GetSprite(sprites[index]) : null;
					
					if (sprite != null) {
						iconName = sprite.name;
						Material mat = item.iconAtlas.spriteMaterial;
						if (mat != null) {
							Texture2D tex = mat.mainTexture as Texture2D;
							
							if (tex != null) {
								drawIcon = true;
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
				else
					item.iconAtlas = itemManager.iconAtlas;

				// Calculate the extra spacing necessary for the icon to show up properly and not overlap anything
				if (drawIcon) {
					extraSpace = Mathf.Max(0f, extraSpace - 0f);
					GUILayout.Space(extraSpace);
				}

				if(!iconName.Equals(item.iconName))
					item.iconName = iconName;

			}
		}
	}


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


}
