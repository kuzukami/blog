import React from 'react';
import {BrowserRouter, Routes, Route} from "react-router-dom";
import Portrait from './pages/Portrait';
import Home from './pages/Home';
import './App.css';

function App() {
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