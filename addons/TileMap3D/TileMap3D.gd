@tool
extends EditorPlugin

const NODE_NAME = "TileMap3D"
const INHERITANCE = "Node3D"
const THE_SCRIPT = preload("TileMap3D.cs")
const THE_ICON = preload("TileMap3D.png")

func _enter_tree():
	add_custom_type(NODE_NAME, INHERITANCE, THE_SCRIPT, THE_ICON)

func _exit_tree():
	remove_custom_type(NODE_NAME)
