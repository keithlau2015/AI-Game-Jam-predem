extends SceneTree

const LOG := "C:\\Users\\justin\\AppData\\Local\\Temp\\opencode\\sim_result.txt"

func _log(msg: String):
	var f = FileAccess.open(LOG, FileAccess.READ_WRITE)
	if f != null:
		f.seek_end()
		f.store_line(msg)
		f.close()

func _initialize():
	FileAccess.open(LOG, FileAccess.WRITE).close()  # truncate
	_log("START")

	# ---- WIN case: portal redirects slime to the goal ----
	var root = Node.new()
	self.root.add_child(root)
	var pc = PortalController.new()
	root.add_child(pc)
	pc.entrance_cell = Vector2i(2, 2)
	pc.entrance_dir = DirectionUtil.Dir.RIGHT
	pc.exit_cell = Vector2i(2, 4)
	pc.exit_dir = DirectionUtil.Dir.UP
	pc.active = true
	var e = Escort.new()
	e.position = LevelData.cell_to_world(Vector2i(0, 2))
	e.dir = DirectionUtil.Dir.RIGHT
	e.speed = 100.0
	root.add_child(e)

	var steps = 0
	var prev = e.position
	var stuck = 0
	while steps < 5000 and e.is_alive and not e.is_rescued:
		e.step(0.05)
		steps += 1
		if e.position.distance_to(prev) < 0.001:
			stuck += 1
		else:
			stuck = 0
		prev = e.position
		if stuck > 200:
			_log("WIN stuck-skip at step " + str(steps))
			break
	_log("WIN  rescued=" + str(e.is_rescued) + " alive=" + str(e.is_alive) +
		" steps=" + str(steps) + " cell=" + str(LevelData.world_to_cell(e.position)))

	# ---- LOSE case: no portal, slime hits the boundary ----
	var root2 = Node.new()
	self.root.add_child(root2)
	var pc2 = PortalController.new()
	root2.add_child(pc2)
	var e2 = Escort.new()
	e2.position = LevelData.cell_to_world(Vector2i(0, 2))
	e2.dir = DirectionUtil.Dir.RIGHT
	e2.speed = 100.0
	root2.add_child(e2)
	var s2 = 0
	while s2 < 5000 and e2.is_alive and not e2.is_rescued:
		e2.step(0.05)
		s2 += 1
	_log("LOSE rescued=" + str(e2.is_rescued) + " alive=" + str(e2.is_alive) +
		" steps=" + str(s2) + " cell=" + str(LevelData.world_to_cell(e2.position)))

	# ---- sanity: coordinate round-trip ----
	var w = LevelData.cell_to_world(Vector2i(2, 4))
	var back = LevelData.world_to_cell(w)
	_log("COORD cell(2,4)->world " + str(w) + " ->cell " + str(back) + " ok=" + str(back == Vector2i(2, 4)))

	_log("DONE")
	quit()
