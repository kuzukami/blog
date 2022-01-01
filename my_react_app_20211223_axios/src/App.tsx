import React from 'react';
import {BrowserRouter, Routes, Route} from "react-router-dom";
import Home from './pages/Home';
import Axios_20211224 from './pages/Axios20211224';
import './App.css';

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/web/axios" element={<Axios_20211224/>} />
        <Route path="/web/home" element={<Home/>} />
        <Route path="/web/" element={<Home/>} />
        <Route path="*" element={<Home/>} />
      </Routes>
    </BrowserRouter>
  );
}

export default App;