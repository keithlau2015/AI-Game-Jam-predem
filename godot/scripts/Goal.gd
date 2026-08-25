extends Node2D
class_name Goal

func _ready():
	var spr = Sprite2D.new()
	spr.texture = preload("res://assets/images/Goal.png")
	spr.scale = Vector2(0.45, 0.45)
	add_child(spr)
