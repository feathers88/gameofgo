#pragma unmanaged

#include "FuegoLib.h"

// Bring in unmanaged types we need from Fuego.
#include "fuego/smartgame/SgSystem.h"
#include "fuego/smartgame/SgInit.h"
#include "fuego/smartgame/SgBlackWhite.h"
#include "fuego/go/GoGtpEngine.h"
#include "fuego/go/GoInit.h"
#include "fuego/gouct/GoUctCommands.h"
//#include <boost/thread.hpp>

#include <stdio.h>

#pragma managed

using namespace std;

GoGtpEngine* _e;

void Init()
{
	SgInit();
	GoInit();
}

void LibStartGame(const int boardSize)
{
	cerr << "Ctor 1!";

	cerr << "boardSize: " << boardSize << ".";
	
	if (_e != 0)
	{
		delete _e;
		_e = 0;
	}

	cerr << "Ctor 2!";

	try
	{
		_e = new ::GoGtpEngine(boardSize, 0, false, true);
	}
	catch (const std::exception& ex)
	{

	}



	//_e->CmdSetup(GtpCommand());	
}
