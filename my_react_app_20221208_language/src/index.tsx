import React from 'react';
import ReactDOM from 'react-dom/client';
import './index.css';
import App from './App';
import reportWebVitals from './reportWebVitals';
import {GlobalContextProvider} from './context/GlobalContext';
import { CookiesProvider } from 'react-cookie';

const root = ReactDOM.createRoot(
  document.getElementById('root') as HTMLElement
);
root.render(
  <React.StrictMode>
    <GlobalContextProvider>
      <CookiesProvider>
        <App />
      </CookiesProvider>
    </GlobalContextProvider>
  </React.StrictMode>
);

reportWebVitals();
