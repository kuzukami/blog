import React from 'react';
import {BrowserRouter, Routes, Route} from "react-router-dom";
import Home from './pages/Home';
import Home2 from './pages/Home2';
import Home3 from './pages/Home3';
import Home4 from './pages/Home4';
import Home5 from './pages/Home5';
import Home6 from './pages/Home6';
import Home7 from './pages/Home7';
import Home8 from './pages/Home8';
import './App.css';

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/web/home8" element={<Home8/>} />
        <Route path="/web/home7" element={<Home7/>} />
        <Route path="/web/home6" element={<Home6/>} />
        <Route path="/web/home5" element={<Home5/>} />
        <Route path="/web/home4" element={<Home4/>} />
        <Route path="/web/home3" element={<Home3/>} />
        <Route path="/web/home2" element={<Home2/>} />
        <Route path="/web/home" element={<Home/>} />
        <Route path="/web/" element={<Home/>} />
        <Route path="*" element={<Home/>} />
      </Routes>
    </BrowserRouter>
  );
}

export default App;