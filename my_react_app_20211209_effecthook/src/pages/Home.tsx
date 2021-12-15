import React, { useContext } from 'react';
import ResponsiveDrawer from '../components/layout/ResponsiveDrawer';
import {AuthContext} from '../components/auth/AuthComponent';

const Home = () => {
  const { userId, setUserId} = useContext(AuthContext);
  return (
    <ResponsiveDrawer>
      <div>
        <p>home page.</p>
        <p>user is {userId}</p>
        <button onClick={() => {setUserId("new " + userId)}}>
          Click me
        </button>
      </div>
    </ResponsiveDrawer>
  );
}
  
export default Home;