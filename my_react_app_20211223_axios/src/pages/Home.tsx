import React, { useEffect, useState } from 'react';
import axios from 'axios';
import ResponsiveDrawer from '../components/layout/ResponsiveDrawer';

const Home = () => {

  const [httpRequestMessage, setHttpRequestMessage] = useState("");
  const apiUrl = "https://api_no_url";

  useEffect(() => {
    if(httpRequestMessage !== ""){
      axios.get(apiUrl + "?message=" + httpRequestMessage)
      .then(function (response) {
        console.log(response);
      })
      .catch(function (error) {
        console.log(error);
      });
    }
  },[httpRequestMessage]);
  
  return (
    <ResponsiveDrawer>
      <div>
        <p>home page.</p>
        <button onClick={() => setHttpRequestMessage("Hello")}>
          Click me
        </button>
      </div>
    </ResponsiveDrawer>
  );
}
  
export default Home;