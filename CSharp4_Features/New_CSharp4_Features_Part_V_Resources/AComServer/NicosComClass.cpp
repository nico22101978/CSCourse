// NicosComClass.cpp : Implementation of CNicosComClass

#include "stdafx.h"
#include "NicosComClass.h"


// CNicosComClass



STDMETHODIMP CNicosComClass::GetData(BSTR* data)
{
	// TODO: Add your implementation code here
	*data = SysAllocString(L"Hello World!");

	return S_OK;
}
