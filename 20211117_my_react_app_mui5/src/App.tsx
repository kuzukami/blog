import React from 'react';
import Home from './pages/Home';
import Portrait from './pages/Portrait';
import {BrowserRouter, Routes, Route} from "react-router-dom";

function App () {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/web/portrait" element={<Portrait/>} />
        <Route path="/web/home" element={<Home/>} />
        <Route path="/web/" element={<Home/>} />
        <Route path="*" element={<Home/>} />
      </Routes>
    </BrowserRouter>
  );
}

export default App;
