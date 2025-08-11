import { useCallback,useEffect } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import {
  fetchAllFlights,
  fetchFlightById,
  addFlight,
  editFlight,
  removeFlight,
  addFlightFromListeners,
  removeFlightFromListeners,
  updateFlightFromLisiter,
  setSelectedFlight,
  setlClearSelectedFlight,
  setLoading,
  getFlightByQuery,
  clearFiltered,
  setMessageError,
  setLastUpdatedId
} from '../Slices/FlightSlice';
import { RootState, AppDispatch } from '../Store/store';
import { Flight } from '../Types/FlightTypes';
import { connection } from '../Services/HubConnection';
import * as signalR from '@microsoft/signalr';

export const useFlight = () => {
  const dispatch = useDispatch<AppDispatch>();
  const {flights,selectedFlight,filteredFlights,loading,error,lastUpdatedId} = useSelector((state: RootState) => state.flights);


  // פונקציות עטופות ב-useCallback
  const loadFlights = useCallback(() => {
    dispatch(fetchAllFlights());
  }, [dispatch]);

  const getFlight = useCallback((id: string) => {
    dispatch(fetchFlightById(id));
  }, [dispatch]);

  const createFlight = useCallback((flight: Flight) => {
    dispatch(addFlight(flight));
  }, [dispatch]);

  const updateFlight = useCallback((flight: Flight) => {
    dispatch(editFlight(flight));
  }, [dispatch]);

  const deleteFlight = useCallback((id: string) => {
    dispatch(removeFlight(id));
  }, [dispatch]);

  const querySearchFlight = useCallback((query: string) => {
    console.log(query)
    dispatch(getFlightByQuery(query));
  }, [dispatch]);

  const setFlight = (id: string) => {
    dispatch(setSelectedFlight(id))
  }
  const clearSelectedFlight = () => {
    dispatch(setlClearSelectedFlight())
  }
  const setLoadingToUpdate = () => {
    dispatch(setLoading())
  }
  const clearFilteredFlight = () => {
    dispatch(clearFiltered())
  }
const clearLastUpdated = () => dispatch(setLastUpdatedId(null));

useEffect(() => {
  const startConnection = async () => {
    try {
      if (connection.state === signalR.HubConnectionState.Disconnected) {
        await connection.start();
        console.log("SignalR connected");
      }
      // הסר קודם כל מאזינים קיימים
      connection.off("FlightAdded");
      connection.off("FlightUpdated");
      connection.off("FlightDeleted");

      // ואז תירשם שוב
      connection.on("FlightAdded", (flight: Flight) => {
        console.log("📡 FlightAdded received", flight);
        dispatch(addFlightFromListeners(flight));
      });

      connection.on("FlightUpdated", (flight: Flight) => {
        dispatch(updateFlightFromLisiter(flight));
      });

      connection.on("FlightDeleted", (flight: Flight) => {
        dispatch(removeFlightFromListeners(flight));
      });
    } catch (err) {
      console.error("SignalR error:", err);
    }
  };
  startConnection();
  return () => {
    connection.off("FlightAdded");
    connection.off("FlightUpdated");
    connection.off("FlightDeleted");
  };
}, []); 


  return {
    flights,
    selectedFlight,
    filteredFlights,
    loading,
    error,
    setMessageError,
    lastUpdatedId,
    clearLastUpdated,
    setLoadingToUpdate,
    loadFlights,
    getFlight,
    createFlight,
    querySearchFlight,
    updateFlight,
    deleteFlight,
    setFlight,
    clearSelectedFlight, 
    clearFilteredFlight,
    };
};
