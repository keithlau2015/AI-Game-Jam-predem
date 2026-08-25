extends CanvasLayer
class_name HUD

var rescued_label: Label
var portal_label: Label
var banner: Label
var pc = null

func _ready():
	var root = Control.new()
	root.set_anchors_and_offsets_preset(Control.PRESET_FULL_RECT)
	add_child(root)

	# Bottom-left wooden panel + slime mascot + counter
	var panel = TextureRect.new()
	panel.texture = preload("res://assets/images/UI_Count.png")
	panel.set_position(Vector2(20, 720 - 120))
	panel.custom_minimum_size = Vector2(240, 100)
	panel.expand_mode = TextureRect.EXPAND_KEEP_SIZE
	root.add_child(panel)

	var mascot = TextureRect.new()
	mascot.texture = preload("res://assets/images/slime.png")
	mascot.set_position(Vector2(34, 720 - 104))
	mascot.size = Vector2(64, 32)
	mascot.expand_mode = TextureRect.EXPAND_KEEP_SIZE
	root.add_child(mascot)

	rescued_label = Label.new()
	rescued_label.text = "x15"
	rescued_label.set_position(Vector2(110, 720 - 96))
	rescued_label.add_theme_font_size_override("font_size", 30)
	root.add_child(rescued_label)

	# Bottom-right: 3 portal icons + state
	for i in range(3):
		var pi = TextureRect.new()
		pi.texture = preload("res://assets/images/Portal.png")
		pi.set_position(Vector2(1280 - 250 + i * 66, 720 - 104))
		pi.size = Vector2(60, 60)
		pi.expand_mode = TextureRect.EXPAND_KEEP_SIZE
		root.add_child(pi)

	portal_label = Label.new()
	portal_label.text = "PORTAL: IDLE"
	portal_label.set_position(Vector2(1280 - 250, 720 - 150))
	portal_label.add_theme_font_size_override("font_size", 20)
	root.add_child(portal_label)

	# Top banner
	banner = Label.new()
	banner.text = ""
	banner.set_position(Vector2(440, 40))
	banner.add_theme_font_size_override("font_size", 56)
	banner.horizontal_alignment = HORIZONTAL_ALIGNMENT_CENTER
	root.add_child(banner)

	# Hint
	var hint = Label.new()
	hint.text = "Left-click: place portal   Arrow keys: direction   R: retry"
	hint.set_position(Vector2(640 - 250, 360))
	hint.add_theme_font_size_override("font_size", 18)
	root.add_child(hint)

	GameManager.rescued_changed.connect(_on_rescued)
	GameManager.state_changed.connect(_on_state)

func _on_rescued(n):
	rescued_label.text = "x" + str(n) + " / " + str(GameManager.total)

func _on_state(s):
	if s == GameManager.State.CLEAR:
		banner.text = "CLEAR!"
	elif s == GameManager.State.FAIL:
		banner.text = "FAIL!"

func _process(delta):
	if pc == null:
		pc = get_tree().get_first_node_in_group("portal")
	if pc != null:
		var txt = "PORTAL: "
		match pc.state:
			PortalController.PState.IDLE: txt += "IDLE (click to place)"
			PortalController.PState.ENT: txt += "SET ENTRANCE (" + dir_name(pc.pending_dir) + ")"
			PortalController.PState.EXIT: txt += "SET EXIT (" + dir_name(pc.pending_dir) + ")"
			PortalController.PState.LOCKED:
				if pc.cooldown > 0:
					txt += "ACTIVE  cd " + str(snappedf(pc.cooldown, 0.1))
				else:
					txt += "ACTIVE"
		portal_label.text = txt

func dir_name(d) -> String:
	match d:
		DirectionUtil.Dir.UP: return "UP"
		DirectionUtil.Dir.RIGHT: return "RIGHT"
		DirectionUtil.Dir.DOWN: return "DOWN"
		DirectionUtil.Dir.LEFT: return "LEFT"
	return ""
