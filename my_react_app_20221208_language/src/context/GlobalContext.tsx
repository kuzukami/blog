import React from "react";
import { useCookies  } from "react-cookie";
import {IGlobalContext, iniGlobalContext} from  "./GlobalType";

//-----------------------------------------------------------------------
// auth context
//
const GlobalContext = React.createContext<IGlobalContext>(iniGlobalContext);
export const useGlobalSetting = () => React.useContext(GlobalContext);
export interface IGlobalContextProviderParams {
    children: React.ReactNode;
}

//-----------------------------------------------------------------------
// auth provider
//
export const GlobalContextProvider = (props: IGlobalContextProviderParams) => {

  const [cookies, setCookie] = useCookies(["language"]);
  const [isEnglish, setIsEnglish] = React.useState<boolean>((cookies.language == "en"));

  const setLanguage = (): void => {
    setIsEnglish(!isEnglish);
    var expDate = new Date();
    expDate.setDate(expDate.getDate() + 365);
    setCookie("language", (!isEnglish ? "en" : "ja"), {expires: expDate, path: '/'});
  }

  return (
    <GlobalContext.Provider value={{
      isEnglish,
      setLanguage,
    }}>{props.children}
    </GlobalContext.Provider>
  );
};

export default GlobalContextProvider;