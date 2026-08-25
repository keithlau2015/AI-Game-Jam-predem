extends Node2D
class_name PortalController

enum PState { IDLE, ENT, EXIT, LOCKED }
signal state_changed

var state: int = PState.IDLE
var entrance_cell: Vector2i = Vector2i(-1, -1)
var entrance_dir: DirectionUtil.Dir = DirectionUtil.Dir.RIGHT
var exit_cell: Vector2i = Vector2i(-1, -1)
var exit_dir: DirectionUtil.Dir = DirectionUtil.Dir.RIGHT
var active: bool = false
var cooldown: float = 0.0
var pending_dir: DirectionUtil.Dir = DirectionUtil.Dir.RIGHT

var entrance_spr: Sprite2D
var exit_spr: Sprite2D

func _ready():
	add_to_group("portal")
	entrance_spr = Sprite2D.new()
	entrance_spr.texture = preload("res://assets/images/Portal.png")
	entrance_spr.scale = Vector2(0.45, 0.45)
	entrance_spr.z_index = 4
	entrance_spr.visible = false
	add_child(entrance_spr)
	exit_spr = Sprite2D.new()
	exit_spr.texture = preload("res://assets/images/Portal.png")
	exit_spr.scale = Vector2(0.45, 0.45)
	exit_spr.z_index = 4
	exit_spr.visible = false
	add_child(exit_spr)

func _process(delta):
	if cooldown > 0:
		cooldown -= delta
		if cooldown <= 0:
			cooldown = 0
			if state == PState.LOCKED:
				active = false
				state = PState.IDLE
				state_changed.emit()

func begin_placement():
	if state == PState.IDLE:
		state = PState.ENT
		pending_dir = DirectionUtil.Dir.RIGHT
		state_changed.emit()

func confirm_at(cell: Vector2i):
	if state == PState.ENT:
		if can_place(cell):
			entrance_cell = cell
			entrance_dir = pending_dir
			state = PState.EXIT
			update_portal_visuals()
			state_changed.emit()
	elif state == PState.EXIT:
		if can_place(cell) and within_range(cell):
			exit_cell = cell
			exit_dir = pending_dir
			active = true
			state = PState.LOCKED
			cooldown = LevelData.COOLDOWN
			update_portal_visuals()
			state_changed.emit()

func update_portal_visuals():
	if entrance_cell != Vector2i(-1, -1):
		entrance_spr.visible = true
		entrance_spr.position = LevelData.cell_to_world(entrance_cell)
		entrance_spr.rotation = DirectionUtil.to_vector(entrance_dir).angle()
	else:
		entrance_spr.visible = false
	if exit_cell != Vector2i(-1, -1):
		exit_spr.visible = true
		exit_spr.position = LevelData.cell_to_world(exit_cell)
		exit_spr.rotation = DirectionUtil.to_vector(exit_dir).angle()
	else:
		exit_spr.visible = false

func can_place(cell: Vector2i) -> bool:
	if not LevelData.in_grid(cell):
		return false
	if cell in LevelData.CRYSTALS:
		return false
	if cell == LevelData.GOAL_CELL:
		return false
	if cell == LevelData.TURRET_CELL:
		return false
	if state == PState.EXIT and cell == entrance_cell:
		return false
	for e in get_tree().get_nodes_in_group("escort"):
		if LevelData.world_to_cell(e.global_position) == cell:
			return false
	return true

func within_range(cell: Vector2i) -> bool:
	var a = LevelData.cell_to_world(entrance_cell)
	var b = LevelData.cell_to_world(cell)
	return a.distance_to(b) <= LevelData.MAX_PORTAL_DIST

func do_teleport(e):
	var exit_c = LevelData.cell_to_world(exit_cell)
	var v = DirectionUtil.to_vector(exit_dir)
	e.global_position = exit_c + v * LevelData.PORTAL_SAFE_OFFSET
	e.dir = exit_dir
	e.teleport_lock = LevelData.TELEPORT_LOCK
