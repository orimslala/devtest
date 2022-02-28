
import { useEffect} from 'react'
import { HubConnectionBuilder } from '@microsoft/signalr';
import React, { useState } from 'react';
import { AgGridReact } from 'ag-grid-react';
import fp from 'lodash/fp'
import 'ag-grid-community/dist/styles/ag-grid.css';
import 'ag-grid-community/dist/styles/ag-theme-alpine.css';


function App() {


  const [columnDefs] = useState([
    { field: 'symbol' },
    { field: 'price' },
    {field: 'average'}
])

  const [ connection, setConnection ] = useState(null);

  const [ message, setMessage ] = useState(null);
  const[gridApi, setGridApi] = useState(null);

  const onGridReady = (params) => {
    setGridApi(params.api);
    params.api.sizeColumnsToFit();
  }
  useEffect(() => {
    const newConnection = new HubConnectionBuilder()
        .withUrl('http://localhost:5000/hubs/prices', {withCredentials: false} )
        .withAutomaticReconnect()
        .build();

    setConnection(newConnection);
    console.log(connection)
}, []);


useEffect(() => {
  if (connection) {
      connection.start()
          .then(result => {
              console.log('Connected!');

              connection.on('OnPriceUpdated', message => {
              setMessage(message)
              });


              connection.on('OnAveragePrice', price => {
                  
                });
          })
          .catch(e => console.log('Connection failed: ', e));
  }
}, [connection]);

var symbols = message !== null ? [...new Set(message.map( x => x.symbol))] : []

var arr = []
var data =  symbols !== null && message !== null ? message.reverse() : []

const getPrices = () => {

  symbols.forEach( x => {

    let result = data.find(  d => d.symbol === x )
    if( result !== undefined ) arr.push( result) 
  });

}

getPrices();

return (
  <div className="ag-theme-alpine" style={{height: 1000, width: 600}}>
      <AgGridReact
          getRowNodeId={message => message.symbol}
          onGridReady={(params) => onGridReady(params)}
          rowData={arr}
          columnDefs={columnDefs}>
      </AgGridReact>
  </div>
);
}

export default App;
