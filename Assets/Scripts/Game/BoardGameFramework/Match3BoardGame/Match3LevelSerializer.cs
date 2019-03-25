using UnityEngine;
using System.Collections;
using System.IO;

/// <summary>
/// Match3BoardSerializer
/// 
/// A Match3 level consists of multiple pluginable elements:
/// - a data structure of type <see cref="AbstractWinCondition"/> that will define the winning conditions for a level.
/// - a <see cref="BoardData"/> configuration (with <see cref="AbstractBoardPiece"/> and <see cref="AbstractTile"/> components each with its own properties and types).
/// </summary>
public class Match3LevelSerializer : AbstractSerializer {
	public const string FileHeaderTag = "Frozen Match3 Level File";
	
	protected Match3BoardGameLogic gameLogic;
	
	public Match3LevelSerializer(Match3BoardGameLogic _gameLogic, string _fileName) : base(_fileName) { 
		fileHeaderTag = FileHeaderTag;
		gameLogic = _gameLogic;
	}

	#region implemented abstract members of AbstractSerializer
	public override void WriteFileData (BinaryWriter writeStream) {
		gameLogic.winConditions.WriteToStream(writeStream);
		gameLogic.boardData.WriteToStream(writeStream);
	}

	public override void ReadFileData (int fileVersion, BinaryReader readStream) {
		//TODO: call events on the Match3BoardGameLogic and Match3BoardRenderer to instantiate corresponding game objects
		throw new System.NotImplementedException();
	}
	#endregion
}
