extends Node2D
class_name Main

var ent: Node2D
var pc: PortalController
var preview: Sprite2D
var arrow: Polygon2D
var spawn_timer: float = 0.0
var spawned: int = 0
var last_mouse_world: Vector2 = Vector2.ZERO

func _ready():
	GameManager.reset()

	# Background (sand path / forest clearing)
	var bg = Sprite2D.new()
	bg.texture = preload("res://assets/images/Map.png")
	bg.scale = Vector2(3.0, 2.5)
	add_child(bg)

	# Grid / tile visualization aligned to LevelData coordinates
	var grid = preload("res://scripts/GridDraw.gd").new()
	add_child(grid)

	ent = Node2D.new()
	add_child(ent)

	# Crystals (lethal obstacles)
	for c in LevelData.CRYSTALS:
		var cry = Crystal.new()
		cry.position = LevelData.cell_to_world(c)
		ent.add_child(cry)

	# Goal
	var goal = Goal.new()
	goal.position = LevelData.cell_to_world(LevelData.GOAL_CELL)
	ent.add_child(goal)

	# Turret
	var turret = Turret.new()
	turret.position = LevelData.cell_to_world(LevelData.TURRET_CELL)
	turret.direction = DirectionUtil.to_vector(LevelData.TURRET_DIR)
	turret.fire_interval = LevelData.TURRET_INTERVAL
	turret.projectile_speed = LevelData.TURRET_PROJ_SPEED
	turret.projectile_size = LevelData.TURRET_PROJ_SIZE
	ent.add_child(turret)

	# Portal controller (player-driven placement)
	pc = PortalController.new()
	ent.add_child(pc)

	# Placement preview + direction arrow
	preview = Sprite2D.new()
	preview.texture = preload("res://assets/images/Portal.png")
	preview.visible = false
	preview.modulate = Color(0, 1, 0)
	ent.add_child(preview)

	arrow = Polygon2D.new()
	arrow.color = Color(1, 1, 0)
	arrow.visible = false
	ent.add_child(arrow)

	# HUD
	var hud = HUD.new()
	add_child(hud)

	# Camera
	var cam = Camera2D.new()
	cam.position = Vector2(0, 0)
	cam.zoom = Vector2(1.6, 1.6)
	add_child(cam)

	# Debug auto-portal (headless verification only)
	if LevelData.AUTO_PORTAL and pc != null:
		pc.entrance_cell = Vector2i(2, 2)
		pc.entrance_dir = DirectionUtil.Dir.RIGHT
		pc.exit_cell = Vector2i(2, 4)
		pc.exit_dir = DirectionUtil.Dir.UP
		pc.active = true
		pc.state = PortalController.PState.LOCKED
		pc.update_portal_visuals()

	spawn_timer = 0.0
	spawned = 0

func _process(delta):
	if GameManager.state == GameManager.State.PLAYING:
		spawn_timer -= delta
		if spawned < LevelData.SPAWN_COUNT and spawn_timer <= 0:
			spawn_timer = LevelData.SPAWN_INTERVAL
			spawn_escort()
			spawned += 1
	update_preview()

func spawn_escort():
	var e = Escort.new()
	e.position = LevelData.cell_to_world(LevelData.SPAWN_CELL)
	e.dir = LevelData.SPAWN_DIR
	e.speed = LevelData.SPAWN_SPEED
	ent.add_child(e)

func _unhandled_input(event):
	if event is InputEventKey and event.pressed:
		var k = event.keycode
		if pc != null and (pc.state == PortalController.PState.ENT or pc.state == PortalController.PState.EXIT):
			if k == KEY_RIGHT or k == KEY_D: pc.pending_dir = DirectionUtil.Dir.RIGHT
			elif k == KEY_LEFT or k == KEY_A: pc.pending_dir = DirectionUtil.Dir.LEFT
			elif k == KEY_UP or k == KEY_W: pc.pending_dir = DirectionUtil.Dir.UP
			elif k == KEY_DOWN or k == KEY_S: pc.pending_dir = DirectionUtil.Dir.DOWN
		if k == KEY_R:
			get_tree().reload_current_scene()
		return
	if event is InputEventMouseMotion:
		_update_mouse(event)
	if event is InputEventMouseButton and event.pressed and event.button_index == MOUSE_BUTTON_LEFT:
		_update_mouse(event)
		var cell = LevelData.world_to_cell(last_mouse_world)
		if pc != null:
			if pc.state == PortalController.PState.IDLE:
				pc.begin_placement()
			elif pc.state == PortalController.PState.ENT or pc.state == PortalController.PState.EXIT:
				pc.confirm_at(cell)

func _update_mouse(event):
	var cam = get_viewport().get_camera_2d()
	if cam != null:
		last_mouse_world = cam.get_screen_transform().affine_inverse() * event.position
	else:
		last_mouse_world = event.position

func update_preview():
	if pc == null:
		return
	if pc.state == PortalController.PState.ENT or pc.state == PortalController.PState.EXIT:
		var wp = last_mouse_world
		var cell = LevelData.world_to_cell(wp)
		preview.visible = true
		preview.position = LevelData.cell_to_world(cell)
		var ok = pc.can_place(cell)
		if pc.state == PortalController.PState.EXIT:
			ok = ok and pc.within_range(cell)
		preview.modulate = Color(0, 1, 0) if ok else Color(1, 0, 0)
		set_arrow(cell, pc.pending_dir)
	else:
		preview.visible = false
		arrow.visible = false

func set_arrow(cell, dir):
	arrow.visible = true
	arrow.position = LevelData.cell_to_world(cell)
	var v = DirectionUtil.to_vector(dir)
	var tip = v * 35.0
	var base1 = Vector2(-v.y, v.x) * 12.0
	var base2 = Vector2(v.y, -v.x) * 12.0
	arrow.polygon = [tip, base1, base2]
