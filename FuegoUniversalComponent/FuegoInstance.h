#pragma once

//#pragma unmanaged

#include <iostream>
#include <sstream>

#include "fuego/smartgame/SgSystem.h"
#include "fuego/smartgame/SgInit.h"
#include "fuego/smartgame/SgBlackWhite.h"
#include "fuego/go/GoGtpEngine.h"
#include "fuego/go/GoInit.h"
#include "fuego/gouct/GoUctCommands.h"

//#pragma managed

#include <collection.h>

namespace FuegoLib
{
	// This class subclasses GoGtpEngine in a way that registers the Uct commands.
	// And instance of this class is created by the FuegoInstance class.
	class FuegoMainEngine : public GoGtpEngine
	{
	public:
		FuegoMainEngine(int fixedBoardSize, const char* programPath,
			bool noHandicap);
		~FuegoMainEngine();

	private:
		GoUctCommands m_uctCommands;

		typedef GoUctPlayer<GoUctGlobalSearch<GoUctPlayoutPolicy<GoUctBoard>,
			GoUctPlayoutPolicyFactory<GoUctBoard> >,
			GoUctGlobalSearchState<GoUctPlayoutPolicy<GoUctBoard> > >
			PlayerType;		
	};

	public ref class FuegoInstance sealed
	{
	private:
		FuegoMainEngine* _e;
		static bool _inited;

		void FuegoInstance::CleanupStreams();
		
		~FuegoInstance();

	public:
		FuegoInstance();
		void StartGame(unsigned char size);
		Platform::String^ HandleCommand(Platform::String^ cmd);	
	};
}
