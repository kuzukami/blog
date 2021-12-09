import React, { useState, useEffect  } from 'react';
import ResponsiveDrawer from '../components/layout/ResponsiveDrawer';

const Home = () => {

  //const [count, setCount] = useState(0);
  let count = 0;

  return (
    <ResponsiveDrawer>
      <div>
        <p>It's amplify test / home page.</p>
        <p>You clicked {count} times count</p>
        {/* <button onClick={() => setCount(count + 1)}> */}
        <button onClick={() => count += 1}>
          Click me
        </button>
      </div>
    </ResponsiveDrawer>
  );
}
  
export default Home;