extends Node2D
class_name Escort

var dir: DirectionUtil.Dir = DirectionUtil.Dir.RIGHT
var speed: float = 100.0
var is_alive: bool = true
var is_rescued: bool = false
var teleport_lock: float = 0.0
var last_cell: Vector2i = Vector2i(-99, -99)

func _ready():
	var spr = Sprite2D.new()
	spr.texture = preload("res://assets/images/slime.png")
	spr.scale = Vector2(0.5, 0.5)
	spr.z_index = 5
	add_child(spr)
	add_to_group("escort")
	last_cell = LevelData.world_to_cell(global_position)

func _physics_process(delta):
	step(delta)

func step(delta):
	if not is_alive or is_rescued:
		return
	if teleport_lock > 0:
		teleport_lock -= delta

	# Portal teleport (direction must match entrance direction)
	var pc = get_tree().get_first_node_in_group("portal")
	if pc != null and pc.active and teleport_lock <= 0:
		var cur = LevelData.world_to_cell(global_position)
		if cur == pc.entrance_cell and dir == pc.entrance_dir:
			pc.do_teleport(self)
			return

	# Floor effects (speed)
	var spd = speed
	var cell = LevelData.world_to_cell(global_position)
	var f = LevelData.floor_at(cell)
	if f.size() > 0 and (f["type"] == "speedup" or f["type"] == "slow"):
		spd = speed * float(f["mult"])

	# Floor effects (turn on cell entry)
	if cell != last_cell:
		last_cell = cell
		if f.size() > 0 and f["type"] == "turn":
			dir = int(f["turn"])

	global_position += DirectionUtil.to_vector(dir) * spd * delta

	# Hazard / win checks
	var ncell = LevelData.world_to_cell(global_position)
	if not LevelData.in_grid(ncell):
		die()
		return
	if ncell in LevelData.CRYSTALS:
		die()
		return
	if ncell == LevelData.GOAL_CELL:
		rescue()
		return
	if ncell == LevelData.TURRET_CELL:
		die()
		return

func die():
	if not is_alive:
		return
	is_alive = false
	GameManager.on_died()
	queue_free()

func rescue():
	if is_rescued:
		return
	is_rescued = true
	GameManager.on_rescued()
	queue_free()
