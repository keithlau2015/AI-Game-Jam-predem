extends Node2D
class_name GridDraw

func _draw():
	var cols := LevelData.GRID_COLS
	var rows := LevelData.GRID_ROWS
	var cell := LevelData.CELL
	for c in range(cols):
		for r in range(rows):
			var center := LevelData.cell_to_world(Vector2i(c, r))
			var rect := Rect2(center.x - cell * 0.5, center.y - cell * 0.5, cell, cell)
			draw_rect(rect, Color(0.15, 0.22, 0.32, 0.35), true)
			draw_rect(rect, Color(0.45, 0.68, 0.9, 0.55), false, 2.0)
	var minc := LevelData.cell_to_world(Vector2i(0, 0))
	var maxc := LevelData.cell_to_world(Vector2i(cols - 1, rows - 1))
	var outer := Rect2(minc.x - cell * 0.5, minc.y - cell * 0.5, cell * cols, cell * rows)
	draw_rect(outer, Color(0.95, 0.95, 1.0, 0.9), false, 3.0)
