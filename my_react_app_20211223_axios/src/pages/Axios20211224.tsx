import React, { useEffect, useState } from 'react';
import axios, { AxiosRequestConfig, Canceler } from 'axios';
import ResponsiveDrawer from '../components/layout/ResponsiveDrawer';

const Axios20211224 = () => {

  interface IhttpRequestPostMessage {
    strikeout: string,
    wakls: string
  }
  
  const apiUrlGet  = "https://www.jma.go.jp/bosai/forecast/data/forecast/130000.json";
  const apiUrlPost = "https://d34rs0sv14b8aw.cloudfront.net/api/sabrbbk";

  //const CancelToken = axios.CancelToken;
  // const source = CancelToken.source();
  //let cancel : Canceler;
  const controller = new AbortController();

  // const getHttpPostResponseGet = () => {
  //   axios.get(apiUrlGet, {
  //     params: {'ID': '12345'},
  //     cancelToken: source.token
  //   }).then(function (response) {
  //     console.log(response);
  //   }).catch(function (thrown) {
  //     if (axios.isCancel(thrown)) {
  //       console.log('Get Request canceled', thrown.message);
  //     } else {
  //       console.log(thrown);
  //     }
  //   });
  // }

  const getHttpPostResponsePost = () => {
    axios.post(apiUrlPost,httpRequestMessage,{
      headers: {'Content-Type': 'text/plain'},
      signal:controller.signal
    }).then(function (response) {
      console.log(response);
    }).catch(function (thrown) {
      if (axios.isCancel(thrown)) {
        console.log('Post Request canceled', thrown.message);
      } else {
        console.log(thrown);
      }
    });
  }

  const [httpRequestMessage, setHttpRequestMessage] = useState<IhttpRequestPostMessage>();

  useEffect(() => {
    if(httpRequestMessage !== undefined){
      getHttpPostResponsePost();
    }
  },[httpRequestMessage]);
  
  return (
    <ResponsiveDrawer>
      <div>
        <p>axios sample page.</p>
        <p>
        <button onClick={() => setHttpRequestMessage({strikeout: "100", wakls:"1"})}>
          Click me
        </button>
        </p><p>
        <button onClick={() => {controller.abort();}}>
          Cancel
        </button>
        </p>
      </div>
    </ResponsiveDrawer>
  );
}
  
export default Axios20211224;