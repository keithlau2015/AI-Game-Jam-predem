extends Node2D
class_name Crystal

func _ready():
	var spr = Sprite2D.new()
	spr.texture = preload("res://assets/images/Obstacle.png")
	spr.scale = Vector2(0.9, 0.9)
	add_child(spr)
