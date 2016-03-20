#pragma managed

#include "pch.h"
#include "FuegoUniversalComponent.h"

#include <stdio.h>

#pragma unmanaged

#include "FuegoLib/FuegoLib.h"

#pragma managed

using namespace std;

using namespace Platform;

namespace FuegoUniversalComponent
{
	void FuegoUniversalComponent::StartGame(const int boardSize)
	{
		
		//std::cerr << "boardSize: " << boardSize << ".";

		try
		{
			Init();
			LibStartGame(boardSize);
		}
		catch (const std::exception& e)
		{
			throw;
		}
	}
}

//void FuegoLib::Fuego::StartGame(int boardSize)
//{
//	throw ref new Platform::NotImplementedException();
//}
