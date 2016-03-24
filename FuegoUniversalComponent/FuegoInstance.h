#pragma once

#include <iostream>
#include <sstream>

#pragma unmanaged

#include "fuego/smartgame/SgSystem.h"
#include "fuego/smartgame/SgInit.h"
#include "fuego/smartgame/SgBlackWhite.h"
#include "fuego/go/GoGtpEngine.h"
#include "fuego/go/GoInit.h"
#include "fuego/gouct/GoUctCommands.h"

#pragma managed

namespace FuegoLib
{
	public ref class FuegoInstance sealed
	{
	private:
		GoGtpEngine* _e;
		GtpInputStream* _goin;
		GtpOutputStream* _goout;
		std::stringstream* _instr;
		std::stringstream* _outstr;
		
	public:
		FuegoInstance();
		void StartGame(unsigned char size);
		void Write(Platform::String^ msg);
		//void Flush();
		//bool EndOfInput();
		Platform::String^ ReadLine();

		property Platform::Guid Guid;
	};
}
