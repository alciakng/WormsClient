using UnityEngine;
using SocketIO;
using System;
using System.Text;


public class GameNetworkController : MonoBehaviour {

	//GameController
	public GameController gc;
	//ServerConnect
	private ServerConnect sc;
	//클라이언트 소켓 
	private SocketIOComponent Socket;


	void Awake(){
		gc = GameObject.Find ("GameController").GetComponent<GameController> ();
		sc = GameObject.Find ("ServerConnect").GetComponent<ServerConnect> ();
		Socket = sc.Socket;
	}

	void Start(){
		EventHandler ();
		//플레이어를 생성한다.
		gc.CreatePlayer(sc.players);
	}

	void EventHandler(){
		Socket.On ("ack_turn_change",gc.turnChange);
		Socket.On ("ack_room_chat", gc.gameChat);
		Socket.On ("ack_to_lobby", gc.toLobby);
		Socket.On ("ack_player_position_sync",gc.positionSync);

		Socket.On("ack_move_right",gc.moveRight);
		Socket.On("ack_move_left", gc.moveLeft);
		Socket.On ("ack_move_stop", gc.moveStop);
		Socket.On ("ack_jump", gc.jump);
		Socket.On ("ack_jump_stop", gc.jumpStop);
		Socket.On ("ack_set_target", gc.setTarget);
		Socket.On ("ack_unset_target", gc.unsetTarget);
		Socket.On ("ack_update_target", gc.updateTarget);
		Socket.On ("ack_set_marker", gc.setMarker);
		Socket.On ("ack_unset_marker", gc.unsetMarker);
		Socket.On ("ack_update_marker", gc.updateMarker);
	//	Socket.On ("ack_update_shoot_detection", gc.updateShootDetection);
	//	Socket.On ("ack_update_shooting", gc.updateShooting);
		Socket.On ("ack_shoot", gc.shoot);
		Socket.On ("ack_set_bazooka", gc.setBazooka);
		Socket.On ("ack_unset_bazooka", gc.unsetBazooka);
		Socket.On ("ack_set_bomb", gc.setBomb);
		Socket.On ("ack_unset_bomb", gc.unsetBomb);
		Socket.On ("ack_set_bat", gc.setBat);
		Socket.On ("ack_unset_bat", gc.unsetBat);
		Socket.On ("ack_batting", gc.batting);
		Socket.On ("ack_player_batted",gc.playerBatted);
		Socket.On ("ack_set_jetpack", gc.setJetPack);
		Socket.On ("ack_unset_jetpack", gc.unsetJetPack);
		Socket.On ("ack_update_jetpack", gc.updateJetPack);
		Socket.On ("ack_set_sheep", gc.setSheep);
		Socket.On ("ack_unset_sheep", gc.unsetSheep);
		Socket.On ("ack_shoot_sheep", gc.shootSheep);
		Socket.On ("ack_sheep_dir_right", gc.sheepDirRight);
		Socket.On ("ack_sheep_dir_left", gc.sheepDirLeft);
		Socket.On ("ack_sheep_jump", gc.sheepJump);
		Socket.On ("ack_explode_sheep", gc.explodeSheep);
		Socket.On ("ack_explode_supersheep", gc.explodeSuperSheep);
		Socket.On ("ack_sync_sheep", gc.syncSheep);
		Socket.On ("ack_supersheep_rotation_right", gc.superSheepRotationRight);
		Socket.On ("ack_supersheep_rotation_left", gc.superSheepRotationLeft);
		Socket.On ("ack_set_donkey", gc.setDonkey);
		Socket.On ("ack_unset_donkey", gc.unsetDonkey);
		Socket.On ("ack_shoot_donkey", gc.shootDonkey);
		Socket.On ("ack_destroy_donkey", gc.destroyDonkey);
		Socket.On ("ack_set_hbomb", gc.setHbomb);
		Socket.On ("ack_unset_hbomb", gc.unsetHbomb);
		Socket.On ("ack_set_teleport", gc.setTeleport);
		Socket.On ("ack_teleporting", gc.teleporting);
		Socket.On ("ack_unset_teleport", gc.unsetTeleport);
		Socket.On ("ack_set_rope",gc.setRope);
		Socket.On ("ack_unset_rope",gc.unsetRope);
		Socket.On ("ack_shoot_rope",gc.shootRope);
		Socket.On ("ack_rope_jump",gc.ropeJump);
		Socket.On ("ack_rope_down",gc.ropeDown);
		Socket.On ("ack_rope_up",gc.ropeUp);
		Socket.On ("ack_destroy_ground", gc.destroyGround);
		Socket.On ("ack_player_dead", gc.playerDead);
		Socket.On ("ack_player_victory", gc.playerVictory);
	}
}