using UnityEngine;
using System.Collections;

public static class Notifications {

	public static string Tick { get { return "OnTick"; } }
	public static string Turn { get { return "OnTurn"; } }
	public static string TileStateChange { get { return "OnTileStateChange"; } }
	public static string TileOwnershipChange { get { return "OnTileOwnershipChange"; } }
	public static string BlockOwnershipChange { get { return "OnBlockOwnershipChange"; } }
	public static string DropUsed { get { return "OnDropUsed"; } }

	public static string TurnChange { get { return "OnTurnChange"; } }

}
