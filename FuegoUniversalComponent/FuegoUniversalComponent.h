#pragma once

#pragma unmanaged

#include "fuego/smartgame/SgSystem.h"
#include "fuego/smartgame/SgInit.h"
#include "fuego/smartgame/SgBlackWhite.h"
#include "fuego/go/GoGtpEngine.h"
#include "fuego/go/GoInit.h"
#include "fuego/gouct/GoUctCommands.h"

#include <stdlib.h>
#include <vector>
#include <memory>
#include <wchar.h>

#pragma managed

namespace FuegoLib
{
	public enum class GoOperation
	{
		Idle,
		Starting,
		NormalMove,
		Pass,
		Resign,
		GenMove,
		Hint,
		Undo
	};

	public enum class MoveType
	{
		Normal = 0,
		Pass = 1,
		Resign = 2
	};

	public enum class GoGameStatus
	{
		Active = 0,
		BlackWon = 1,
		WhiteWon = 2,
		BlackWonDueToResignation = 3,
		WhiteWonDueToResignation = 4
	};

	public enum class PlayerType
	{
		Human = 0,
		AI = 1,
		Remote = 2
	};

	public enum class GoResultCode
	{
		Success,
		CommunicationError,
		ServerInternalError,
		GameDoesNotExist,
		GameAlreadyExists,
		ClientOutOfSync,
		SimultaneousRequests,
		IllegalMoveSpaceOccupied,
		IllegalMoveSuicide,
		IllegalMoveSuperKo,
		OtherIllegalMove,
		OtherEngineError,
		EngineBusy,
		CannotScore,
		CannotSaveSGF
	};

	public enum class GoColor
	{
		Black,
		White
	};
	
	public ref class GoResponse sealed
	{
		// This empty constructor is so WCF's DataContractSerializer is able to build an instance of this type.
	public:
		GoResponse()
		{
			ResultCode = GoResultCode::Success;
		}

		GoResponse(GoResultCode resultCode)
		{
			ResultCode = resultCode;
		}

		property GoResultCode ResultCode;
	};


	public ref class GoScoreResponse sealed
	{
	public:
		GoScoreResponse(GoResultCode resultCode, GoColor winner, float score)
		{
			Winner = winner;
			Score = score;
			ResultCode = resultCode;
		}

		property GoColor Winner;
		property float Score;
		property GoResultCode ResultCode;
	};

	public ref class GoNameResponse sealed
	{
		// This empty constructor is so WCF's DataContractSerializer is able to build an instance of this type.
	public:
		GoNameResponse(GoResultCode resultCode, Platform::String^ name)
		{
			Name = name;
			ResultCode = resultCode;
		}
		
		property Platform::String^ Name;
		property GoResultCode ResultCode;
	};

	public ref class GoPlayer2 sealed
	{
	public:
		property Platform::String^ Name;
		property PlayerType PlayerType;
		property double Score;
		property int Level;
	};

	public ref class GoMove sealed
	{
	public:
		GoMove(MoveType moveType, GoColor color, Platform::String^ position)
		{
			MoveType = moveType;
			Color = color;
			Position = position;
		}

		property GoColor Color;
		property Platform::String^ Position;

		property MoveType MoveType;

		virtual Platform::String^ ToString() override
		{
			if (MoveType == FuegoLib::MoveType::Normal)
				return Position;
			return MoveType.ToString();
		}
	};

	public ref class GoMoveResult sealed
	{
	public:
		GoMoveResult(Platform::String^ capturedStones)
		{
			CapturedStones = capturedStones;
		}

		property Platform::String^ CapturedStones;
		property GoGameStatus Status;
		property double WinMargin;

	private:
	};

	public ref class GoMoveHistoryItem sealed
	{
	public:
		property GoMove^ Move;
		property int Sequence;
		property GoMoveResult^ Result;
	};

	public ref class GoGameState sealed
	{
	public:
		GoGameState(unsigned char size,
			GoPlayer2^ player1, GoPlayer2^ player2,
			GoGameStatus status,
			GoColor whoseTurn, Platform::String^ blackPositions, Platform::String^ whitePositions,
			const Platform::Array<GoMoveHistoryItem^>^ goMoveHistory, double winMargin)
		{
			Size = size;
			Player1 = player1;
			Player2 = player2;
			Status = status;
			WhoseTurn = whoseTurn;
			BlackPositions = blackPositions;
			WhitePositions = whitePositions;
			GoMoveHistory = goMoveHistory;
			WinMargin = winMargin;
		}
				
		property GoGameStatus Status;
		property double WinMargin;
		property GoPlayer2^ Player1;
		property GoPlayer2^ Player2;
		property GoOperation Operation;

		property unsigned char Size;

		property GoColor WhoseTurn;

		property Platform::String^ BlackPositions;

		property Platform::String^ WhitePositions;

		property Platform::Array<GoMoveHistoryItem^>^ GoMoveHistory;
	};

	public ref class GoGameStateResponse sealed
	{
	public:
		GoGameStateResponse(GoResultCode resultCode, GoGameState^ gameState)
		{			
			GameState = gameState;
			ResultCode = resultCode;
		}

		property GoGameState^ GameState;
		property GoResultCode ResultCode;
	};

	public ref class GoMoveResponse sealed
	{
	public:
		GoMoveResponse(GoResultCode resultCode, GoMove^ move, GoMoveResult^ result)
		{
			Move = move;
			MoveResult = result;
			ResultCode = resultCode;
		}

		property GoMoveResult^ MoveResult;
		property GoMove^ Move;
		property GoResultCode ResultCode;
	};

	public ref class GoHintResponse sealed
	{
		// This empty constructor is so WCF's DataContractSerializer is able to build an instance of this type.
	public:
		GoHintResponse(GoResultCode resultCode, GoMove^ move)
		{
			Move = move;
			ResultCode = resultCode;
		}

		property GoMove^ Move;
		property GoResultCode ResultCode;
	};

	public ref class GoScore sealed
	{
	public:
		GoScore(GoColor winner, double score)
		{
			Winner = winner;
			Score = score;
		}

		double ToFloat(GoColor color)
		{
			if (Winner != color)
				return -Score;
			return Score;
		}

		property GoColor Winner;
		property double Score;
	};

	public ref class FuegoUniversalComponent sealed
	{
	private:
		GoGtpEngine* _e;

	public:
		void Init();
		void StartGame(GoGameState^ state);
	};
}
