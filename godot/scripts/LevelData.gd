extends Node
class_name LevelData

const GRID_COLS := 6
const GRID_ROWS := 5
const CELL := 100

# Spawn (DirectionUtil.Dir values: UP=0, RIGHT=1, DOWN=2, LEFT=3)
const SPAWN_CELL := Vector2i(0, 2)
const SPAWN_DIR := 1
const SPAWN_COUNT := 15
const SPAWN_INTERVAL := 1.0
const SPAWN_SPEED := 100.0   # 2 world units/sec (1 unit = 50px)

# Goal
const GOAL_CELL := Vector2i(5, 4)

# Crystals (lethal obstacles)
const CRYSTALS := [Vector2i(3, 2), Vector2i(1, 4), Vector2i(4, 1)]

# Turret (DirectionUtil.Dir: UP=0)
const TURRET_CELL := Vector2i(2, 0)
const TURRET_DIR := 0
const TURRET_INTERVAL := 2.0
const TURRET_PROJ_SPEED := 150.0
const TURRET_PROJ_SIZE := 0.5

# Floor effects (type: slow / speedup / turn ; turn is DirectionUtil.Dir int, -1 = none)
const FLOORS := [
	{"cell": Vector2i(1, 2), "type": "slow", "mult": 0.5, "turn": -1},
	{"cell": Vector2i(2, 4), "type": "turn", "mult": 1.0, "turn": 1}  # 1 = RIGHT
]

# Portal rules
const MAX_PORTAL_DIST := 300.0   # 6 world units (1 unit = 50px)
const COOLDOWN := 3.0
const PORTAL_SAFE_OFFSET := 30.0
const TELEPORT_LOCK := 0.15

# Debug: auto-configure a winning portal (used only for headless verification)
const AUTO_PORTAL := false

static func cell_to_world(c: Vector2i) -> Vector2:
	return Vector2(-250 + c.x * CELL, 200 - c.y * CELL)

static func world_to_cell(pos: Vector2) -> Vector2i:
	var col = round((pos.x + 250) / CELL)
	var row = round((200 - pos.y) / CELL)
	return Vector2i(int(col), int(row))

static func in_grid(c: Vector2i) -> bool:
	return c.x >= 0 and c.x < GRID_COLS and c.y >= 0 and c.y < GRID_ROWS

static func floor_at(c: Vector2i) -> Dictionary:
	for f in FLOORS:
		if f["cell"] == c:
			return f
	return {}
