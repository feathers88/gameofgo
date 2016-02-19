#pragma unmanaged

// Bring in unmanaged types we need from Fuego.
#include <SgSystem.h>
#include <SgBlackWhite.h>
#include <GoGtpEngine.h>
#include <GoUctCommands.h>
//#include <boost/thread.hpp>

#pragma managed

//namespace boost {
//	namespace detail {
//		namespace win32 {
//			struct _SECURITY_ATTRIBUTES : public ::SECURITY_ATTRIBUTES {};
//		};
//	};
//};

namespace FuegoInterop
{

	//////////////////////////////////////////////////////////////
	// _impl idiom for including a native object in a ref class
	template<typename T> public ref class AutoPtr
	{
	private:

		T* _t;

	public:

		AutoPtr() : _t(new T){;}
		AutoPtr(T* t) : _t(t){;}

		T* operator->() {return _t;}

	protected:

		~AutoPtr() // deterministic destructor
		{ delete _t; _t = 0; }

		!AutoPtr() // non-deterministic finalizer
		{ delete _t; _t = 0; }
	};

#pragma managed


	public ref class SimpleFuegoEngine
	{
	private:
		::GoGtpEngine* _e;

	public:
		SimpleFuegoEngine() { _e = 0; }
		
		~SimpleFuegoEngine()
		{
			if (_e != 0)
				delete _e;
		}
		
		void StartGame(int boardSize, long maxMemory) 
		{
			if (_e != 0)
			{
				delete _e;
				_e = 0;
			}
			
			_e = new ::GoGtpEngine(boardSize, 0, false, true);
			//GoUctCommands::CmdMaxMemory(GtpCommand& cmd)

			//_e.Start();
		}
	};

}