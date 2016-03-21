#pragma managed

#include "FuegoUniversalComponent.h"

using namespace std;
using namespace Platform;

namespace FuegoUniversalComponent
{
	void FuegoUniversalComponent::Init()
	{
		SgInit();
		GoInit();
	}

	void FuegoUniversalComponent::StartGame(const int boardSize)
	{		
		if (_e != 0)
		{
			delete _e;
			_e = 0;
		}

		try
		{
			_e = new ::GoGtpEngine(boardSize, 0, false, true);
		}
		catch (const std::exception& ex)
		{
			throw;
		}

		//_e->CmdSetup(GtpCommand());	
	}
}

//void FuegoLib::Fuego::StartGame(int boardSize)
//{
//	throw ref new Platform::NotImplementedException();
//}
