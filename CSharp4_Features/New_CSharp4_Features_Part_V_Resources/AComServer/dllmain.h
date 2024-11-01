// dllmain.h : Declaration of module class.

class CAComServerModule : public ATL::CAtlDllModuleT< CAComServerModule >
{
public :
	DECLARE_LIBID(LIBID_AComServerLib)
	DECLARE_REGISTRY_APPID_RESOURCEID(IDR_ACOMSERVER, "{1DE89C7C-33D1-4A34-AAD0-E504DCEFC7B9}")
};

extern class CAComServerModule _AtlModule;
