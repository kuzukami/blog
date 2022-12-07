import React from 'react';
import {BrowserRouter as Router, Routes, Route} from "react-router-dom";

import Index from "./pages/Index";
import Home from "./pages/Home";

const App: React.FC = () => {

  return (
    <Router>
      <Routes>
        <Route index element={<Index/>}></Route>
        <Route path="/index" element={<Index/>}></Route>
        <Route path="/home" element={<Home/>}></Route>
      </Routes>
    </Router>
  );
}

export default App;