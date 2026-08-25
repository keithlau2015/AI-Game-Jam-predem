extends Node2D
class_name Projectile

var speed: float = 150.0
var direction: Vector2 = Vector2.RIGHT
var _life: float = 6.0

func setup(start: Vector2, dir: Vector2, spd: float, size: float):
	position = start
	direction = dir
	speed = spd
	var spr = Sprite2D.new()
	spr.texture = preload("res://assets/images/Projectile.png")
	spr.scale = Vector2(size * 3.0, size * 3.0)
	add_child(spr)

func _physics_process(delta):
	position += direction * speed * delta
	_life -= delta
	if _life <= 0:
		queue_free()
		return
	var cell = LevelData.world_to_cell(position)
	if not LevelData.in_grid(cell):
		queue_free()
		return
	if cell in LevelData.CRYSTALS:
		queue_free()
		return
	for e in get_tree().get_nodes_in_group("escort"):
		if e.is_alive and not e.is_rescued and position.distance_to(e.position) < 30.0:
			e.die()
			queue_free()
			return
