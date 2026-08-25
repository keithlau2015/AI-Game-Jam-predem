extends Node2D
class_name Turret

var fire_interval: float = 2.0
var direction: Vector2 = Vector2.UP
var projectile_speed: float = 150.0
var projectile_size: float = 0.5

var _timer: float = 0.0

func _ready():
	var spr = Sprite2D.new()
	spr.texture = preload("res://assets/images/SzDRt.png")
	spr.scale = Vector2(0.5, 0.5)
	add_child(spr)
	_timer = fire_interval  # wait one interval before first shot

func _process(delta):
	_timer -= delta
	if _timer <= 0:
		_timer = fire_interval
		fire()

func _physics_process(delta):
	for e in get_tree().get_nodes_in_group("escort"):
		if e.is_alive and not e.is_rescued and global_position.distance_to(e.global_position) < 44.0:
			e.die()

func fire():
	var p = Projectile.new()
	var muzzle = global_position + direction * 30.0
	p.setup(muzzle, direction, projectile_speed, projectile_size)
	get_parent().add_child(p)
