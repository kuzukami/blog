import React, { useEffect, useState } from 'react';
import axios from 'axios';
import ResponsiveDrawer from '../components/layout/ResponsiveDrawer';

const Home = () => {

  const [msg, setMsg] = useState("");
  const apiUrl = "";

  useEffect(() => {
    if(msg !== ""){
      axios.get(apiUrl)
      .then(function (response) {
        console.log(response);
      })
      .catch(function (error) {
        console.log(error);
      });
    }
  },[msg]);
  
  return (
    <ResponsiveDrawer>
      <div>
        <p>home page.</p>
        <button onClick={() => setMsg("Hello nomurabbit")}>
          Click me
        </button>
      </div>
    </ResponsiveDrawer>
  );
}
  
export default Home;