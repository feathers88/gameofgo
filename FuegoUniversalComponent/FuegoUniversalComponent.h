#pragma once

#pragma unmanaged

#include "fuego/smartgame/SgSystem.h"
#include "fuego/smartgame/SgInit.h"
#include "fuego/smartgame/SgBlackWhite.h"
#include "fuego/go/GoGtpEngine.h"
#include "fuego/go/GoInit.h"
#include "fuego/gouct/GoUctCommands.h"

#pragma managed

namespace FuegoUniversalComponent
{
	public ref class FuegoUniversalComponent sealed
	{
	private:
		GoGtpEngine* _e;

	public:
		void Init();
		void StartGame(int boardSize);
	};
}
