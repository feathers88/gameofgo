//#pragma unmanaged

#include <iostream>
#include <sstream>

#include "FuegoInstance.h"

//#include "fuego/gouct/GoUctCommands.h"
//#include "fuego/gouct/GoUctPlayer.h"

using namespace std;
using namespace Platform;
using namespace Platform::Collections;
using namespace Windows::Foundation::Collections;

namespace FuegoLib
{
	FuegoMainEngine::FuegoMainEngine(int fixedBoardSize, const char* programPath,
		bool noHandicap)
		: GoGtpEngine(fixedBoardSize, programPath, false, noHandicap),
		m_uctCommands(Board(), m_player)
	{
		m_uctCommands.Register(*this);
		SetPlayer(new PlayerType(Board()));
	}

	FuegoMainEngine::~FuegoMainEngine() { }

	FuegoInstance::FuegoInstance()
	{
		if (!_inited)
		{
			// This has to happen once and only once.
			SgInit();
			GoInit();

			_inited = true;
		}
	}

	bool FuegoInstance::_inited = false;

	FuegoInstance::~FuegoInstance()
	{
		// Fini funcs should be called only once after no
		// the library won't be used anymore, so really no 
		// need to ever call them.
		//GoFini();
		//SgFini();

		if (_e != nullptr)
		{
			delete _e;
			_e = nullptr;
		}
	}

	void FuegoInstance::StartGame(unsigned char size)
	{
		try
		{
			if (_e != nullptr)
			{
				delete _e;
				_e = nullptr;
			}
			_e = new FuegoMainEngine(size, 0, false);
	
			//_e->DumpState(*_outstr);
		}
		catch (const std::exception& ex)
		{
			auto err = ex.what();
			throw;
		}

		//_e->CmdSetup(GtpCommand());	
	}

	Platform::String^ FuegoInstance::HandleCommand(Platform::String^ cmd)
	{
		// Write command to new stream, converting wchar_t* to string.
		wstring ws(cmd->Data());
		string str(ws.begin(), ws.end());
		
		stringstream instr(ios::in | ios::out);
		GtpInputStream goin(instr);
		instr.str(str);
		stringstream outstr(ios::in | ios::out);
		GtpOutputStream goout(outstr);

		// Process command, which writes to _outstr.
		_e->MainLoop(goin, goout);

		// Change std::string to platform string.
		auto strin = outstr.str();
		auto widestr = std::wstring(strin.begin(), strin.end());
		auto output = ref new Platform::String(widestr.c_str());
		
		return output;
	}
}