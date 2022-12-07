import * as React from "react";
import { Button } from '@mui/material';
import { styled } from "@mui/material/styles";

// local module
import { useGlobalSetting } from "../context/GlobalContext";

//-----------------------------------------------------------------------
// const
//
const globalHeaderFontSize = "x-small";
const globalHeaderPadding  = ".4em";
const globalHeaderHeight = "24px";

//-----------------------------------------------------------------------
// styled component (area)
//
const StyledGlobalHeader = styled("header")(({ theme }) => ({
  // background
  backgroundColor: theme.palette.primary.main,
  // size
  height: globalHeaderHeight,
  // content
  display: "flex",
  alignItems: "center",
}));

//-----------------------------------------------------------------------
// styled component (content)
//
const StyledSpan = styled("span")(({ theme }) => ({
  // text
  color: theme.palette.primary.contrastText,
  fontSize: globalHeaderFontSize,
  padding: globalHeaderPadding,
  display: "flex",
  flexGrow: 1,
}));

const StyledButton = styled(Button)(({ theme }) => ({
  // text
  display: "flex",
  fontSize: globalHeaderFontSize,
}));

//-----------------------------------------------------------------------
// props
//
interface Props {
  children: React.ReactNode;
}

//-----------------------------------------------------------------------
// function component
//
const GlobalHeader: React.FC<Props> = ({children}) =>  {

  const {isEnglish, setLanguage} = useGlobalSetting();

  //---------------------------------------------------------------------
  // main
  //
  return (
    <div>
      <StyledGlobalHeader>
        <StyledSpan>{"Language"}</StyledSpan>
        <StyledButton onClick={() => {setLanguage()}}>
          {!isEnglish && <StyledSpan>{"English"}</StyledSpan>}
          {isEnglish  && <StyledSpan>{"Japanese"}</StyledSpan>}
        </StyledButton>
      </StyledGlobalHeader>
      {children}
    </div>
  );
}

export default GlobalHeader;
