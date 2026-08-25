extends Node
class_name GameManagerNode

signal state_changed(state)
signal rescued_changed(n)

enum State { PLAYING, CLEAR, FAIL }

var state: int = State.PLAYING
var total: int = 15
var rescued: int = 0
var dead: int = 0

func _ready():
	reset()

func reset():
	state = State.PLAYING
	total = LevelData.SPAWN_COUNT
	rescued = 0
	dead = 0

func on_rescued():
	if state != State.PLAYING:
		return
	rescued += 1
	rescued_changed.emit(rescued)
	if rescued >= total:
		state = State.CLEAR
		state_changed.emit(state)
		print("GameManager state -> CLEAR (rescued=", rescued, "/", total, ", dead=", dead, ")")

func on_died():
	if state != State.PLAYING:
		return
	dead += 1
	if dead >= 1:
		state = State.FAIL
		state_changed.emit(state)
		print("GameManager state -> FAIL (dead=", dead, ")")
