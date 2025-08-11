import React, { useEffect } from 'react';
import logo from './logo.svg';
import './App.css';
import BoardFlight from './Componenets/BoardFlight';
import { useFlight } from './UseHook/useFlight';
function App() {
  const {loadFlights} = useFlight()

  useEffect(() => {
    loadFlights()
    
  },[loadFlights])
  return (
    <div className="App">
      <BoardFlight/>
    </div>
  );
}

export default App;
