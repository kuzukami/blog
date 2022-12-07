import React from "react";
import { Grid } from '@mui/material';
import { styled } from "@mui/material/styles"

// local module
import GlobalHeader from "../components/GlobalHeader";
import { useGlobalSetting } from "../context/GlobalContext";

//---------------------------------------------------------------------
// const
//
const defaultWidth = "100%";
const defaultHeight = "128px";

//-----------------------------------------------------------------------
// styled component (header)
//
const StyledHeaderContent = styled("div")(({ theme }) => ({
  // position
  position: "relative",
  // color
  backgroundColor: theme.palette.secondary.light,
  // size
  width: defaultWidth,
  height: defaultHeight,
  // font / text
  display: "flex",
  alignItems: "center",
}));

const StyledHeaderSpan = styled("span")(({ theme }) => ({
  // position
  position: "absolute",
  // color
  color: theme.palette.primary.contrastText,
  // font / text
  fontSize: "small",
  padding: ".4em",
}));

//-----------------------------------------------------------------------
// function component
//
const Home: React.FC = () => {

  const {isEnglish} = useGlobalSetting();

  return (
    <GlobalHeader>
      <Grid container>
        <Grid item xs={12}>
          <StyledHeaderContent>
            <StyledHeaderSpan>
              {isEnglish && "Hello"}
              {!isEnglish && "こんにちは"}
            </StyledHeaderSpan>
          </StyledHeaderContent>
        </Grid>
      </Grid>
      <Grid container>
        <Grid item xs={12}>
          <span>home</span>
        </Grid>
        <Grid item xs={12}>
          <a href="/index">to index</a>
        </Grid>
      </Grid>
    </GlobalHeader>
  );
}

export default Home;