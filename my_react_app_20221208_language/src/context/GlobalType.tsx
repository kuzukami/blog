//-----------------------------------------------------------------------
// common const
//
export const stub = (): never => {
    throw new Error(
      "Context initialize Error."
    );
  }
  
  //-----------------------------------------------------------------------
  // global context
  //  
  export interface IGlobalContextState {
    isEnglish       : boolean;
  }
  
  export const iniGlobalContextState: IGlobalContextState = {
    isEnglish       : false,
  }
  
  export interface IGlobalContext extends IGlobalContextState{
    setLanguage        : () => void;
  }
  
  export const iniGlobalContext: IGlobalContext = {
    ...iniGlobalContextState,
    setLanguage        : stub,
  }
  