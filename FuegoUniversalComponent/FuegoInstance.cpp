#pragma unmanaged

#include <iostream>
#include <sstream>

#pragma managed

#include "FuegoInstance.h"

using namespace std;
using namespace Platform;

namespace FuegoLib
{
	FuegoInstance::FuegoInstance()
	{
		SgInit();
		GoInit();
	}

	void FuegoInstance::StartGame(unsigned char size)
	{
		try
		{
			if (_e != 0)
			{
				delete _e;
				delete _goin;
				delete _goout;
				_e = nullptr;
				_goin = nullptr;
				_goout = nullptr;
			}

			_e = new GoGtpEngine(size, 0, false, true);
			
			// Start main loop.
			_instr = new stringstream(ios::in | ios::out);
			_outstr = new stringstream(ios::in | ios::out);

			_goin = new GtpInputStream(*_instr);
			_goout = new GtpOutputStream(*_outstr);
		
			_e->MainLoop(*_goin, *_goout);

			//_e->DumpState(*_outstr);
		}
		catch (const std::exception& ex)
		{
			throw;
		}

		//_e->CmdSetup(GtpCommand());	
	}

	void FuegoInstance::Write(Platform::String^ msg)
	{
		if (msg == nullptr)
			throw ref new Platform::NullReferenceException("Write(msg): msg was null");

		wstring ws(msg->Data());
		string str(ws.begin(), ws.end());
		*_instr << str;

		//_e->MainLoop(*_goin, *_goout);
	}

	/*void FuegoInstance::Flush()
	{
		_goin->Flush();		
	}*/

	/*bool FuegoInstance::EndOfInput()
	{
		return _goout->EndOfInput();
	}
*/
	Platform::String^ FuegoInstance::ReadLine()
	{
		std::string line;
		if (getline(*_outstr, line))
		{
			auto widestr = std::wstring(line.begin(), line.end());
			auto line = ref new Platform::String(widestr.c_str());
			return line;
		}
		return nullptr;
	}
}