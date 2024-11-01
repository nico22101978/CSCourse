

/* this ALWAYS GENERATED file contains the definitions for the interfaces */


 /* File created by MIDL compiler version 7.00.0555 */
/* at Wed Oct 31 21:20:55 2012
 */
/* Compiler settings for AComServer.idl:
    Oicf, W1, Zp8, env=Win32 (32b run), target_arch=X86 7.00.0555 
    protocol : dce , ms_ext, c_ext, robust
    error checks: allocation ref bounds_check enum stub_data 
    VC __declspec() decoration level: 
         __declspec(uuid()), __declspec(selectany), __declspec(novtable)
         DECLSPEC_UUID(), MIDL_INTERFACE()
*/
/* @@MIDL_FILE_HEADING(  ) */

#pragma warning( disable: 4049 )  /* more than 64k source lines */


/* verify that the <rpcndr.h> version is high enough to compile this file*/
#ifndef __REQUIRED_RPCNDR_H_VERSION__
#define __REQUIRED_RPCNDR_H_VERSION__ 475
#endif

#include "rpc.h"
#include "rpcndr.h"

#ifndef __RPCNDR_H_VERSION__
#error this stub requires an updated version of <rpcndr.h>
#endif // __RPCNDR_H_VERSION__

#ifndef COM_NO_WINDOWS_H
#include "windows.h"
#include "ole2.h"
#endif /*COM_NO_WINDOWS_H*/

#ifndef __AComServer_i_h__
#define __AComServer_i_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __INicosComClass_FWD_DEFINED__
#define __INicosComClass_FWD_DEFINED__
typedef interface INicosComClass INicosComClass;
#endif 	/* __INicosComClass_FWD_DEFINED__ */


#ifndef __NicosComClass_FWD_DEFINED__
#define __NicosComClass_FWD_DEFINED__

#ifdef __cplusplus
typedef class NicosComClass NicosComClass;
#else
typedef struct NicosComClass NicosComClass;
#endif /* __cplusplus */

#endif 	/* __NicosComClass_FWD_DEFINED__ */


/* header files for imported files */
#include "oaidl.h"
#include "ocidl.h"

#ifdef __cplusplus
extern "C"{
#endif 


#ifndef __INicosComClass_INTERFACE_DEFINED__
#define __INicosComClass_INTERFACE_DEFINED__

/* interface INicosComClass */
/* [unique][nonextensible][dual][uuid][object] */ 


EXTERN_C const IID IID_INicosComClass;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("88B4EC6A-8A7C-4169-98CF-1F600C03C248")
    INicosComClass : public IDispatch
    {
    public:
        virtual /* [id] */ HRESULT STDMETHODCALLTYPE GetData( 
            /* [retval][out] */ BSTR *data) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct INicosComClassVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            INicosComClass * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            INicosComClass * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            INicosComClass * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            INicosComClass * This,
            /* [out] */ UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            INicosComClass * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            INicosComClass * This,
            /* [in] */ REFIID riid,
            /* [size_is][in] */ LPOLESTR *rgszNames,
            /* [range][in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            INicosComClass * This,
            /* [in] */ DISPID dispIdMember,
            /* [in] */ REFIID riid,
            /* [in] */ LCID lcid,
            /* [in] */ WORD wFlags,
            /* [out][in] */ DISPPARAMS *pDispParams,
            /* [out] */ VARIANT *pVarResult,
            /* [out] */ EXCEPINFO *pExcepInfo,
            /* [out] */ UINT *puArgErr);
        
        /* [id] */ HRESULT ( STDMETHODCALLTYPE *GetData )( 
            INicosComClass * This,
            /* [retval][out] */ BSTR *data);
        
        END_INTERFACE
    } INicosComClassVtbl;

    interface INicosComClass
    {
        CONST_VTBL struct INicosComClassVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define INicosComClass_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define INicosComClass_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define INicosComClass_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define INicosComClass_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define INicosComClass_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define INicosComClass_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define INicosComClass_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 


#define INicosComClass_GetData(This,data)	\
    ( (This)->lpVtbl -> GetData(This,data) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __INicosComClass_INTERFACE_DEFINED__ */



#ifndef __AComServerLib_LIBRARY_DEFINED__
#define __AComServerLib_LIBRARY_DEFINED__

/* library AComServerLib */
/* [version][uuid] */ 


EXTERN_C const IID LIBID_AComServerLib;

EXTERN_C const CLSID CLSID_NicosComClass;

#ifdef __cplusplus

class DECLSPEC_UUID("82E6CD66-802C-4A05-AD55-6CFFB9E665A1")
NicosComClass;
#endif
#endif /* __AComServerLib_LIBRARY_DEFINED__ */

/* Additional Prototypes for ALL interfaces */

unsigned long             __RPC_USER  BSTR_UserSize(     unsigned long *, unsigned long            , BSTR * ); 
unsigned char * __RPC_USER  BSTR_UserMarshal(  unsigned long *, unsigned char *, BSTR * ); 
unsigned char * __RPC_USER  BSTR_UserUnmarshal(unsigned long *, unsigned char *, BSTR * ); 
void                      __RPC_USER  BSTR_UserFree(     unsigned long *, BSTR * ); 

/* end of Additional Prototypes */

#ifdef __cplusplus
}
#endif

#endif


