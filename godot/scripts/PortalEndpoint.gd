extends Node2D
class_name PortalEndpoint

@export var is_entrance: bool = true
@export var direction: Vector2i = Vector2i(1, 0)

func _ready() -> void:
    var spr = Sprite2D.new()
    spr.texture = preload("res://assets/images/Portal.png")
    spr.scale = Vector2(0.45, 0.45)
    spr.z_index = 4
    add_child(spr)
