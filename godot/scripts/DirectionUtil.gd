extends RefCounted
class_name DirectionUtil

enum Dir { UP, RIGHT, DOWN, LEFT }

static func to_vector(d: Dir) -> Vector2:
	match d:
		Dir.UP: return Vector2(0, -1)
		Dir.RIGHT: return Vector2(1, 0)
		Dir.DOWN: return Vector2(0, 1)
		Dir.LEFT: return Vector2(-1, 0)
	return Vector2.ZERO

static func to_angle(d: Dir) -> float:
	match d:
		Dir.RIGHT: return 0.0
		Dir.DOWN: return 90.0
		Dir.LEFT: return 180.0
		Dir.UP: return -90.0
	return 0.0
